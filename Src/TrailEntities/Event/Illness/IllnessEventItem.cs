using System;

namespace TrailEntities
{
    public sealed class IllnessEventItem : EventItem<Person, IllnessEvent>
    {
        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        /// <param name="eventTarget">Simulation compatible entity which will be affected.</param>
        /// <param name="eventEnum">Enumeration that will be passed to result along with target for event execution.</param>
        public IllnessEventItem(Person eventTarget, IllnessEvent eventEnum) : base(eventTarget, eventEnum)
        {
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventTarget">Entity which will be affected by this method.</param>
        /// <param name="eventEnum">Enumeration that helps this method determine what should be done.</param>
        protected override void OnEventExecute(Person eventTarget, IllnessEvent eventEnum)
        {
            switch (eventEnum)
            {
                case IllnessEvent.SprainedMuscle:
                    break;
                case IllnessEvent.SprainedShoulder:
                    break;
                case IllnessEvent.TyphoidFever:
                    break;
                case IllnessEvent.Concussion:
                    break;
                case IllnessEvent.BrokenArm:
                    break;
                case IllnessEvent.DeathCompanion:
                    break;
                case IllnessEvent.DeathPlayer:
                    break;
                case IllnessEvent.SufferingExhaustion:
                    break;
                case IllnessEvent.TurnForWorse:
                    break;
                case IllnessEvent.Cholera:
                    break;
                case IllnessEvent.MountainFever:
                    break;
                case IllnessEvent.Dysentery:
                    break;
                case IllnessEvent.Measles:
                    break;
                case IllnessEvent.Gangrene:
                    break;
                case IllnessEvent.WellAgain:
                    break;
                case IllnessEvent.Fever:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventEnum), eventEnum, null);
            }
        }
    }
}