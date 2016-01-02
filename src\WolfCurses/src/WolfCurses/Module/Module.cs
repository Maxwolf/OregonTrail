// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit
{
    /// <summary>
    ///     Base implementation of the IModule interface which allows for some overrides to be virtual so every implementation
    ///     doesn't have to use them.
    /// </summary>
    public abstract class Module : IModule
    {
        /// <summary>
        ///     Determines if the module has been created by the simulation.
        /// </summary>
        private bool _moduleCreated;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SimUnit.Module" /> class.
        /// </summary>
        protected Module()
        {
            _moduleCreated = true;
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public virtual void Destroy()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public virtual void OnTick(bool systemTick, bool skipDay = false)
        {
            // Nothing to see here, move along...
        }
    }
}