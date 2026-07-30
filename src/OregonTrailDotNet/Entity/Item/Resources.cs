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
            StorePrice.Scaled(ItemPrices.ClothingSet), 1, 1, 0, 2, 1, maxSaleDigits: 2);

        /// <summary>
        ///     Ammunition used in hunting game Windows so the players can acquire food by hunting animals.
        ///     Capped at 65,535 bullets: the 1985 game had no in-play ammunition limit, but its endgame handed inventory to the
        ///     scoring program through a two-byte pair, so 65,535 is the effective original ceiling (1,310 base points).
        ///     Sold by the box of twenty for $2.00, as Matt sold it: "I sell ammunition in boxes of 20 bullets. Each box
        ///     costs $2.00. How many boxes do you want?" The price is held per bullet at a twentieth of the box so a box
        ///     bills exactly $2.00 while quantity keeps counting single bullets everywhere else in the simulation -
        ///     hunting spends one per trigger pull, scoring pays a point per fifty, the supply panel lists them. Only the
        ///     shop counter speaks in boxes. The two-digit field is the original's as well, which caps a single purchase
        ///     at ninety-nine boxes: 1,980 bullets.
        /// </summary>
        public static SimItem Bullets => new SimItem(EntitiesEnum.Ammo, "Ammunition", "bullets", "bullet", 65535,
            StorePrice.Scaled(ItemPrices.Bullet), 0, 20, 0, 1, 50,
            lotSize: 20, lotUnit: "box", lotPluralForm: "boxes", maxSaleDigits: 2);

        /// <summary>
        ///     Serves as a generic reference item that represents a given amount of food. This could be from any animal or known
        ///     game resource marked as such.
        /// </summary>
        public static SimItem Food => new SimItem(EntitiesEnum.Food, "Food", "pounds", "pound", 2000,
            StorePrice.Scaled(ItemPrices.FoodPound), 1, 1, 0, 1, 25, maxSaleDigits: 4);

        /// <summary>
        ///     Medical supplies used to cure serious illness and infection among the party members. Sold in kits like other store
        ///     goods; without one on hand a seriously ill traveler cannot be treated and may worsen. Unlike everything else in
        ///     the store this is not an original line item, so its price is ours rather than MECC's.
        /// </summary>
        public static SimItem Medicine => new SimItem(EntitiesEnum.Medicine, "Medicine", "kits", "kit", 99,
            StorePrice.Scaled(ItemPrices.MedicineKit), 1, 1, 0, 1);

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