namespace TrailEntities.Entity
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
        public static SimulationItem Clothing
        {
            get { return new SimulationItem(SimulationEntity.Clothes, "Clothing", "sets", "set", 50, 10); }
        }

        /// <summary>
        ///     Ammunition used in hunting game mode so the players can acquire food by hunting animals.
        /// </summary>
        public static SimulationItem Bullets
        {
            get { return new SimulationItem(SimulationEntity.Ammo, "Ammunition", "boxes", "box", 99, 2, 0, 20); }
        }

        /// <summary>
        ///     Serves as a generic reference item that represents a given amount of food. This could be from any animal or known
        ///     game resource marked as such.
        /// </summary>
        public static SimulationItem Food
        {
            get { return new SimulationItem(SimulationEntity.Food, "Food", "pounds", "pound", 2000, 0.20f); }
        }

        /// <summary>
        ///     Represents a vehicle entity, this is not used as actual vehicle the people travel in but rather a reference to a
        ///     vehicle in the collection of simulation entities.
        /// </summary>
        public static SimulationItem Vehicle
        {
            get { return new SimulationItem(SimulationEntity.Vehicle, "Vehicle", "vehicles", "vehicle", 2000, 50); }
        }

        /// <summary>
        ///     Represents a person entity, this is not used as actual person but rather a reference to a person object in the
        ///     collection of vehicle entities.
        /// </summary>
        public static SimulationItem Person
        {
            get { return new SimulationItem(SimulationEntity.Person, "Person", "people", "person", 2000, 0); }
        }

        /// <summary>
        ///     Represents monies the player can spend, rather than just binding some integer to a property it makes more sense to
        ///     tabulate and treat it like an item like anything else in the simulation.
        /// </summary>
        public static SimulationItem Cash
        {
            get { return new SimulationItem(SimulationEntity.Cash, "Cash", "dollars", "dollar", int.MaxValue, 1, 0); }
        }
    }
}