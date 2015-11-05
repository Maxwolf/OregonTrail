using System;

namespace TrailEntities
{
    /// <summary>
    ///     Zero weight animal that is attached to the vehicle but not actually 'inside' of it, but is still in the list of
    ///     inventory items that define the vehicle the player and his party is making the journey in.
    /// </summary>
    public sealed class Oxen : Part
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        public Oxen(float cost) : base(cost, 1)
        {
            // Give the oxen a random amount of starting food to eat, he will consume this before trying to take from climate.
            GrassAvaliable = GameSimulationApp.Instance.Random.Next(0, 3);

            // Give the oxen a random starting health, you cannot trust those store keepers!
            var values = Enum.GetValues(typeof (RepairStatus));
            var randomStatus = (RepairStatus) values.GetValue(GameSimulationApp.Instance.Random.Next(values.Length));
            OxenHealth = randomStatus;
        }

        /// <summary>
        ///     Internal amount of grass which the oxen has eaten and can process before he needs to take more from climate, if he
        ///     cannot do that he will die after several turns of starving.
        /// </summary>
        public int GrassAvaliable { get; }

        /// <summary>
        ///     Defines the current health level of the oxen, this is his health and if it drops below a certain level or stays at
        ///     a low enough thresholds there is a chance the oxen could die. If this happens the player vehicle will go slower and
        ///     cannot pull as much weight.
        /// </summary>
        public RepairStatus OxenHealth { get; }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Oxen"; }
        }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen item each one of those items is referred to as an
        ///     'ox'.
        /// </summary>
        public override string DelineatingUnit
        {
            get { return "ox"; }
        }

        /// <summary>
        ///     When multiple of this item exist in a stack or need to be referenced, such as "10 pounds of food" the 'pounds' is
        ///     very important to get correct in context. Another example of this property being used is for Oxen item, a single Ox
        ///     is the delineating and the plural form would be "Oxen".
        /// </summary>
        public override string PluralForm
        {
            get { return "oxen"; }
        }

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        public override int CarryLimit
        {
            get { return 20; }
        }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        protected override int Weight
        {
            get { return 0; }
        }

        /// <summary>
        ///     Consumes food from the animals internal 'stomach' of food before attempting to take some from the simulation
        ///     climate. If the oxen cannot find any grass to eat in the current point of interests climate then he will begin to
        ///     stave, each time eat is called (every day) and there is not enough his health will have random chance to lower
        ///     until he finally dies.
        /// </summary>
        public void Eat()
        {
            // TODO: Nom on some grass, the level should be in the climate simulation.
            // TODO: Nom internal levels of grass before trying to take from climate.
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Called by random events (sudden heart attack!), or by the eating mechanism giving up on trying to eat food and
        ///     finally lowering repair status to bad and there is a chance death can occur then.
        /// </summary>
        public void Die()
        {
            // TODO: Trigger event manager to force game mode for random event of death of oxen to occur.
            throw new NotImplementedException();
        }
    }
}