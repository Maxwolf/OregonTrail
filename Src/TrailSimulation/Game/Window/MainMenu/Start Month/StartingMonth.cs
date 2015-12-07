namespace TrailSimulation.Game
{
    /// <summary>
    ///     Special enumeration used for defining the starting month of the game simulation. Since we want to user to select
    ///     one through five from March to July we need a special way to keep track of what months are valid for starting and
    ///     have them in selectable order that makes sense to the user.
    /// </summary>
    public enum StartingMonth
    {
        March = 1,
        April = 2,
        May = 3,
        June = 4,
        July = 5
    }
}