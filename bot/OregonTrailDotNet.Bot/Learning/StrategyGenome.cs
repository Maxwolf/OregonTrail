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

        // Oxen floor is 6, not 3: daily mileage is (oxenValue - 110)/2.5 + noise, so below ~5.5 oxen the team term goes
        // negative and the wagon can't outrun the 246-day cap - samples in [3,5] are near-guaranteed dead evaluations.
        public int OxenTarget => ClampRound(Raw[OxenIdx], 6, 20);
        // Food floor is 300, not 0: a five-person party burns ~25 lb/day (ration x livingCount^2 at Filling), so a
        // near-empty larder starves out of the gate before hunting can ever backfill it.
        public int FoodTarget => ClampRound(Raw[FoodIdx], 300, 2000);
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

        /// <summary>The expert warm-start prior distilled from the strategy knowledge base (docs/STRATEGY.md), reconciled
        ///     against this port's real mechanics. It seeds the Farmer (x3) + May basin rather than starting uniform, because
        ///     the score analysis is conclusive: the only route to Stephen Meek's 7650 is a full five-person party arriving in
        ///     Good health as a Farmer ((5 x 500 + 50 wagon) x 3). Carpenter stays live as the robust fallback through the
        ///     preference spread and the 1.0 exploration std, so the optimizer still discovers whichever actually survives.</summary>
        public static double[] DefaultMean()
        {
            var m = new double[Length];

            // Profession preference (argmax over indices 0..2). Bias hard toward Farmer: reaching it from a uniform start is
            // a deceptive jump (you must flip the profession AND trim the loadout to $400 at once, or the sample starves and
            // is culled), so seed directly inside that basin. Banker (x1) is score-dominated and never seeded toward.
            m[ProfessionOffset + 0] = 0.0; // Banker   (x1)
            m[ProfessionOffset + 1] = 0.5; // Carpenter (x2) - the reliable fallback the search retreats to
            m[ProfessionOffset + 2] = 1.5; // Farmer   (x3) - the score ceiling; start the argmax here

            // Start-month preference (argmax over indices 0..4 -> March..July). May balances a warm-enough departure against
            // reaching the high passes before winter; March is cold (more hail/illness weather), July risks winter mountains.
            m[MonthOffset + 0] = 0.0; // March
            m[MonthOffset + 1] = 0.3; // April
            m[MonthOffset + 2] = 1.0; // May   (argmax -> StartMonth 3)
            m[MonthOffset + 3] = 0.3; // June
            m[MonthOffset + 4] = 0.0; // July

            m[OxenIdx] = 13;      // seed inside the viable basin: empirically ~14-16 oxen reliably finish while ~10 strands the
                                  // wagon, so start well above the 5.5-ox mileage break-even (oxen also score 4 pts each)
            m[FoodIdx] = 1400;    // ~56 days at the five-person Filling burn (~25 lb/day); hunting backfills any overrun
            m[ClothesIdx] = 15;   // a full set per person survives the freezing-country cold-exposure penalty; well above the
                                  // 2xliving hail-freeze guard (10) with slack for the Shoshoni guide's 1-5 sets
            m[MedicineIdx] = 7;   // the best per-dollar survival item (heals + clears infection on rest); cold now drives more
                                  // infections, so carry a few more, but it is unscored - don't hoard
            m[AmmoIdx] = 30;      // enough bullets for repeated hunts: a single hunt yields up to 250 lb of meat (~10 party-days
                                  // of food) for one day, so hunting is a cheap way to top up food the tight budget can't buy
            m[WheelIdx] = 1;      // spare parts are re-buyable at every fort, so one of each up front suffices
            m[AxleIdx] = 1;
            m[TongueIdx] = 1;
            m[RestHealthIdx] = 300;  // rest the weakest member at <= Poor: every one of the five x3 heads is worth protecting
            m[RestDaysIdx] = 3;
            m[HuntFoodIdx] = 300; // hunt PROACTIVELY - top the larder up whenever food dips below ~300 rather than waiting for
                                  // near-starvation. With time to spare (wins finish in ~55 of 246 days) and a tight food budget,
                                  // trading a hunting day for up to 250 lb of meat cuts starvation deaths and lifts the win rate.
                                  // Needs ammo on hand: HuntFood and ammo are a deceptive combo - either alone does nothing.
            m[RiverFerryIdx] = 1.0;  // paid/guided crossings have no drowning path at any depth
            m[RiverIndianIdx] = 0.9; // safe AND no time delay (unlike the ferry's 1-9 lost days)
            m[RiverCaulkIdx] = 0.3;  // float/caulk beats ford on the free rivers (safe to depth 5 vs 3)
            m[RiverFordIdx] = 0.2;
            m[ForkSecondIdx] = 0;    // branch 1 routes through forts (resupply) and sidesteps the Columbia crossing
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
