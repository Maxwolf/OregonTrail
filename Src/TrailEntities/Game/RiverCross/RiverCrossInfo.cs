using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Defines information about the current river crossing game mode the player has come across and needs to decide how
    ///     they would like to proceed.
    /// </summary>
    public sealed class RiverCrossInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TileEntities.RiverCrossInfo" /> class.
        /// </summary>
        public RiverCrossInfo()
        {
            // Randomly generates statistics about the river each time you cross it.
            Depth = GameSimulationApp.Instance.Random.Next(1, 20);
            FerryCost = GameSimulationApp.Instance.Random.Next(3, 8);
            RiverWidth = GameSimulationApp.Instance.Random.Next(20, 350);
            CrossingType = CrossChoice.Ford;
        }

        /// <summary>
        ///     Determines how the vehicle and party members would like to cross the river.
        /// </summary>
        public CrossChoice CrossingType { get; set; }

        /// <summary>
        ///     Determines how deep the river is in feet.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        ///     Determines how much the ferry operator will charge to cross the river.
        /// </summary>
        public int FerryCost { get; }

        /// <summary>
        ///     Determines how wide the river is and how big the dice roll is for something terrible happening to the vehicle and
        ///     party members.
        /// </summary>
        public int RiverWidth { get; }
    }
}