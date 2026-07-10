using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
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
            if (available.Contains(TravelCommands.StopToRest) &&
                (int) state.Health <= _genome.RestHealthThreshold &&
                state.Medicine > 0 && state.DaysRemaining > 40)
                return TravelCommands.StopToRest;

            if (available.Contains(TravelCommands.HuntForFood) && state.Food < _genome.HuntFoodThreshold && state.Ammo > 0)
                return TravelCommands.HuntForFood;

            return available.Contains(TravelCommands.ContinueOnTrail)
                ? TravelCommands.ContinueOnTrail
                : available.First();
        }

        public int Pace(GameSnapshot state) => 1;   // Steady (pace is not in this port's mileage formula)
        public int Ration(GameSnapshot state) => 1; // Filling (eats least, lowest illness in this port)
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
