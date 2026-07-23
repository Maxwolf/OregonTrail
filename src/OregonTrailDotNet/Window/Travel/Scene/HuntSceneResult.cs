using System;
using System.Text;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Presentation;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The end of a real-time hunt: the wrapper applied once to the whole bag, in the original's own order
    ///     (<c>HUNT.LIB:50011-50016</c>) — dress the meat by halving anything from three pounds up, zero it against
    ///     a full wagon, clamp it to the space that remains, cap what one hunter can carry at 100 pounds — then the
    ///     day charged and everything written back to the wagon on ENTER. Deliberately a separate form from the
    ///     word-hunt's <see cref="Hunt.HuntingResult" />, whose rendered text the bot and tests pin.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class HuntSceneResult : InputForm<TravelInfo>
    {
        private HuntOutcome _outcome;
        private int _finalPounds;

        /// <summary>Initializes a new instance of the <see cref="HuntSceneResult" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public HuntSceneResult(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // However the hunt went, it cost the day — the same charge the word hunt makes.
            GameSimulationApp.Instance.TakeTurn(false);
        }

        /// <summary>
        ///     The wrapper, as a pure function so the order can be pinned by tests: dress (halve 3 lb and up), zero
        ///     against a full wagon, clamp to the wagon's remaining space, cap at the 100 lb carry.
        /// </summary>
        /// <param name="rawPounds">Raw pounds shot over the whole hunt.</param>
        /// <param name="foodQuantity">Pounds of food already in the wagon.</param>
        /// <param name="foodCapacity">The wagon's food ceiling.</param>
        internal static int DressAndLoad(int rawPounds, int foodQuantity, int foodCapacity)
        {
            var dressed = HuntGame.Bag(rawPounds);
            if (dressed <= 0)
                return 0;

            if (foodQuantity >= foodCapacity)
                return 0;

            var loaded = Math.Min(dressed, foodCapacity - foodQuantity);
            return Math.Min(loaded, HuntGame.CarryCap);
        }

        /// <inheritdoc />
        protected override string OnDialogPrompt()
        {
            _outcome = UserData.HuntOutcome ?? new HuntOutcome(0, 0, 0);

            var food = GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Food];
            var dressed = HuntGame.Bag(_outcome.RawPounds);
            _finalPounds = DressAndLoad(_outcome.RawPounds, food.Quantity, food.MaxQuantity);

            var prompt = new StringBuilder();
            prompt.AppendLine($"{Environment.NewLine}The hunt is over.");
            prompt.AppendLine(
                $"{Environment.NewLine}You fired {_outcome.ShotsFired} rounds and shot {_outcome.RawPounds} pounds of game.");

            if (_outcome.RawPounds <= 0)
            {
                prompt.AppendLine($"{Environment.NewLine}You were unable to shoot any food.");
                return prompt.ToString();
            }

            prompt.AppendLine($"After dressing, {dressed} pounds of meat remain.");

            if (_finalPounds <= 0)
                prompt.AppendLine($"{Environment.NewLine}However, your wagon is full.");
            else if (_finalPounds < dressed)
                prompt.AppendLine(
                    $"{Environment.NewLine}However, you were only able to carry{Environment.NewLine}{_finalPounds} pounds back to the wagon.");
            else
                prompt.AppendLine($"{Environment.NewLine}You carry {_finalPounds} pounds back to the wagon.");

            return prompt.ToString();
        }

        /// <inheritdoc />
        protected override void OnDialogResponse(DialogResponseEnum reponse)
        {
            var game = GameSimulationApp.Instance;

            // The bag into the stores, one round per trigger pull out of them, and every kill counted for the
            // Shoshoni guide's asking price.
            if (_finalPounds > 0)
                game.Vehicle.Inventory[EntitiesEnum.Food].AddQuantity(_finalPounds);

            if (_outcome.ShotsFired > 0)
                game.Vehicle.Inventory[EntitiesEnum.Ammo].ReduceQuantity(_outcome.ShotsFired);

            for (var kill = 0; kill < _outcome.Kills; kill++)
                game.Vehicle.IncrementAnimalKillCount();

            UserData.HuntOutcome = null;
            ClearForm();
        }
    }
}
