// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com)
// Timestamp 01/03/2016@1:50 AM

using WolfCurses.Utility;

namespace OregonTrailDotNet.Entity.Person
{
    /// <summary>
    ///     Explains how a party member met their end so the death screen can tell the player what happened. Rendered through the
    ///     description attribute like the other simulation enumerations.
    /// </summary>
    public enum CauseOfDeath
    {
        /// <summary>
        ///     No specific cause was recorded (default). The death screen falls back to a generic message.
        /// </summary>
        [Description("")] Unknown = 0,

        /// <summary>
        ///     The party member ran out of food and starved to death.
        /// </summary>
        [Description("died of starvation")] Starvation = 1,

        /// <summary>
        ///     The party member succumbed to a disease such as cholera, dysentery, or typhoid.
        /// </summary>
        [Description("died of illness")] Illness = 2,

        /// <summary>
        ///     The party member succumbed to a physical injury.
        /// </summary>
        [Description("succumbed to their injuries")] Injury = 3
    }
}
