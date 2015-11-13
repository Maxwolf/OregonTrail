using TrailEntities.Entity;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Attached by the event director when it wants to execute an event against the simulation. It will attach this mode,
    ///     which then hooks the event delegate it will trigger right after this class finishes initializing.
    /// </summary>
    [GameMode(ModeType.RandomEvent)]
    // ReSharper disable once UnusedMember.Global
    public sealed class RandomEventMode : ModeProduct<RandomEventCommands>
    {
        /// <summary>
        ///     Holds information and the event the director would like us to fire.
        /// </summary>
        private RandomEventInfo eventInfo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        public RandomEventMode() : base(false)
        {
            eventInfo = new RandomEventInfo();

            // Event director has event to know when events are triggered.
            GameSimulationApp.Instance.Director.OnEventTriggered += Director_OnEventTriggered;
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.RandomEvent; }
        }

        /// <summary>
        ///     Fired when the event director triggers an event because it rolled the dice and hit it or it was forcefully
        ///     triggered by some method under a defined condition.
        /// </summary>
        private void Director_OnEventTriggered(IEntity simEntity, DirectorEvent directorEvent)
        {
            // Attached the random event state when we intercept an event it would like us to trigger.
            eventInfo.DirectorEvent = directorEvent;
            eventInfo.SourceEntity = simEntity;
            //CurrentState = new RandomEventState(this, eventInfo);
            SetState(typeof (RandomEventState));
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        protected override void OnModeRemoved(ModeType modeType)
        {
            base.OnModeRemoved(modeType);

            // Event director has event for when he triggers events.
            if (GameSimulationApp.Instance.Director != null)
                GameSimulationApp.Instance.Director.OnEventTriggered -= Director_OnEventTriggered;
        }
    }
}