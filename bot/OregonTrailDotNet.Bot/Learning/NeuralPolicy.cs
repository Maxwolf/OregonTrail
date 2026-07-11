using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A policy whose tactical decisions come from a small neural network evaluated against the live game state, giving a
    ///     genuinely different, state-adaptive play style. The search vector is <c>[16 setup params] + [MLP weights]</c>: the
    ///     setup slice (profession, month, store targets) is decoded by reusing <see cref="StrategyGenome" />'s getters, while
    ///     the MLP decides whether to rest/hunt and which river crossing to prefer, from normalized features of the situation.
    /// </summary>
    public sealed class NeuralPolicy : IPolicy
    {
        /// <summary>Number of leading vector entries that map to <see cref="StrategyGenome" />'s setup fields (indices 0..15).</summary>
        public const int SetupLength = 16;

        public static int VectorLength => SetupLength + Mlp.WeightCount;

        private readonly StrategyGenome _setup;
        private readonly Mlp _brain;

        public NeuralPolicy(double[] vector, string leaderName)
        {
            LeaderName = leaderName;

            // Reuse the strategy genome's decoding for the one-time setup by padding the setup slice out to its full length.
            var padded = new double[StrategyGenome.Length];
            Array.Copy(vector, padded, Math.Min(SetupLength, vector.Length));
            _setup = new StrategyGenome { Raw = padded };

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

            // Same safety guards as the genome policy, but the "should I?" comes from the network's read of the situation.
            if (available.Contains(TravelCommands.StopToRest) && o[0] > 0 &&
                state.Medicine > 0 && state.DaysRemaining > 40)
                return TravelCommands.StopToRest;

            if (available.Contains(TravelCommands.HuntForFood) && o[1] > 0 && state.Ammo > 0)
                return TravelCommands.HuntForFood;

            return available.Contains(TravelCommands.ContinueOnTrail)
                ? TravelCommands.ContinueOnTrail
                : available.First();
        }

        public int Pace(GameSnapshot state) => 1;
        public int Ration(GameSnapshot state) => 1;
        public int RestDays(GameSnapshot state) => 3;

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

            double ScoreOf(RiverChoiceKind kind) => kind switch
            {
                RiverChoiceKind.Ferry => o[2],
                RiverChoiceKind.Indian => o[3],
                RiverChoiceKind.Caulk => o[4],
                RiverChoiceKind.Ford => o[5],
                _ => double.NegativeInfinity
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

        public int Fork(GameSnapshot state, int branchCount) => 1;

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
