using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrailEntities
{
    public sealed class RandomEventMode : GameMode<RandomEventCommands>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public RandomEventMode() : base(false)
        {
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.RandomEvent; }
        }
    }
}
