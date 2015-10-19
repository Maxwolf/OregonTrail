using System;
using TrailCommon;

namespace TrailEntities
{
    public abstract class Receiver : IReceiver
    {
        public virtual void Action()
        {
            Console.WriteLine("Called Receiver.Action()");
        }
    }
}