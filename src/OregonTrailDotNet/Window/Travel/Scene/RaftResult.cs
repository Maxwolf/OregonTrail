using System;
using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The reckoning at the end of the Columbia run: what the river took, whether the landing was made, and —
    ///     when the raft was destroyed or nobody is left standing — the handoff into the death flow. A safe or
    ///     survivable run charges the day and carries on down the last leg to Oregon City.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class RaftResult : InputForm<TravelInfo>
    {
        private RaftReport _report;

        /// <summary>Initializes a new instance of the <see cref="RaftResult" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public RaftResult(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Running the river costs the day, exactly as any other crossing does.
            GameSimulationApp.Instance.TakeTurn(false);
        }

        /// <inheritdoc />
        protected override string OnDialogPrompt()
        {
            _report = UserData.RaftReport ?? new RaftReport(new System.Collections.Generic.List<string>(), false, false);

            var prompt = new StringBuilder();
            if (_report.Destroyed)
            {
                prompt.AppendLine($"{Environment.NewLine}The raft is destroyed,");
                prompt.AppendLine($"everything has been lost.{Environment.NewLine}");
            }
            else if (_report.MissedLanding)
            {
                prompt.AppendLine($"{Environment.NewLine}You missed the landing, and the river");
                prompt.AppendLine("carried you to shore further down.");
                prompt.AppendLine($"{Environment.NewLine}");
            }
            else
            {
                prompt.AppendLine($"{Environment.NewLine}You land your raft at the trail");
                prompt.AppendLine($"to the Willamette Valley.{Environment.NewLine}");
            }

            if (_report.Losses.Count > 0)
            {
                foreach (var line in _report.Losses)
                    prompt.AppendLine(line);
            }
            else if (!_report.Destroyed)
            {
                prompt.AppendLine("You come through without loss.");
            }

            return prompt.ToString();
        }

        /// <inheritdoc />
        protected override void OnDialogResponse(DialogResponseEnum reponse)
        {
            var game = GameSimulationApp.Instance;
            UserData.RaftReport = null;

            // Nobody left: re-adding the travel window runs the arrival check, which sees the dead party and
            // raises the game-over flow — the same path any fatal day on the trail takes.
            if (game.Vehicle.PassengersDead)
            {
                ClearForm();
                game.WindowManager.Add(typeof(Travel));
                return;
            }

            // Ashore: on down the last leg to Oregon City.
            SetForm(TravelInfo.DepartFormType);
        }
    }
}
