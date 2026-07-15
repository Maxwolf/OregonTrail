using System.Text;
using OregonTrailDotNet.Bot.Game;

namespace OregonTrailDotNet.Bot.Diagnostics
{
    public enum BugCategoryEnum
    {
        /// <summary>An unhandled exception escaped a tick/command/score computation.</summary>
        Crash,

        /// <summary>The bot recognized a screen but couldn't make it progress (stuck).</summary>
        SoftLock,

        /// <summary>The bot landed on a form it has no handler for.</summary>
        RecognizerGap,

        /// <summary>Public game state violated an invariant (negative money, too many passengers, etc.).</summary>
        InvariantViolation
    }
}
