# Oregon Trail Clone #

Clone of popular 90's computer game for C#.

![Oregon Trail Main Menu](https://raw.githubusercontent.com/Maxwolf/OregonTrail/master/media/TitleScreen.jpg)

## Download and Play ##

Ready-to-run builds are attached to every [release](https://github.com/Maxwolf/OregonTrail/releases/latest).
There is nothing to install and no .NET runtime to fetch first — each archive holds one self-contained
executable with the runtime, the original MECC artwork, and the chiptunes all bundled inside.

| Your machine | File |
| --- | --- |
| Windows 10/11, Intel or AMD | `OregonTrail-<version>-win-x64.zip` |
| Windows on ARM | `OregonTrail-<version>-win-arm64.zip` |
| Mac with Apple silicon (M1 and later) | `OregonTrail-<version>-osx-arm64.tar.gz` |
| Mac with an Intel processor | `OregonTrail-<version>-osx-x64.tar.gz` |
| Linux, Intel or AMD | `OregonTrail-<version>-linux-x64.tar.gz` |
| Linux on ARM (64-bit, e.g. Raspberry Pi 4/5) | `OregonTrail-<version>-linux-arm64.tar.gz` |

Unpack it, read the `FIRST-RUN.txt` inside, and run `OregonTrailDotNet`. Give it a terminal at least
80 columns by 25 rows — macOS Terminal and most Linux terminals open one row short of that, and the
game silently drops whatever does not fit. The artwork draws on all three platforms; music and sound
effects are Windows-only, because the audio stack talks to Windows' own waveOut device.

Versions are `year.month.day.hour` stamps in UTC, so `2026.7.29.17` was built at 17:00 UTC on the
29th of July 2026.

## Cloning Instructions ##

```cmd
git clone https://github.com/Maxwolf/OregonTrail.git
```

## Compilation Instructions ##

The solution builds with the standard .NET CLI on any platform:

```cmd
dotnet build OregonTrailDotNet.sln
dotnet run --project src/OregonTrailDotNet
dotnet test OregonTrailDotNet.sln
```

`./publish.ps1` produces self-contained single-file executables (game, minigame workbench, and training
bot) in the repo-root `publish` folder. It targets win-x64 by default; `-Rid` retargets it at any of the
six platforms above, and `-Package` stages them into the same archive a release ships:

```cmd
./publish.ps1
./publish.ps1 -Rid linux-arm64 -Version 2026.7.29.17 -Package
```

Releases themselves are built by `.github/workflows/release.yml`, which runs that same script once per
platform. Pushing a `v`-prefixed version tag builds all six and publishes them:

```cmd
git tag v2026.7.29.17
git push origin v2026.7.29.17
```

The workflow can also be started by hand from the Actions tab, where it stamps the current UTC hour
and only publishes a release if asked to.

## Simulation Features ##

The list below describes how **this clone** actually behaves. Where the clone deliberately
reimplements a rule from the original 1980s/1990s game differently, that is called out with a
*(differs from original: …)* note.

### Travel ###
 1. The trip is simulated one day at a time; each day the vehicle advances toward the next
   landmark until it is reached *(differs from original, which used fixed two-week segments)*
 2. A turn counter increments once per simulated day; there is no fixed 18-turn cap
   *(differs from original's up-to-18 two-week turns)*
 3. There is no time limit: like the 1985 game (which let a party idle for years and still finish),
   only reaching Oregon or losing the whole party ends the trip. The 246-day figure survives solely
   as the training bot's pacing horizon
 4. Each day's ideal mileage is calculated from the value of the party's oxen plus a small random
   amount *(differs from original's per-two-week ~200 mile projection)*
 5. Mileage is an ideal figure; problems (dead/wandering oxen, floods, fog, hail, illness) subtract
   from it, floored so the wagon always makes at least a little progress
 6. The travel screen continuously shows miles traveled (odometer) and distance to the next landmark
 7. Daily mileage is driven by the oxen-value formula rather than a fixed weekly average
   *(differs from original's ~75 miles/week)*
 8. The going gets harder later in the trip: later locations use harsher climates, and high mountain
   passes apply a slow-going mileage penalty
 9. Weather changes day to day; carrying fewer sets of clothing than there are party members raises
   the chance of illness
 10. Stopping at a fort dramatically reduces the miles covered on the very next travel turn
 11. Bad weather is driven by whether a random daily temperature falls at/below the month's average
   for the region *(differs from original's flat 20%)*
 12. Injuries (broken arm, concussion, sprains) occur as random Person-category events
   *(differs from original's flat 5%)*
 13. Random events are selected by context (weather while moving, vehicle events while traveling,
   river events at crossings, cave-ins/blizzards in the high country)
 14. Snow appears through the cold-weather climate system, river disasters occur at river crossings,
   and blizzards occur at high elevations
 15. Every price is marked up by a quarter of its base at each of six fort thresholds along the
   trail, topping out at 2.5x base; the markup follows trail position, so a fork that skips a
   fort does not skip the price rise
 16. In dollars, each threshold adds 2.50 to a set of clothing and to each wagon part, 0.50 to a
   box of ammunition, 0.05 to a pound of food, and 10.00 to a yoke of oxen
 17. Goods are sold in the units the store quotes: at Matt's in Independence oxen go by the yoke of
   two at 40 dollars and ammunition by the box of twenty bullets at 2 dollars, with food by the
   pound and clothing by the set; the forts out on the trail sell oxen singly at 20 dollars
 18. Each quantity prompt is a fixed-width field, which is what caps a single purchase: at Matt's
   nine yoke of oxen, ninety-nine boxes of ammunition, ninety-nine sets of clothes, nine of any
   spare part; the forts allow three digits, and four for food
 19. Money is kept to the cent, so a pound of food really costs twenty cents; points are awarded on
   whole dollars carried into Oregon
 20. Resting at a landmark lets sick or injured party members recover (quickly if the party carries
   medical supplies, slowly otherwise)
 21. Locations carry a fresh-water flag; a bad-water location doubles the daily chance of contracting
   dysentery or cholera
 22. The maximum weight of food that can be carried back from a single hunt is 100 lbs
 23. The fewer animals you kill while hunting, the cheaper the Shoshoni river guide's price in clothing
 24. A river configured for an Indian guide will ferry the wagon across for a base of 1-5 sets of
   clothing (rising with the number of animals killed)

### Hunting ###
 1. Hunting is the original's real-time field hunt: the hunter walks a scrolling field and animals
   wander through, with the roster (bison, antlered deer, bear, small game) gated by how far down
   the trail the party is
 2. Aim with the arrow keys (or the ring of keys around L, or the numpad); SPACE fires one shot,
   and each shot costs exactly one bullet
 3. RETURN toggles walking in the aim direction; ESC ends the hunt early keeping the bag; the hunt
   otherwise runs out its timer
 4. Dressed meat is halved on the walk back, zeroed if the wagon is already full, clamped to the
   space left, and capped at 100 lbs
 5. There is one hunt and every host plays it. The training bot and the test suites run the same
   field hunt on the same simulation, aiming and firing with the same keys; only the drawing of it
   is skipped when nobody is watching

### Eating ###
 1. Food consumption in pounds is calculated from the ration level each day (a higher ration level
   consumes more food)

### Random Events ###
 1. A 0-99 dice roll gates whether a category event fires on a given tick (fires on a roll of 0)
 2. Which event fires is chosen by cumulative per-event probability weights declared on each event
 3. Events are laid out on a cumulative number line (e.g. 0-6=eventA, 6-11=eventB, …) and a single
   roll selects one; equal weights reproduce a uniform pick
 4. An event typically prints a message and can subtract mileage and destroy supplies
 5. More complex events (bandits, wild-animal attacks, weather, illness) combine several effects such
   as item loss, ammo consumption, and passenger death

### Climate ###
 1. Illness risk is checked against the party's clothing count versus the number of living members
 2. Insufficient clothing for the party triggers the illness routine
 3. The illness routine is also driven by poor eating (Meager or Bare Bones rations)

### Illness ###
 1. Checks how well the party has been eating (via ration level)
 2. There is a chance to contract a mild, a bad (moderate), or a very serious illness
 3. Mild and bad illnesses can be shrugged off; a very serious illness leaves the person infected and
   needs medical supplies (or lengthy rest) to recover

### Mountains ###
 1. Higher elevations risk cave-ins, losing your way, and slow going
 2. 80% chance of getting stuck when departing South Pass
 3. 70% chance of getting stuck when departing the Blue Mountains
 4. Being stuck in the mountains never lasts more than 10 days
 5. At high elevations, storms are blizzards 90% of the time

### Death ###
 1. Death can come from running out of food (starvation), lack of clothing (illness), or being sick
   with no medical supplies; running out of both food and ammunition accelerates starvation
 2. A short message tells you the cause of death
 3. How far you traveled and the supplies you had left are shown on the death screen
 4. Historically fewer than 50% (realistically ~20%) of emigrants completed the journey (flavor only,
   not modeled in code)

### Winning ###
 1. Displays the total time and distance of the journey
 2. Remaining supplies are shown
 3. Scoring matches the 1985 Apple II game, verified against its decompiled disk: 500/400/300/200
   points per survivor by party health, 50 for the wagon, 4 per ox (cap 20), 2 per spare part
   (cap 3 of each of 3 types), 2 per set of clothing (cap 255), 1 per 50 bullets (cap 65,535),
   1 per 25 lb of food (cap 2,000), 1 per $5 cash; floored per line, then multiplied x1/x2/x3 for
   Banker/Carpenter/Farmer. Ratings: Trail Guide 6000+, Adventurer 3000+, Greenhorn below
 4. The highest possible score is 13,860: a farmer party of five arriving in good health with every
   capped item maxed and $360 of the $400 stake unspent (the $40 minimum is the required yoke of
   oxen). The clothing and bullet caps mirror the original's endgame memory-handoff limits