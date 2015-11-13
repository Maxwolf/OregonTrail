namespace TrailEntities.Entity
{
    /// <summary>
    ///     Defines a bunch of predefined animal items that can be hunted for food using bullets by the player.
    /// </summary>
    public static class Animals
    {
        public static SimulationItem Bear
        {
            get { return new SimulationItem(SimulationEntity.Food, "Bear", "pounds", "pound", 2000, 0); }
        }

        /// <summary>
        ///     You must use *all* the buffalo...
        /// </summary>
        public static SimulationItem Buffalo
        {
            get
            {
                return new SimulationItem(SimulationEntity.Food, "Buffalo", "pounds", "pound", 2000, 0,
                    GameSimulationApp.Instance.Random.Next(350, 500));
            }
        }

        public static SimulationItem Caribou
        {
            get
            {
                return new SimulationItem(SimulationEntity.Food, "Caribou", "pounds", "pound", 2000, 0,
                    GameSimulationApp.Instance.Random.Next(300, 350));
            }
        }

        public static SimulationItem Deer
        {
            get { return new SimulationItem(SimulationEntity.Food, "Deer", "pounds", "pound", 2000, 0, 50); }
        }

        public static SimulationItem Duck
        {
            get { return new SimulationItem(SimulationEntity.Food, "Duck", "pounds", "pound", 2000, 0); }
        }

        public static SimulationItem Goose
        {
            get { return new SimulationItem(SimulationEntity.Food, "Goose", "pounds", "pound", 2000, 0, 2); }
        }

        public static SimulationItem Rabbit
        {
            get { return new SimulationItem(SimulationEntity.Food, "Rabbit", "pounds", "pound", 2000, 0, 2); }
        }

        public static SimulationItem Squirrel
        {
            get { return new SimulationItem(SimulationEntity.Food, "Squirrel", "pounds", "pound", 2000, 0); }
        }
    }
}