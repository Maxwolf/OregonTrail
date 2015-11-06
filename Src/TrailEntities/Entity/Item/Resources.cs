using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Defines items which are used by the vehicle party members, typically consuming them everyday.
    /// </summary>
    public static class Resources
    {
        /// <summary>
        ///     References all of the default store items that any clerk will offer to sell you. This is also true for the store
        ///     purchasing mode that keeps track of purchases that need to be made.
        /// </summary>
        public static IDictionary<SimEntity, Item> DefaultStore
        {
            get
            {
                // Build up the default items every store will have, their prices increase with distance from starting point.
                var defaultStoreInventory = new Dictionary<SimEntity, Item>
                {
                    {SimEntity.Animal, Parts.Oxen},
                    {SimEntity.Clothes, Clothing},
                    {SimEntity.Ammo, Bullets},
                    {SimEntity.Wheel, Parts.Wheel},
                    {SimEntity.Axle, Parts.Axle},
                    {SimEntity.Tongue, Parts.Tongue},
                    {SimEntity.Food, Food}
                };
                return defaultStoreInventory;
            }
        }

        /// <summary>
        ///     Worn by the vehicle party members to keep them warm when it is cold outside from climate simulation, without them
        ///     the players risk illness and death.
        /// </summary>
        public static Item Clothing
        {
            get { return new Item(SimEntity.Clothes, "Clothing", "sets", "set", 50, 10); }
        }

        /// <summary>
        ///     Ammunition used in hunting game mode so the players can acquire food by hunting animals.
        /// </summary>
        public static Item Bullets
        {
            get { return new Item(SimEntity.Ammo, "Ammunition", "boxes", "box", 99, 2, 0, 20); }
        }

        /// <summary>
        ///     Serves as a generic reference item that represents a given amount of food. This could be from any animal or known
        ///     game resource marked as such.
        /// </summary>
        public static Item Food
        {
            get { return new Item(SimEntity.Food, "Food", "pounds", "pound", 2000, 0.20f); }
        }

        /// <summary>
        ///     Represents a vehicle entity, this is not used as actual vehicle the people travel in but rather a reference to a
        ///     vehicle in the collection of simulation entities.
        /// </summary>
        public static Item Vehicle
        {
            get { return new Item(SimEntity.Vehicle, "Vehicle", "vehicles", "vehicle", 2000, 50); }
        }

        /// <summary>
        ///     Represents a person entity, this is not used as actual person but rather a reference to a person object in the
        ///     collection of vehicle entities.
        /// </summary>
        public static Item Person
        {
            get { return new Item(SimEntity.Person, "Person", "people", "person", 2000, 0); }
        }

        /// <summary>
        ///     Represents monies the player can spend, rather than just binding some integer to a property it makes more sense to
        ///     tabulate and treat it like an item like anything else in the simulation.
        /// </summary>
        public static Item Cash
        {
            get { return new Item(SimEntity.Cash, "Cash", "dollars", "dollar", int.MaxValue, 1, 0); }
        }
    }
}