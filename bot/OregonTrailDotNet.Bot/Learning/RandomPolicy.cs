using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A genuinely naive control: at every decision it picks a random LEGAL choice, with no strategy whatsoever. It is the
    ///     honest weak floor the learning models are supposed to beat — unlike the expert-seeded "Random Search" optimizer,
    ///     which samples the hand-tuned prior and is actually strong. Randomness is drawn from the live game's own seeded
    ///     <c>Randomizer</c>, so the baseline is automatically reproducible under the trainer's common-random-numbers seeds.
    /// </summary>
    public sealed class RandomPolicy : IPolicy
    {
        // Per-item purchase ceilings the recognizer will cap to what's affordable. A random amount up to these is always a
        // legal store target (buying nothing is legal too), matching the bounds the strategy genome searches within.
        private static readonly IReadOnlyDictionary<EntitiesEnum, int> BuyCeiling = new Dictionary<EntitiesEnum, int>
        {
            { EntitiesEnum.Animal, 20 }, { EntitiesEnum.Food, 2000 }, { EntitiesEnum.Clothes, 255 }, { EntitiesEnum.Medicine, 99 },
            { EntitiesEnum.Ammo, 1000 }, { EntitiesEnum.Wheel, 3 }, { EntitiesEnum.Axle, 3 }, { EntitiesEnum.Tongue, 3 }
        };

        // Chosen once (lazily, on first read after the game has booted) so the profession/month don't wobble between reads.
        private int? _profession;
        private int? _startMonth;

        public RandomPolicy(string leaderName) => LeaderName = leaderName;

        public string Name => "naive";
        public string LeaderName { get; }

        public int Profession => _profession ??= Next(1, 4);   // 1=Banker, 2=Carpenter, 3=Farmer
        public int StartMonth => _startMonth ??= Next(1, 6);   // 1=March .. 5=July

        public int TargetQuantity(EntitiesEnum item, GameSnapshot state) =>
            BuyCeiling.TryGetValue(item, out var max) ? Next(0, max + 1) : 0;

        public TravelCommandsEnum ChooseTravel(GameSnapshot state, IReadOnlyCollection<TravelCommandsEnum> available) =>
            available.ElementAt(Next(0, available.Count));

        public int Pace(GameSnapshot state) => Next(1, 4);     // 1=Steady, 2=Strenuous, 3=Grueling
        public int Ration(GameSnapshot state) => Next(1, 4);   // 1=Filling, 2=Meager, 3=BareBones
        public int RestDays(GameSnapshot state) => Next(1, 10);

        public bool YesNo(string formName, GameSnapshot state) => NextBool();

        public RiverChoiceKindEnum River(GameSnapshot state, IReadOnlyCollection<RiverChoiceKindEnum> options) =>
            options.ElementAt(Next(0, options.Count));

        public int Fork(GameSnapshot state, int branchCount) => Next(1, Math.Max(1, branchCount) + 1);

        // The naive policy consumes the game's own seeded randomizer. It only exists while a game is booted (it is asked for
        // decisions), so Instance is never null here.
        private static int Next(int minInclusive, int maxExclusive) =>
            GameSimulationApp.Instance!.Random.Next(minInclusive, maxExclusive);

        private static bool NextBool() => GameSimulationApp.Instance!.Random.NextBool();
    }
}
