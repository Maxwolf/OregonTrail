using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Primary game mode of the simulation, used to show simulation advancing through linear time. Shows all major stats
    ///     of party and vehicle, plus climate and other things like distance traveled and distance to next point.
    /// </summary>
    public sealed class TravelingMode : GameMode<TravelCommands>, ITravelingMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public TravelingMode()
        {
            CanRest = true;
        }

        public override ModeType ModeType
        {
            get { return ModeType.Travel; }
        }

        public void GoToStore()
        {
            GameSimulationApp.Instance.AddMode(ModeType.Store);
        }

        public bool CanRest { get; }

        public void ContinueOnTrail()
        {
            throw new NotImplementedException();
        }

        public void CheckSupplies()
        {
            throw new NotImplementedException();
        }

        public void LookAtMap()
        {
            throw new NotImplementedException();
        }

        public void ChangePace()
        {
            throw new NotImplementedException();
        }

        public void ChangeFoodRations()
        {
            throw new NotImplementedException();
        }

        public void StopToRest()
        {
            throw new NotImplementedException();
        }

        public void AttemptToTrade()
        {
            throw new NotImplementedException();
        }

        public void TalkToPeople()
        {
            throw new NotImplementedException();
        }

        public void BuySupplies()
        {
            throw new NotImplementedException();
        }

        public void Hunt()
        {
            throw new NotImplementedException();
        }
    }
}