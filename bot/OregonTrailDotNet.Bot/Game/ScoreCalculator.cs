using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Recomputes the end-of-game score directly from live <see cref="Entity.Vehicle.Vehicle" /> state, mirroring the
    ///     tabulation in <c>src/Window/GameOver/FinalPoints.cs</c> line-for-line. The game itself only ever holds the final
    ///     number as a local variable inside that form (it is added to the scoring module and then discarded), so the bot
    ///     reproduces the formula here to obtain its own fitness signal. A test asserts this equals the value the game records.
    /// </summary>
    public static class ScoreCalculator
    {
        /// <summary>
        ///     Total score including the profession multiplier, exactly as <c>FinalPoints.OnDialogPrompt</c> computes it.
        /// </summary>
        public static int Compute(Entity.Vehicle.Vehicle vehicle)
        {
            var leader = vehicle.PassengerLeader;
            if (leader == null)
                return 0;

            return ComputeBase(vehicle) * (int) leader.Profession;
        }

        /// <summary>
        ///     Score before the profession multiplier (the "Total:" line in the game's tally).
        /// </summary>
        public static int ComputeBase(Entity.Vehicle.Vehicle vehicle)
        {
            var inv = vehicle.Inventory;

            var total = 0;

            // People still alive, weighted by average health enum value (Good=500 .. VeryPoor=200). A party that committed
            // to the Columbia is scored on the health they had when they chose it rather than on whatever the river left
            // them with, so read the locked-in value when there is one — FinalPoints does the same, and this has to agree
            // with it or the bot trains against a fitness signal the game never awards.
            var health = vehicle.LockedHealthStatus ?? vehicle.PassengerHealthStatus;
            total += vehicle.PassengerLivingCount * (int) health;

            // Wagon. The party always travels in exactly one wagon, so award its per-unit points (50). Mirrors the matching
            // line in FinalPoints so the bot's fitness signal still equals the score the game records. (Previously this read
            // Resources.Vehicle.Points off a quantity-0 template, which scored 0 — the reason the seeded 7650 was unreachable.)
            total += Resources.Vehicle.PointsAwarded;

            // Oxen.
            total += inv[EntitiesEnum.Animal].Points;

            // Spare wagon parts.
            total += inv[EntitiesEnum.Axle].Points + inv[EntitiesEnum.Tongue].Points + inv[EntitiesEnum.Wheel].Points;

            // Clothing, bullets, food, cash.
            total += inv[EntitiesEnum.Clothes].Points;
            total += inv[EntitiesEnum.Ammo].Points;
            total += inv[EntitiesEnum.Food].Points;
            total += inv[EntitiesEnum.Cash].Points;

            return total;
        }
    }
}
