using System.Text.RegularExpressions;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>A parsed "  N. label" line from a rendered menu.</summary>
    public readonly record struct MenuOption(int Number, string Label);

    /// <summary>
    ///     Translates the current game screen into the concrete string to type, by asking the active <see cref="IPolicy" />
    ///     for the semantic decision and mapping it onto whatever number/word this particular screen expects. Classification is
    ///     driven off the concrete form type name (the most reliable signal) plus light text parsing for dynamic menus.
    /// </summary>
    public sealed class ScreenRecognizer
    {
        // Buy priority: movement first (oxen are required to leave the first store), then survival, then spares.
        private static readonly Entities[] BuyOrder =
        {
            Entities.Animal, Entities.Food, Entities.Clothes, Entities.Medicine, Entities.Ammo,
            Entities.Wheel, Entities.Axle, Entities.Tongue
        };

        private static readonly HashSet<string> YesNoForms = new()
        {
            "TollRoadQuestion", "Trading", "TombstoneQuestion", "LocationArrive", "UseFerryConfirm",
            "IndianGuidePrompt", "VehicleBrokenPrompt", "CurrentTopTen", "EraseCurrentTopTen",
            "EraseTombstone", "EpitaphQuestion", "EpitaphConfirm"
        };

        private static readonly Regex MenuLine = new(@"^\s*(\d+)\.\s+(.*\S)\s*$", RegexOptions.Compiled);
        private static readonly Regex AffordRx = new(@"You can afford\s+(\d+)", RegexOptions.Compiled);
        private static readonly Regex ShootingWordRx = new(@"Shooting Word:\s*([A-Za-z]+)", RegexOptions.Compiled);
        private static readonly Regex TypeWordRx = new(@"Type the word '([A-Za-z]+)'", RegexOptions.Compiled);

        private readonly IPolicy _policy;

        // Per-store-visit state so we buy each item once and know which item StorePurchase refers to.
        private readonly HashSet<Entities> _storeHandled = new();
        private Entities? _storeSelected;

        // Per-crossing counter: if we keep bouncing back to the river menu (an unaffordable ferry/guide choice), force a free
        // crossing so we can never loop forever there.
        private int _riverVisits;

        // Same idea for forks: an unaffordable toll-road branch sends us straight back to the fork, so after a couple bounces
        // take the first branch instead.
        private int _forkVisits;

        // Counts the non-leader party members named so far this game, so they get sequential numbers 2, 3, 4 (crew #1 is
        // always the leader). Reset whenever the leader-name prompt appears, in case the naming flow restarts.
        private int _partyMembersNamed;

        public ScreenRecognizer(IPolicy policy) => _policy = policy;

        /// <summary>Form names whose input the recognizer did not recognize, surfaced for developers after a run.</summary>
        public HashSet<string> UnknownForms { get; } = new();

        public void ResetStoreSession()
        {
            _storeHandled.Clear();
            _storeSelected = null;
        }

        public void ResetRiverSession() => _riverVisits = 0;

        public void ResetForkSession() => _forkVisits = 0;

        /// <summary>Picks a travel-menu command number for the currently offered subset.</summary>
        public string TravelChoice(GameDriver driver, GameSnapshot state)
        {
            var available = ParseMenu(driver.RenderWindowText())
                .Select(o => (TravelCommands) o.Number)
                .Where(c => Enum.IsDefined(typeof(TravelCommands), c))
                .ToList();

            if (available.Count == 0)
                return "1";

            var choice = _policy.ChooseTravel(state, available);
            if (!available.Contains(choice))
                choice = available.Contains(TravelCommands.ContinueOnTrail)
                    ? TravelCommands.ContinueOnTrail
                    : available[0];

            return ((int) choice).ToString();
        }

        /// <summary>Decides the string to type for a form that accepts typed input (menu int, quantity, Y/N, name, word).</summary>
        public string TypedInput(GameDriver driver, GameSnapshot state)
        {
            var form = driver.FormName;
            var screen = driver.RenderWindowText();

            switch (form)
            {
                case "ProfessionSelector":
                    return _policy.Profession.ToString();
                case "InputPlayerNames":
                    // Crew #1 is always the wagon leader and keeps the policy's "(bot)" name so it brands the in-game
                    // high-score list. The three other members are numbered 2, 3, 4 so a viewer watching a playthrough can
                    // tell which party member died. (Match the leader question itself — the member roster below it also
                    // contains the word "leader", so a bare substring check would misfire.)
                    if (screen.Contains(MainMenu.LEADER_QUESTION, StringComparison.OrdinalIgnoreCase))
                    {
                        _partyMembersNamed = 0;
                        return _policy.LeaderName;
                    }

                    return (_partyMembersNamed++ + 2).ToString();
                case "ConfirmPlayerNames":
                    return "Y";
                case "SelectStartingMonthState":
                    return _policy.StartMonth.ToString();
                case "Store":
                    return StoreMenuChoice(state);
                case "StorePurchase":
                    return StoreQuantity(screen, state);
                case "ChangePace":
                    return _policy.Pace(state).ToString();
                case "ChangeRations":
                    return _policy.Ration(state).ToString();
                case "RestAmount":
                    return _policy.RestDays(state).ToString();
                case "RiverCross":
                    return RiverChoice(screen, state);
                case "LocationFork":
                    return ForkChoice(screen, state);
                case "Hunting":
                    return HuntWord(screen);
                case "EpitaphEditor":
                    return ""; // party wiped; a blank epitaph is fine (rarely reached — death ends the run first)
                case "ManagementOptions":
                    return "4"; // back out (the bot never opens management options)
                default:
                    if (YesNoForms.Contains(form) || screen.Contains("Y/N"))
                        return _policy.YesNo(form, state) ? "Y" : "N";

                    var options = ParseMenu(screen);
                    if (options.Count > 0)
                        return options[0].Number.ToString();

                    UnknownForms.Add(form);
                    return string.Empty;
            }
        }

        private string StoreMenuChoice(GameSnapshot state)
        {
            foreach (var item in BuyOrder)
            {
                if (_storeHandled.Contains(item))
                    continue;

                _storeHandled.Add(item);

                var target = _policy.TargetQuantity(item, state);
                if (target > state.OwnedOf(item))
                {
                    _storeSelected = item;
                    return ((int) item).ToString();
                }
            }

            // Everything handled — leave the store (menu key 9 maps to Entities.Vehicle which triggers LeaveStore).
            return "9";
        }

        private string StoreQuantity(string screen, GameSnapshot state)
        {
            var item = _storeSelected ?? Entities.Food;
            var afford = ParseAfford(screen);
            var gap = Math.Max(0, _policy.TargetQuantity(item, state) - state.OwnedOf(item));
            var qty = Math.Min(gap, afford);

            // Never leave the first store unable to move: guarantee at least a few oxen while we can afford them.
            if (item == Entities.Animal && state.OwnedOf(Entities.Animal) == 0)
                qty = Math.Max(qty, Math.Min(3, afford));

            return qty <= 0 ? "0" : qty.ToString();
        }

        private string RiverChoice(string screen, GameSnapshot state)
        {
            var options = ParseMenu(screen)
                .Select(o => (o.Number, Kind: ClassifyRiver(o.Label)))
                .Where(o => o.Kind.HasValue)
                .Select(o => (o.Number, Kind: o.Kind!.Value))
                .ToList();

            if (options.Count == 0)
                return "1";

            _riverVisits++;

            RiverChoiceKind chosen;
            if (_riverVisits > 2)
                // We keep landing back on the river menu — the policy's pick isn't executable (can't afford it). Force a
                // free crossing that always proceeds.
                chosen = options.Any(o => o.Kind == RiverChoiceKind.Ford) ? RiverChoiceKind.Ford : RiverChoiceKind.Caulk;
            else
                chosen = _policy.River(state, options.Select(o => o.Kind).ToList());

            var match = options.FirstOrDefault(o => o.Kind == chosen);
            return (match.Kind == chosen ? match.Number : options[0].Number).ToString();
        }

        private string ForkChoice(string screen, GameSnapshot state)
        {
            var options = ParseMenu(screen);
            var branches = options.Where(o => !o.Label.Contains("see the map", StringComparison.OrdinalIgnoreCase)).ToList();
            if (branches.Count == 0)
                return options.Count > 0 ? options[0].Number.ToString() : "1";

            _forkVisits++;
            if (_forkVisits > 2)
                // We keep returning to this fork — the chosen branch isn't executable (an unaffordable toll). Take the first
                // branch, which is never a toll road, so we always make progress.
                return branches[0].Number.ToString();

            var pick = Math.Clamp(_policy.Fork(state, branches.Count), 1, branches.Count);
            return branches[pick - 1].Number.ToString();
        }

        private static string HuntWord(string screen)
        {
            var m = ShootingWordRx.Match(screen);
            if (m.Success && !m.Groups[1].Value.Equals("NONE", StringComparison.OrdinalIgnoreCase))
                return m.Groups[1].Value.ToLowerInvariant();

            var m2 = TypeWordRx.Match(screen);
            return m2.Success ? m2.Groups[1].Value.ToLowerInvariant() : string.Empty;
        }

        private static RiverChoiceKind? ClassifyRiver(string label)
        {
            var l = label.ToLowerInvariant();
            if (l.Contains("ford")) return RiverChoiceKind.Ford;
            if (l.Contains("caulk") || l.Contains("float")) return RiverChoiceKind.Caulk;
            if (l.Contains("ferry")) return RiverChoiceKind.Ferry;
            if (l.Contains("indian") || l.Contains("hire")) return RiverChoiceKind.Indian;
            if (l.Contains("wait")) return RiverChoiceKind.Wait;
            if (l.Contains("information")) return RiverChoiceKind.MoreInfo;
            return null;
        }

        private static int ParseAfford(string screen)
        {
            var m = AffordRx.Match(screen);
            return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : 0;
        }

        public static List<MenuOption> ParseMenu(string text)
        {
            var list = new List<MenuOption>();
            foreach (var line in text.Split('\n'))
            {
                var m = MenuLine.Match(line.TrimEnd('\r'));
                if (m.Success && int.TryParse(m.Groups[1].Value, out var n))
                    list.Add(new MenuOption(n, m.Groups[2].Value.Trim()));
            }

            return list;
        }
    }
}
