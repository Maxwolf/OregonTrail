namespace TrailEntities
{
    /// <summary>
    ///     Ammunition used in hunting game mode so the players can acquire food by hunting animals.
    /// </summary>
    public sealed class Bullets : Item
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Bullets" /> class.
        /// </summary>
        public Bullets(float cost) : base(cost, 20)
        {
        }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Ammunition"; }
        }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen item each one of those items is referred to as an
        ///     'ox'.
        /// </summary>
        public override string DelineatingUnit
        {
            get { return "box"; }
        }

        /// <summary>
        ///     When multiple of this item exist in a stack or need to be referenced, such as "10 pounds of food" the 'pounds' is
        ///     very important to get correct in context. Another example of this property being used is for Oxen item, a single Ox
        ///     is the delineating and the plural form would be "Oxen".
        /// </summary>
        public override string PluralForm
        {
            get { return "boxes"; }
        }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        protected override int Weight
        {
            get { return 0; }
        }

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        public override int CarryLimit
        {
            get { return 99; }
        }
    }
}