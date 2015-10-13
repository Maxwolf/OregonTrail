using TrailCommon;

namespace TrailEntities
{
    public class ClothingItem : ItemBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Item" /> class.
        /// </summary>
        public ClothingItem(string name, int cost, int quantity)
            : base(ConditionTier.Good, name, cost, quantity, ItemCategory.Clothing)
        {
        }
    }
}