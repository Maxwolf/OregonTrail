using System;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Asks the command to carry out the request.
    /// </summary>
    public interface IInvoker
    {
        ReadOnlyCollection<Tuple<string, ICommand>> Commands { get; }
        void AddCommand(Tuple<string, ICommand> command);
        void ExecuteCommandByName(string commandName);
        string[] GetCommandsNameStrings();
    }
}