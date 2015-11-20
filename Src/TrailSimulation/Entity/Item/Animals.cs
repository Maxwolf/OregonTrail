using TrailSimulation.Game;

namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Defines a bunch of predefined animal items that can be hunted for food using bullets by the player.
    /// </summary>
    public static class Animals
    {
        public static SimItem Bear
        {
            get { return new SimItem(SimEntity.Food, "Bear", "pounds", "pound", 2000, 0); }
        }

        /// <summary>
        ///     You must use *all* the buffalo...
        /// </summary>
        public static SimItem Buffalo
        {
            get
            {
                return new SimItem(SimEntity.Food, "Buffalo", "pounds", "pound", 2000, 0,
                    GameSimulationApp.Instance.Randomizer.Next(350, 500));
            }
        }

        public static SimItem Caribou
        {
            get
            {
                return new SimItem(SimEntity.Food, "Caribou", "pounds", "pound", 2000, 0,
                    GameSimulationApp.Instance.Randomizer.Next(300, 350));
            }
        }

        public static SimItem Deer
        {
            get { return new SimItem(SimEntity.Food, "Deer", "pounds", "pound", 2000, 0, 50); }
        }

        public static SimItem Duck
        {
            get { return new SimItem(SimEntity.Food, "Duck", "pounds", "pound", 2000, 0); }
        }

        public static SimItem Goose
        {
            get { return new SimItem(SimEntity.Food, "Goose", "pounds", "pound", 2000, 0, 2); }
        }

        public static SimItem Rabbit
        {
            get { return new SimItem(SimEntity.Food, "Rabbit", "pounds", "pound", 2000, 0, 2); }
        }

        public static SimItem Squirrel
        {
            get { return new SimItem(SimEntity.Food, "Squirrel", "pounds", "pound", 2000, 0); }
        }
    }
}