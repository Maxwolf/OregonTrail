using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class RandomEventMode : GameMode, IRandomEvent
    {
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.RandomEventMode" /> class.
        /// </summary>
        public RandomEventMode(IGameServer game) : base(game)
        {
            _name = "Unknown Random Event";
        }

        public string Name
        {
            get { return _name; }
        }

        public override ModeType Mode
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
    }
}