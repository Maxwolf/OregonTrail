using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a binding between a Receiver object and an action. Implements Execute by invoking the corresponding
    ///     operation(s) on Receiver.
    /// </summary>
    public abstract class ConcreteCommand : Command
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ConcreteCommand" /> class.
        /// </summary>
        protected ConcreteCommand(IGameSimulation game) : base(game)
        {
        }


        public override void Execute()
        {
            Receiver.Action();
        }
    }
}