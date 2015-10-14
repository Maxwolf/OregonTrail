using TrailCommon;

namespace TrailEntities
{
    public class RandomEventModel : RandomEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.RandomEvent" /> class.
        /// </summary>
        public RandomEventModel(IVehicle vehicle) : base(vehicle)
        {
        }
    }
}