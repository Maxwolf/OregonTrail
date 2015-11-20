using System;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Numbers events and allows them to propagate through it and to other parts of the simulation. Lives inside of the
    ///     game simulation normally.
    /// </summary>
    [SimulationModule]
    public sealed class DirectorModule : SimulationModule
    {
        /// <summary>
        ///     Fired when an event has been triggered by the director.
        /// </summary>
        public delegate void EventTriggered(IEntity simEntity, DirectorEvent directorEvent);

        /// <summary>
        ///     Creates event items on behalf of the director when he rolls the dice looking for one to trigger.
        /// </summary>
        private EventFactory _eventFactory;

        /// <summary>
        ///     Determines how important this module is to the simulation in regards to when it should be ticked after sorting all
        ///     loaded modules by this priority level.
        /// </summary>
        public override ModulePriority Priority
        {
            get { return ModulePriority.None; }
        }

        /// <summary>
        ///     Holds reference to the type of class that will be treated as a simulation module.
        /// </summary>
        public override ModuleCategory Category
        {
            get { return ModuleCategory.Application; }
        }

        /// <summary>
        ///     Fired when an event has been triggered by the director.
        /// </summary>
        public event EventTriggered OnEventTriggered;

        /// <summary>
        ///     Gathers all of the events by specified type and then rolls the virtual dice to determine if any of the events in
        ///     the enumeration should trigger.
        /// </summary>
        /// <param name="sourceEntity">Entity which will be affected by event if triggered.</param>
        /// <param name="eventCategory">Event type the dice will be rolled against and attempted to trigger.</param>
        public void TriggerEventByType(IEntity sourceEntity, EventCategory eventCategory)
        {
            // Roll the dice here to determine if the event is triggered at all.
            var diceRoll = GameSimulationApp.Instance.Randomizer.Next(100);
            if (diceRoll > 0)
                return;

            // Create a random event by type enumeration, event factory will randomly pick one for us based on the enum value.
            var randomEventProduct = _eventFactory.CreateRandomByType(eventCategory);
            ExecuteEvent(sourceEntity, randomEventProduct);
        }

        /// <summary>
        ///     Triggers an event directly by type of reference. Event must have [EventDirectorModule] attribute to be registered
        ///     in
        ///     the
        ///     factory correctly.
        /// </summary>
        /// <param name="sourceEntity">Entity which will be affected by event if triggered.</param>
        /// <param name="eventType">System type that represents the type of event to trigger.</param>
        public void TriggerEvent(IEntity sourceEntity, Type eventType)
        {
            // Grab the event item from the factory that makes them.
            var eventProduct = _eventFactory.CreateInstance(eventType);
            ExecuteEvent(sourceEntity, eventProduct);
        }

        /// <summary>
        ///     Primary worker for the event factory, pulled into it's own method here so all the trigger event types can call it.
        ///     This will attach the random event game mode and then fire an event to trigger the event execution in that mode
        ///     then it will be able to display any relevant data about what happened.
        /// </summary>
        /// <param name="sourceEntity">Entity which will be affected by event if triggered.</param>
        /// <param name="directorEvent">Created instance of event that will be executed on simulation in random game mode.</param>
        private void ExecuteEvent(IEntity sourceEntity, DirectorEvent directorEvent)
        {
            // Attach random event game mode before triggering event since it will listen for it using event delegate.
            GameSimulationApp.Instance.WindowManager.AddMode(GameMode.RandomEvent);

            // Fire off event so primary game simulation knows we executed an event with an event.
            OnEventTriggered?.Invoke(sourceEntity, directorEvent);
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void OnModuleDestroy()
        {
            _eventFactory = null;
        }

        /// <summary>
        ///     Fired when the simulation loads and creates the module and allows it to create any data structures it cares about
        ///     without calling constructor.
        /// </summary>
        public override void OnModuleCreate()
        {
            // Creates a new event factory, and event history list. 
            _eventFactory = new EventFactory();
        }
    }
}