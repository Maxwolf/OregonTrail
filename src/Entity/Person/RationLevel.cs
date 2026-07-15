// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using WolfCurses.Utility;

namespace OregonTrailDotNet.Entity.Person
{
    /// <summary>
    ///     Amount of food people in party eat each day can change.
    /// </summary>
    public enum RationLevel
    {
        /// <summary>
        ///     Meals are large and generous: three pounds of food per person per day (matching the original game). Eats the most
        ///     but keeps the party healthiest.
        /// </summary>
        Filling = 3,

        /// <summary>
        ///     Meals are small, but adequate: two pounds of food per person per day.
        /// </summary>
        Meager = 2,

        /// <summary>
        ///     Meals are very small; everyone stays hungry: one pound of food per person per day. Eats the least, but going this
        ///     hungry raises the risk of illness.
        /// </summary>
        [Description("Bare Bones")] BareBones = 1
    }
}