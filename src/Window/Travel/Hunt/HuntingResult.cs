// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Entity;
using WolfCurses.Window;
using WolfCurses.Window.Control;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.Hunt
{
    /// <summary>
    ///     Tabulates results about the hunting session after it ends, depending on the performance of the player and how many
    ///     animals they killed, if any will be calculated in weight. Players can only ever take <see cref="HuntManager.MAXFOOD" />
    ///     pounds of meat back to the vehicle so this discourages mass killing.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class HuntingResult : InputForm<TravelInfo>
    {
        /// <summary>
        ///     References the total weight of all the animals the player killed so we only have to reference it once.
        /// </summary>
        private int _finalKillWeight;

        /// <summary>
        ///     Holds all of the data for our final hunting result before rendering out to player.
        /// </summary>
        private readonly StringBuilder _huntScore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InputForm{T}" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public HuntingResult(IWindow window) : base(window)
        {
            _huntScore = new StringBuilder();
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // After hunting we roll the dice on the party and player and skip a day.
            GameSimulationApp.Instance.TakeTurn(false);
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The dialog prompt text.<see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Clear previous hunting score information.
            _huntScore.Clear();

            // Total weight of everything shot, how many animals that was, and the ammunition the outing cost.
            var killWeight = UserData.Hunt.KillWeight;
            var killCount = UserData.Hunt.KillCount;
            var bulletsFired = UserData.Hunt.BulletsFired;

            // The wagon can only carry MAXFOOD pounds home regardless of how much was shot; the rest is wasted meat.
            _finalKillWeight = Math.Min(killWeight, HuntManager.MAXFOOD);

            // Framed summary panel that echoes the hunt HUD's food meter, so the result reads as "how full a load did I
            // haul back?" against the same 250 lb ceiling.
            var foodBar = new ProgressBar {Width = 20, Label = "Food bag"}.Render(_finalKillWeight, HuntManager.MAXFOOD);
            var panel = new StringBuilder();
            panel.AppendLine(foodBar);
            panel.AppendLine($"{new string(' ', 9)}{_finalKillWeight} / {HuntManager.MAXFOOD} lb");
            // Close the panel with the ammunition the hunt cost, so the player can weigh the meat hauled back against the
            // bullets spent to get it. Bullets only leave inventory on a landed shot, so this is the true cost of the outing.
            panel.Append($"Bullets fired: {bulletsFired:N0}");

            _huntScore.AppendLine();
            _huntScore.AppendLine(new Box
            {
                Border = BoxBorder.Double,
                Title = "HUNT OVER",
                TitleAlignment = BoxAlignment.Center,
                Padding = 1
            }.Render(panel.ToString()));
            _huntScore.AppendLine();

            // A short line under the panel: empty-handed, a clean haul, or an over-the-limit haul that wasted meat.
            if (killWeight <= 0)
            {
                _huntScore.Append("  You came back empty-handed.");
            }
            else
            {
                var animalText = killCount == 1 ? "animal" : "animals";
                if (killWeight <= HuntManager.MAXFOOD)
                {
                    _huntScore.AppendLine($"  You bagged {killCount:N0} {animalText} for");
                    _huntScore.Append($"  {killWeight:N0} pounds of meat.");
                }
                else
                {
                    _huntScore.AppendLine($"  You bagged {killCount:N0} {animalText} —");
                    _huntScore.Append($"  {killWeight:N0} lb shot, {HuntManager.MAXFOOD:N0} lb carried.");
                }
            }

            // Return the hunting result to text renderer.
            return _huntScore.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Transfers the total finalized kill weight we calculated to vehicle inventory as food in pounds.
            if (_finalKillWeight > 0)
                GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Food].AddQuantity(_finalKillWeight);

            // Destroys all hunting related data now that we are done with it.
            UserData.DestroyHunt();

            // Returns to the travel menu so the player can continue down the trail.
            ClearForm();
        }
    }
}