// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit
{
    /// <summary>
    ///     The Module interface.
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