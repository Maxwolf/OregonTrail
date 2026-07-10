using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Module.Time;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Read-only snapshot of the things a policy needs to make a decision, captured from the live game singleton. Keeping
    ///     the policy behind this view (rather than handing it the whole simulation) keeps decisions testable and decoupled
    ///     from game internals.
    /// </summary>
    public sealed class GameSnapshot
    {
        public int Oxen { get; init; }
        public int Food { get; init; }
        public int Clothing { get; init; }
        public int Ammo { get; init; }
        public int Medicine { get; init; }
        public int Wheels { get; init; }
        public int Axles { get; init; }
        public int Tongues { get; init; }
        public int Cash { get; init; }

        /// <summary>Passengers still alive.</summary>
        public int LivingCount { get; init; }

        /// <summary>Total party size including the dead.</summary>
        public int PartySize { get; init; }

        public HealthStatus Health { get; init; }
        public int DaysElapsed { get; init; }
        public int DaysRemaining { get; init; }
        public int Miles { get; init; }
        public RationLevel Ration { get; init; }
        public TravelPace Pace { get; init; }
        public string LocationName { get; init; } = "";

        /// <summary>True while nobody in the party has died yet.</summary>
        public bool AllAlive => LivingCount >= PartySize && PartySize > 0;

        /// <summary>Current owned quantity of a purchasable store item.</summary>
        public int OwnedOf(Entities item) => item switch
        {
            Entities.Animal => Oxen,
            Entities.Food => Food,
            Entities.Clothes => Clothing,
            Entities.Ammo => Ammo,
            Entities.Medicine => Medicine,
            Entities.Wheel => Wheels,
            Entities.Axle => Axles,
            Entities.Tongue => Tongues,
            _ => 0
        };

        public static GameSnapshot Capture(GameSimulationApp game)
        {
            var v = game.Vehicle;
            var inv = v.Inventory;

            int Qty(Entities e) => inv.TryGetValue(e, out var item) ? item.Quantity : 0;

            return new GameSnapshot
            {
                Oxen = Qty(Entities.Animal),
                Food = Qty(Entities.Food),
                Clothing = Qty(Entities.Clothes),
                Ammo = Qty(Entities.Ammo),
                Medicine = Qty(Entities.Medicine),
                Wheels = Qty(Entities.Wheel),
                Axles = Qty(Entities.Axle),
                Tongues = Qty(Entities.Tongue),
                Cash = Qty(Entities.Cash),
                LivingCount = v.PassengerLivingCount,
                PartySize = v.Passengers.Count,
                Health = v.PassengerHealthStatus,
                DaysElapsed = game.Time.TotalDays,
                DaysRemaining = TimeModule.MaxTravelDays - game.Time.TotalDays,
                Miles = v.Odometer,
                Ration = v.Ration,
                Pace = v.Pace,
                LocationName = game.Trail.CurrentLocation?.Name ?? ""
            };
        }
    }
}
