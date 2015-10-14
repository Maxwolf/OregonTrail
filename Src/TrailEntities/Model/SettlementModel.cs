namespace TrailEntities
{
    public class SettlementModel : Settlement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Settlement" /> class.
        /// </summary>
        public SettlementModel(string name, bool canRest, Store store) : base(name, canRest, store)
        {
        }
    }
}