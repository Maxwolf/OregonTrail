using System.Text;
using OregonTrailDotNet.Bot.Game;

namespace OregonTrailDotNet.Bot.Diagnostics
{
    /// <summary>
    ///     A structured, developer-facing report captured when the bot detects a crash or a bug while driving the game. The
    ///     bot stops on any of these so a human can fix the underlying issue rather than the bot silently limping on.
    /// </summary>
    public sealed class BugReport
    {
        public BugCategoryEnum Category { get; init; }
        public string TimestampUtc { get; init; } = DateTime.UtcNow.ToString("o");
        public string WindowType { get; init; } = "";
        public string FormType { get; init; } = "";
        public bool InputFillsBuffer { get; init; }
        public string Detail { get; init; } = "";
        public IReadOnlyList<string> RecentInputs { get; init; } = Array.Empty<string>();
        public string ScreenSnapshot { get; init; } = "";
        public string VehicleSnapshot { get; init; } = "";
        public string? ExceptionDetail { get; init; }

        public static BugReport Capture(BugCategoryEnum category, GameDriver driver, string detail, Exception? exception = null)
            => new()
            {
                Category = category,
                WindowType = driver.WindowName,
                FormType = driver.FormName,
                InputFillsBuffer = driver.InputFillsBuffer,
                Detail = detail,
                RecentInputs = driver.RecentInputs.ToArray(),
                ScreenSnapshot = driver.LastScreen,
                VehicleSnapshot = CaptureVehicle(),
                ExceptionDetail = exception?.ToString()
            };

        private static string CaptureVehicle()
        {
            var game = GameSimulationApp.Instance;
            if (game?.Vehicle == null)
                return "(no live vehicle)";

            var v = game.Vehicle;
            var sb = new StringBuilder();
            sb.AppendLine($"balance={v.Balance:C0} odometer={v.Odometer} day={game.Time?.TotalDays} " +
                          $"status={v.Status} location={game.Trail?.CurrentLocation?.Name}");
            foreach (var person in v.Passengers)
                sb.AppendLine($"  - {person.Name}: {person.HealthStatus} ({person.Cause}){(person.Leader ? " [leader]" : "")}");
            return sb.ToString().TrimEnd();
        }

        /// <summary>Renders the report as a clear block for the developer to read on the console.</summary>
        public string Format()
        {
            var sb = new StringBuilder();
            sb.AppendLine("==================== BOT STOPPED: POSSIBLE GAME BUG ====================");
            sb.AppendLine($"Category : {Category}");
            sb.AppendLine($"When     : {TimestampUtc}");
            sb.AppendLine($"Screen   : window={WindowType} form={FormType} inputFillsBuffer={InputFillsBuffer}");
            sb.AppendLine($"What     : {Detail}");
            sb.AppendLine();
            sb.AppendLine("Last inputs the bot sent (oldest -> newest):");
            sb.AppendLine("  " + (RecentInputs.Count > 0 ? string.Join(" ", RecentInputs) : "(none)"));
            sb.AppendLine();
            sb.AppendLine("Vehicle state:");
            sb.AppendLine(Indent(VehicleSnapshot));
            sb.AppendLine();
            sb.AppendLine("Screen the bot was looking at:");
            sb.AppendLine(Indent(ScreenSnapshot));
            if (!string.IsNullOrEmpty(ExceptionDetail))
            {
                sb.AppendLine();
                sb.AppendLine("Exception:");
                sb.AppendLine(Indent(ExceptionDetail));
            }

            sb.AppendLine();
            sb.AppendLine("Please fix the issue above, then re-run the bot.");
            sb.AppendLine("=======================================================================");
            return sb.ToString();
        }

        private static string Indent(string text) =>
            string.Join(Environment.NewLine,
                (text ?? "").Split('\n').Select(line => "    " + line.TrimEnd('\r')));
    }
}
