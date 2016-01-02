// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit.Form.Input
{
    /// <summary>
    ///     Used by game Windows state prefab to determine what the response was to a particular dialog prompt.
    /// </summary>
    public enum DialogResponse
    {
        /// <summary>
        ///     User indicated their reply to the prompt was NO in one form or another.
        /// </summary>
        No = 0,

        /// <summary>
        ///     User indicated their reply to the prompt was YES in one form or another.
        /// </summary>
        Yes = 1,

        /// <summary>
        ///     No response was given, only a blank command meaning the user is trying to continue.
        /// </summary>
        Custom = 3
    }
}