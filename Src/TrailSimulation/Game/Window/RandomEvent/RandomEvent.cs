using System;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Attached by the event director when it wants to execute an event against the simulation. It will attach this
    ///     Windows, which then hooks the event delegate it will trigger right after this class finishes initializing.
    /// </summary>
    public sealed class RandomEvent : Window<RandomEventCommands, RandomEventInfo>
    {
        /// <summary>
        ///     Defines the current game Windows the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameWindow Windows
        {
            get { return GameWindow.RandomEvent; }
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
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the simulation adds a game Windows that is not this Windows. Used to execute code in other modes that
        ///     are not
        ///     the active Windows anymore one last time.
        /// </summary>
        public override void OnWindowAdded()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the event director triggers an event because it rolled the dice and hit it or it was forcefully
        ///     triggered by some method under a defined condition.
        /// </summary>
        private void Director_OnEventTriggered(IEntity simEntity, EventProduct directorEvent)
        {
            // Attached the random event state when we intercept an event it would like us to trigger.
            UserData.DirectorEvent = directorEvent;
            UserData.SourceEntity = simEntity;
            SetForm(typeof (EventExecutor));
        }

        /// <summary>
        ///     Fired when this game Windows is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        protected override void OnModeRemoved(GameWindow windows)
        {
            base.OnModeRemoved(windows);

            // Event director has event for when he triggers events.
            if (GameSimulationApp.Instance.EventDirector != null)
                GameSimulationApp.Instance.EventDirector.OnEventTriggered -= Director_OnEventTriggered;
        }
    }
}