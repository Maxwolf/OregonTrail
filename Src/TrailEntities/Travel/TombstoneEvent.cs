using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Informs the player that there is a grave site here with an epitaph they can read the other player left. There are
    ///     no items or parts or any diseases here, it serves as purely a marker of others travels.
    /// </summary>
    public class TombstoneEvent : RandomEventBase
    {
        private string _epitaph;

        public TombstoneEvent(RandomEvent action, string name, float rollChance, uint rollCount, string epitaph)
            : base(action, name, rollChance, rollCount)
        {
            _epitaph = epitaph;
        }

        public string Epitaph
        {
            get { return _epitaph; }
        }
    }
}