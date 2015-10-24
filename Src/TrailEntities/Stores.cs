using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailEntities
{
    public static class Stores
    {
        /// <summary>
        ///     Matt's General Store is the first place in the game to buy supplies. This is where you stock up with everything
        ///     you need to start your journey on the trail. Matt's General Store is also the cheapest place (other than trading)
        ///     in the game to buy items.
        /// </summary>
        public static ReadOnlyCollection<Item> MattsStore
        {
            get
            {
                var _store = new List<Item>
                {
                    new OxenItem(20.0f),
                    new ClothingItem(10.00f),
                    new BulletsItem(2.00f),
                    new VehicleWheelItem(10.0f),
                    new VehicleAxleItem(10.0f),
                    new VehicleTongueItem(10.0f),
                    new FoodItem(0.20f)
                };
                return _store.AsReadOnly();
            }
        }

        /// <summary>
        /// Fort Kearney General Store
        /// </summary>
        public static ReadOnlyCollection<Item> FortKearneyStore
        {
            get
            {
                var _store = new List<Item>
                {
                    new OxenItem(25.0f),
                    new ClothingItem(12.50f),
                    new BulletsItem(2.50f),
                    new VehicleWheelItem(12.50f),
                    new VehicleAxleItem(12.50f),
                    new VehicleTongueItem(12.50f),
                    new FoodItem(0.25f)
                };
                return _store.AsReadOnly();
            }
        }

        /// <summary>
        /// Fort Laramie General Store
        /// </summary>
        public static ReadOnlyCollection<Item> FortLaramieStore
        {
            get
            {
                var _store = new List<Item>
                {
                    new OxenItem(30.0f),
                    new ClothingItem(15.0f),
                    new BulletsItem(3.00f),
                    new VehicleWheelItem(15.0f),
                    new VehicleAxleItem(15.0f),
                    new VehicleTongueItem(15.0f),
                    new FoodItem(0.30f)
                };
                return _store.AsReadOnly();
            }
        }

        /// <summary>
        /// Fort Hall General Store
        /// </summary>
        public static ReadOnlyCollection<Item> FortHallStore
        {
            get
            {
                var _store = new List<Item>
                {
                    new OxenItem(40.0f),
                    new ClothingItem(20.0f),
                    new BulletsItem(4.0f),
                    new VehicleWheelItem(20.0f),
                    new VehicleAxleItem(20.0f),
                    new VehicleTongueItem(20.0f),
                    new FoodItem(0.40f)
                };
                return _store.AsReadOnly();
            }
        }

        /// <summary>
        /// Fort Boise General Store
        /// </summary>
        public static ReadOnlyCollection<Item> FortBoiseStore
        {
            get
            {
                var _store = new List<Item>
                {
                    new OxenItem(45.0f),
                    new ClothingItem(22.50f),
                    new BulletsItem(4.50f),
                    new VehicleWheelItem(22.50f),
                    new VehicleAxleItem(22.50f),
                    new VehicleTongueItem(22.50f),
                    new FoodItem(0.45f)
                };
                return _store.AsReadOnly();
            }
        }

        /// <summary>
        /// Fort Walla Walla General Store
        /// </summary>
        public static ReadOnlyCollection<Item> FortWallaWallaStore
        {
            get
            {
                var _store = new List<Item>
                {
                    new OxenItem(50.0f),
                    new ClothingItem(25.0f),
                    new BulletsItem(5.0f),
                    new VehicleWheelItem(25.0f),
                    new VehicleAxleItem(25.0f),
                    new VehicleTongueItem(25.0f),
                    new FoodItem(0.50f)
                };
                return _store.AsReadOnly();
            }
        }

        /// <summary>
        /// Fort Bridger General Store
        /// </summary>
        public static ReadOnlyCollection<Item> FortBridgerStore
        {
            get
            {
                var _store = new List<Item>
                {
                    new OxenItem(35.0f),
                    new ClothingItem(17.50f),
                    new BulletsItem(3.50f),
                    new VehicleWheelItem(17.50f),
                    new VehicleAxleItem(17.50f),
                    new VehicleTongueItem(17.50f),
                    new FoodItem(0.35f)
                };
                return _store.AsReadOnly();
            }
        }
    }
}