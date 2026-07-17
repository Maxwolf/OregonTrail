using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
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

        // Per-game supply decisions (fort restocking, rescue/opportunistic trades) with their own loop guards.
        private readonly SupplyPlanner _supplies = new();

        // Per-game endgame score grind (trade browses at the final stops) with its own budget counter.
        private readonly EndgameGrinder _grinder = new();

        public GenomePolicy(StrategyGenome genome, string leaderName)
        {
            _genome = genome;
            LeaderName = leaderName;
        }

        public string Name => "genome";
        public string LeaderName { get; }

        public int Profession => _genome.Profession;
        public int StartMonth => _genome.StartMonth;

        public int TargetQuantity(EntitiesEnum item, GameSnapshot state) => _genome.TargetQuantity(item);

        public int CashReserve(GameSnapshot state) => _genome.RestockReserve;

        public TravelCommandsEnum ChooseTravel(GameSnapshot state, IReadOnlyCollection<TravelCommandsEnum> available)
        {
            // Supply decisions come first: a stranded wagon goes for the store/trade that un-strands it before anything
            // else, forts get one restock visit per stop, and a thin team or missing spare browses a few emigrant offers.
            if (_supplies.Decide(state, available, item => _genome.TargetQuantity(item)) is { } supplyMove)
                return supplyMove;

            // Set the party's pace and rations to whatever the genome has learned (no longer hardcoded), so the optimizer can
            // trade travel speed against the party's health rather than always flooring it. Do this before anything else.
            if (available.Contains(TravelCommandsEnum.ChangePace) && state.Pace != _genome.DesiredPace)
                return TravelCommandsEnum.ChangePace;
            if (available.Contains(TravelCommandsEnum.ChangeFoodRations) && state.Ration != _genome.DesiredRation)
                return TravelCommandsEnum.ChangeFoodRations;

            // Rest for the party's weakest member, not the average: a full, healthy party scores far higher than a lone
            // survivor, so every member in danger is worth stopping for. Resting is no longer gated on carrying medicine -
            // stopping heals through natural recovery either way, and medicine now also treats the sick while moving, so the
            // old "only rest if we have medicine" gate perversely taught the optimizer to drop medicine to avoid resting.
            // The schedule guard (don't rest away the margin) only applies mid-journey: DaysRemaining goes negative in
            // long games now that the 246-day forced ending is gone, and near the trail's end — especially during the
            // endgame grind — recovery must stay available or lost health could never be won back.
            if (available.Contains(TravelCommandsEnum.StopToRest) && WantRecovery(state) &&
                (state.DaysRemaining > 40 || state.NearTrailEnd))
                return TravelCommandsEnum.StopToRest;

            if (available.Contains(TravelCommandsEnum.HuntForFood) && state.Food < _genome.HuntFoodThreshold && state.Ammo > 0)
                return TravelCommandsEnum.HuntForFood;

            // Endgame score grind: at the final stops before Oregon City, browse emigrant trades to convert hunted food
            // into clothes and bullets. Runs after the hunt rule so the larder (the trade currency) refills first.
            if (_grinder.WantTrade(state, _genome.GrindTrades, _genome.GrindHealthFloor, available))
                return TravelCommandsEnum.AttemptToTrade;

            return available.Contains(TravelCommandsEnum.ContinueOnTrail)
                ? TravelCommandsEnum.ContinueOnTrail
                : available.First();
        }

        // The weakest living member has fallen far enough that the party should stop pushing and recover.
        private bool WantRecovery(GameSnapshot state) => (int) state.LowestHealth <= _genome.RestHealthThreshold;

        public int Pace(GameSnapshot state) => _genome.PaceChoice;     // learned menu 1=Steady / 2=Strenuous / 3=Grueling
        public int Ration(GameSnapshot state) => _genome.RationChoice; // learned menu 1=Filling / 2=Meager / 3=BareBones
        public int RestDays(GameSnapshot state) => _genome.RestDays;

        public bool YesNo(string formName, GameSnapshot state) => formName switch
        {
            "VehicleBrokenPrompt" => true,
            "TollRoadQuestion" => true,
            "LocationArrive" => true,
            "UseFerryConfirm" => true,
            "IndianGuidePrompt" => true,
            "Trading" => SupplyTactics.AcceptTrade(state, _genome.TradeMargin),
            "TombstoneQuestion" => false,
            _ => false
        };

        public RiverChoiceKindEnum River(GameSnapshot state, IReadOnlyCollection<RiverChoiceKindEnum> options)
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
