using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrailEntities
{
    public class NewGameModeView : NewGameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.NewGameMode" /> class.
        /// </summary>
        public NewGameModeView(Vehicle vehicle) : base(vehicle)
        {
        }
    }
}
