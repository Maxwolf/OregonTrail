using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A river-crossing method, decoupled from the per-location integer the game happens to assign it (the game renumbers
    ///     these 1..N differently at each river, so the recognizer maps a chosen kind back to the current number).
    /// </summary>
    public enum RiverChoiceKind
    {
        Ford,
        Caulk,
        Ferry,
        Indian,
        Wait,
        MoreInfo
    }

    /// <summary>
    ///     The bot's decision-making brain. Every choice the game asks of a player routes through one of these hooks, so the
    ///     game-driving layer never bakes in a strategy. Implementations range from a fixed <c>HeuristicPolicy</c> to the
    ///     learning <c>CemPolicy</c>; they are swapped without touching the driver. All learning state serializes through
    ///     <see cref="Serialize" />/<see cref="Load" /> for persistence in a profile.
    /// </summary>
    public interface IPolicy
    {
        string Name { get; }

        // ---- One-time new-game setup ----

        /// <summary>Profession selection: 1=Banker, 2=Carpenter, 3=Farmer.</summary>
        int Profession { get; }

        /// <summary>Starting month: 1=March .. 5=July.</summary>
        int StartMonth { get; }

        /// <summary>Wagon leader's name; must include "(bot)" so leaderboard entries are marked.</summary>
        string LeaderName { get; }

        // ---- Store ----

        /// <summary>Absolute quantity of a store item the policy wants to end up holding. The recognizer buys the gap up to
        ///     what is affordable, in priority order, then leaves.</summary>
        int TargetQuantity(Entities item, GameSnapshot state);

        // ---- Travel loop ----

        /// <summary>Which travel-menu command to issue given the subset currently offered.</summary>
        TravelCommands ChooseTravel(GameSnapshot state, IReadOnlyCollection<TravelCommands> available);

        /// <summary>Pace when changing it: 1=Steady, 2=Strenuous, 3=Grueling.</summary>
        int Pace(GameSnapshot state);

        /// <summary>Ration level when changing it: 1=Filling, 2=Meager, 3=BareBones.</summary>
        int Ration(GameSnapshot state);

        /// <summary>Days to rest (1..9) when the policy elected to rest.</summary>
        int RestDays(GameSnapshot state);

        // ---- Confirmations & branch choices ----

        /// <summary>Answer to a yes/no dialog, keyed by the form's class name so the policy can special-case (e.g. always
        ///     repair the wagon, decline trades).</summary>
        bool YesNo(string formName, GameSnapshot state);

        /// <summary>Preferred river-crossing method among those available at this river.</summary>
        RiverChoiceKind River(GameSnapshot state, IReadOnlyCollection<RiverChoiceKind> options);

        /// <summary>Which branch to take at a fork in the road (1-based index among the offered branches).</summary>
        int Fork(GameSnapshot state, int branchCount);
    }
}
