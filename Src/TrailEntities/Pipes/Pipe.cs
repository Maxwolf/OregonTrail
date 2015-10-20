using TrailCommon;

namespace TrailEntities
{
    public abstract class Pipe : Receiver, IPipe
    {
        protected const string DefaultPipeName = "TrailGame";
        public abstract bool IsClosing { get; }
        public abstract void Start();
        public abstract void Stop();
    }
}