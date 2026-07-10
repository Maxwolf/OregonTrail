using System.Text.Json;
using OregonTrailDotNet.Entity;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     The compact real-valued strategy vector the CEM optimizer searches over. The raw doubles are unconstrained (so the
    ///     Gaussian search is well behaved); the decode properties map them to concrete, bounded game decisions. Categorical
    ///     choices (profession, month) are argmax over a small block of preferences; quantities and thresholds are clamped.
    /// </summary>
    public sealed class StrategyGenome
    {
        // Vector layout (indices into Raw).
        private const int ProfessionOffset = 0;   // 3 preferences -> profession 1..3
        private const int MonthOffset = 3;         // 5 preferences -> month 1..5
        private const int OxenIdx = 8;
        private const int FoodIdx = 9;
        private const int ClothesIdx = 10;
        private const int MedicineIdx = 11;
        private const int AmmoIdx = 12;
        private const int WheelIdx = 13;
        private const int AxleIdx = 14;
        private const int TongueIdx = 15;
        private const int RestHealthIdx = 16;
        private const int RestDaysIdx = 17;
        private const int HuntFoodIdx = 18;
        private const int RiverFerryIdx = 19;
        private const int RiverIndianIdx = 20;
        private const int RiverCaulkIdx = 21;
        private const int RiverFordIdx = 22;
        private const int ForkSecondIdx = 23;

        public const int Length = 24;

        public double[] Raw { get; init; } = new double[Length];

        // ---- Decoded decisions ----

        public int Profession => ArgMax(ProfessionOffset, 3) + 1;
        public int StartMonth => ArgMax(MonthOffset, 5) + 1;

        public int OxenTarget => ClampRound(Raw[OxenIdx], 3, 20);
        public int FoodTarget => ClampRound(Raw[FoodIdx], 0, 2000);
        public int ClothesTarget => ClampRound(Raw[ClothesIdx], 0, 50);
        public int MedicineTarget => ClampRound(Raw[MedicineIdx], 0, 99);
        public int AmmoTarget => ClampRound(Raw[AmmoIdx], 0, 99);
        public int WheelTarget => ClampRound(Raw[WheelIdx], 0, 3);
        public int AxleTarget => ClampRound(Raw[AxleIdx], 0, 3);
        public int TongueTarget => ClampRound(Raw[TongueIdx], 0, 3);

        public double RestHealthThreshold => Math.Clamp(Raw[RestHealthIdx], 0, 500);
        public int RestDays => ClampRound(Raw[RestDaysIdx], 1, 9);
        public double HuntFoodThreshold => Math.Clamp(Raw[HuntFoodIdx], 0, 500);
        public bool ForkTakeSecond => Raw[ForkSecondIdx] > 0;

        public int TargetQuantity(Entities item) => item switch
        {
            Entities.Animal => OxenTarget,
            Entities.Food => FoodTarget,
            Entities.Clothes => ClothesTarget,
            Entities.Medicine => MedicineTarget,
            Entities.Ammo => AmmoTarget,
            Entities.Wheel => WheelTarget,
            Entities.Axle => AxleTarget,
            Entities.Tongue => TongueTarget,
            _ => 0
        };

        public double RiverScore(RiverChoiceKind kind) => kind switch
        {
            RiverChoiceKind.Ferry => Raw[RiverFerryIdx],
            RiverChoiceKind.Indian => Raw[RiverIndianIdx],
            RiverChoiceKind.Caulk => Raw[RiverCaulkIdx],
            RiverChoiceKind.Ford => Raw[RiverFordIdx],
            _ => double.NegativeInfinity // never electively wait or ask for more info
        };

        // ---- Serialization ----

        public string ToJson() => JsonSerializer.Serialize(Raw);

        public static StrategyGenome FromJson(string json) =>
            new() { Raw = JsonSerializer.Deserialize<double[]>(json) ?? DefaultMean() };

        /// <summary>A sensible starting point: neutral categorical preferences (explore all professions/months) and
        ///     well-provisioned, safety-first tactical defaults similar to the hand-tuned heuristic.</summary>
        public static double[] DefaultMean()
        {
            var m = new double[Length];
            // profession/month preferences all 0 -> uniform to start (indices 0..7 already 0)
            m[OxenIdx] = 10;
            m[FoodIdx] = 1200;
            m[ClothesIdx] = 12;
            m[MedicineIdx] = 5;
            m[AmmoIdx] = 20;
            m[WheelIdx] = 2;
            m[AxleIdx] = 1;
            m[TongueIdx] = 1;
            m[RestHealthIdx] = 300;
            m[RestDaysIdx] = 3;
            m[HuntFoodIdx] = 50;
            m[RiverFerryIdx] = 1.0;
            m[RiverIndianIdx] = 0.8;
            m[RiverCaulkIdx] = 0.3;
            m[RiverFordIdx] = 0.2;
            m[ForkSecondIdx] = 0;
            return m;
        }

        /// <summary>Per-dimension exploration widths for the initial CEM distribution.</summary>
        public static double[] DefaultStd()
        {
            var s = new double[Length];
            for (var i = ProfessionOffset; i < OxenIdx; i++) s[i] = 1.0; // categorical preference logits
            s[OxenIdx] = 3; s[FoodIdx] = 300; s[ClothesIdx] = 4; s[MedicineIdx] = 3; s[AmmoIdx] = 10;
            s[WheelIdx] = 1; s[AxleIdx] = 1; s[TongueIdx] = 1;
            s[RestHealthIdx] = 80; s[RestDaysIdx] = 2; s[HuntFoodIdx] = 40;
            s[RiverFerryIdx] = 0.5; s[RiverIndianIdx] = 0.5; s[RiverCaulkIdx] = 0.5; s[RiverFordIdx] = 0.5;
            s[ForkSecondIdx] = 1.0;
            return s;
        }

        public static StrategyGenome Default() => new() { Raw = DefaultMean() };

        private int ArgMax(int offset, int count)
        {
            var best = 0;
            for (var i = 1; i < count; i++)
                if (Raw[offset + i] > Raw[offset + best])
                    best = i;
            return best;
        }

        private static int ClampRound(double value, int lo, int hi) =>
            (int) Math.Clamp(Math.Round(value), lo, hi);
    }
}
