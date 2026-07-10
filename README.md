# Oregon Trail Clone #

Clone of popular 90's computer game for C#.

![Oregon Trail Main Menu](https://raw.githubusercontent.com/Maxwolf/OregonTrail/master/media/TitleScreen.jpg)

## Cloning Instructions ##

```cmd
git clone --recursive https://github.com/Maxwolf/OregonTrail.git
```

Make sure your git client recursively grabs all the sub-modules for the repo. Most Git GUI's (e.g, SourceTree, SmartGit, GitEye) will all do this automatically for you. 

## Compilation Instructions ##

You *should* be able to run the Cake build script by invoking the bootstrapper with a script tailored to the target platform.

### Windows ###

```cmd
build.bat
```

If script execution fail due to the execution policy, you might have to tell PowerShell to allow running scripts. You do this by [changing the execution policy](https://technet.microsoft.com/en-us/library/ee176961.aspx).

### Linux/OS X ###

```bash
bash build.sh
```

## Simulation Features ##

The list below describes how **this clone** actually behaves. Where the clone deliberately
reimplements a rule from the original 1980s/1990s game differently, that is called out with a
*(differs from original: …)* note.

### Travel ###
 1. The trip is simulated one day at a time; each day the vehicle advances toward the next
   landmark until it is reached *(differs from original, which used fixed two-week segments)*
 2. A turn counter increments once per simulated day; there is no fixed 18-turn cap
   *(differs from original's up-to-18 two-week turns)*
 3. Reaching 246 days (20+ weeks) on the trail forces the end-game routine even if the party has
   not yet reached Oregon
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
 15. Clothing and wagon parts cost 2.5 dollars more at each fort further down the trail
 16. Food costs 0.10 dollars more and bullets 2.5 dollars more at each fort
 17. Oxen cost 5 dollars more at each fort
 18. Resting at a landmark lets sick or injured party members recover (quickly if the party carries
   medical supplies, slowly otherwise)
 19. Locations carry a fresh-water flag; a bad-water location doubles the daily chance of contracting
   dysentery or cholera
 20. The maximum weight of food that can be carried back from a single hunt is 250 lbs
 21. The fewer animals you kill while hunting, the cheaper the Shoshoni river guide's price in clothing
 22. A river configured for an Indian guide will ferry the wagon across for a base of 1-5 sets of
   clothing (rising with the number of animals killed)

### Hunting ###
 1. A random shooting word is selected for the player to type
 2. Each animal is given a randomized on-screen "targeting" lifetime measured in simulation ticks
   *(differs from original's real-clock start time)*
 3. The player must type the correct shooting word while an animal is targeted
 4. Reaction speed is measured by how many ticks the animal has been targeted
   *(differs from original's end-minus-start-time subtraction)*
 5. Firing within the first half of the animal's targeting lifetime is a good (successful) shot
   *(differs from original's fixed 2-second threshold)*
 6. Ammunition is consumed on each kill, based on current ammo and a random factor
   *(differs from original's shoot-time-based consumption)*

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
 4. Historically fewer than 50% (realistically ~20%) of emigrants completed the journey — flavor only,
   not modeled in code

### Winning ###
 1. Displays the total time and distance of the journey
 2. Remaining supplies are shown