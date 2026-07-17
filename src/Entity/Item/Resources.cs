// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

namespace OregonTrailDotNet.Entity.Item
{
    /// <summary>
    ///     Defines items which are used by the vehicle party members, typically consuming them everyday.
    /// </summary>
    public static class Resources
    {
        /// <summary>
        ///     Worn by the vehicle party members to keep them warm when it is cold outside from climate simulation, without them
        ///     the players risk Person and death.
        ///     Capped at 255 sets: the 1985 game had no in-play clothing limit, but its endgame handed inventory to the scoring
        ///     program through a single memory byte, so 255 is the effective original ceiling (510 base points).
        /// </summary>
        public static SimItem Clothing => new SimItem(EntitiesEnum.Clothes, "Clothing", "sets", "set", 255,
            StorePrice.Scaled(10f, 2.5f), 1, 1, 0, 2);

        /// <summary>
        ///     Ammunition used in hunting game Windows so the players can acquire food by hunting animals.
        ///     Capped at 65,535 bullets: the 1985 game had no in-play ammunition limit, but its endgame handed inventory to the
        ///     scoring program through a two-byte pair, so 65,535 is the effective original ceiling (1,310 base points).
        /// </summary>
        public static SimItem Bullets => new SimItem(EntitiesEnum.Ammo, "Ammunition", "boxes", "box", 65535,
            StorePrice.Scaled(2f, 2.5f), 0, 20, 0, 1, 50);

        /// <summary>
        ///     Serves as a generic reference item that represents a given amount of food. This could be from any animal or known
        ///     game resource marked as such.
        /// </summary>
        public static SimItem Food => new SimItem(EntitiesEnum.Food, "Food", "pounds", "pound", 2000,
            StorePrice.Scaled(0.10f, 0.10f), 1, 1, 0, 1, 25);

        /// <summary>
        ///     Medical supplies used to cure serious illness and infection among the party members. Sold in kits like other store
        ///     goods; without one on hand a seriously ill traveler cannot be treated and may worsen.
        /// </summary>
        public static SimItem Medicine => new SimItem(EntitiesEnum.Medicine, "Medicine", "kits", "kit", 99,
            StorePrice.Scaled(15f, 2.5f), 1, 1, 0, 1);

        /// <summary>
        ///     Represents a vehicle entity, this is not used as actual vehicle the people travel in but rather a reference to a
        ///     vehicle in the collection of simulation entities.
        /// </summary>
        public static SimItem Vehicle => new SimItem(EntitiesEnum.Vehicle, "Vehicle", "vehicles", "vehicle", 2000, 50, 500, 1, 0, 50);

        /// <summary>
        ///     Represents a person entity, this is not used as actual person but rather a reference to a person object in the
        ///     collection of vehicle entities. Worth 500 points on the help screens to match what the tally actually awards
        ///     for a survivor in good health (the health bands pay 500/400/300/200 per person).
        /// </summary>
        public static SimItem Person => new SimItem(EntitiesEnum.Person, "Person", "people", "person", 2000, 0, 1, 1, 0, 500);

        /// <summary>
        ///     Represents monies the player can spend, rather than just binding some integer to a property it makes more sense to
        ///     tabulate and treat it like an item like anything else in the simulation.
        /// </summary>
        public static SimItem Cash => new SimItem(EntitiesEnum.Cash, "Cash", "dollars", "dollar", int.MaxValue, 1, 0, 1, 0, 1, 5);
    }
}