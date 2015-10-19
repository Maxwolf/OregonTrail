using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Asks the command to carry out the request.
    /// </summary>
    public interface IInvoker
    {
        ReadOnlyCollection<ICommand> Commands { get; }
    }
}