# Oregon Trail Clone #

Clone of popular 90's computer game for C#.

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

### Travel ###
 1. Iterates through 2-week trip segments
 2. Play then proceeds through a series of up to 18 turns 
   (each representing two weeks)
 3. 20+ weeks (246 days) will trigger end game routine
 4. Expected mileage for next two-weeks calculated with current stats
 5. Mileage figure is ideal, problems subtract from this figure
 6. Mileage printed at start of next trip segment
 7. Average mileage is about 75 miles a week
 8. The going gets slower and harder later in the trip
 9. Weather changes, requiring more clothing
 10. Stopping at a fort for supplies dramatically reduces 
   the miles you can cover in a single turn
 11. Bad weather occurs 20% of the time
 12. Injuries 5% of the time
 13. Detects where the player is on the trail and adjust random events
 14. Snows in the mountains and river disasters occur on the plains
 15. Clothes and wagon parts increase 2.5 dollars
 16. Food increases .10 dollars and bullets increase 2.5 dollars
 17. Oxen go up 5 dollars at each fort
 18. When you rest at a land mark people often heal quicker than on the trail
 19. Locations have fresh water flag if enabled doubles change for dysentery
   and cholera
 20. Maximum amount of weight that can be carried back after a hunt
   is 250 lbs of food
 21. The less buffalo you kill the better deal you shall receive
   from Indian wanting clothes
 22. Random chance for Indian Guide to help cross river for 1-5 sets of clothes
 
### Hunting ###
 1. Random shooting word selected
 2. Date is taken for hunting start time
 3. Wait for correct shooting word input
 4. Subtract start time from end time
 5. If 2 seconds or less good shot, longer bad
 6. Ammunition consumption calculated from shoot time

### Eating ###
 1. Food consumption in pounds calculated from ration level per day

### Random Events ###
 1. Select random number from 0 to 100 that will be dice roll
 2. Probability determined by sucessive numbers in array
 3. 0-6=event1, 6-11=event2, 11-13=event3, and so on...
 4. Event typically prints message, subtracts mileage, and supplies
 5. Events cold weather, bandits, wild-animal attack, illness
 are more complicated

### Climate ###
 1. Check if set of clothing for every member in party
 2. If not enough clothes illness routine is called
 3. Illness routine can also be called if starvation flag is set

### Illness ###
 1. Check how well the player has been eating
 2. If bad chance to contract mild, bad, or very serious illness
 3. Mild and serious can be ignored, the others medical services

### Mountains ###
 1. Higher elevations risk cave-ins, losing your way, and slow going
 2. 80% chance of getting stuck in South Pass
 3. 70% chance of getting stuck in Blue Mountains
 4. Will never be stuck for more than 10 days
 5. 90% chance for blizzard at high elevations

### Death ###
 1. Not enough food, clothing, ammunition, or medical supplies
 2. Short message is displayed telling you what happened
 3. How far you traveled is shown, remaining supplies
 4. Assumed less than 50% made it, realistic 20%

### Winning ###
 1. Display total time of journey
 2. Remaining supplies shown, if any