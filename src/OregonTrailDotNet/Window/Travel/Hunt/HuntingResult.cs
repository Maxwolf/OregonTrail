// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

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
            var killCount = UserData.Hunt.KillCount;
            var bulletsFired = UserData.Hunt.BulletsFired;

            // Only half of a big animal is meat you can actually use; the rest is hide, bone and what spoils before it can
            // be carried off. Small game is too little to bother halving - a rabbit is a rabbit.
            var killWeight = UserData.Hunt.KillWeight;
            if (killWeight >= 3)
                killWeight /= 2;

            // Work out how much of the kill actually reaches the wagon. Two separate limits apply, in the original's
            // order: first the room left in the wagon, then how much one hunter can drag home in a day. A wagon already
            // at its food ceiling takes nothing at all, and says so rather than silently voiding the meat.
            var food = GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Food];
            string capacityNote = null;
            _finalKillWeight = killWeight;

            if (_finalKillWeight > 0 && food.Quantity >= food.MaxQuantity)
            {
                _finalKillWeight = 0;
                capacityNote = "However, your wagon is full.";
            }
            else if (_finalKillWeight > 0 && _finalKillWeight + food.Quantity > food.MaxQuantity)
            {
                _finalKillWeight = food.MaxQuantity - food.Quantity;
                if (_finalKillWeight > 0 && _finalKillWeight <= HuntManager.MAXFOOD)
                    capacityNote = $"However, your wagon will only hold another {_finalKillWeight:N0} pounds of food.";
            }

            if (_finalKillWeight > HuntManager.MAXFOOD)
            {
                _finalKillWeight = HuntManager.MAXFOOD;
                capacityNote =
                    $"However, you were only able to carry {HuntManager.MAXFOOD:N0} pounds back to the wagon.";
            }

            // Framed summary panel that echoes the hunt HUD's food meter, so the result reads as "how full a load did I
            // haul back?" against the same ceiling.
            var foodBar = new ProgressBar {Width = 20, Label = "Food bag"}.Render(_finalKillWeight, HuntManager.MAXFOOD);
            var panel = new StringBuilder();
            panel.AppendLine(foodBar);
            panel.AppendLine($"{new string(' ', 9)}{_finalKillWeight} / {HuntManager.MAXFOOD} lb");
            // Close the panel with the ammunition the hunt cost, so the player can weigh the meat hauled back against the
            // bullets spent to get it. Bullets only leave inventory on a landed shot, so this is the true cost of the outing.
            panel.Append($"Bullets fired: {bulletsFired:N0}");

            _huntScore.AppendLine();
            _huntScore.AppendLine(FramedPanel.Render("HUNT OVER", panel.ToString()));
            _huntScore.AppendLine();

            // A short line under the panel: empty-handed, a clean haul, or a haul the wagon could not take in full.
            if (killWeight <= 0)
            {
                _huntScore.Append("  You came back empty-handed.");
            }
            else
            {
                var animalText = killCount == 1 ? "animal" : "animals";
                _huntScore.AppendLine($"  You bagged {killCount:N0} {animalText} for");
                _huntScore.Append($"  {killWeight:N0} pounds of meat.");

                if (capacityNote != null)
                {
                    _huntScore.AppendLine();
                    _huntScore.Append($"  {capacityNote}");
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
        protected override void OnDialogResponse(DialogResponseEnum reponse)
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