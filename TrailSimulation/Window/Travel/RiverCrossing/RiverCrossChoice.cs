// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Window.Travel.RiverCrossing
{
    using System.ComponentModel;

    /// <summary>
    ///     Determines what kind of river crossing the player would like to perform the time comes to dice roll the probability
    ///     of failure and what will happen.
    /// </summary>
    public enum RiverCrossChoice
    {
        /// <summary>
        ///     Default choice when crossing the river, not shown in the menu but is set to this value by default until user
        ///     changes it to something.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Rides directly into the river without any special precautions, if it is greater than three feet of water the
        ///     vehicle will be submerged and highly damaged.
        /// </summary>
        [Description("attempt to ford the river")]
        Ford = 1,

        /// <summary>
        ///     Attempts to float the vehicle over the river to the other side, there is a much higher chance for bad things to
        ///     happen.
        /// </summary>
        [Description("caulk the wagon and float it across")]
        Float = 2,

        /// <summary>
        ///     Prompts to pay monies for a ferry operator that will take the vehicle across the river without the danger of user
        ///     trying it themselves.
        /// </summary>
        [Description("take a ferry across")]
        Ferry = 3,

        /// <summary>
        ///     Prompts to play in sets of clothing for Indian guide that will take you across a the river, he acts like ferry
        ///     operator but depending on how many animals you killed hunting his price will change and go up the more animals
        ///     killed.
        /// </summary>
        [Description("hire an Indian to help")]
        Indian = 4,

        /// <summary>
        ///     Waits for a day still ticking events but waiting to see if weather will improve and make crossing easier.
        /// </summary>
        [Description("wait to see if conditions improve")]
        WaitForWeather = 5,

        /// <summary>
        ///     Attached a state on top of the river crossing Windows to explain what the different options mean and how they work.
        /// </summary>
        [Description("get more information")]
        GetMoreInformation = 6
    }
}