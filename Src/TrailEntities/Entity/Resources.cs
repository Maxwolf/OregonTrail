namespace TrailEntities
{
    /// <summary>
    ///     Defines items which are used by the vehicle party members, typically consuming them everyday.
    /// </summary>
    public static class Resources
    {
        /// <summary>
        ///     Worn by the vehicle party members to keep them warm when it is cold outside from climate simulation, without them
        ///     the players risk Illness and death.
        /// </summary>
        public static SimItem Clothing
        {
            get { return new SimItem(SimEntity.Clothes, "Clothing", "sets", "set", 50, 10); }
        }

        /// <summary>
        ///     Ammunition used in hunting game mode so the players can acquire food by hunting animals.
        /// </summary>
        public static SimItem Bullets
        {
            get { return new SimItem(SimEntity.Ammo, "Ammunition", "boxes", "box", 99, 2, 0, 20); }
        }

        /// <summary>
        ///     Serves as a generic reference item that represents a given amount of food. This could be from any animal or known
        ///     game resource marked as such.
        /// </summary>
        public static SimItem Food
        {
            get { return new SimItem(SimEntity.Food, "Food", "pounds", "pound", 2000, 0.20f); }
        }

        /// <summary>
        ///     Represents a vehicle entity, this is not used as actual vehicle the people travel in but rather a reference to a
        ///     vehicle in the collection of simulation entities.
        /// </summary>
        public static SimItem Vehicle
        {
            get { return new SimItem(SimEntity.Vehicle, "Vehicle", "vehicles", "vehicle", 2000, 50); }
        }

        /// <summary>
        ///     Represents a person entity, this is not used as actual person but rather a reference to a person object in the
        ///     collection of vehicle entities.
        /// </summary>
        public static SimItem Person
        {
            get { return new SimItem(SimEntity.Person, "Person", "people", "person", 2000, 0); }
        }

        /// <summary>
        ///     Represents monies the player can spend, rather than just binding some integer to a property it makes more sense to
        ///     tabulate and treat it like an item like anything else in the simulation.
        /// </summary>
        public static SimItem Cash
        {
            get { return new SimItem(SimEntity.Cash, "Cash", "dollars", "dollar", int.MaxValue, 1, 0); }
        }

        /// <summary>
        ///     Defines items that fit into no other category in the list but still need to be simulated. Typically this takes the
        ///     form on non-consumable but still useful items like a coffee cup or a band-aid that the party can use to help
        ///     themselves or provide some creature comfort but cannot heal them.
        /// </summary>
        public static SimItem Aid
        {
            get { return new SimItem(SimEntity.Aid, "Aid", "pills", "pill", 50, 0.50f, 0); }
        }
    }
}