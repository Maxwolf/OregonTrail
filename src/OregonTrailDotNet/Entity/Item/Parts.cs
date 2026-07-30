// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

namespace OregonTrailDotNet.Entity.Item
{
    /// <summary>
    ///     Defines a bunch of items that are used as parts in the vehicle.
    /// </summary>
    public static class Parts
    {
        /// <summary>
        ///     Zero weight animal that is attached to the vehicle but not actually 'inside' of it, but is still in the list of
        ///     inventory items that define the vehicle the player and his party is making the journey in.
        ///     <para>
        ///         Sold by the yoke, two at a time, because that is how a wagon is pulled and how Matt sold them: "There
        ///         are 2 oxen in a yoke; I recommend at least 3 yoke. I charge $40 a yoke." A single ox is not a thing
        ///         you can buy. The price stays per-ox at $20 so a yoke bills exactly the original's $40, and the wagon
        ///         still counts individual animals - the mileage formula, the ox-lost events and the 4-points-per-ox
        ///         tally all work in single oxen. The one-digit field is the original's too: nine yoke at a time.
        ///     </para>
        /// </summary>
        public static SimItem Oxen => new SimItem(EntitiesEnum.Animal, "Oxen", "oxen", "ox", 20,
            StorePrice.Scaled(ItemPrices.Ox), 0, 1, 0, 4, 1,
            lotSize: 2, lotUnit: "yoke", lotPluralForm: "yoke", maxSaleDigits: 1);

        /// <summary>
        ///     Required to keep the vehicle moving if this part is broken it must be replaced before the player can
        ///     continue their journey.
        /// </summary>
        public static SimItem Axle => new SimItem(EntitiesEnum.Axle, "Vehicle Axle", "axles", "axle", 3,
            StorePrice.Scaled(ItemPrices.WagonPart), 0, 1, 0, 2, 1, maxSaleDigits: 1);

        /// <summary>
        ///     Required to keep the vehicle running, if the tongue breaks then the player will have to fix or replace it before
        ///     they can continue on the journey again.
        /// </summary>
        public static SimItem Tongue => new SimItem(EntitiesEnum.Tongue, "Vehicle Tongue", "tongues", "tongue", 3,
            StorePrice.Scaled(ItemPrices.WagonPart), 0, 1, 0, 2, 1, maxSaleDigits: 1);

        /// <summary>
        ///     Required to keep the vehicle moving down the path, if any of the wheel parts break they must be replaced before the
        ///     journey can continue.
        /// </summary>
        public static SimItem Wheel => new SimItem(EntitiesEnum.Wheel, "Vehicle Wheel", "wheels", "wheel", 3,
            StorePrice.Scaled(ItemPrices.WagonPart), 0, 1, 0, 2, 1, maxSaleDigits: 1);
    }
}