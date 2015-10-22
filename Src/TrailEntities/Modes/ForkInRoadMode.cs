using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ForkInRoadMode : GameMode<ForkInRoadCommands>, IForkInRoadMode
    {
        private readonly List<PointOfInterest> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public ForkInRoadMode()
        {
            _skipChoices = new List<PointOfInterest>();
        }

        public override SimulationMode Mode
        {
            get { return SimulationMode.ForkInRoad; }
        }

        public ReadOnlyCollection<PointOfInterest> SkipChoices
        {
            get { return new ReadOnlyCollection<PointOfInterest>(_skipChoices); }
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Called by the active game mode when the text user interface is called. This will create a string builder with all
        ///     the data and commands that represent the concrete handler for this game mode.
        /// </summary>
        protected override string OnGetModeTUI()
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