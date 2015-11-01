using TrailCommon;

namespace TrailEntities
{
    public sealed class BearItem : AnimalItem
    {
        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Bear"; }
        }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        protected override uint Weight
        {
            get { return 100; }
        }
    }
}