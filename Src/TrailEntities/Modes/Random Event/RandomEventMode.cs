using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class RandomEventMode : GameMode<RandomEventCommands>, IRandomEventMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.RandomEventMode" /> class.
        /// </summary>
        public RandomEventMode()
        {
            Name = "Unknown Random Event";
        }

        public string Name { get; }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
        }

        public override ModeType ModeType
        {
            get { return ModeType.RandomEvent; }
        }

        public void MakeEvent()
        {
            throw new NotImplementedException();
        }

        public void CheckForRandomEvent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        public override void OnModeRemoved()
        {
            throw new NotImplementedException();
        }
    }
}