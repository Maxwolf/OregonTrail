using System;

namespace TrailCommon
{
    public abstract class Receiver
    {
        public void TestOn()
        {
            Console.WriteLine("Test ON");
        }

        public void TestOff()
        {
            Console.WriteLine("Test OFF");
        }
    }
}