// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit.Form.Input
{
    /// <summary>
    ///     Determines what type of dialog this will be, as in should it just display a message and wait for any input before
    ///     firing response method.
    /// </summary>
    public enum DialogType
    {
        /// <summary>
        ///     Dialog will only display the message prompt and not ask for any input and instead ask the user to press RETURN KEY
        ///     to continue. Used when you need to just tell the user something like a pop-up.
        /// </summary>
        Prompt = 1,

        /// <summary>
        ///     Dialog will accept only YES or NO answer and nothing else, if the user enters invalid data then it will be ignored
        ///     until a valid response is given by the user.c
        /// </summary>
        YesNo = 2,

        /// <summary>
        ///     Dialog will accept either YES or NO response, if neither are received then it will process it as a cancel or
        ///     continue method. Up to implementation to deal with it at this point.
        /// </summary>
        Custom = 3
    }
}