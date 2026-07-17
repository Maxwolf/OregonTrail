// Created by Maxwolf (bigmaxwolf.com)

namespace OregonTrailDotNet.Entity.Location.Point
{
    /// <summary>
    ///     What the bed of a river is like underfoot. This only matters on a crossing shallow enough to be safe: the water
    ///     will not drown anybody, but the going can still be bad enough to bog a wagon down for a day or turn it over.
    /// </summary>
    public enum RiverBottomEnum
    {
        /// <summary>
        ///     Firm going. A shallow ford here is simply uneventful.
        /// </summary>
        Firm = 0,

        /// <summary>
        ///     Mud, which can bog the wagon down and cost a day.
        /// </summary>
        Muddy = 1,

        /// <summary>
        ///     Rocks and rough water, which can tip the wagon over and spill what is inside.
        /// </summary>
        Rough = 2
    }
}
