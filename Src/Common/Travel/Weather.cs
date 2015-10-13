namespace OregonTrail
{
    /// <summary>
    ///     Determines what the environment is like and goes into helping calculate roll chance. For example, experiencing
    ///     stormy weather will mean there is a higher chance of a travel event being triggered.
    /// </summary>
    public enum Weather
    {
        /// <summary>
        ///     Very hot weather, makes you more tired, need more water.
        /// </summary>
        Hot,

        /// <summary>
        ///     Cold weather means you need more clothing to stay healthy.
        /// </summary>
        Cold,

        /// <summary>
        ///     Rainy weather means you need to have clothing in order to stay healthy.
        /// </summary>
        Rainy,

        /// <summary>
        ///     Stormy weather is the same as rain except it can also dramatically increase the chance of something bad happening
        ///     to either party of their vehicle.
        /// </summary>
        Stormy,

        /// <summary>
        ///     Freezing weather will stress both party and vehicle to their extreme, anything can happen at any moment and roll
        ///     chances have a very high chance of succeeding.
        /// </summary>
        Freezing
    }
}