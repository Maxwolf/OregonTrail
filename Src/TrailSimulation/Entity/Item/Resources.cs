namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Defines items which are used by the vehicle party members, typically consuming them everyday.
    /// </summary>
    public static class Resources
    {
        /// <summary>
        ///     Worn by the vehicle party members to keep them warm when it is cold outside from climate simulation, without them
        ///     the players risk Person and death.
        /// </summary>
        public static SimItem Clothing
        {
            get { return new SimItem(Entities.Clothes, "Clothing", "sets", "set", 50, 10, 1, 1, 0, 2); }
        }

        /// <summary>
        ///     Ammunition used in hunting game Windows so the players can acquire food by hunting animals.
        /// </summary>
        public static SimItem Bullets
        {
            get { return new SimItem(Entities.Ammo, "Ammunition", "boxes", "box", 99, 2, 0, 20, 0, 1, 50); }
        }

        /// <summary>
        ///     Serves as a generic reference item that represents a given amount of food. This could be from any animal or known
        ///     game resource marked as such.
        /// </summary>
        public static SimItem Food
        {
            get { return new SimItem(Entities.Food, "Food", "pounds", "pound", 2000, 0.20f, 1, 1, 0, 1, 25); }
        }

        /// <summary>
        ///     Represents a vehicle entity, this is not used as actual vehicle the people travel in but rather a reference to a
        ///     vehicle in the collection of simulation entities.
        /// </summary>
        public static SimItem Vehicle
        {
            get { return new SimItem(Entities.Vehicle, "Vehicle", "vehicles", "vehicle", 2000, 50, 500, 1, 0, 50); }
        }

        /// <summary>
        ///     Represents a person entity, this is not used as actual person but rather a reference to a person object in the
        ///     collection of vehicle entities.
        /// </summary>
        public static SimItem Person
        {
            get { return new SimItem(Entities.Person, "Person", "people", "person", 2000, 0, 1, 1, 0, 800); }
        }

        /// <summary>
        ///     Represents monies the player can spend, rather than just binding some integer to a property it makes more sense to
        ///     tabulate and treat it like an item like anything else in the simulation.
        /// </summary>
        public static SimItem Cash
        {
            get { return new SimItem(Entities.Cash, "Cash", "dollars", "dollar", int.MaxValue, 1, 0, 1, 0, 1, 5); }
        }
    }
}