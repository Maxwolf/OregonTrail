using System;

namespace TrailEntities
{
    public sealed class MedicalEventItem : EventItem<Person, MedicalEvent>
    {
        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        /// <param name="eventTarget">Simulation compatible entity which will be affected.</param>
        /// <param name="eventEnum">Enumeration that will be passed to result along with target for event execution.</param>
        public MedicalEventItem(Person eventTarget, MedicalEvent eventEnum) : base(eventTarget, eventEnum)
        {
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventTarget">Entity which will be affected by this method.</param>
        /// <param name="eventEnum">Enumeration that helps this method determine what should be done.</param>
        protected override void OnEventExecute(Person eventTarget, MedicalEvent eventEnum)
        {
            switch (eventEnum)
            {
                case MedicalEvent.SprainedMuscle:
                    break;
                case MedicalEvent.SprainedShoulder:
                    break;
                case MedicalEvent.TyphoidFever:
                    break;
                case MedicalEvent.Concussion:
                    break;
                case MedicalEvent.BrokenArm:
                    break;
                case MedicalEvent.DeathCompanion:
                    break;
                case MedicalEvent.DeathPlayer:
                    break;
                case MedicalEvent.SufferingExhaustion:
                    break;
                case MedicalEvent.TurnForWorse:
                    break;
                case MedicalEvent.Cholera:
                    break;
                case MedicalEvent.MountainFever:
                    break;
                case MedicalEvent.Dysentery:
                    break;
                case MedicalEvent.Measles:
                    break;
                case MedicalEvent.Gangrene:
                    break;
                case MedicalEvent.WellAgain:
                    break;
                case MedicalEvent.Fever:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventEnum), eventEnum, null);
            }
        }
    }
}