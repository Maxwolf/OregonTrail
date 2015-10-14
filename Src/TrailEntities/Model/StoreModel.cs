using TrailCommon;

namespace TrailEntities
{
    public class StoreModel : Store
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Store" /> class.
        /// </summary>
        public StoreModel(IVehicle vehicle) : base(vehicle)
        {
        }
    }
}