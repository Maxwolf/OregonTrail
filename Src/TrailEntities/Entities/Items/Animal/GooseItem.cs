using TrailCommon;

namespace TrailEntities
{
    public sealed class GooseItem : AnimalItem
    {
        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Goose"; }
        }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        public override uint Weight
        {
            get { return 2; }
        }
    }
}