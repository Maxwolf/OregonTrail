// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Location;
using WolfCurses;
using WolfCurses.Utility;
using WolfCurses.Window.Control;

namespace OregonTrailDotNet.Window.Travel.Hunt
{
    /// <summary>
    ///     Represents all of the data related to a hunt where the player wants to kill the prey with bullets and then collect
    ///     their bodies for food. This class manages the generation of prey and sorts them based on how long they should
    ///     appear on the field in descending order. Each one is ticked and then removed from the field as they reach their
    ///     maximum shoot time.
    /// </summary>
    public sealed class HuntManager : ITick
    {
        /// <summary>
        ///     Used to send data to any hooked objects that request to know when a targeted prey item has sensed danger and fled.
        /// </summary>
        /// <param name="target">Prey item which sensed danger and ran away.</param>
        public delegate void TargetFlee(PreyItem target);

        /// <summary>
        ///     Default amount of time that every hunt is given, measured in ticks.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const int HUNTINGTIME = 30;

        /// <summary>
        ///     Determines the maximum number of animals that will be spawned in this area for the player to hunt.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private const int MAXPREY = 15;

        /// <summary>
        ///     Determines the total weight of all the food the player is allowed to take away from a given hunting session.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const int MAXFOOD = 250;

        /// <summary>
        ///     Determines the total number of seconds a given prey item is allowed to be a target by the player, if this value is
        ///     exceeded the animal will sense the player and run away.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const int MAXTARGETINGTIME = 10;

        /// <summary>
        ///     Minimum amount of time that a prey item will be available to shoot by the player if it is selected as a valid
        ///     target.e
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const int MINTARGETINGTIME = 3;

        /// <summary>
        ///     Reference list for all of the prey that was killed by the player using their bullets.
        /// </summary>
        private readonly List<PreyItem> _killedPrey;

        /// <summary>
        ///     Defines a rolling list of all the prey that has escaped death from the players bullets.
        /// </summary>
        private readonly List<PreyItem> _preyEscaped;

        /// <summary>
        ///     Total number of seconds that the player is allowed to hunt, measured in ticks.
        /// </summary>
        private int _secondsRemaining;

        /// <summary>
        ///     Ammunition the party had in the wagon at the moment this hunt began. Used as the full mark for the
        ///     "bullets remaining" meter so it starts full and drains as the player lands shots.
        /// </summary>
        private readonly int _startingBullets;

        /// <summary>
        ///     Running total of bullets actually spent on kills during this hunt, surfaced on the results screen so the
        ///     player can see how much ammunition the outing cost them.
        /// </summary>
        private int _bulletsFired;

        /// <summary>
        ///     List of all the shooting words generated from the get values on hunt word enumeration.
        /// </summary>
        private readonly List<HuntWordEnum> _shootWords;

        /// <summary>
        ///     Reference to all of the created prey in the area which the player will be able to hunt and kill with their bullets.
        /// </summary>
        private List<PreyItem> _sortedPrey;

        /// <summary>
        ///     Sets the target animal which the player has killed if it exists, NULL if no animal has been killed.
        /// </summary>
        private PreyItem _target;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrailDotNet.Window.Travel.Hunt.HuntManager" /> class.
        /// </summary>
        public HuntManager()
        {
            // Clears out any previous killed prey.
            _killedPrey = new List<PreyItem>();
            _sortedPrey = new List<PreyItem>();
            _preyEscaped = new List<PreyItem>();

            // Player has set amount of time in seconds to perform a hunt.
            _secondsRemaining = HUNTINGTIME;

            // Snapshot the ammunition on hand so the hunt HUD can show a bullets-remaining bar that drains as shots land.
            _startingBullets = GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Ammo].Quantity;

            // Grab all of the shooting words from enum that holds them.
            _shootWords = Enum.GetValues(typeof(HuntWordEnum)).Cast<HuntWordEnum>().ToList();

            // Create animals for the player to shoot with their bullets.
            GeneratePrey();
        }

        /// <summary>
        ///     Determines the current hunting word the player needs to type if an animal exists.
        /// </summary>
        public HuntWordEnum ShootingWord { get; private set; }

        /// <summary>
        ///     Renders out a bunch of text that shows all the state data about current hunt.
        /// </summary>
        public string HuntInfo
        {
            get
            {
                // Grab instance of game simulation.
                var game = GameSimulationApp.Instance;

                // Framed status HUD: a titled panel groups the two "how am I doing" meters (daylight left, food bagged
                // against the carry limit) so they read at a glance, then the target and the call-to-action sit below it.
                var locationName = game.Trail.CurrentLocation.Status != LocationStatusEnum.Departed
                    ? $"outside {game.Trail.CurrentLocation.Name}"
                    : $"near {game.Trail.NextLocation.Name}";

                // Reuse the framework progress-bar control rather than hand-rolling bars. Labels are padded to one shared
                // width so every bar's opening bracket lines up in the panel regardless of the label's natural length.
                // Width 20 fits an 80-column window and much narrower ones.
                const int barLabelWidth = 8;
                var daylightBar = new ProgressBar {Width = 20, Label = "Daylight".PadRight(barLabelWidth)}
                    .Render(_secondsRemaining, HUNTINGTIME);
                // Ammunition drains as the player lands shots (each kill costs bullets in TryShoot), so it reads like the
                // daylight clock: starts full at whatever the party set out with and ticks down toward empty.
                var bulletsBar = new ProgressBar {Width = 20, Label = "Bullets".PadRight(barLabelWidth)}
                    .Render(BulletsRemaining, StartingBullets);
                var foodBar = new ProgressBar {Width = 20, Label = "Food bag".PadRight(barLabelWidth)}
                    .Render(KillWeight, MAXFOOD);

                var panel = new StringBuilder();
                panel.AppendLine(daylightBar);
                panel.AppendLine(bulletsBar);
                // Indent the raw counts so they line up under the bars' brackets (label width + the control's own space).
                panel.AppendLine($"{new string(' ', barLabelWidth + 1)}{BulletsRemaining} / {StartingBullets} bullets");
                panel.AppendLine(foodBar);
                panel.AppendLine($"{new string(' ', barLabelWidth + 1)}{KillWeight} / {MAXFOOD} lb");
                panel.Append($"Weather: {game.Trail.CurrentLocation.Weather.ToDescriptionAttribute()}");

                var huntStatus = new StringBuilder();
                huntStatus.AppendLine();
                huntStatus.AppendLine(FramedPanel.Render($"HUNTING · {locationName}", panel.ToString()));
                huntStatus.AppendLine();

                // Below the panel: either the current target (with its rising-awareness bar and a shoot-or-not caption)
                // or an idle "nothing in your sights" note.
                if (_target != null)
                {
                    huntStatus.AppendLine($"  A {_target.Animal.Name.ToUpperInvariant()} breaks cover!");

                    // Awareness climbs toward the animal spooking; surface it as a bar tied to the same values HuntInfo
                    // used to print as a bare percentage.
                    var awarenessBar = new ProgressBar {Width = 20, Label = "Awareness"}
                        .Render(_target.TargetTime, _target.TargetTimeMax);
                    huntStatus.AppendLine($"  {awarenessBar}");

                    // A shot fired after awareness passes the halfway mark always misses (see TryShoot), so the caption
                    // flips at that exact threshold to tell the player when the clean-shot window has closed.
                    huntStatus.AppendLine(_target.TargetTime <= _target.TargetTimeMax/2
                        ? "  Steady — take the shot!"
                        : "  Spooked — it may bolt!");
                    huntStatus.AppendLine();

                    // BOT CONTRACT: the literal "Type the word '<word>'" shape is scraped by the headless training bot
                    // (ScreenRecognizer.TypeWordRx in bot/OregonTrailDotNet.Bot/Game/ScreenRecognizer.cs) to learn which
                    // word to type. Keep the exact "Type the word '...'" form — capital T, single quotes, one word — or
                    // update that regex in tandem, otherwise the bot silently stops hunting.
                    huntStatus.Append($"  ►  Type the word '{ShootingWord.ToString().ToLowerInvariant()}' to shoot!");
                }
                else
                {
                    // Read out "animal" for one, "animals" for none or many.
                    var animalText = _sortedPrey.Count == 1 ? "animal" : "animals";
                    huntStatus.AppendLine("  No animal in your sights yet —");
                    huntStatus.Append($"  you sense {_sortedPrey.Count:N0} {animalText} nearby. Stay ready.");
                }

                return huntStatus.ToString();
            }
        }

        /// <summary>
        ///     Reference dictionary for all the animals in the game, used to help hunting mode determine what types of animals
        ///     will spawn when the player is out looking for them.
        /// </summary>
        internal static IList<SimItem> DefaultAnimals
        {
            get
            {
                // Create inventory of items with default starting amounts.
                var defaultAnimals = new List<SimItem>
                {
                    Animals.Bear,
                    Animals.Buffalo,
                    Animals.Caribou,
                    Animals.Deer,
                    Animals.Duck,
                    Animals.Goose,
                    Animals.Rabbit,
                    Animals.Squirrel
                };

                // Zero out all of the quantities by removing their max quantity.
                foreach (var animal in defaultAnimals)
                    animal.ReduceQuantity(animal.MaxQuantity);

                // Now we have default animals for hunting with all quantities zeroed out.
                return defaultAnimals;
            }
        }

        /// <summary>
        ///     Determines if the hunt currently has a animal prey on the field available for the player to kill.
        /// </summary>
        public bool PreyAvailable => ShootingWord != HuntWordEnum.None;

        /// <summary>
        ///     Determines if the hunting session is over and the results form should be displayed.
        /// </summary>
        public bool ShouldEndHunt => _secondsRemaining <= 0;

        /// <summary>
        ///     Calculates the total weight of all the killed prey targets.
        /// </summary>
        public int KillWeight
        {
            get
            {
                // Skip if there are no prey items.
                if (_killedPrey.Count <= 0)
                    return 0;

                // Loop through every killed prey and tabulate total weight.
                var totalWeight = 0;
                foreach (var preyItem in _killedPrey)
                    totalWeight += preyItem.Animal.TotalWeight;

                return totalWeight;
            }
        }

        /// <summary>
        ///     Number of prey the player successfully bagged during this hunt.
        /// </summary>
        public int KillCount => _killedPrey.Count;

        /// <summary>
        ///     Ammunition the party set out with when this hunt began — the full mark for the bullets-remaining meter.
        /// </summary>
        public int StartingBullets => _startingBullets;

        /// <summary>
        ///     Ammunition still in the wagon right now, read live from inventory so it drops the instant a shot lands.
        /// </summary>
        public int BulletsRemaining => GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Ammo].Quantity;

        /// <summary>
        ///     Total bullets spent on kills over the course of this hunt, shown on the results screen.
        /// </summary>
        public int BulletsFired => _bulletsFired;

        /// <summary>
        ///     Gets the last known prey that became aware of the hunter and fled the hunting grounds.
        /// </summary>
        public PreyItem LastEscapee => _preyEscaped.LastOrDefault();

        /// <summary>
        ///     Gets the last known killed prey item from the list of killed prey.
        /// </summary>
        public PreyItem LastTarget => _killedPrey.LastOrDefault();

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public void OnTick(bool systemTick, bool skipDay)
        {
            // No work is done on system ticks.
            if (systemTick)
                return;

            // No work is done if force ticked.
            if (skipDay)
                return;

            // Check if we are still allowed to hunt.
            if (_secondsRemaining <= 0)
                return;

            // Remove one (1) second from the total remaining hunting time.
            _secondsRemaining--;

            // Increments timer on targets prey increasing the chance they will run away.
            TickTargetAwareness();

            // Advances the lifetime of each prey object in the list.
            TickPrey();

            // Pick a random shooting word, and if not none, an animal for prey target.
            TryPickPrey();
        }

        /// <summary>
        ///     Removes prey that has exceeded their total lifetime in seconds.
        /// </summary>
        private void TickPrey()
        {
            // Loop through every sorted prey and check lifetime.
            var copyPrey = new List<PreyItem>(_sortedPrey);
            foreach (var prey in copyPrey)
                if (prey.Lifetime >= prey.LifetimeMax)
                    _sortedPrey.Remove(prey);
                else
                    prey.OnTick(false, false);

            // Cleanup copied list of prey for iteration.
            copyPrey.Clear();
        }

        /// <summary>
        ///     Ticks the target prey if there is any selected.
        /// </summary>
        /// <returns>FALSE if the target prey ran away, TRUE if everything is normal.</returns>
        private void TickTargetAwareness()
        {
            // Check if there is a target at all to tick.
            if (_target == null)
                return;

            // There is a change we will not tick awareness this time.
            if (GameSimulationApp.Instance.Random.NextBool())
                return;

            // Check target ticking if not null and shooting word not none.
            if ((_target != null) && (ShootingWord != HuntWordEnum.None))
                _target.TickTarget();

            // Check if the target wants to run away from the hunter.
            if (!_target.ShouldRunAway)
                return;

            // Add the prey to list of things that got away.
            _preyEscaped.Add(new PreyItem(_target));

            // Fire event that hunting mode will attach form in response to.
            TargetFledEvent?.Invoke(_target);

            // Reset target and shooting word.
            ClearTarget();
        }

        /// <summary>
        ///     Clears the currently targeted prey item from the hunting form, also clears out associated shooting word used to
        ///     kill it. Finally it will clear the input buffer.
        /// </summary>
        private void ClearTarget()
        {
            // Clear the target.
            _target = null;

            // Set the shooting word back to none.
            ShootingWord = HuntWordEnum.None;

            // Clear the input buffer.
            GameSimulationApp.Instance.InputManager.ClearBuffer();
        }

        /// <summary>
        ///     Selects a random shooting word from the enumeration of values, if the value is not none it will select a random
        ///     animal to be used as prey from the list of prey still on the field. If there are no animals to use for prey, the
        ///     shooting word is just reset to none.
        /// </summary>
        private void TryPickPrey()
        {
            // Skip if we already have a target.
            if (_target != null)
                return;

            // Check if there is any prey we are currently hunting.
            if (_sortedPrey.Count <= 0)
                return;

            // There is a chance that you will not get prey this tick.
            if (GameSimulationApp.Instance.Random.NextBool())
                return;

            // Randomly select one of the hunting words from the list.
            var tempShootWord = (HuntWordEnum) GameSimulationApp.Instance.Random.Next(_shootWords.Count);

            // Check if we are already trying to hunt a particular animal.
            if ((tempShootWord == HuntWordEnum.None) || (tempShootWord == ShootingWord))
                return;

            // Set the shooting word to the one we have now verified. 
            ShootingWord = tempShootWord;

            // Randomly select one of the prey from the list.
            var randomPreyIndex = GameSimulationApp.Instance.Random.Next(_sortedPrey.Count);
            var randomPrey = _sortedPrey[randomPreyIndex];

            // Check the prey to make sure it is still alive.
            if (randomPrey.Lifetime > randomPrey.LifetimeMax)
                return;

            // Set the verified prey as hunting target.
            _target = new PreyItem(randomPrey);

            // Remove the old prey from the list now that it is a target.
            _sortedPrey.Remove(randomPrey);
        }

        /// <summary>
        ///     Generate random number of animals to occupy this area.
        /// </summary>
        private void GeneratePrey()
        {
            // Check to make sure spawn count is above zero.
            var preySpawnCount = GameSimulationApp.Instance.Random.Next(MAXPREY);
            if (preySpawnCount <= 0)
                return;

            // Create the number of prey required by the dice roll.
            var unsortedPrey = new List<PreyItem>();
            for (var i = 0; i < preySpawnCount; i++)
                unsortedPrey.Add(new PreyItem());

            // Sort the list of references in memory without creating duplicate objects.
            _sortedPrey = unsortedPrey.OrderByDescending(o => o.LifetimeMax).Distinct().ToList();
        }

        /// <summary>
        ///     Called when the target prey item senses danger of the hunter and runs away.
        /// </summary>
        public event TargetFlee TargetFledEvent;

        /// <summary>
        ///     Determines if the player was able to successfully shoot an animal. Depending on how long it takes them to type the
        ///     shooting word correctly, and a roll of the dice will determine if they hit their mark or not.
        /// </summary>
        /// <returns>Prey item if player shot an animal, NULL if the player missed.</returns>
        public bool TryShoot()
        {
            // Skip there is no valid target to shoot at.
            if (_target == null)
                return false;

            // Grab game instance to make check logic legible.
            var game = GameSimulationApp.Instance;

            // Check if the player outright missed their target. The chance to miss (as a percent) grows the longer the
            // animal has sensed the hunter and is higher for a less capable hunter - a banker (profession 1) misses
            // three times as often as a farmer (profession 3) for the same shot. A snap shot (TargetTime 0) never misses
            // here. The old test compared 100*Random.Next() against ((int)Profession - 13)*TargetTime, which is always
            // <= 0, so the branch was dead code and profession had no effect on hunting at all.
            if (game.Random.Next(100) < (4 - (int) game.Vehicle.PassengerLeader.Profession)*_target.TargetTime)
            {
                _preyEscaped.Add(_target);
                ClearTarget();
                return false;
            }

            // Check if player fired in less than half the maximum target time for this prey.
            if (_target.TargetTime > _target.TargetTimeMax/2)
            {
                _preyEscaped.Add(_target);
                ClearTarget();
                return false;
            }

            // Each successful shot consumes 10-13 bullets, mirroring the original game's "B = B - 10 - INT(RND*4)". The
            // amount is always positive, so bagging an animal can only ever reduce ammunition, never add it back.
            var bulletCost = 10 + game.Random.Next(0, 4);

            // Remove the amount of bullets from vehicle inventory, and tally what was actually taken. ReduceQuantity floors
            // at zero, so on a near-empty wagon fewer than bulletCost bullets may leave — count the real delta so the
            // "bullets fired" total on the results screen always equals the ammunition the hunt genuinely spent.
            var ammo = game.Vehicle.Inventory[EntitiesEnum.Ammo];
            var ammoBefore = ammo.Quantity;
            ammo.ReduceQuantity(bulletCost);
            _bulletsFired += ammoBefore - ammo.Quantity;

            // Add the target to the list of animals that have been killed.
            _killedPrey.Add(_target);

            // Record the kill against the vehicle's lifetime tally so later river-crossing trades know how much the party has
            // hunted. The fewer animals killed, the better the deal the Shoshoni guide offers for clothing.
            game.Vehicle.IncrementAnimalKillCount();

            // Resets the targeting system now that the animal is bagged and tagged.
            ClearTarget();
            return true;
        }
    }
}