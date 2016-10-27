// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace OregonTrailDotNet.Window.MainMenu.Start_Month
{
    /// <summary>
    ///     Special enumeration used for defining the starting month of the game simulation. Since we want to user to select
    ///     one through five from March to July we need a special way to keep track of what months are valid for starting and
    ///     have them in selectable order that makes sense to the user.
    /// </summary>
    public enum StartingMonth
    {
        /// <summary>
        ///     The march.
        /// </summary>
        March = 1,

        /// <summary>
        ///     The april.
        /// </summary>
        April = 2,

        /// <summary>
        ///     The may.
        /// </summary>
        May = 3,

        /// <summary>
        ///     The june.
        /// </summary>
        June = 4,

        /// <summary>
        ///     The july.
        /// </summary>
        July = 5
    }
}