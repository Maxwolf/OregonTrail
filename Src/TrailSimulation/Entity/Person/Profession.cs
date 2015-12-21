// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Profession.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   The profession.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Entity
{
    using System.ComponentModel;

    /// <summary>
    ///     The profession.
    /// </summary>
    public enum Profession
    {
        /// <summary>
        ///     The banker.
        /// </summary>
        [Description("Be a banker from Boston")]
        Banker = 1, 

        /// <summary>
        ///     The carpenter.
        /// </summary>
        [Description("Be a carpenter from Ohio")]
        Carpenter = 2, 

        /// <summary>
        ///     The farmer.
        /// </summary>
        [Description("Be a farmer from Illinois")]
        Farmer = 3
    }
}