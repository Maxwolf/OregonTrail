using System;
using System.Text;
using TrailEntities.Entity;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Allows the player to alter how many 'miles' their vehicle will attempt to travel in a given day, this also changes
    ///     the rate at which random events that are considered bad will occur along with other factors in the simulation such
    ///     as making players more susceptible to disease and also making them hungry more often.
    /// </summary>
    public sealed class ChangePaceState : StateProduct<TravelInfo>
    {
        /// <summary>
        ///     String builder for the changing pace text.
        /// </summary>
        private StringBuilder _pace;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ChangePaceState(IModeProduct gameMode, TravelInfo userData) : base(gameMode, userData)
        {
            _pace = new StringBuilder();
            _pace.Append($"{Environment.NewLine}Change pace{Environment.NewLine}");
            _pace.Append(
                $"(currently \"{GameSimulationApp.Instance.Vehicle.Pace}\"){Environment.NewLine}{Environment.NewLine}");
            _pace.Append($"The pace at which you travel{Environment.NewLine}");
            _pace.Append($"can change. Your choices are:{Environment.NewLine}{Environment.NewLine}");
            _pace.Append($"1. a steady pace{Environment.NewLine}");
            _pace.Append($"2. a strenuous pace{Environment.NewLine}");
            _pace.Append($"3. a grueling pace{Environment.NewLine}");
            _pace.Append($"4. find out what these{Environment.NewLine}");
            _pace.Append($"   different paces mean");
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _pace.ToString();
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
                    GameSimulationApp.Instance.Vehicle.ChangePace(TravelPace.Steady);
                    //parentGameMode.CurrentState = null;
                    ClearState();
                    break;
                case "2":
                    GameSimulationApp.Instance.Vehicle.ChangePace(TravelPace.Strenuous);
                    //parentGameMode.CurrentState = null;
                    ClearState();
                    break;
                case "3":
                    GameSimulationApp.Instance.Vehicle.ChangePace(TravelPace.Grueling);
                    //parentGameMode.CurrentState = null;
                    ClearState();
                    break;
                case "4":
                    //parentGameMode.CurrentState = new PaceAdviceState(parentGameMode, UserData);
                    SetState(typeof (PaceAdviceState));
                    break;
                default:
                    //parentGameMode.CurrentState = new ChangePaceState(parentGameMode, UserData);
                    SetState(typeof (ChangePaceState));
                    break;
            }
        }
    }
}