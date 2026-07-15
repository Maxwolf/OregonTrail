// Created by Maxwolf (bigmaxwolf.com)
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
        [Description("succumbed to their injuries")] Injury = 3,

        /// <summary>
        ///     The party member drowned during a river crossing disaster.
        /// </summary>
        [Description("drowned")] Drowned = 4,

        /// <summary>
        ///     The party member froze to death in a storm without enough warm clothing.
        /// </summary>
        [Description("froze to death")] Frozen = 5,

        /// <summary>
        ///     The party member was killed by wild animals (a wolf attack or buffalo stampede).
        /// </summary>
        [Description("was killed by wild animals")] Mauled = 6,

        /// <summary>
        ///     The party member was killed by bandits or thieves.
        /// </summary>
        [Description("was murdered by bandits")] Murdered = 7,

        /// <summary>
        ///     The party member died in a wagon accident such as a fire or the wagon tipping over.
        /// </summary>
        [Description("died in an accident")] Accident = 8
    }
}
