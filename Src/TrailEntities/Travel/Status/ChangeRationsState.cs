using System;
using System.Text;
using TrailEntities.Entity;
using TrailEntities.Mode;

namespace TrailEntities.Travel
{
    /// <summary>
    ///     Allows the player to change the amount of food their party members will have access to in a given day, the purpose
    ///     of which is to limit the amount they take in to slow the loss of food per pound. This has many affects on the
    ///     simulation such as disease, chance for breaking body parts, and or complete death from starvation.
    /// </summary>
    public sealed class ChangeRationsState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     Builds up the ration information and selection text.
        /// </summary>
        private StringBuilder _ration;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ChangeRationsState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
        {
            _ration = new StringBuilder();
            _ration.Append($"{Environment.NewLine}Change food rations{Environment.NewLine}");
            _ration.Append(
                $"(currently \"{GameSimApp.Instance.Vehicle.Ration}\"){Environment.NewLine}{Environment.NewLine}");
            _ration.Append($"The amount of food the people in{Environment.NewLine}");
            _ration.Append($"your party eat each day can{Environment.NewLine}");
            _ration.Append($"change. These amounts are:{Environment.NewLine}{Environment.NewLine}");
            _ration.Append($"1. filling - meals are large and{Environment.NewLine}");
            _ration.Append($"   generous.{Environment.NewLine}{Environment.NewLine}");
            _ration.Append($"2. meager - meals are small, but{Environment.NewLine}");
            _ration.Append($"   adequate.{Environment.NewLine}{Environment.NewLine}");
            _ration.Append($"3. bare bones - meals are very{Environment.NewLine}");
            _ration.Append($"   small, everyone stays hungry.");
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _ration.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            switch (input.ToUpperInvariant())
            {
                case "1":
                    GameSimApp.Instance.Vehicle.ChangeRations(RationLevel.Filling);
                    ParentMode.CurrentState = null;
                    break;
                case "2":
                    GameSimApp.Instance.Vehicle.ChangeRations(RationLevel.Meager);
                    ParentMode.CurrentState = null;
                    break;
                case "3":
                    GameSimApp.Instance.Vehicle.ChangeRations(RationLevel.BareBones);
                    ParentMode.CurrentState = null;
                    break;
                default:
                    ParentMode.CurrentState = new ChangeRationsState(ParentMode, UserData);
                    break;
            }
        }
    }
}