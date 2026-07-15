using System.Text.Json;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;

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
        private const int PaceIdx = 24;
        private const int RationIdx = 25;

        public const int Length = 26;

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

        // Pace and rations are learned menu choices (1..3) instead of being hardcoded, so the optimizer can trade speed for
        // the party's health. Pace menu numbers line up with the TravelPaceEnum enum directly (1=Steady, 2=Strenuous, 3=Grueling).
        // Ration menu numbers are the REVERSE of the RationLevelEnum enum (menu 1=Filling..3=BareBones, but enum Filling=3..
        // BareBones=1), so DesiredRation maps the menu number back to the enum for comparing against the live ration.
        public int PaceChoice => ClampRound(Raw[PaceIdx], 1, 3);
        public int RationChoice => ClampRound(Raw[RationIdx], 1, 3);
        public TravelPaceEnum DesiredPace => (TravelPaceEnum) PaceChoice;
        public RationLevelEnum DesiredRation => (RationLevelEnum) (4 - RationChoice);

        public int TargetQuantity(EntitiesEnum item) => item switch
        {
            EntitiesEnum.Animal => OxenTarget,
            EntitiesEnum.Food => FoodTarget,
            EntitiesEnum.Clothes => ClothesTarget,
            EntitiesEnum.Medicine => MedicineTarget,
            EntitiesEnum.Ammo => AmmoTarget,
            EntitiesEnum.Wheel => WheelTarget,
            EntitiesEnum.Axle => AxleTarget,
            EntitiesEnum.Tongue => TongueTarget,
            _ => 0
        };

        public double RiverScore(RiverChoiceKindEnum kind) => kind switch
        {
            RiverChoiceKindEnum.Ferry => Raw[RiverFerryIdx],
            RiverChoiceKindEnum.Indian => Raw[RiverIndianIdx],
            RiverChoiceKindEnum.Caulk => Raw[RiverCaulkIdx],
            RiverChoiceKindEnum.Ford => Raw[RiverFordIdx],
            _ => double.NegativeInfinity // never electively wait or ask for more info
        };

        // ---- Serialization ----

        public string ToJson() => JsonSerializer.Serialize(Raw);

        public static StrategyGenome FromJson(string json)
        {
            var raw = JsonSerializer.Deserialize<double[]>(json);
            return new StrategyGenome { Raw = raw == null ? DefaultMean() : Sized(raw) };
        }

        /// <summary>Fits a raw vector to the current gene layout: copies what overlaps and zero-fills the rest. Tolerates a
        ///     vector saved under an older, shorter (or longer) layout so decoding never indexes out of range.</summary>
        public static double[] Sized(double[] raw)
        {
            var sized = new double[Length];
            Array.Copy(raw, sized, Math.Min(raw.Length, Length));
            return sized;
        }

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
            m[ProfessionOffset + 0] = 0.0; // Banker   (x1) - score-dominated, never seeded toward
            m[ProfessionOffset + 1] = 0.7; // Carpenter (x2) - the reliable fallback, now within ~1 std of the argmax
            m[ProfessionOffset + 2] = 1.1; // Farmer   (x3) - still the seeded argmax, but the gap to Carpenter is small enough
                                           //                 that the optimizer genuinely explores both professions for headroom

            // Start-month preference (argmax over indices 0..4 -> March..July). May balances a warm-enough departure against
            // reaching the high passes before winter; March is cold (more hail/illness weather), July risks winter mountains.
            m[MonthOffset + 0] = 0.0; // March
            m[MonthOffset + 1] = 0.3; // April
            m[MonthOffset + 2] = 1.0; // May   (argmax -> StartMonth 3)
            m[MonthOffset + 3] = 0.3; // June
            m[MonthOffset + 4] = 0.0; // July

            // The whole loadout is kept comfortably UNDER the Farmer's $400 so the opening store applies it in a single clean
            // pass (over-budgeting trips a debt-warning path that leaves a broken partial loadout). Roughly: 6 oxen $120 +
            // 800 food $80 + 10 clothes $100 + 2 medicine $30 + 15 ammo $30 + 1 of each spare part $30 ~= $390.
            m[OxenIdx] = 6;       // the strategy-guide team: 3 yoke pulls at the full daily maximum now that mileage scales by
                                  // oxen COUNT (not $ value), so a small team is fast and cheap - freeing the budget the old
                                  // ~16-ox teams swallowed for FOOD and clothing, which is the whole unlock.
            m[FoodIdx] = 800;     // carry the journey's food outright. Cheap oxen free enough budget to buy ~a season of food
                                  // (~55 travel days x 15 lb Filling), so the party isn't forced to rely on hunting - which this
                                  // port only offers intermittently in the travel menu - and won't starve mid-drive.
            m[ClothesIdx] = 10;   // a big clothing buy protects health in the freezing country and leaves enough for the
                                  // Shoshoni guide's 1-5 sets at the Snake River
            m[MedicineIdx] = 2;   // a couple of kits (they treat the sick while travelling); rest + food does most of the healing
            m[AmmoIdx] = 15;      // some bullets to top up food by hunting when the option is offered, and to defend the party
            m[WheelIdx] = 1;      // spare parts are re-buyable at every fort, so one of each up front suffices
            m[AxleIdx] = 1;
            m[TongueIdx] = 1;
            m[RestHealthIdx] = 300;  // rest the weakest member at <= Poor: every one of the five x3 heads is worth protecting
            m[RestDaysIdx] = 3;
            m[HuntFoodIdx] = 300; // top the larder up at a location whenever it dips below ~300 lb. The party can only stop to
                                  // hunt at locations and a drive between them runs many days, so it enters each drive with a
                                  // reserve. (Hunting alone can't fully offset the heavy in-trail mileage setbacks - see notes -
                                  // but keeps food positive longer.) Needs ammo on hand.
            m[RiverFerryIdx] = 1.0;  // paid/guided crossings have no drowning path at any depth
            m[RiverIndianIdx] = 0.9; // safe AND no time delay (unlike the ferry's 1-9 lost days)
            m[RiverCaulkIdx] = 0.3;  // float/caulk beats ford on the free rivers (safe to depth 5 vs 3)
            m[RiverFordIdx] = 0.2;
            m[ForkSecondIdx] = 0;    // branch 1 routes through forts (resupply) and sidesteps the Columbia crossing
            m[PaceIdx] = 3;          // Grueling (menu 3): the high-score default - covers the most ground per day. The optimizer
                                     // can now learn to ease off (Strenuous/Steady) to protect the party's health when needed.
            m[RationIdx] = 1;        // Filling (menu 1): keeps the party healthiest; the optimizer may stretch to Meager/BareBones
                                     // to make the food last when it is short.
            return m;
        }

        /// <summary>Per-dimension exploration widths for the initial CEM distribution.</summary>
        public static double[] DefaultStd()
        {
            var s = new double[Length];
            for (var i = ProfessionOffset; i < OxenIdx; i++) s[i] = 1.2; // categorical logits: wider so both professions and
                                                                         // more start months are genuinely explored
            s[OxenIdx] = 4; s[FoodIdx] = 400; s[ClothesIdx] = 6; s[MedicineIdx] = 4; s[AmmoIdx] = 12;
            s[WheelIdx] = 1; s[AxleIdx] = 1; s[TongueIdx] = 1;
            s[RestHealthIdx] = 80; s[RestDaysIdx] = 2; s[HuntFoodIdx] = 40;
            s[RiverFerryIdx] = 0.5; s[RiverIndianIdx] = 0.5; s[RiverCaulkIdx] = 0.5; s[RiverFordIdx] = 0.5;
            s[ForkSecondIdx] = 1.0;
            s[PaceIdx] = 0.9; s[RationIdx] = 0.9; // enough spread to explore Strenuous/Steady and Meager/BareBones
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
