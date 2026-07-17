using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Trade;
using WeatherCondition = OregonTrailDotNet.Entity.Location.Weather.WeatherConditionsEnum;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>The emigrant trade currently on the table, read off the live Trading form for the policy to judge.</summary>
    public readonly record struct TradeOfferView(
        EntitiesEnum Offered, int OfferedQuantity, EntitiesEnum Wanted, int WantedQuantity, bool CanPay);

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
        public HealthStatusEnum Health { get; init; }

        /// <summary>
        ///     Health of the party's weakest living member — the one nearest to dying. The average can stay high while one
        ///     member is failing, so policies watch this to protect every individual (each death is lost score), not just the
        ///     party as a whole.
        /// </summary>
        public HealthStatusEnum LowestHealth { get; init; }

        public int DaysElapsed { get; init; }
        public int DaysRemaining { get; init; }
        public int Miles { get; init; }
        public RationLevelEnum Ration { get; init; }
        public TravelPaceEnum Pace { get; init; }
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

        /// <summary>Category of the wagon's currently broken part, or null when nothing is broken. A broken part with no
        ///     spare on hand disables the wagon just like having no oxen does.</summary>
        public EntitiesEnum? BrokenPart { get; init; }

        /// <summary>The emigrant trade currently on the table (null when not on the trading screen or nobody offers).</summary>
        public TradeOfferView? Trade { get; init; }

        /// <summary>True while nobody in the party has died yet.</summary>
        public bool AllAlive => LivingCount >= PartySize && PartySize > 0;

        /// <summary>
        ///     True at the last few stops before Oregon City, where the journey is effectively made: schedule pressure no
        ///     longer applies (the game has no time limit), so policies may rest freely and run the endgame score grind.
        /// </summary>
        public bool NearTrailEnd => LocationIndex >= LocationCount - 3;

        /// <summary>Current owned quantity of a purchasable store item.</summary>
        public int OwnedOf(EntitiesEnum item) => item switch
        {
            EntitiesEnum.Animal => Oxen,
            EntitiesEnum.Food => Food,
            EntitiesEnum.Clothes => Clothing,
            EntitiesEnum.Ammo => Ammo,
            EntitiesEnum.Medicine => Medicine,
            EntitiesEnum.Wheel => Wheels,
            EntitiesEnum.Axle => Axles,
            EntitiesEnum.Tongue => Tongues,
            _ => 0
        };

        public static GameSnapshot Capture(GameSimulationApp game)
        {
            var v = game.Vehicle;
            var inv = v.Inventory;
            var loc = game.Trail.CurrentLocation;

            int Qty(EntitiesEnum e) => inv.TryGetValue(e, out var item) ? item.Quantity : 0;

            return new GameSnapshot
            {
                Oxen = Qty(EntitiesEnum.Animal),
                Food = Qty(EntitiesEnum.Food),
                Clothing = Qty(EntitiesEnum.Clothes),
                Ammo = Qty(EntitiesEnum.Ammo),
                Medicine = Qty(EntitiesEnum.Medicine),
                Wheels = Qty(EntitiesEnum.Wheel),
                Axles = Qty(EntitiesEnum.Axle),
                Tongues = Qty(EntitiesEnum.Tongue),
                Cash = Qty(EntitiesEnum.Cash),
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
                HuntBagged = BaggedThisHunt(game),
                BrokenPart = v.BrokenPart?.Category,
                Trade = CurrentTrade(game)
            };
        }

        // The offer shown on the live Trading form, or null when the party is not at the trading screen (same live-form
        // seam as BaggedThisHunt below).
        private static TradeOfferView? CurrentTrade(GameSimulationApp game)
        {
            if (game.WindowManager.FocusedWindow is not Travel travel || travel.CurrentForm is not Trading trading)
                return null;

            var offer = trading.CurrentOffer;
            if (offer?.OfferedItem == null || offer.WantedItem == null)
                return null;

            return new TradeOfferView(offer.OfferedItem.Category, offer.OfferedItem.Quantity,
                offer.WantedItem.Category, offer.WantedItem.Quantity, trading.PlayerCanTrade);
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
        private static HealthStatusEnum LowestLivingHealth(Vehicle v)
        {
            HealthStatusEnum? lowest = null;
            foreach (var person in v.Passengers)
            {
                if (person.HealthStatus == HealthStatusEnum.Dead)
                    continue;
                if (lowest == null || (int) person.HealthStatus < (int) lowest)
                    lowest = person.HealthStatus;
            }

            return lowest ?? v.PassengerHealthStatus;
        }
    }
}
