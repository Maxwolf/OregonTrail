using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Window.Travel;
using WeatherCondition = OregonTrailDotNet.Entity.Location.Weather.WeatherConditions;

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

        /// <summary>Average health across the living party (what the end-of-game score is weighted by).</summary>
        public HealthStatus Health { get; init; }

        /// <summary>
        ///     Health of the party's weakest living member — the one nearest to dying. The average can stay high while one
        ///     member is failing, so policies watch this to protect every individual (each death is lost score), not just the
        ///     party as a whole.
        /// </summary>
        public HealthStatus LowestHealth { get; init; }

        public int DaysElapsed { get; init; }
        public int DaysRemaining { get; init; }
        public int Miles { get; init; }
        public RationLevel Ration { get; init; }
        public TravelPace Pace { get; init; }
        public string LocationName { get; init; } = "";

        /// <summary>Current calendar month (1=January .. 12=December) — a season signal for weather and illness risk.</summary>
        public int CurrentMonth { get; init; }

        /// <summary>The weather condition at the party's current location.</summary>
        public WeatherCondition Weather { get; init; }

        /// <summary>Outside temperature at the location, in Celsius — the cold exposure the party's clothing must offset.</summary>
        public int Temperature { get; init; }

        /// <summary>True at a mountain-pass location (slow going, blizzards, a chance of getting stuck).</summary>
        public bool HighGround { get; init; }

        /// <summary>True where the party can resupply at a store (a fort or settlement).</summary>
        public bool ShoppingAllowed { get; init; }

        /// <summary>The current location's index along the trail and the total number of locations — together, trail progress.</summary>
        public int LocationIndex { get; init; }

        public int LocationCount { get; init; }

        /// <summary>
        ///     Pounds of meat bagged so far in the hunt currently in progress (0 when not hunting). Lets the policy decide,
        ///     like a player, when it has enough food and should stop the hunt rather than waste more daylight and bullets.
        /// </summary>
        public int HuntBagged { get; init; }

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
            var loc = game.Trail.CurrentLocation;

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
                LowestHealth = LowestLivingHealth(v),
                DaysElapsed = game.Time.TotalDays,
                DaysRemaining = TimeModule.MaxTravelDays - game.Time.TotalDays,
                Miles = v.Odometer,
                Ration = v.Ration,
                Pace = v.Pace,
                LocationName = loc?.Name ?? "",
                CurrentMonth = (int) game.Time.CurrentMonth,
                Weather = loc?.Weather ?? default,
                Temperature = loc?.Temperature ?? 0,
                HighGround = loc?.HighGround ?? false,
                ShoppingAllowed = loc?.ShoppingAllowed ?? false,
                LocationIndex = game.Trail.LocationIndex,
                LocationCount = game.Trail.Locations.Count,
                HuntBagged = BaggedThisHunt(game)
            };
        }

        // Meat bagged so far in the hunt in progress, read straight off the live hunt session on the travel window, or 0 when
        // the party is not currently hunting.
        private static int BaggedThisHunt(GameSimulationApp game)
        {
            return game.WindowManager.FocusedWindow is Travel travel && travel.ActiveHunt != null
                ? travel.ActiveHunt.KillWeight
                : 0;
        }

        // The worst health among the still-living party members (Dead excluded). Falls back to the party average only if the
        // party is empty, which never happens mid-game.
        private static HealthStatus LowestLivingHealth(Vehicle v)
        {
            HealthStatus? lowest = null;
            foreach (var person in v.Passengers)
            {
                if (person.HealthStatus == HealthStatus.Dead)
                    continue;
                if (lowest == null || (int) person.HealthStatus < (int) lowest)
                    lowest = person.HealthStatus;
            }

            return lowest ?? v.PassengerHealthStatus;
        }
    }
}
