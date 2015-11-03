using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Holds a bunch of pre-made stores that adjust the prices of things along the way.
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
                new Oxen(20.0f),
                new Clothing(10.00f),
                new Bullets(2.00f),
                new PartWheel(10.0f),
                new PartAxle(10.0f),
                new PartTongue(10.0f),
                new Food(0.20f)
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
                new Oxen(25.0f),
                new Clothing(12.50f),
                new Bullets(2.50f),
                new PartWheel(12.50f),
                new PartAxle(12.50f),
                new PartTongue(12.50f),
                new Food(0.25f)
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
                new Oxen(30.0f),
                new Clothing(15.0f),
                new Bullets(3.00f),
                new PartWheel(15.0f),
                new PartAxle(15.0f),
                new PartTongue(15.0f),
                new Food(0.30f)
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
                new Oxen(40.0f),
                new Clothing(20.0f),
                new Bullets(4.0f),
                new PartWheel(20.0f),
                new PartAxle(20.0f),
                new PartTongue(20.0f),
                new Food(0.40f)
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
                new Oxen(45.0f),
                new Clothing(22.50f),
                new Bullets(4.50f),
                new PartWheel(22.50f),
                new PartAxle(22.50f),
                new PartTongue(22.50f),
                new Food(0.45f)
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
                new Oxen(50.0f),
                new Clothing(25.0f),
                new Bullets(5.0f),
                new PartWheel(25.0f),
                new PartAxle(25.0f),
                new PartTongue(25.0f),
                new Food(0.50f)
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
                new Oxen(35.0f),
                new Clothing(17.50f),
                new Bullets(3.50f),
                new PartWheel(17.50f),
                new PartAxle(17.50f),
                new PartTongue(17.50f),
                new Food(0.35f)
            };
            return store;
        }
    }
}