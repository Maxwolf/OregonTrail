// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventExecution.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Defines the event type, used by the director to determine how the collection and execution of event should occur.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Defines the event type, used by the director to determine how the collection and execution of event should occur.
    /// </summary>
    public enum EventExecution
    {
        /// <summary>
        ///     Event can be called randomly by category or manually by type.
        /// </summary>
        RandomOrManual = 0, 

        /// <summary>
        ///     Event can only be called manually and will not be included in random selection by category.
        /// </summary>
        ManualOnly = 1
    }
}