namespace TrailEntities
{
    /// <summary>
    ///     Defines a bunch of predefined animal items that can be hunted for food using bullets by the player.
    /// </summary>
    public static class Animals
    {
        public static Item Bear
        {
            get { return new Item(SimEntity.Food, "Bear", "pounds", "pound", 2000, 0); }
        }

        /// <summary>
        ///     You must use *all* the buffalo...
        /// </summary>
        public static Item Buffalo
        {
            get
            {
                return new Item(SimEntity.Food, "Buffalo", "pounds", "pound", 2000, 0, GameSimulationApp.Instance.Random.Next(350, 500));
            }
        }

        public static Item Caribou
        {
            get
            {
                return new Item(SimEntity.Food, "Caribou", "pounds", "pound", 2000, 0, GameSimulationApp.Instance.Random.Next(300, 350));
            }
        }

        public static Item Deer
        {
            get { return new Item(SimEntity.Food, "Deer", "pounds", "pound", 2000, 0, 50); }
        }

        public static Item Duck
        {
            get { return new Item(SimEntity.Food, "Duck", "pounds", "pound", 2000, 0); }
        }

        public static Item Goose
        {
            get { return new Item(SimEntity.Food, "Goose", "pounds", "pound", 2000, 0, 2); }
        }

        public static Item Rabbit
        {
            get { return new Item(SimEntity.Food, "Rabbit", "pounds", "pound", 2000, 0, 2); }
        }

        public static Item Squirrel
        {
            get { return new Item(SimEntity.Food, "Squirrel", "pounds", "pound", 2000, 0); }
        }
    }
}