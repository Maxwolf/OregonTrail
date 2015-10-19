using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    /// Declares an interface for executing an operation.
    /// </summary>
    public abstract class Command : ICommand
    {
        private Receiver _receiver;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Command" /> class.
        /// </summary>
        protected Command(Receiver receiver)
        {
            _receiver = receiver;
        }

        public IReceiver Receiver
        {
            get { return _receiver; }
        }

        public abstract void Execute();
    }
}