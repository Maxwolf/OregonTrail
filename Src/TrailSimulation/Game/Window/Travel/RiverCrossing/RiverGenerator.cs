namespace TrailSimulation.Game
{
    /// <summary>
    ///     Special data class that is used to generate river data in the travel info as requested. Creation of this object
    ///     will automatically create a new river with random width and depth.
    /// </summary>
    public sealed class RiverGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.RiverGenerator" /> class.
        /// </summary>
        public RiverGenerator()
        {
            // Randomly generates statistics about the river each time you cross it.
            RiverDepth = GameSimulationApp.Instance.Random.Next(1, 20);
            FerryCost = GameSimulationApp.Instance.Random.Next(3, 8);
            FerryDelayInDays = GameSimulationApp.Instance.Random.Next(1, 10);
            RiverWidth = GameSimulationApp.Instance.Random.Next(100, 1500);
            CrossingType = RiverCrossChoice.None;
        }

        /// <summary>
        ///     Determines how the vehicle and party members would like to cross the river.
        /// </summary>
        public RiverCrossChoice CrossingType { get; set; }

        /// <summary>
        ///     Determines how deep the river is in feet.
        /// </summary>
        public int RiverDepth { get; }

        /// <summary>
        ///     Determines how much the ferry operator will charge to cross the river.
        /// </summary>
        public float FerryCost { get; set; }

        /// <summary>
        ///     Determines how many days of the simulation the ferry operator is backed up with other vehicles and cannot process
        ///     yours until this time.
        /// </summary>
        public int FerryDelayInDays { get; set; }

        /// <summary>
        ///     Determines how wide the river is and how big the dice roll is for something terrible happening to the vehicle and
        ///     party members.
        /// </summary>
        public int RiverWidth { get; }
    }
}