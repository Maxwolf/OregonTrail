// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/27/2015@4:20 AM

namespace TrailSimulation.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core;
    using Entity;

    /// <summary>
    ///     Represents all of the data related to a hunt where the player wants to kill the prey with bullets and then collect
    ///     their bodies for food. This class manages the generation of prey and sorts them based on how long they should
    ///     appear on the field in descending order. Each one is ticked and then removed from the field as they reach their
    ///     maximum shoot time.
    /// </summary>
    public sealed class HuntManager : ITick
    {
        /// <summary>
        ///     Default amount of time that every hunt is given, measured in ticks.
        /// </summary>
        public const int HUNTINGTIME = 30;

        /// <summary>
        ///     Determines the maximum number of animals that will be spawned in this area for the player to hunt.
        /// </summary>
        private const int MAXPREY = 15;

        /// <summary>
        ///     Total number of seconds that the player is allowed to hunt, measured in ticks.
        /// </summary>
        private int _secondsRemaining;

        /// <summary>
        ///     Determines the current hunting word the player needs to type if an animal exists.
        /// </summary>
        public HuntWord ShootingWord { get; private set; }

        /// <summary>
        ///     List of all the shooting words generated from the get values on hunt word enumeration.
        /// </summary>
        private List<HuntWord> _shootWords;

        /// <summary>
        ///     Reference to all of the created prey in the area which the player will be able to hunt and kill with their bullets.
        /// </summary>
        private List<PreyItem> _sortedPrey;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public HuntManager()
        {
            // Player has thirty (30) seconds to perform a hunt.
            _secondsRemaining = HUNTINGTIME;

            // Grab all of the shooting words from enum that holds them.
            _shootWords = Enum.GetValues(typeof (HuntWord)).Cast<HuntWord>().ToList();

            // Create animals for the player to shoot with their bullets.
            GeneratePrey();
        }

        /// <summary>
        ///     Renders out a bunch of text that shows all the state data about current hunt.
        /// </summary>
        public string HuntInfo
        {
            get
            {
                // Grab instance of game simulation.
                var game = GameSimulationApp.Instance;

                // Build up the status for the vehicle as it moves through the simulation.
                var huntStatus = new StringBuilder();
                huntStatus.AppendLine("--------------------------------");
                huntStatus.AppendLine($"Time Remaining: {_secondsRemaining.ToString("N0")} seconds");
                huntStatus.AppendLine($"Weather: {game.Trail.CurrentLocation.Weather.ToDescriptionAttribute()}");
                huntStatus.AppendLine($"Prey: {_sortedPrey.Count.ToString("N0")} animals");
                huntStatus.AppendLine($"Shooting Word: {ShootingWord.ToString().ToUpperInvariant()}");
                huntStatus.AppendLine("--------------------------------");
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
                {
                    animal.ReduceQuantity(animal.MaxQuantity);
                }

                // Now we have default animals for hunting with all quantities zeroed out.
                return defaultAnimals;
            }
        }

        /// <summary>
        ///     Determines if the hunt currently has a animal prey on the field available for the player to kill.
        /// </summary>
        public bool PreyAvailable
        {
            get { return ShootingWord != HuntWord.None; }
        }

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

            // Check if there is any prey we are currently hunting.
            if (_sortedPrey.Count <= 0)
                return;

            // Randomly select one of the hunting words from the list.
            ShootingWord = (HuntWord) GameSimulationApp.Instance.Random.Next(_shootWords.Count);

            // Check if the selected value is none.
            if (ShootingWord == HuntWord.None)
                return;
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
            {
                unsortedPrey.Add(new PreyItem());
            }

            // Sort the list of references in memory without creating duplicate objects.
            _sortedPrey = unsortedPrey.OrderByDescending(o => o.ShootTimeMax).Distinct().ToList();
        }
    }
}