using OregonTrail.Common;

namespace OregonTrail.Entity
{
    public class FoodItem : ItemBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Item" /> class.
        /// </summary>
        public FoodItem(ConditionTier condition, string name, int cost, int quantity)
            : base(condition, name, cost, quantity, ItemCategory.Food)
        {
        }
    }
}