using System.ComponentModel;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Defines all of the commands that are needed to cross a river.
    /// </summary>
    public enum RiverCrossCommands
    {
        /// <summary>
        ///     Rides directly into the river without any special precautions, if it is greater than three feet of water the
        ///     vehicle will be submerged and highly damaged.
        /// </summary>
        [Description("attempt to ford the river")]
        FordRiver = 1,

        /// <summary>
        ///     Attempts to float the vehicle over the river to the other side, there is a much higher chance for bad things to
        ///     happen.
        /// </summary>
        [Description("caulk the wagon and float it across")]
        CaulkVehicle = 2,

        /// <summary>
        ///     Prompts to pay monies for a ferry operator that will take the vehicle across the river without the danger of user
        ///     trying it themselves.
        /// </summary>
        [Description("take a ferry across")]
        UseFerry = 3,

        /// <summary>
        ///     Waits for a day still ticking events but waiting to see if weather will improve and make crossing easier.
        /// </summary>
        [Description("wait to see if conditions improve")]
        WaitForWeather = 4,

        /// <summary>
        ///     Attached a state on top of the river crossing mode to explain what the different options mean and how they work.
        /// </summary>
        [Description("get more information")]
        GetMoreInformation = 5
    }
}