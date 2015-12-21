// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModule.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   The Module interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TrailSimulation.Core
{
    /// <summary>
    /// The Module interface.
    /// </summary>
    public interface IModule : ITick
    {
        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        void Destroy();
    }
}