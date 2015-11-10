using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Random event mode does not have any special information to carry around between states since it's sole purpose in
    ///     life is to execute events and print the information before removing itself.
    /// </summary>
    public sealed class RandomEventInfo : IModeInfo
    {
        // Move along... nothing to see here.
    }
}