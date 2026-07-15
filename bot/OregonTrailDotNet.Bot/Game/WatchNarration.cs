using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Turns the bot's next input into a short, human-readable "thought" for watch mode, so a viewer can see what the bot
    ///     is about to do and, where it helps, why (e.g. "Food is low — hunting"). Purely cosmetic; never used in training.
    /// </summary>
    public static class WatchNarration
    {
        /// <summary>A one-line thought describing the decision the bot is about to make on the current screen.</summary>
        public static string Describe(string windowName, string formName, string input, GameSnapshot state)
        {
            // A numbered command menu (no active form).
            if (string.IsNullOrEmpty(formName))
            {
                if (windowName == "MainMenu")
                    return "Setting off on the Oregon Trail";
                if (windowName == "Travel" && int.TryParse(input, out var t) &&
                    Enum.IsDefined(typeof(TravelCommandsEnum), (TravelCommandsEnum) t))
                    return TravelThought((TravelCommandsEnum) t, state);
                return $"Choosing option {input}";
            }

            return formName switch
            {
                "ProfessionSelector" => $"Going as a {Profession(input)}",
                "InputPlayerNames" => input.Length == 0 ? "Rounding out the party" : $"Naming the wagon leader \"{input}\"",
                "ConfirmPlayerNames" => "Confirming the party",
                "SelectStartingMonthState" => $"Leaving in {Month(input)}",
                "Store" => input == "9" ? "Done shopping — leaving the store" : $"Buying {StoreItem(input)}",
                "StorePurchase" => input == "0" ? "Skipping this item" : $"Purchasing {input}",
                "ChangePace" => "Changing the pace",
                "ChangeRations" => "Changing the rations",
                "RestAmount" => $"Resting {input} day(s) to recover (health: {state.Health})",
                "RiverCross" => "Deciding how to cross the river",
                "LocationFork" => "Choosing which way to go",
                "Hunting" => $"Taking a shot — \"{input}\"!",
                _ => input.ToUpperInvariant() switch
                {
                    "Y" => YesThought(formName),
                    "N" => NoThought(formName),
                    _ => "Reading the situation"
                }
            };
        }

        /// <summary>A short status shown while an animated/waiting screen advances.</summary>
        public static string PromptStatus(string formName) => formName switch
        {
            "ContinueOnTrail" => "Rolling down the trail...",
            "Resting" => "Resting...",
            "CrossingTick" => "Crossing the river...",
            "EventExecutor" or "EventSkipDay" => "Something is happening on the trail...",
            "GameWin" => "Made it to Oregon!",
            "GameFail" => "The journey has ended.",
            "FinalPoints" => "Tallying the final score...",
            _ => ""
        };

        private static string TravelThought(TravelCommandsEnum command, GameSnapshot state) => command switch
        {
            TravelCommandsEnum.ContinueOnTrail => "Pressing on down the trail",
            TravelCommandsEnum.CheckSupplies => "Checking the supplies",
            TravelCommandsEnum.LookAtMap => "Looking at the map",
            TravelCommandsEnum.ChangePace => "Reconsidering the pace",
            TravelCommandsEnum.ChangeFoodRations => "Reconsidering the rations",
            TravelCommandsEnum.StopToRest => $"Health is {state.Health} — stopping to rest",
            TravelCommandsEnum.AttemptToTrade => "Looking for a trade",
            TravelCommandsEnum.HuntForFood => $"Food is low ({state.Food} lb) — hunting for meat",
            TravelCommandsEnum.BuySupplies => "Stopping to buy supplies",
            TravelCommandsEnum.TalkToPeople => "Chatting with the locals",
            _ => "Deciding what to do"
        };

        private static string YesThought(string formName) => formName switch
        {
            "TollRoadQuestion" => "Paying the toll to pass",
            "VehicleBrokenPrompt" => "Trying to repair the wagon",
            "LocationArrive" => "Looking around",
            "UseFerryConfirm" => "Taking the ferry across",
            "IndianGuidePrompt" => "Hiring the river guide",
            _ => "Answering yes"
        };

        private static string NoThought(string formName) => formName switch
        {
            "Trading" => "Passing on the trade",
            "TombstoneQuestion" => "Moving past the gravesite",
            _ => "Answering no"
        };

        private static string Profession(string input) => input switch
        {
            "1" => "banker", "2" => "carpenter", "3" => "farmer", _ => "traveler"
        };

        private static string Month(string input) => input switch
        {
            "1" => "March", "2" => "April", "3" => "May", "4" => "June", "5" => "July", _ => "the spring"
        };

        private static string StoreItem(string input) => input switch
        {
            "1" => "oxen", "2" => "food", "3" => "clothing", "4" => "ammunition",
            "5" => "spare wheels", "6" => "spare axles", "7" => "spare tongues", "8" => "medicine",
            _ => "supplies"
        };
    }
}
