using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A river-crossing method, decoupled from the per-location integer the game happens to assign it (the game renumbers
    ///     these 1..N differently at each river, so the recognizer maps a chosen kind back to the current number).
    /// </summary>
    public enum RiverChoiceKindEnum
    {
        Ford,
        Caulk,
        Ferry,
        Indian,
        Wait,
        MoreInfo
    }
}
