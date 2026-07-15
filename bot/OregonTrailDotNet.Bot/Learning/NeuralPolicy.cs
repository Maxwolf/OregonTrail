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
        public int TargetQuantity(EntitiesEnum item, GameSnapshot state) => _setup.TargetQuantity(item);

        public TravelCommandsEnum ChooseTravel(GameSnapshot state, IReadOnlyCollection<TravelCommandsEnum> available)
        {
            var o = _brain.Forward(Features(state));

            // Set pace and rations to the genome's learned choice PLUS the network's state-adaptive nudge (o[6]/o[7]). At zero
            // weights the nudge is 0, so this is exactly the warm-started genome choice (Grueling/Filling at the expert prior),
            // and as the weights evolve the network can ease the pace or stretch the rations in response to the live situation.
            if (available.Contains(TravelCommandsEnum.ChangePace) && state.Pace != (TravelPaceEnum) NudgedPace(o))
                return TravelCommandsEnum.ChangePace;
            if (available.Contains(TravelCommandsEnum.ChangeFoodRations) && state.Ration != (RationLevelEnum) (4 - NudgedRation(o)))
                return TravelCommandsEnum.ChangeFoodRations;

            // Rest: the expert rule (stop when the weakest member has fallen to the genome's health threshold) plus the
            // network's state-adaptive nudge o[0]. At zero weights o[0] is 0, so this is exactly the expert decision.
            var restMargin = (_setup.RestHealthThreshold - (int) state.LowestHealth) / 500.0 + o[0];
            if (available.Contains(TravelCommandsEnum.StopToRest) && restMargin > 0 && state.DaysRemaining > 40)
                return TravelCommandsEnum.StopToRest;

            // Hunt: the expert rule (top up the larder below the genome's food threshold) plus the network's nudge o[1], still
            // gated on having ammo. Normalized by the same 500-scale as the feature so o[1] is a comparable-magnitude signal.
            var huntMargin = (_setup.HuntFoodThreshold - state.Food) / 500.0 + o[1];
            if (available.Contains(TravelCommandsEnum.HuntForFood) && huntMargin > 0 && state.Ammo > 0)
                return TravelCommandsEnum.HuntForFood;

            return available.Contains(TravelCommandsEnum.ContinueOnTrail)
                ? TravelCommandsEnum.ContinueOnTrail
                : available.First();
        }

        public int Pace(GameSnapshot state) => NudgedPace(_brain.Forward(Features(state)));     // menu 1=Steady..3=Grueling
        public int Ration(GameSnapshot state) => NudgedRation(_brain.Forward(Features(state))); // menu 1=Filling..3=BareBones
        public int RestDays(GameSnapshot state) => _setup.RestDays;

        // The network nudges the genome's pace/ration menu choice by its output head, clamped to the legal 1..3 range. At zero
        // weights the nudge rounds to 0, so this is exactly the warm-started genome choice.
        private int NudgedPace(double[] o) => Math.Clamp(_setup.PaceChoice + (int) Math.Round(o[6]), 1, 3);
        private int NudgedRation(double[] o) => Math.Clamp(_setup.RationChoice + (int) Math.Round(o[7]), 1, 3);

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

        public RiverChoiceKindEnum River(GameSnapshot state, IReadOnlyCollection<RiverChoiceKindEnum> options)
        {
            var o = _brain.Forward(Features(state));

            // Expert river preference (the seeded genome scores) plus the network's per-option correction. At zero weights the
            // corrections are 0, so this is the expert's argmax; the network can re-rank the crossings as it learns.
            double ScoreOf(RiverChoiceKindEnum kind) => _setup.RiverScore(kind) + kind switch
            {
                RiverChoiceKindEnum.Ferry => o[2],
                RiverChoiceKindEnum.Indian => o[3],
                RiverChoiceKindEnum.Caulk => o[4],
                RiverChoiceKindEnum.Ford => o[5],
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
                Norm(s.Food / (double) (living * living), 60),

                // Situational signals (new): let the network condition on WHERE and WHEN it is, not just aggregate stock levels.
                Norm(s.CurrentMonth, 12),                             // season (illness/weather risk rises late in the year)
                Math.Clamp((s.Temperature + 30) / 70.0, 0.0, 1.0),    // cold exposure (Celsius, ~ -30..40 -> 0..1)
                s.HighGround ? 1.0 : 0.0,                             // mountain pass: slow going, blizzards, risk of getting stuck
                Norm(s.LocationIndex, Math.Max(1, s.LocationCount)),  // progress along the trail (early vs. nearly there)
                s.ShoppingAllowed ? 1.0 : 0.0                         // can resupply here (a fort/settlement)
            };
        }

        private static double Norm(double value, double scale) => Math.Clamp(value / scale, 0.0, 1.0);
    }
}
