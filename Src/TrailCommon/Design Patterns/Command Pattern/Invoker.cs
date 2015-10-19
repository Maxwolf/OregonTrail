using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrailCommon
{
    public abstract class Invoker
    {
        private List<ICommand> _commands = new List<ICommand>();

        public void StoreAndExecute(ICommand command)
        {
            _commands.Add(command);
            command.Execute();
        }
    }
}
