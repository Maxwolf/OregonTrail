using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A hand-tuned, non-learning strategy whose goal is to reliably FINISH the trail. It doubles as a smoke-test driver
    ///     (a policy that completes a full game exercises every screen in the dispatch table) and as a sane initial mean for
    ///     the CEM optimizer. Priorities: buy plenty of oxen/food/clothing/medicine up front, travel steadily on filling
    ///     rations, rest when the party gets sick, and cross rivers by the safest available method.
    /// </summary>
    public sealed class HeuristicPolicy : IPolicy
    {
        public string Name => "heuristic";

        // Carpenter: the x2 multiplier doubles the old Banker build's score, while $800 still comfortably provisions a full
        // five-person party. Farmer's x3 is the true ceiling (the only route to Meek's 7650) but its $400 can't reliably keep
        // five alive - the learning genome chases that fragile frontier; this expert's job is to finish healthy, every time.
        public int Profession => 2;

        // May — enough grass, not so late that the party hits the winter mountains.
        public int StartMonth => 3;

        public string LeaderName => "Trailblazer (bot)";

        public int TargetQuantity(Entities item, GameSnapshot state) => item switch
        {
            Entities.Animal => 12,   // a strong team finishes fast - fewer days on the trail means less of the ~25 lb/day
                                     // five-person food burn - with slack to survive ox deaths and a bad river crossing
            Entities.Food => 1600,   // ~64 days at 25 lb/day; hunting covers any overrun before the 2000 lb cap
            Entities.Clothes => 13,  // above the 10-set hail-freeze guard for a party of five, with slack for the Shoshoni guide
            Entities.Medicine => 6,  // the decisive survival lever (heals + clears infection while resting); unscored, so 6 is plenty
            Entities.Ammo => 20,
            Entities.Wheel => 2,
            Entities.Axle => 2,
            Entities.Tongue => 2,
            _ => 0
        };

        public TravelCommands ChooseTravel(GameSnapshot state, IReadOnlyCollection<TravelCommands> available)
        {
            // Recover when ANY party member is sick (not just when the average dips) and there's still time — keeping every
            // member alive and healthy is what maximizes the final score. Resting heals through natural recovery with or
            // without medicine, and medicine now also treats the sick on the move, so resting is no longer gated on it.
            if (available.Contains(TravelCommands.StopToRest) &&
                (int) state.LowestHealth <= (int) HealthStatus.Poor &&
                state.DaysRemaining > 60)
                return TravelCommands.StopToRest;

            // Top up food by hunting only if it runs dangerously low and we can shoot.
            if (available.Contains(TravelCommands.HuntForFood) && state.Food < 50 && state.Ammo > 0)
                return TravelCommands.HuntForFood;

            return available.Contains(TravelCommands.ContinueOnTrail)
                ? TravelCommands.ContinueOnTrail
                : available.First();
        }

        public int Pace(GameSnapshot state) => 1;   // Steady (pace does not affect mileage in this port; safest for health)
        public int Ration(GameSnapshot state) => 1; // Filling (eats the least and lowers illness in this port)
        public int RestDays(GameSnapshot state) => 3;

        public bool YesNo(string formName, GameSnapshot state) => formName switch
        {
            "VehicleBrokenPrompt" => true,  // always attempt a repair
            "TollRoadQuestion" => true,     // pay the (cheap) toll to keep moving
            "LocationArrive" => true,       // look around so we reach the travel menu (lets us rest/decide)
            "UseFerryConfirm" => true,
            "IndianGuidePrompt" => true,
            "Trading" => false,             // ignore trades
            "TombstoneQuestion" => false,   // don't stop at gravesites
            _ => false
        };

        public RiverChoiceKind River(GameSnapshot state, IReadOnlyCollection<RiverChoiceKind> options)
        {
            // Prefer the crossings that can't drown the wagon (and its oxen): the ferry, then the Shoshoni guide (safe but
            // costs clothing). Only fall back to caulk/ford, which risk a wash-out in deep water. If the guide turns out to be
            // unaffordable the recognizer's bounce guard forces a free crossing.
            foreach (var preferred in new[] { RiverChoiceKind.Ferry, RiverChoiceKind.Indian, RiverChoiceKind.Caulk, RiverChoiceKind.Ford })
                if (options.Contains(preferred))
                    return preferred;

            return options.FirstOrDefault();
        }

        public int Fork(GameSnapshot state, int branchCount) => 1; // first (generally the safer, fort-bearing route)
    }
}
