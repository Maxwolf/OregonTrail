// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/25/2015@7:56 PM

namespace TrailSimulation.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core;
    using Entity;

    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class Hunting : Form<TravelInfo>
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
        ///     Representation of hunting form for text user interface.
        /// </summary>
        private StringBuilder _huntPrompt;

        /// <summary>
        ///     Total number of seconds that the player is allowed to hunt, measured in ticks.
        /// </summary>
        private int _secondsRemaining;

        /// <summary>
        ///     Determines the current hunting word the player needs to type if an animal exists.
        /// </summary>
        private HuntWord _shootingWord;

        /// <summary>
        ///     List of all the shooting words generated from the get values on hunt word enumeration.
        /// </summary>
        private List<HuntWord> _shootWords;

        /// <summary>
        ///     Reference to all of the created prey in the area which the player will be able to hunt and kill with their bullets.
        /// </summary>
        private List<PreyItem> _sortedPrey;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Hunting" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public Hunting(IWindow window) : base(window)
        {
            // Creates the text reference to hold string data.
            _huntPrompt = new StringBuilder();

            // Grab all of the shooting words from enum that holds them.
            _shootWords = Enum.GetValues(typeof (HuntWord)).Cast<HuntWord>().ToList();

            // Create animals for the player to shoot with their bullets.
            GeneratePrey();
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
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return _shootingWord != HuntWord.None; }
        }

        /// <summary>
        ///     Determines if this dialog state is allowed to receive any input at all, even empty line returns. This is useful for
        ///     preventing the player from leaving a particular dialog until you are ready or finished processing some data.
        /// </summary>
        public override bool AllowInput
        {
            get { return _shootingWord != HuntWord.None; }
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

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Player has thirty (30) seconds to perform a hunt.
            _secondsRemaining = HUNTINGTIME;
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
        public override void OnTick(bool systemTick, bool skipDay)
        {
            base.OnTick(systemTick, skipDay);

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
            _shootingWord = (HuntWord) GameSimulationApp.Instance.Random.Next(_shootWords.Count);

            // Check if the selected value is none.
            if (_shootingWord == HuntWord.None)
                return;
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string OnRenderForm()
        {
            // Clear any previous hunting text.
            _huntPrompt.Clear();

            // Display how long the hunt has left, animals in area, 
            _huntPrompt.AppendLine($"Time Remaining: {_secondsRemaining.ToString("N0")} seconds");

            // Displays the current weather for this area.
            _huntPrompt.AppendLine(
                $"Weather: {GameSimulationApp.Instance.Trail.CurrentLocation.Weather.ToDescriptionAttribute()}");

            // Display current amount of animals in area.
            _huntPrompt.AppendLine($"Prey: {_sortedPrey.Count.ToString("N0")} animals");

            // Display the current shooting word.
            _huntPrompt.AppendLine($"Shooting Word: {_shootingWord.ToString().ToUpperInvariant()}");

            // Return the new hunting text data.
            return _huntPrompt.ToString();
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Check if we have a hunting word right now.
            if (_shootingWord == HuntWord.None)
                return;

            // Skip if the input is null or empty.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return;

            // Attempt to cast string to enum value, can be characters or integer.
            HuntWord huntWord;
            Enum.TryParse(input, out huntWord);

            // Check if the user spelled the shooting word correctly.
            if (input.Equals(huntWord.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
            }
        }
    }
}