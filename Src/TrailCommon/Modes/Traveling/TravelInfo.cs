namespace TrailCommon
{
    /// <summary>
    ///     Holds all the information about traveling that we want to know, such as how long we need to go until next point,
    ///     what our current mode is like moving, paused, etc.
    /// </summary>
    public sealed class TravelInfo : ModeInfo
    {
        protected override string Name
        {
            get { return "Travel Information"; }
        }
    }
}