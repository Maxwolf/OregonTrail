// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventDirectorModule.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Numbers events and allows them to propagate through it and to other parts of the simulation. Lives inside of the
//   game simulation normally.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Game
{
    using System;
    using Core;
    using Entity;
    using Event;

    /// <summary>
    ///     Numbers events and allows them to propagate through it and to other parts of the simulation. Lives inside of the
    ///     game simulation normally.
    /// </summary>
    public sealed class EventDirectorModule : Module
    {
        /// <summary>
        ///     Fired when an event has been triggered by the director.
        /// </summary>
        public delegate void EventTriggered(IEntity simEntity, EventProduct directorEvent);

        /// <summary>
        ///     Creates event items on behalf of the director when he rolls the dice looking for one to trigger.
        /// </summary>
        private EventFactory _eventFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventDirectorModule" /> class.
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.ModuleProduct" /> class.
        /// </summary>
        public EventDirectorModule()
        {
            // Creates a new event factory, and event history list. 
            _eventFactory = new EventFactory();
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            _eventFactory = null;
        }

        /// <summary>
        ///     Fired when an event has been triggered by the director.
        /// </summary>
        public event EventTriggered OnEventTriggered;

        /// <summary>Gathers all of the events by specified type and then rolls the virtual dice to determine if any of the events in
        ///     the enumeration should trigger.</summary>
        /// <param name="sourceEntity">Entities which will be affected by event if triggered.</param>
        /// <param name="eventCategory">Event type the dice will be rolled against and attempted to trigger.</param>
        public void TriggerEventByType(IEntity sourceEntity, EventCategory eventCategory)
        {
            // Roll the dice here to determine if the event is triggered at all.
            var diceRoll = GameSimulationApp.Instance.Random.Next(100);
            if (diceRoll > 0)
                return;

            // Create a random event by type enumeration, event factory will randomly pick one for us based on the enum value.
            var randomEventProduct = _eventFactory.CreateRandomByType(eventCategory);

            // Check to make sure the event returned actually exists.
            if (randomEventProduct == null)
                return;

            // Invokes the event which will give it full control over simulation.
            ExecuteEvent(sourceEntity, randomEventProduct);
        }

        /// <summary>Triggers an event directly by type of reference. Event must have [EventDirector] attribute to be
        ///     registered in the factory correctly.</summary>
        /// <param name="sourceEntity">Entities which will be affected by event if triggered.</param>
        /// <param name="eventType">System type that represents the type of event to trigger.</param>
        public void TriggerEvent(IEntity sourceEntity, Type eventType)
        {
            // Grab the event item from the factory that makes them.
            var eventProduct = _eventFactory.CreateInstance(eventType);
            ExecuteEvent(sourceEntity, eventProduct);
        }

        /// <summary>Primary worker for the event factory, pulled into it's own method here so all the trigger event types can call it.
        ///     This will attach the random event game Windows and then fire an event to trigger the event execution in that
        ///     Windows
        ///     then it will be able to display any relevant data about what happened.</summary>
        /// <param name="sourceEntity">Entities which will be affected by event if triggered.</param>
        /// <param name="directorEvent">Created instance of event that will be executed on simulation in random game Windows.</param>
        private void ExecuteEvent(IEntity sourceEntity, EventProduct directorEvent)
        {
            // Attach random event game Windows before triggering event since it will listen for it using event delegate.
            GameSimulationApp.Instance.WindowManager.Add(GameWindow.RandomEvent);

            // Fire off event so primary game simulation knows we executed an event with an event.
            OnEventTriggered?.Invoke(sourceEntity, directorEvent);
        }
    }
}