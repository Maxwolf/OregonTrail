Oregon Trail Clone **__VERSION__**, built __BUILT__ from `__SHA__`.

Pick the archive for your machine, unpack it, and run `OregonTrailDotNet`. Every build is
self-contained: the .NET runtime, the original 1990 MECC artwork, and the chiptunes all ride inside
the executable, so there is nothing to install and no runtime to fetch first.

| Your machine | Download |
| --- | --- |
| Windows 10/11, Intel or AMD | `OregonTrail-__VERSION__-win-x64.zip` |
| Windows on ARM (Snapdragon, Surface Pro X) | `OregonTrail-__VERSION__-win-arm64.zip` |
| Mac with Apple silicon (M1 and later) | `OregonTrail-__VERSION__-osx-arm64.tar.gz` |
| Mac with an Intel processor | `OregonTrail-__VERSION__-osx-x64.tar.gz` |
| Linux, Intel or AMD | `OregonTrail-__VERSION__-linux-x64.tar.gz` |
| Linux on ARM (64-bit, e.g. Raspberry Pi 4/5) | `OregonTrail-__VERSION__-linux-arm64.tar.gz` |

## First run ##

**Windows** - unzip and double-click `OregonTrailDotNet.exe`. Nothing here is code-signed, so
SmartScreen may object once: *More info* then *Run anyway*.

**macOS** - unpack and run it from Terminal. macOS quarantines browser downloads and this build has
no Apple developer signature, so clear the flag the first time:

```sh
tar -xzf OregonTrail-__VERSION__-osx-arm64.tar.gz
cd OregonTrail-__VERSION__-osx-arm64
xattr -d com.apple.quarantine ./OregonTrailDotNet
./OregonTrailDotNet
```

**Linux** - unpack and run it. The archive keeps the executable bit, so there is nothing to chmod:

```sh
tar -xzf OregonTrail-__VERSION__-linux-x64.tar.gz
cd OregonTrail-__VERSION__-linux-x64
./OregonTrailDotNet
```

Give it a terminal at least 80 columns by 25 rows. macOS Terminal and most Linux terminals open at
80x24, one row short, and the game silently drops whatever does not fit - menu options included - so
stretch the window before you start. Each archive carries a `FIRST-RUN.txt` repeating all of this.

## What is in the archive ##

| File | What it is |
| --- | --- |
| `OregonTrailDotNet` | the game |
| `OregonTrailDotNet.Minigames` | standalone workbench for the hunt, the Columbia raft, the river crossings, and the map |
| `OregonTrailDotNet.Bot` | the headless training bot that plays the game by itself |

Only the first one is needed to play.

## Worth knowing ##

* The artwork, the music and the sound effects all work on all three platforms - waveOut on Windows,
  Core Audio on macOS, ALSA on Linux. On Linux sound needs `libasound` (the `libasound2` or
  `alsa-lib` package) - it plays through your sound server if you have one and straight to the ALSA
  card if you do not. With no `libasound` and no card, the game runs the same, silently.
* High scores and tombstones are written to `game.db` beside the executable, so unpack it somewhere
  you can write to.
* Version numbers are `year.month.day.hour` stamps in UTC, so a release says out loud when it was
  built and how old it is.
* To check a download, grab `SHA256SUMS.txt` from the assets below. It lists all six archives, so on
  macOS or Linux ask it to check only the one you actually have - plain `-c` reports the five missing
  files as failures:

  ```sh
  shasum -a 256 --ignore-missing -c SHA256SUMS.txt
  ```

  On Windows, `Get-FileHash <file> -Algorithm SHA256` and compare the hash to its line in the file.
