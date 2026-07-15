// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Linq;
using OregonTrailDotNet.Window.Travel.Hunt.Help;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Hunt
{
    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class Hunting : Form<TravelInfo>
    {
        /// <summary>
        ///     Words the player can type to leave the hunt early once they have bagged enough food, instead of waiting out the
        ///     whole session. Pressing Escape (handled by the game loop) does the same thing.
        /// </summary>
        private static readonly string[] StopWords = { "stop", "quit", "done", "leave", "exit", "q" };
        /// <summary>
        ///     Initializes a new instance of the <see cref="Hunting" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public Hunting(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>
        ///     Always TRUE while hunting: the player types the shooting word to fire when prey is on the field, and can type a
        ///     stop word (e.g. "stop"/"quit") at any time — even between animals — to leave the hunt early.
        /// </remarks>
        public override bool InputFillsBuffer => true;

        /// <summary>
        ///     Determines if this dialog state is allowed to receive any input at all, even empty line returns. Always TRUE so a
        ///     player can quit the hunt whenever they want, not only while an animal is being aimed at.
        /// </summary>
        public override bool AllowInput => true;

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Listen to hunting manager for event about targeting prey running away.
            UserData.Hunt.TargetFledEvent += Hunt_TargetFledEvent;
        }

        /// <summary>
        ///     Called when the currently targeted prey decides to run away from the hunter.
        /// </summary>
        /// <param name="target">Prey that sensed danger and ran away.</param>
        private void Hunt_TargetFledEvent(PreyItem target)
        {
            SetForm(typeof(PreyFlee));
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public override void OnTick(bool systemTick, bool skipDay)
        {
            base.OnTick(systemTick, skipDay);

            // Depending on the state of the hunt we will keep ticking or show result of players efforts for the session.
            if (UserData.Hunt.ShouldEndHunt)
            {
                // Unhook event for when targeted prey flees.
                UserData.Hunt.TargetFledEvent -= Hunt_TargetFledEvent;

                // Attach the hunting result form.
                SetForm(typeof(HuntingResult));
            }
            else
            {
                // Tick the hunting session normally.
                UserData.Hunt?.OnTick(systemTick, skipDay);
            }
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The text user interface.<see cref="string" />.
        /// </returns>
        public override string OnRenderForm()
        {
            ParentWindow.PromptText = "Type the word to shoot.";
            return UserData.Hunt.HuntInfo;
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if the input is null or empty (an empty ENTER should never end the hunt — that keeps the headless bot,
            // which submits blank input between animals, hunting normally).
            if (string.IsNullOrWhiteSpace(input))
                return;

            // A stop word means the player is done hunting and wants to collect whatever they have bagged so far.
            if (StopWords.Contains(input.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                StopHunting();
                return;
            }

            // Check if we have anything to shoot at right now.
            if (!UserData.Hunt.PreyAvailable)
                return;

            // Check if the user spelled the shooting word correctly.
            if (!input.Equals(UserData.Hunt.ShootingWord.ToString(), StringComparison.OrdinalIgnoreCase))
                return;

            // Determine if the player shot an animal or missed their shot.
            SetForm(UserData.Hunt.TryShoot() ? typeof(PreyHit) : typeof(PreyMissed));
        }

        /// <summary>
        ///     Ends the hunt right now and moves on to tally up whatever the party managed to bag. Exposed so the game loop can
        ///     hook the Escape key to it, and shared by the typed stop words. Safe to call at any point during a hunt.
        /// </summary>
        public void StopHunting()
        {
            // Unhook event for when targeted prey flees so it does not fire after we have left the hunt.
            UserData.Hunt.TargetFledEvent -= Hunt_TargetFledEvent;

            // Jump straight to the results screen with the kills gathered so far.
            SetForm(typeof(HuntingResult));
        }
    }
}