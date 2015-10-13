using TrailCommon;

namespace TrailEntities
{
    public class BulletItem : ItemBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Item" /> class.
        /// </summary>
        public BulletItem(string name, int cost, int quantity)
            : base(ConditionTier.Good, name, cost, quantity, ItemCategory.Bullets)
        {
        }
    }
}