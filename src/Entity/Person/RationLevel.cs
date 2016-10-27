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
        ///     Meals are large and generous.
        /// </summary>
        Filling = 1,

        /// <summary>
        ///     Meals are small, but adequate.
        /// </summary>
        Meager = 2,

        /// <summary>
        ///     Meals are very small; everyone stays hungry.
        /// </summary>
        [Description("Bare Bones")] BareBones = 3
    }
}