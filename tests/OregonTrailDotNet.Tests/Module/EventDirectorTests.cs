using System;
using System.Linq;
using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Event;
using OregonTrailDotNet.Event.Person;
using OregonTrailDotNet.Event.Vehicle;
using OregonTrailDotNet.Event.Wild;
using OregonTrailDotNet.Module.Director;
using Xunit;
using PersonEntity = OregonTrailDotNet.Entity.Person.Person;
using VehicleEntity = OregonTrailDotNet.Entity.Vehicle.Vehicle;

namespace OregonTrailDotNet.Tests.Module
{
    /// <summary>
    ///     Covers the reflection-driven random event system: attribute discovery, factory creation for
    ///     every registered event, and the full synchronous trigger-execute-notify pipeline.
    /// </summary>
    public class EventDirectorTests : SimulationTestBase
    {
        /// <summary>
        ///     Every concrete event class in the game assembly, found the same way EventFactory does.
        /// </summary>
        private static Type[] ConcreteEventTypes =>
            typeof(EventProduct).Assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(EventProduct)) && !type.GetTypeInfo().IsAbstract)
                .ToArray();

        [Fact]
        public void EveryConcreteEvent_CarriesDirectorEventAttribute()
        {
            // An event class missing the attribute would be invisible to the factory and crash the
            // game the first time something tried to trigger it.
            foreach (var eventType in ConcreteEventTypes)
                Assert.True(eventType.GetCustomAttributes<DirectorEventAttribute>(false).Any(),
                    $"{eventType.Name} is an EventProduct but has no [DirectorEvent] attribute!");
        }

        [Fact]
        public void EventFactory_CreatesInstanceOfEveryRegisteredEvent()
        {
            var factory = new EventFactory();

            foreach (var eventType in ConcreteEventTypes)
            {
                var created = factory.CreateInstance(eventType);

                Assert.NotNull(created);
                Assert.IsType(eventType, created);
                Assert.Equal(eventType.Name, created.Name);
            }
        }

        [Fact]
        public void EventFactory_UnregisteredType_Throws()
        {
            var factory = new EventFactory();

            Assert.Throws<ArgumentException>(() => factory.CreateInstance(typeof(string)));
        }

        [Fact]
        public void EventFactory_CreateRandomByType_HonorsRequestedCategory()
        {
            var factory = new EventFactory();

            // Every category in the game ships with at least one randomly-triggerable event.
            foreach (EventCategoryEnum category in Enum.GetValues(typeof(EventCategoryEnum)))
            {
                var created = factory.CreateRandomByType(category);

                Assert.NotNull(created);
                var attribute = created.GetType().GetCustomAttributes<DirectorEventAttribute>(false).First();
                Assert.Equal(category, attribute.EventCategory);
                Assert.Equal(EventExecutionEnum.RandomOrManual, attribute.EventExecutionType);
            }
        }

        [Fact]
        public void TriggerEvent_ExecutesEventAndNotifiesSubscribers()
        {
            var person = new PersonEntity(ProfessionEnum.Banker, "Alice", true);
            person.Damage(70);
            Assert.Equal(HealthStatusEnum.Poor, person.HealthStatus);

            IEntity notifiedEntity = null;
            EventProduct notifiedEvent = null;
            Game.EventDirector.OnEventTriggered += (entity, directorEvent) =>
            {
                notifiedEntity = entity;
                notifiedEvent = directorEvent;
            };

            // Events execute synchronously through the random event window as soon as triggered.
            Game.EventDirector.TriggerEvent(person, typeof(WellAgain));

            Assert.Same(person, notifiedEntity);
            Assert.IsType<WellAgain>(notifiedEvent);
            Assert.Equal(HealthStatusEnum.Good, person.HealthStatus);
        }

        [Fact]
        public void TriggerEvent_BrokenPartOnNonSingletonVehicle_RendersWithoutNullReference()
        {
            // A vehicle that is NOT the game singleton's vehicle (as the detached-vehicle unit tests use). The broken-part
            // prompt must describe the part on the event's source vehicle, not blindly reach for the singleton's — else it
            // null-references. Executes synchronously through the random event window, so the prompt renders inside here.
            var vehicle = new VehicleEntity();
            Assert.NotSame(Game.Vehicle, vehicle);

            Game.EventDirector.TriggerEvent(vehicle, typeof(BrokenVehiclePart));

            Assert.NotNull(vehicle.BrokenPart);
            var screen = Game.WindowManager.FocusedWindow?.OnRenderWindow() ?? string.Empty;
            Assert.Contains("repair it", screen);
            Assert.Contains(vehicle.BrokenPart.Name.ToLowerInvariant(), screen);
        }

        [Fact]
        public void TriggerEvent_MagicRepair_ClearsTheBrokenPart()
        {
            // The "you were able to repair it" outcome must clear the broken part: CheckStatus treats a lingering broken
            // part as disabling, so a stale flag would re-strand the wagon long after the repair (and silently block any
            // future part from ever breaking again).
            var vehicle = Game.Vehicle;
            vehicle.Inventory[EntitiesEnum.Animal].AddQuantity(2);
            vehicle.BreakRandomPart();
            Assert.NotNull(vehicle.BrokenPart);
            vehicle.Status = VehicleStatusEnum.Disabled;

            Game.EventDirector.TriggerEvent(vehicle, typeof(RepairVehiclePart));

            Assert.Null(vehicle.BrokenPart);
            Assert.Equal(VehicleStatusEnum.Stopped, vehicle.Status);

            // And with the part cleared, the wagon rolls again on the next status check.
            vehicle.CheckStatus();
            Assert.Equal(VehicleStatusEnum.Moving, vehicle.Status);
        }

        [Fact]
        public void FoodSpoilage_WithFoodInTheOldCrashRange_DoesNotThrow()
        {
            // 8 pounds -> spoiledFood = 8 / 4 = 2 -> Random.Next(3, 2) used to throw ArgumentOutOfRangeException. The
            // guard now requires at least 12 pounds (so a quarter is >= the three-piece minimum) and spoils nothing
            // below that, executing harmlessly instead of crashing the game.
            var vehicle = Game.Vehicle;
            vehicle.ResetVehicle();
            vehicle.Inventory[EntitiesEnum.Food].AddQuantity(8);

            var ex = Record.Exception(() => Game.EventDirector.TriggerEvent(vehicle, typeof(FoodSpoilage)));

            Assert.Null(ex);
            Assert.Equal(8, vehicle.Inventory[EntitiesEnum.Food].Quantity);
        }

        [Fact]
        public void EventKey_EqualityIsByValue()
        {
            var key = new EventKey(EventCategoryEnum.Person, "TestEvent", EventExecutionEnum.RandomOrManual);
            var sameKey = new EventKey(EventCategoryEnum.Person, "TestEvent", EventExecutionEnum.RandomOrManual);
            var otherKey = new EventKey(EventCategoryEnum.Person, "OtherEvent", EventExecutionEnum.RandomOrManual);

            Assert.True(key.Equals(sameKey));
            Assert.False(key.Equals(otherKey));
        }
    }
}
