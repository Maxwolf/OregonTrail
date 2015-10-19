using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TrailCommon;

namespace TrailEntities
{
    public abstract class Invoker : IInvoker
    {
        private List<Tuple<string, ICommand>> _commands;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Invoker" /> class.
        /// </summary>
        protected Invoker()
        {
            _commands = new List<Tuple<string, ICommand>>();
        }

        public ReadOnlyCollection<Tuple<string, ICommand>> Commands
        {
            get { return new ReadOnlyCollection<Tuple<string, ICommand>>(_commands); }
        }

        public void AddCommand(Tuple<string, ICommand> command)
        {
            // Adds a command for this invoker if it does not already exist.
            if (!_commands.Contains(command))
            {
                _commands.Add(command);
            }
        }

        /// <summary>
        ///     Executes a command from the list of commands in this invoker if the name matches the name of the command.
        /// </summary>
        /// <param name="commandName">Name of the command that should be executed on this invoker.</param>
        public void ExecuteCommandByName(string commandName)
        {
            foreach (var command in _commands)
            {
                if (command.Item1 == commandName)
                {
                    command.Item2.Execute();
                    break;
                }
            }
        }

        public string[] GetCommandsNameStrings()
        {
            ISet<string> validCommands = new HashSet<string>();
            foreach (var command in _commands)
            {
                validCommands.Add(command.Item1);
            }
            return validCommands.ToArray();
        }
    }
}