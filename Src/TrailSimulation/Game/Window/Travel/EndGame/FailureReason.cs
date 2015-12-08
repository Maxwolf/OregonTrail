using System.ComponentModel;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Defines all of the reasons why the player could not continue on the trail and died.
    /// </summary>
    public enum FailureReason
    {
        None = 0,

        [Description("All of the people in your party have died.")]
        Dead = 1,

        [Description("Vehicle is broken beyond repair, and stranded.")]
        Stuck = 2
    }
}