using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A policy whose tactical decisions are the expert strategy PLUS a small, state-adaptive correction from a neural
    ///     network. The search vector is <c>[full StrategyGenome] + [MLP weights]</c>: the genome slice (profession, month, store
    ///     targets, and the rest/hunt/river/fork thresholds) is warm-started from the same expert prior as the strategy models,
    ///     and the MLP weights start at zero. So with an all-zero network this plays the exact expert policy (a zero residual),
    ///     and as the weights evolve the network nudges the rest/hunt/river decisions based on its read of the live situation.
    ///     That warm-start is what lets neuro-evolution reach its first win as fast as the linear bots instead of discovering a
    ///     whole tactical policy from random weights.
    /// </summary>
    public sealed class NeuralPolicy : IPolicy
    {
        /// <summary>Leading vector entries mapped to the full <see cref="StrategyGenome" /> (the warm-started expert baseline).</summary>
        public const int SetupLength = StrategyGenome.Length;

        public static int VectorLength => SetupLength + Mlp.WeightCount;

        private readonly StrategyGenome _setup;
        private readonly Mlp _brain;

        public NeuralPolicy(double[] vector, string leaderName)
        {
            LeaderName = leaderName;

            // Tolerate a stored vector of a different length (e.g. a best-genome saved before the layout changed): copy what
            // fits and zero-pad the rest so decoding/replay never indexes out of range. A fresh training run always matches.
            if (vector.Length != VectorLength)
            {
                var sized = new double[VectorLength];
                Array.Copy(vector, sized, Math.Min(vector.Length, VectorLength));
                vector = sized;
            }

            var setup = new double[StrategyGenome.Length];
            Array.Copy(vector, setup, StrategyGenome.Length);
            _setup = new StrategyGenome { Raw = setup };

            _brain = new Mlp(vector, SetupLength);
        }

        public string Name => "neuro";
        public string LeaderName { get; }

        public int Profession => _setup.Profession;
        public int StartMonth => _setup.StartMonth;
        public int TargetQuantity(Entities item, GameSnapshot state) => _setup.TargetQuantity(item);

        public TravelCommands ChooseTravel(GameSnapshot state, IReadOnlyCollection<TravelCommands> available)
        {
            var o = _brain.Forward(Features(state));

            // Adopt the same known high-score travel tactics as the strategy policy - grueling pace on filling rations - so the
            // network doesn't have to rediscover them. These are the invariant that gets the wagon to Oregon inside the cap.
            if (available.Contains(TravelCommands.ChangePace) && state.Pace != TravelPace.Grueling)
                return TravelCommands.ChangePace;
            if (available.Contains(TravelCommands.ChangeFoodRations) && state.Ration != RationLevel.Filling)
                return TravelCommands.ChangeFoodRations;

            // Rest: the expert rule (stop when the weakest member has fallen to the genome's health threshold) plus the
            // network's state-adaptive nudge o[0]. At zero weights o[0] is 0, so this is exactly the expert decision.
            var restMargin = (_setup.RestHealthThreshold - (int) state.LowestHealth) / 500.0 + o[0];
            if (available.Contains(TravelCommands.StopToRest) && restMargin > 0 && state.DaysRemaining > 40)
                return TravelCommands.StopToRest;

            // Hunt: the expert rule (top up the larder below the genome's food threshold) plus the network's nudge o[1], still
            // gated on having ammo. Normalized by the same 500-scale as the feature so o[1] is a comparable-magnitude signal.
            var huntMargin = (_setup.HuntFoodThreshold - state.Food) / 500.0 + o[1];
            if (available.Contains(TravelCommands.HuntForFood) && huntMargin > 0 && state.Ammo > 0)
                return TravelCommands.HuntForFood;

            return available.Contains(TravelCommands.ContinueOnTrail)
                ? TravelCommands.ContinueOnTrail
                : available.First();
        }

        public int Pace(GameSnapshot state) => (int) TravelPace.Grueling;                              // menu 3: full daily maximum
        public int Ration(GameSnapshot state) => 1;                                                    // menu 1: Filling
        public int RestDays(GameSnapshot state) => _setup.RestDays;

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
            var o = _brain.Forward(Features(state));

            // Expert river preference (the seeded genome scores) plus the network's per-option correction. At zero weights the
            // corrections are 0, so this is the expert's argmax; the network can re-rank the crossings as it learns.
            double ScoreOf(RiverChoiceKind kind) => _setup.RiverScore(kind) + kind switch
            {
                RiverChoiceKind.Ferry => o[2],
                RiverChoiceKind.Indian => o[3],
                RiverChoiceKind.Caulk => o[4],
                RiverChoiceKind.Ford => o[5],
                _ => 0.0
            };

            var best = options.FirstOrDefault();
            var bestScore = double.NegativeInfinity;
            foreach (var option in options)
            {
                var score = ScoreOf(option);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = option;
                }
            }

            return best;
        }

        public int Fork(GameSnapshot state, int branchCount) =>
            _setup.ForkTakeSecond && branchCount >= 2 ? 2 : 1;

        private static double[] Features(GameSnapshot s)
        {
            var living = Math.Max(1, s.LivingCount);
            return new[]
            {
                Norm(s.Food, 2000),
                Norm(s.Ammo, 99),
                Norm(s.Cash, 1600),
                Norm(s.Oxen, 20),
                Norm(s.Clothing, 50),
                Norm(s.Medicine, 99),
                Norm((int) s.LowestHealth, 500), // the weakest member's health — the one at risk of dying next
                Norm(s.DaysElapsed, 246),
                Norm(s.DaysRemaining, 246),
                Norm(s.Miles, 2000),
                Norm(s.LivingCount, 5),          // party of five (leader + four companions)
                // Clothing safety vs this port's hail-freeze / illness guard: 1.0 once clothing >= 2 x living members, else scales to 0.
                Norm(s.Clothing, 2.0 * living),
                // Food runway in days at the five-person Filling burn (ration x living^2 => living^2 lb/day), scaled to ~60 days.
                Norm(s.Food / (double) (living * living), 60)
            };
        }

        private static double Norm(double value, double scale) => Math.Clamp(value / scale, 0.0, 1.0);
    }
}
