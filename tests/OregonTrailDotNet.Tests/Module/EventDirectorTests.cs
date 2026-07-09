using System;
using System.Linq;
using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Event;
using OregonTrailDotNet.Event.Person;
using OregonTrailDotNet.Module.Director;
using Xunit;
using PersonEntity = OregonTrailDotNet.Entity.Person.Person;

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
            foreach (EventCategory category in Enum.GetValues(typeof(EventCategory)))
            {
                var created = factory.CreateRandomByType(category);

                Assert.NotNull(created);
                var attribute = created.GetType().GetCustomAttributes<DirectorEventAttribute>(false).First();
                Assert.Equal(category, attribute.EventCategory);
                Assert.Equal(EventExecution.RandomOrManual, attribute.EventExecutionType);
            }
        }

        [Fact]
        public void TriggerEvent_ExecutesEventAndNotifiesSubscribers()
        {
            var person = new PersonEntity(Profession.Banker, "Alice", true);
            person.Damage(200);
            Assert.Equal(HealthStatus.Poor, person.HealthStatus);

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
            Assert.Equal(HealthStatus.Good, person.HealthStatus);
        }

        [Fact]
        public void EventKey_EqualityIsByValue()
        {
            var key = new EventKey(EventCategory.Person, "TestEvent", EventExecution.RandomOrManual);
            var sameKey = new EventKey(EventCategory.Person, "TestEvent", EventExecution.RandomOrManual);
            var otherKey = new EventKey(EventCategory.Person, "OtherEvent", EventExecution.RandomOrManual);

            Assert.True(key.Equals(sameKey));
            Assert.False(key.Equals(otherKey));
        }
    }
}
