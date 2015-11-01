using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    /// Holds a bunch of pre-made stores that adjust the prices of things along the way.
    /// </summary>
    public static class StoreRegistry
    {
        /// <summary>
        ///     Matt's General Store is the first place in the game to buy supplies. This is where you stock up with everything
        ///     you need to start your journey on the trail. Matt's General Store is also the cheapest place (other than trading)
        ///     in the game to buy items.
        /// </summary>
        public static IEnumerable<Item> MattsStore()
        {
            var store = new HashSet<Item>
            {
                new OxenItem(20.0f),
                new ClothingItem(10.00f),
                new BulletsItem(2.00f),
                new PartWheelItem(10.0f),
                new PartAxleItem(10.0f),
                new PartTongueItem(10.0f),
                new FoodItem(0.20f)
            };
            return store;
        }

        /// <summary>
        ///     Fort Kearney General Store
        /// </summary>
        public static IEnumerable<Item> FortKearneyStore()
        {
            var store = new HashSet<Item>
            {
                new OxenItem(25.0f),
                new ClothingItem(12.50f),
                new BulletsItem(2.50f),
                new PartWheelItem(12.50f),
                new PartAxleItem(12.50f),
                new PartTongueItem(12.50f),
                new FoodItem(0.25f)
            };
            return store;
        }

        /// <summary>
        ///     Fort Laramie General Store
        /// </summary>
        public static IEnumerable<Item> FortLaramieStore()
        {
            var store = new HashSet<Item>
            {
                new OxenItem(30.0f),
                new ClothingItem(15.0f),
                new BulletsItem(3.00f),
                new PartWheelItem(15.0f),
                new PartAxleItem(15.0f),
                new PartTongueItem(15.0f),
                new FoodItem(0.30f)
            };
            return store;
        }

        /// <summary>
        ///     Fort Hall General Store
        /// </summary>
        public static IEnumerable<Item> FortHallStore()
        {
            var store = new HashSet<Item>
            {
                new OxenItem(40.0f),
                new ClothingItem(20.0f),
                new BulletsItem(4.0f),
                new PartWheelItem(20.0f),
                new PartAxleItem(20.0f),
                new PartTongueItem(20.0f),
                new FoodItem(0.40f)
            };
            return store;
        }

        /// <summary>
        ///     Fort Boise General Store
        /// </summary>
        public static IEnumerable<Item> FortBoiseStore()
        {
            var store = new HashSet<Item>
            {
                new OxenItem(45.0f),
                new ClothingItem(22.50f),
                new BulletsItem(4.50f),
                new PartWheelItem(22.50f),
                new PartAxleItem(22.50f),
                new PartTongueItem(22.50f),
                new FoodItem(0.45f)
            };
            return store;
        }

        /// <summary>
        ///     Fort Walla Walla General Store
        /// </summary>
        public static IEnumerable<Item> FortWallaWallaStore()
        {
            var store = new HashSet<Item>
            {
                new OxenItem(50.0f),
                new ClothingItem(25.0f),
                new BulletsItem(5.0f),
                new PartWheelItem(25.0f),
                new PartAxleItem(25.0f),
                new PartTongueItem(25.0f),
                new FoodItem(0.50f)
            };
            return store;
        }

        /// <summary>
        ///     Fort Bridger General Store
        /// </summary>
        public static IEnumerable<Item> FortBridgerStore()
        {
            var store = new HashSet<Item>
            {
                new OxenItem(35.0f),
                new ClothingItem(17.50f),
                new BulletsItem(3.50f),
                new PartWheelItem(17.50f),
                new PartAxleItem(17.50f),
                new PartTongueItem(17.50f),
                new FoodItem(0.35f)
            };
            return store;
        }
    }
}