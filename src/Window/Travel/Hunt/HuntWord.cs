// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace OregonTrailDotNet.Window.Travel.Hunt
{
    /// <summary>
    ///     Defines all the shooting words that are used to determine how quickly the player responded while hunting. Used to
    ///     determine if they hit the animal.
    /// </summary>
    public enum HuntWord
    {
        None = 0,
        // ReSharper disable once UnusedMember.Global
        Bang = 1,
        // ReSharper disable once UnusedMember.Global
        Blam = 2,
        // ReSharper disable once UnusedMember.Global
        Pow = 3,
        // ReSharper disable once UnusedMember.Global
        Wham = 4
    }
}