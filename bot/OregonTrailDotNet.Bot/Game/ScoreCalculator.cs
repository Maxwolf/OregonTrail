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

            // People still alive, weighted by average health enum value (Good=500 .. VeryPoor=200).
            total += vehicle.PassengerLivingCount * (int) vehicle.PassengerHealthStatus;

            // Wagon. Kept verbatim: the shipped game reads Resources.Vehicle.Points off a template item whose quantity is 0,
            // so this contributes 0 in practice. Calling the same property guarantees the bot matches the game exactly.
            total += Resources.Vehicle.Points;

            // Oxen.
            total += inv[Entities.Animal].Points;

            // Spare wagon parts.
            total += inv[Entities.Axle].Points + inv[Entities.Tongue].Points + inv[Entities.Wheel].Points;

            // Clothing, bullets, food, cash.
            total += inv[Entities.Clothes].Points;
            total += inv[Entities.Ammo].Points;
            total += inv[Entities.Food].Points;
            total += inv[Entities.Cash].Points;

            return total;
        }
    }
}
