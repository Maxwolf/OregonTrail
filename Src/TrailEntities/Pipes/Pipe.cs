using TrailCommon;

namespace TrailEntities
{
    public abstract class Pipe : IPipe
    {
        /// <summary>
        ///     Used in the pipe system to help identify the correct address, this should not be changed unless you know what you
        ///     are doing.
        /// </summary>
        protected const string SimulationName = ".";

        protected const string ServerPipeName = "ToSrvPipe";
        protected const string ClientPipeName = "FromSrvPipe";
        private string _currentID;

        public abstract bool Connected { get; }

        public string CurrentID
        {
            get { return _currentID; }
            set { _currentID = value; }
        }

        public bool ShouldStop { get; private set; }

        public abstract void TickPipe();

        public abstract void Start();

        public void Stop()
        {
            ShouldStop = true;
        }
    }
}