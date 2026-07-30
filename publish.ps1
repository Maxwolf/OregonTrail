#!/usr/bin/env pwsh
#Requires -Version 5.1
<#
.SYNOPSIS
    Publishes the game, minigames, and bot as self-contained, single-file
    executables into the repo-root "publish" folder.

.DESCRIPTION
    Each project carries a "SelfContained" publish profile
    (Properties/PublishProfiles/SelfContained.pubxml) that produces one
    self-contained single-file exe with the runtime, dependencies, and native
    SQLite bundled in. All three land in the same ./publish folder, which is
    created if it does not exist.

    The profile targets win-x64; -Rid retargets it to any of the six runtime
    identifiers the release ships (win-x64, win-arm64, linux-x64, linux-arm64,
    osx-x64, osx-arm64). Command-line properties are global, so they win over
    the profile's own RuntimeIdentifier/PublishDir assignments.

    With -Package the three executables are staged into a versioned folder
    alongside README/LICENSE and a first-run note, then compressed into one
    archive per platform (.zip for Windows, .tar.gz elsewhere, which is what
    preserves the executable permission bit). This is the mode the release
    workflow (.github/workflows/release.yml) calls on each runner.

.PARAMETER Clean
    Delete the publish folder before publishing, so no stale files linger.

.PARAMETER Rid
    .NET runtime identifier to publish for. Defaults to win-x64.

.PARAMETER Version
    Version stamp, in this project's year.month.day.hour CalVer form (e.g.
    2026.7.29.17). Blank leaves the assembly version at its default; blank with
    -Package stamps the current UTC time.

.PARAMETER Package
    Stage the executables into a versioned folder and compress them into a
    single distributable archive instead of leaving them loose in ./publish.

.PARAMETER PublishRoot
    Where the executables (or archives) land. Defaults to ./publish.

.EXAMPLE
    ./publish.ps1
    ./publish.ps1 -Clean
    ./publish.ps1 -Rid linux-arm64 -Version 2026.7.29.17 -Package
#>
[CmdletBinding()]
param(
    [switch]$Clean,

    [ValidateSet('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'osx-x64', 'osx-arm64')]
    [string]$Rid = 'win-x64',

    [ValidatePattern('^$|^\d{4}\.\d{1,2}\.\d{1,2}\.\d{1,2}$')]
    [string]$Version = '',

    [switch]$Package,

    [string]$PublishRoot
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$publishDir = if ($PublishRoot) { $PublishRoot } else { Join-Path $root 'publish' }

# MSBuild wants forward slashes and a trailing separator, and a path handed to
# dotnet must not end in a backslash right before a closing quote (Windows reads
# that as an escape). Normalizing to "c:/x/y/" sidesteps both.
function Get-MsBuildPath([string]$path) {
    return (($path -replace '\\', '/').TrimEnd('/') + '/')
}

if ($Clean -and (Test-Path $publishDir)) {
    Write-Host "Cleaning $publishDir ..." -ForegroundColor Yellow
    Remove-Item $publishDir -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $publishDir | Out-Null

# The version this project iterates on: year.month.day.hour, UTC, unpadded - the
# same stamp the WolfCurses package it builds on uses, so a build says out loud
# when it was made.
if ($Package -and -not $Version) {
    $Version = [DateTime]::UtcNow.ToString('yyyy.M.d.H', [Globalization.CultureInfo]::InvariantCulture)
    Write-Host "No -Version given; stamping this build $Version (UTC)." -ForegroundColor Yellow
}

$isWindowsRid = $Rid.StartsWith('win-')
$exeSuffix = if ($isWindowsRid) { '.exe' } else { '' }

# $IsWindows only exists on PowerShell Core; this reads the same on 5.1.
$hostIsWindows = [Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
    [Runtime.InteropServices.OSPlatform]::Windows)

# The three executables that ship. Each uses its own SelfContained profile; the
# assembly name is what the published file is called.
$projects = @(
    @{ Path = 'src\OregonTrailDotNet\OregonTrailDotNet.csproj'; Name = 'OregonTrailDotNet' }
    @{ Path = 'src\OregonTrailDotNet.Minigames\OregonTrailDotNet.Minigames.csproj'; Name = 'OregonTrailDotNet.Minigames' }
    @{ Path = 'bot\OregonTrailDotNet.Bot\OregonTrailDotNet.Bot.csproj'; Name = 'OregonTrailDotNet.Bot' }
)

# Packaging publishes into a versioned staging folder so the archive unpacks as
# one directory; a plain publish keeps dropping the exes straight into ./publish.
$stageName = "OregonTrail-$Version-$Rid"
$stageDir = if ($Package) { Join-Path $publishDir $stageName } else { $publishDir }
if ($Package -and (Test-Path $stageDir)) { Remove-Item $stageDir -Recurse -Force }
New-Item -ItemType Directory -Force -Path $stageDir | Out-Null

foreach ($proj in $projects) {
    $full = Join-Path $root $proj.Path
    Write-Host "`nPublishing $($proj.Path) [$Rid] ..." -ForegroundColor Cyan

    # OutputPath is pinned to C:\OregonTrail\bin\ inside the game's csproj, which
    # is meaningless on a Linux or macOS build host (and would collide between
    # RIDs). Point every RID at its own folder under the ignored obj/ tree.
    # SelfContained and PublishSingleFile come from the profile, but a profile that
    # goes missing or gets renamed is only warning NETSDK1198 - the publish then
    # quietly produces a framework-dependent, non-bundled app that needs a runtime
    # installed. State the two settings that would ruin the download explicitly.
    $msbuildArgs = @(
        'publish', $full
        '-c', 'Release'
        '-p:PublishProfile=SelfContained'
        "-p:RuntimeIdentifier=$Rid"
        '-p:SelfContained=true'
        '-p:PublishSingleFile=true'
        "-p:PublishDir=`"$(Get-MsBuildPath $stageDir)`""
        "-p:OutputPath=`"$(Get-MsBuildPath (Join-Path $root "obj/release/$Rid"))`""
        '--nologo'
    )
    if ($Version) { $msbuildArgs += "-p:Version=$Version" }

    & dotnet @msbuildArgs
    if ($LASTEXITCODE -ne 0) { throw "Publish failed for $($proj.Path) (exit $LASTEXITCODE)" }
}

# Fail loudly if an executable the archive promises never showed up - and if one
# came out too small to be carrying the runtime and the artwork, which is what a
# silently framework-dependent publish looks like (these run 36-40 MB apiece).
$shipped = @()
foreach ($proj in $projects) {
    $exe = Join-Path $stageDir ($proj.Name + $exeSuffix)
    if (-not (Test-Path $exe)) { throw "Expected executable missing after publish: $exe" }
    $item = Get-Item $exe
    if ($item.Length -lt 20MB) {
        throw ("$($item.Name) is only {0:N1} MB; a self-contained single-file build is far larger. " -f ($item.Length / 1MB)) +
              'The SelfContained publish profile probably did not apply.'
    }
    $shipped += $item
}

# Publishing for a Linux RID also drops libe_sqlite3.a - a static archive only a
# native-AOT link would want. Prune the staging folder to exactly what ships.
# Only in packaging mode: the staging folder is one we just created, whereas a
# plain publish writes into a folder that may hold other things already.
if ($Package) {
    Get-ChildItem $stageDir -File |
        Where-Object { $shipped.Name -notcontains $_.Name } |
        ForEach-Object {
            Write-Host "  dropping stray $($_.Name)" -ForegroundColor DarkGray
            Remove-Item $_.FullName -Force
        }
}

if (-not $Package) {
    Write-Host "`nDone. Self-contained executables in $publishDir :" -ForegroundColor Green
    $shipped | Sort-Object Name |
        ForEach-Object { Write-Host ("  {0,-34} {1,8:N1} MB" -f $_.Name, ($_.Length / 1MB)) }
    return
}

# --- packaging -------------------------------------------------------------

Copy-Item (Join-Path $root 'README.md') $stageDir
Copy-Item (Join-Path $root 'LICENSE') $stageDir

# A player unpacking this on a Mac or a Linux box needs to know three things the
# archive cannot tell them: how to launch it, that Gatekeeper will object, and
# that the music is Windows-only (the audio stack talks to winmm and no-ops
# everywhere else). Keep it short enough that it actually gets read.
$firstRun = switch -Wildcard ($Rid) {
    'win-*' {
        @"
Oregon Trail Clone $Version ($Rid)

Double-click OregonTrailDotNet.exe, or run it from a terminal.

  * Windows has no signature to check here, so SmartScreen may warn about an
    unrecognized app: choose "More info" then "Run anyway".
  * Use a terminal at least 80 columns by 25 rows. Anything shorter than 25
    rows starts dropping menu options off the bottom. Windows Terminal draws
    the original artwork best; the old conhost window works too.
  * Music and sound effects play through Windows' own waveOut device. F8 mutes,
    F9 and F10 set the volume, and the main menu can turn sound off entirely.
  * High scores and tombstones are kept in game.db next to the executable, so
    put it somewhere you can write to (not Program Files).

OregonTrailDotNet.Minigames.exe is the standalone minigame workbench and
OregonTrailDotNet.Bot.exe is the headless training bot. Neither is needed to
play the game.
"@
    }
    'osx-*' {
        @"
Oregon Trail Clone $Version ($Rid)

Open Terminal (or iTerm2), cd into this folder, and run:

    ./OregonTrailDotNet

  * macOS quarantines anything downloaded from a browser and this build is not
    signed by an Apple developer account, so the first launch may be refused.
    Clear the flag once:

        xattr -d com.apple.quarantine ./OregonTrailDotNet

    (Or Control-click the file in Finder and choose Open.) If the file is not
    executable, run: chmod +x ./OregonTrailDotNet
  * Use a window at least 80 columns by 25 rows. Terminal.app opens at 80x24 by
    default, one row short, and the game silently drops whatever does not fit,
    menu options included. Stretch the window before you start.
  * There is no sound on macOS. The audio stack drives Windows' waveOut device
    directly and stays silent everywhere else; the artwork is unaffected.
  * High scores and tombstones are kept in game.db next to the executable, so
    keep it in a folder you can write to.

OregonTrailDotNet.Minigames is the standalone minigame workbench and
OregonTrailDotNet.Bot is the headless training bot. Neither is needed to play
the game.
"@
    }
    default {
        @"
Oregon Trail Clone $Version ($Rid)

From a terminal, cd into this folder and run:

    ./OregonTrailDotNet

  * The archive preserves the executable bit. If you copied the file around and
    lost it, run: chmod +x ./OregonTrailDotNet
  * Use a terminal at least 80 columns by 25 rows. Most terminals open at 80x24
    by default, one row short, and the game silently drops whatever does not
    fit, menu options included. Stretch the window before you start. A truecolor
    terminal draws the original artwork best.
  * There is no sound on Linux. The audio stack drives Windows' waveOut device
    directly and stays silent everywhere else; the artwork is unaffected.
  * High scores and tombstones are kept in game.db next to the executable, so
    keep it in a folder you can write to.

OregonTrailDotNet.Minigames is the standalone minigame workbench and
OregonTrailDotNet.Bot is the headless training bot. Neither is needed to play
the game.
"@
    }
}
# LF and no BOM: this file gets read on all three platforms.
[IO.File]::WriteAllText((Join-Path $stageDir 'FIRST-RUN.txt'), ($firstRun -replace "`r`n", "`n"), (New-Object Text.UTF8Encoding $false))

# Publishing on Unix marks the apphost executable already; re-assert it in case a
# copy lost it. NTFS has no permission bit to hand over and the tar on Windows is
# bsdtar (no --mode), so a Linux/macOS archive built here unpacks as 0644 and the
# player has to chmod it themselves. The release workflow builds each platform's
# archive on its own runner precisely so that cannot happen.
if (-not $isWindowsRid) {
    if ($hostIsWindows) {
        Write-Warning "Packaging $Rid on Windows: the executable bit cannot be stored, so this archive unpacks non-executable. Build it on Linux/macOS (as CI does) for a release."
    }
    else {
        foreach ($exe in $shipped) { & chmod '+x' (Join-Path $stageDir $exe.Name) }
    }
}

# macOS will not launch an unsigned arm64 binary at all, so the SDK ad-hoc signs
# the apphost when it publishes for an osx RID on a Mac - and appending the
# single-file bundle after signing is exactly what would invalidate it. Verify,
# and re-sign if the bundle came out unsigned, rather than trusting either way.
if ($Rid.StartsWith('osx-')) {
    if (-not (Get-Command codesign -ErrorAction SilentlyContinue)) {
        Write-Warning "Packaging $Rid without codesign available: the binaries are unsigned and Gatekeeper will refuse them on Apple silicon. Build this archive on macOS (as CI does) for a release."
    }
    else {
        # Only arm64 is strict: macOS SIGKILLs unsigned arm64 code, so a signature that
        # will not verify there means an archive nobody can run and the build should
        # stop. Unsigned x86_64 still runs (under Rosetta), and signing an Intel slice
        # from an Apple silicon host is not something this project has been able to
        # test on real hardware - so osx-x64 warns rather than failing the release on
        # an assumption.
        $signatureIsRequired = $Rid -eq 'osx-arm64'
        foreach ($exe in $shipped) {
            $target = Join-Path $stageDir $exe.Name
            & codesign --verify --strict $target 2>&1 | Out-Null
            if ($LASTEXITCODE -ne 0) {
                Write-Host "  ad-hoc signing $($exe.Name)" -ForegroundColor DarkGray
                & codesign --force --sign - $target
            }
            & codesign --verify --strict $target 2>&1 | Out-Null
            if ($LASTEXITCODE -ne 0) {
                $message = "$($exe.Name) has no signature that codesign will verify."
                if ($signatureIsRequired) { throw "$message Apple silicon would refuse to run it." }
                Write-Warning "$message Intel Macs run unsigned code, so this archive is still usable."
            }
        }
    }
}

$archive = if ($isWindowsRid) { "$stageName.zip" } else { "$stageName.tar.gz" }
$archivePath = Join-Path $publishDir $archive
if (Test-Path $archivePath) { Remove-Item $archivePath -Force }

Write-Host "`nPackaging $archive ..." -ForegroundColor Cyan
if ($isWindowsRid) {
    # Not Compress-Archive: on Windows PowerShell 5.1 it stores backslash path
    # separators, which the ZIP spec forbids (APPNOTE 4.4.17.1) and which makes
    # unzip on macOS or Linux spill six literal "folder\file" names into the
    # current directory instead of unpacking one folder. bsdtar ships in Windows
    # 10 and on the runners, and writes conformant entries; -a picks zip from the
    # file extension.
    if (Get-Command tar -ErrorAction SilentlyContinue) {
        & tar -a -cf $archivePath -C $publishDir $stageName
        if ($LASTEXITCODE -ne 0) { throw "tar failed for $archive (exit $LASTEXITCODE)" }
    }
    else {
        Write-Warning 'tar was not found; falling back to Compress-Archive, whose backslash separators break unzip on macOS and Linux. Do not ship this archive.'
        Compress-Archive -Path $stageDir -DestinationPath $archivePath -CompressionLevel Optimal
    }
}
else {
    # tar carries the executable bit through; zip does not. COPYFILE_DISABLE keeps
    # bsdtar on macOS from padding the archive with ._AppleDouble entries.
    $env:COPYFILE_DISABLE = '1'
    & tar -czf $archivePath -C $publishDir $stageName
    Remove-Item Env:\COPYFILE_DISABLE
    if ($LASTEXITCODE -ne 0) { throw "tar failed for $archive (exit $LASTEXITCODE)" }
}
Remove-Item $stageDir -Recurse -Force

$built = Get-Item $archivePath
$hash = (Get-FileHash $archivePath -Algorithm SHA256).Hash.ToLowerInvariant()
Write-Host "`nDone." -ForegroundColor Green
Write-Host ("  {0,-40} {1,8:N1} MB" -f $built.Name, ($built.Length / 1MB))
Write-Host "  sha256 $hash"
