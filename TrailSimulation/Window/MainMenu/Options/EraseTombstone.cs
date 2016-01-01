// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation.Window.MainMenu.Options
{
    using System;
    using System.Text;

    /// <summary>
    ///     Erases all the saved JSON Tombstone epitaphs on the disk so other players will not encounter them, new ones can
    ///     be created then.
    /// </summary>
    [ParentWindow(typeof (MainMenu))]
    public sealed class EraseTombstone : InputForm<NewGameInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EraseTombstone" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public EraseTombstone(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get { return DialogType.YesNo; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            var eraseEpitaphs = new StringBuilder();

            // Text above the table to declare what this state is.
            eraseEpitaphs.Append(
                $"{Environment.NewLine}Erase tombstone messages{Environment.NewLine}{Environment.NewLine}");

            // Tell the user how tombstones work before destroying them.
            eraseEpitaphs.Append($"There may be one tombstone on{Environment.NewLine}");
            eraseEpitaphs.Append($"the first half of the trail and{Environment.NewLine}");
            eraseEpitaphs.Append($"one tombstone on the second{Environment.NewLine}");
            eraseEpitaphs.Append($"half. If you erase the{Environment.NewLine}");
            eraseEpitaphs.Append($"tombstone messages, they will{Environment.NewLine}");
            eraseEpitaphs.Append($"not be replaced until team{Environment.NewLine}");
            eraseEpitaphs.Append($"leaders die along the trail.{Environment.NewLine}{Environment.NewLine}");

            eraseEpitaphs.Append("Do you want to do this? Y/N");
            return eraseEpitaphs.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Actually erase Tombstone messages.
            GameSimulationApp.Instance.Tombstone.Reset();

            SetForm(typeof (ManagementOptions));
        }
    }
}