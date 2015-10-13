using OregonTrail.Common;

namespace OregonTrail.Entity
{
    /// <summary>
    ///     Part is something that is attached to the vehicle and helps keep it moving so the distance traveled may increase.
    ///     If any part on the vehicle breaks then the party cannot advance forward until it has been replaced or repaired out
    ///     of the broken condition.
    /// </summary>
    public class PartItem : ItemBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Item" /> class.
        /// </summary>
        public PartItem(string name, int cost, int quantity)
            : base(ConditionTier.Good, name, cost, quantity, ItemCategory.Part)
        {
        }
    }
}