namespace OregonTrail.Common
{
    /// <summary>
    ///     Core entity that powers the interaction of all other entities in the simulation. Every entity needs a name and a
    ///     condition.
    /// </summary>
    public interface IEntity
    {
        string Name { get; }
        ConditionTier Condition { get; }
    }
}