#!/usr/bin/env pwsh
#Requires -Version 5.1
<#
.SYNOPSIS
    Publishes the game, minigames, and bot as self-contained, single-file
    Windows executables into the repo-root "publish" folder.

.DESCRIPTION
    Each project carries a "SelfContained" publish profile
    (Properties/PublishProfiles/SelfContained.pubxml) that produces one
    win-x64 self-contained single-file exe with the runtime, dependencies,
    and native SQLite bundled in. All three land in the same ./publish folder,
    which is created if it does not exist.

.PARAMETER Clean
    Delete the publish folder before publishing, so no stale files linger.

.EXAMPLE
    ./publish.ps1
    ./publish.ps1 -Clean
#>
[CmdletBinding()]
param(
    [switch]$Clean
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$publishDir = Join-Path $root 'publish'

if ($Clean -and (Test-Path $publishDir)) {
    Write-Host "Cleaning $publishDir ..." -ForegroundColor Yellow
    Remove-Item $publishDir -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $publishDir | Out-Null

# The three executables that ship. Each uses its own SelfContained profile,
# which anchors PublishDir back to this same ./publish folder.
$projects = @(
    'src\OregonTrailDotNet\OregonTrailDotNet.csproj',
    'src\OregonTrailDotNet.Minigames\OregonTrailDotNet.Minigames.csproj',
    'bot\OregonTrailDotNet.Bot\OregonTrailDotNet.Bot.csproj'
)

foreach ($proj in $projects) {
    $full = Join-Path $root $proj
    Write-Host "`nPublishing $proj ..." -ForegroundColor Cyan
    dotnet publish $full -c Release -p:PublishProfile=SelfContained --nologo
    if ($LASTEXITCODE -ne 0) { throw "Publish failed for $proj (exit $LASTEXITCODE)" }
}

Write-Host "`nDone. Self-contained executables in $publishDir :" -ForegroundColor Green
Get-ChildItem $publishDir -Filter *.exe |
    Sort-Object Name |
    ForEach-Object { Write-Host ("  {0,-34} {1,8:N1} MB" -f $_.Name, ($_.Length / 1MB)) }
