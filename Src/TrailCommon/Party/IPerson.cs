using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Person entity that is one of the players, can be given a name, ailments, money. The money on the person contributes
    ///     to the parties total amount.
    /// </summary>
    public interface IPerson : IEntity
    {
        uint Money { get; }
        ReadOnlyCollection<Disease> Ailments { get; }
        PersonTier SocialStatus { get; }
    }
}