using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A concrete playing strategy decoded from a <see cref="StrategyGenome" />. This is what the CEM optimizer evaluates:
    ///     each candidate genome becomes one of these, plays K games, and its scores feed back into the search. Structurally it
    ///     mirrors the heuristic, but every threshold/target/preference comes from the genome so the optimizer can tune them.
    /// </summary>
    public sealed class GenomePolicy : IPolicy
    {
        private readonly StrategyGenome _genome;

        public GenomePolicy(StrategyGenome genome, string leaderName)
        {
            _genome = genome;
            LeaderName = leaderName;
        }

        public string Name => "genome";
        public string LeaderName { get; }

        public int Profession => _genome.Profession;
        public int StartMonth => _genome.StartMonth;

        public int TargetQuantity(Entities item, GameSnapshot state) => _genome.TargetQuantity(item);

        public TravelCommands ChooseTravel(GameSnapshot state, IReadOnlyCollection<TravelCommands> available)
        {
            // Travel the original high-score way: push a grueling pace on bare-bones rations to cover ground cheaply, then
            // switch to filling rations whenever the weakest member needs to recover. Get this configuration right first.
            if (available.Contains(TravelCommands.ChangePace) && state.Pace != TravelPace.Grueling)
                return TravelCommands.ChangePace;
            if (available.Contains(TravelCommands.ChangeFoodRations) && state.Ration != DesiredRation(state))
                return TravelCommands.ChangeFoodRations;

            // Rest for the party's weakest member, not the average: a full, healthy party scores far higher than a lone
            // survivor, so every member in danger is worth stopping for. Resting is no longer gated on carrying medicine -
            // stopping heals through natural recovery either way, and medicine now also treats the sick while moving, so the
            // old "only rest if we have medicine" gate perversely taught the optimizer to drop medicine to avoid resting.
            if (available.Contains(TravelCommands.StopToRest) && WantRecovery(state) && state.DaysRemaining > 40)
                return TravelCommands.StopToRest;

            if (available.Contains(TravelCommands.HuntForFood) && state.Food < _genome.HuntFoodThreshold && state.Ammo > 0)
                return TravelCommands.HuntForFood;

            return available.Contains(TravelCommands.ContinueOnTrail)
                ? TravelCommands.ContinueOnTrail
                : available.First();
        }

        // The weakest living member has fallen far enough that the party should stop pushing and recover.
        private bool WantRecovery(GameSnapshot state) => (int) state.LowestHealth <= _genome.RestHealthThreshold;

        // Eat Filling to stay healthy. In this port bare-bones rations roll a daily illness check that is far too punishing to
        // sustain, and because the strategy HUNTS for its food (rather than stretching a fixed larder), there is no need to
        // ration hard - keep the party fed and well, and let hunting refill the wagon.
        private RationLevel DesiredRation(GameSnapshot state) => RationLevel.Filling;

        public int Pace(GameSnapshot state) => (int) TravelPace.Grueling;                 // menu 3: 100% of the daily maximum
        public int Ration(GameSnapshot state) => DesiredRation(state) == RationLevel.Filling ? 1 : 3; // menu 1 Filling / 3 Bare Bones
        public int RestDays(GameSnapshot state) => _genome.RestDays;

        public bool YesNo(string formName, GameSnapshot state) => formName switch
        {
            "VehicleBrokenPrompt" => true,
            "TollRoadQuestion" => true,
            "LocationArrive" => true,
            "UseFerryConfirm" => true,
            "IndianGuidePrompt" => true,
            "Trading" => false,
            "TombstoneQuestion" => false,
            _ => false
        };

        public RiverChoiceKind River(GameSnapshot state, IReadOnlyCollection<RiverChoiceKind> options)
        {
            var best = options.FirstOrDefault();
            var bestScore = double.NegativeInfinity;
            foreach (var option in options)
            {
                var score = _genome.RiverScore(option);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = option;
                }
            }

            return best;
        }

        public int Fork(GameSnapshot state, int branchCount) =>
            _genome.ForkTakeSecond && branchCount >= 2 ? 2 : 1;
    }
}
