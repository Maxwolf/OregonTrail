namespace TrailEntities
{
    public sealed class BuffaloItem : AnimalItem
    {
        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Buffalo"; }
        }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        protected override uint Weight
        {
            get { return (uint) GameSimulationApp.Instance.Random.Next(350, 500); }
        }
    }
}