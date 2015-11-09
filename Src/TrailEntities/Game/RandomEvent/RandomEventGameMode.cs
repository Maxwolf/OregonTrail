using TrailEntities.Entity;
using TrailEntities.Event;
using TrailEntities.Game.Travel;
using TrailEntities.Mode;
using TrailEntities.Simulation;

namespace TrailEntities.Game.RandomEvent
{
    /// <summary>
    ///     Attached by the event director when it wants to execute an event against the simulation. It will attach this gameMode,
    ///     which then hooks the event delegate it will trigger right after this class finishes initializing.
    /// </summary>
    public sealed class RandomEventGameMode : ModeProduct
    {
        /// <summary>
        ///     Holds information and the event the director would like us to fire.
        /// </summary>
        private RandomEventInfo eventInfo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        public RandomEventGameMode() : base(false)
        {
            eventInfo = new RandomEventInfo();

            // Event director has event to know when events are triggered.
            GameSimulationApp.Instance.EventDirector.OnEventTriggered += Director_OnEventTriggered;
        }

        /// <summary>
        ///     Defines the current game gameMode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode ModeType
        {
            get { return GameMode.RandomEvent; }
        }

        /// <summary>
        ///     Fired when the event director triggers an event because it rolled the dice and hit it or it was forcefully
        ///     triggered by some method under a defined condition.
        /// </summary>
        private void Director_OnEventTriggered(IEntity simEntity, DirectorEventItem directorEventItem)
        {
            // Attached the random event state when we intercept an event it would like us to trigger.
            // TODO: Put event data into random event info object.
            AddState(typeof(RandomEventState));
        }

        /// <summary>
        ///     Fired when this game gameMode is removed from the list of available and ticked GameMode in the simulation.
        /// </summary>
        protected override void OnModeRemoved(GameMode modeType)
        {
            base.OnModeRemoved(modeType);

            // Event director has event for when he triggers events.
            if (GameSimulationApp.Instance.EventDirector != null)
                GameSimulationApp.Instance.EventDirector.OnEventTriggered -= Director_OnEventTriggered;
        }
    }
}