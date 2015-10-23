using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class RandomEventMode : GameMode<RandomEventCommands>, IRandomEventMode
    {
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.RandomEventMode" /> class.
        /// </summary>
        public RandomEventMode()
        {
            _name = "Unknown Random Event";
        }

        public string Name
        {
            get { return _name; }
        }

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

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return false; }
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