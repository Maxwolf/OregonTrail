// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation.Window.RandomEvent
{
    using Entity;
    using Module.Director;

    /// <summary>
    ///     Random event window is attached by the event director which then listens for the event it will throw at it over
    ///     event delegate the random event window will subscribe to.
    /// </summary>
    public sealed class RandomEvent : Window<RandomEventCommands, RandomEventInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Window{TCommands,TData}" /> class.
        /// </summary>
        /// <param name="simUnit">Core simulation which is controlling the form factory.</param>
        public RandomEvent(SimulationApp simUnit) : base(simUnit)
        {
        }

        /// <summary>
        ///     Called after the Windows has been added to list of modes and made active.
        /// </summary>
        public override void OnWindowPostCreate()
        {
            // Event director has event to know when events are triggered.
            GameSimulationApp.Instance.EventDirector.OnEventTriggered += Director_OnEventTriggered;
        }

        /// <summary>
        ///     Called when the Windows manager in simulation makes this Windows the currently active game Windows. Depending on
        ///     order of
        ///     modes this might not get called until the Windows is actually ticked by the simulation.
        /// </summary>
        public override void OnWindowActivate()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Fired when the simulation adds a game Windows that is not this Windows. Used to execute code in other modes that
        ///     are not
        ///     the active Windows anymore one last time.
        /// </summary>
        public override void OnWindowAdded()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Fired when the event director triggers an event because it rolled the dice and hit it or it was forcefully
        ///     triggered by some method under a defined condition.
        /// </summary>
        /// <param name="simEntity">The sim Entity.</param>
        /// <param name="directorEvent">The director Event.</param>
        private void Director_OnEventTriggered(IEntity simEntity, EventProduct directorEvent)
        {
            // Attached the random event state when we intercept an event it would like us to trigger.
            UserData.DirectorEvent = directorEvent;
            UserData.SourceEntity = simEntity;
            SetForm(typeof (EventExecutor));
        }

        /// <summary>Fired when this game Windows is removed from the list of available and ticked modes in the simulation.</summary>
        protected override void OnModeRemoved()
        {
            base.OnModeRemoved();

            // Event director has event for when he triggers events.
            if (GameSimulationApp.Instance.EventDirector != null)
                GameSimulationApp.Instance.EventDirector.OnEventTriggered -= Director_OnEventTriggered;
        }
    }
}