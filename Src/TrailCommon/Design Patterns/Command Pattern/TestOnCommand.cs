using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrailCommon
{
    public class TestOnCommand : ICommand
    {
        private ReceiverTest _receiverTest;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TestOnCommand(ReceiverTest receiverTest)
        {
            _receiverTest = receiverTest;
        }

        public void Execute()
        {
            _receiverTest.TestOn();
        }
    }
}
