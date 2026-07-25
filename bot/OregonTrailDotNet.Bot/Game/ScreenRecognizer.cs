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
        private static readonly EntitiesEnum[] BuyOrder =
        {
            EntitiesEnum.Animal, EntitiesEnum.Food, EntitiesEnum.Clothes, EntitiesEnum.Medicine, EntitiesEnum.Ammo,
            EntitiesEnum.Wheel, EntitiesEnum.Axle, EntitiesEnum.Tongue
        };

        private static readonly HashSet<string> YesNoForms = new()
        {
            "TollRoadQuestion", "Trading", "TombstoneQuestion", "LocationArrive", "UseFerryConfirm",
            "IndianGuidePrompt", "VehicleBrokenPrompt", "CurrentTopTen", "EraseCurrentTopTen",
            "EraseTombstone", "EpitaphQuestion", "EpitaphConfirm"
        };

        private static readonly Regex MenuLine = new(@"^\s*(\d+)\.\s+(.*\S)\s*$", RegexOptions.Compiled);
        // The affordable quantity is rendered with thousands separators (e.g. "You can afford 2,600 pounds of food"), so match
        // digits AND commas - a bare \d+ stops at the comma and reads 2,600 as 2, which had the bot buying 2 lb of cheap food.
        private static readonly Regex AffordRx = new(@"You can afford\s+([\d,]+)", RegexOptions.Compiled);

        private readonly IPolicy _policy;

        // Per-store-visit state so we buy each item once and know which item StorePurchase refers to.
        private readonly HashSet<EntitiesEnum> _storeHandled = new();
        private EntitiesEnum? _storeSelected;

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
                .Select(o => (TravelCommandsEnum) o.Number)
                .Where(c => Enum.IsDefined(typeof(TravelCommandsEnum), c))
                .ToList();

            if (available.Count == 0)
                return "1";

            var choice = _policy.ChooseTravel(state, available);
            if (!available.Contains(choice))
                choice = available.Contains(TravelCommandsEnum.ContinueOnTrail)
                    ? TravelCommandsEnum.ContinueOnTrail
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
                    // Name every crew member "<bot> <n>" — e.g. "Skynet 1", "Skynet 2", ... — so a viewer can tell which
                    // one died. Crew #1 is the wagon leader (its name is what brands the in-game high-score list). Match the
                    // leader question itself: the member roster below it also says "(leader)", so a bare substring check
                    // would misfire.
                    var partyName = PartyBaseName(_policy.LeaderName);
                    if (screen.Contains(MainMenu.LEADER_QUESTION, StringComparison.OrdinalIgnoreCase))
                    {
                        _partyMembersNamed = 0;
                        return $"{partyName} 1";
                    }

                    return $"{partyName} {_partyMembersNamed++ + 2}";
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
                case "EpitaphEditor":
                    // Party wiped; leave a random silly epitaph like a person would (rarely reached — the run ends at the
                    // game-over screen first, and the grave already gets a silly default when the party dies).
                    return OregonTrailDotNet.Module.Tombstone.EpitaphCatalog.Random();
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

        // The bot's leader name is the profile name with a " (bot)" tag; the party shares the plain base name plus a crew
        // number ("Skynet 1", "Skynet 2", ...), so strip that tag to get e.g. "Skynet" from "Skynet (bot)".
        private static string PartyBaseName(string leaderName)
        {
            const string botSuffix = " (bot)";
            return leaderName.EndsWith(botSuffix, StringComparison.OrdinalIgnoreCase)
                ? leaderName[..^botSuffix.Length]
                : leaderName;
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
            var item = _storeSelected ?? EntitiesEnum.Food;

            // The store counter sells in LOTS, and both the "You can afford N" quote and the answer it wants are in
            // those lots: a yoke is two oxen, a box is twenty bullets, everything else is one to one. Work the whole
            // decision in units (which is what the policy's targets and the wagon's inventory are in) and divide at
            // the very end, so an ammunition target of 200 bullets asks for 10 boxes rather than 200 of them.
            var lot = LotSizeOf(item);
            var afford = ParseAfford(screen)*lot;

            // Mid-trail restocking holds back the policy's cash reserve so ferries and tolls stay payable (a broke party
            // gets forced into ford/caulk crossings, which are what drown oxen). The opening store (Independence, index 0)
            // spends freely, and a party buying the oxen that un-strand it ignores the reserve — moving again comes first.
            // The screen only shows the affordable QUANTITY, so back the unit price out of cash/afford (rounding the kept
            // quantity up, which errs toward keeping slightly more cash than asked).
            var reserve = state.LocationIndex > 0 ? _policy.CashReserve(state) : 0;
            if (reserve > 0 && afford > 0 && state.Cash > 0 && !(item == EntitiesEnum.Animal && state.Oxen == 0))
            {
                var keep = (int) Math.Ceiling(reserve * (double) afford / state.Cash);
                afford = Math.Max(0, afford - keep);
            }

            var gap = Math.Max(0, _policy.TargetQuantity(item, state) - state.OwnedOf(item));
            var qty = Math.Min(gap, afford);

            // Never leave the first store unable to move: guarantee at least three yoke while we can afford them, which
            // is also the six oxen Matt recommends.
            if (item == EntitiesEnum.Animal && state.OwnedOf(EntitiesEnum.Animal) == 0)
                qty = Math.Max(qty, Math.Min(3*lot, afford));

            // Back into whole lots, rounding UP so a target that lands mid-lot is met rather than missed — the store
            // charges by the lot either way, so asking for the fraction below would just leave the party short.
            var lots = qty <= 0 ? 0 : (qty + lot - 1)/lot;

            // Never order more lots than the quote allows, whatever the rounding did.
            var affordableLots = afford/lot;
            if (lots > affordableLots)
                lots = affordableLots;

            return lots <= 0 ? "0" : lots.ToString();
        }

        /// <summary>
        ///     How many units the store sells at a time for this good. Mirrors the sale lots declared on the item
        ///     definitions (Parts.Oxen, Resources.Bullets) — kept as a small local table rather than reading the
        ///     SimItem, because those getters re-price themselves off the live trail position on every call.
        /// </summary>
        private static int LotSizeOf(EntitiesEnum item) => item switch
        {
            EntitiesEnum.Animal => 2, // a yoke
            EntitiesEnum.Ammo => 20,  // a box
            _ => 1
        };

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

            RiverChoiceKindEnum chosen;
            if (_riverVisits > 2)
                // We keep landing back on the river menu — the policy's pick isn't executable (can't afford it). Force a
                // free crossing that always proceeds.
                chosen = options.Any(o => o.Kind == RiverChoiceKindEnum.Ford) ? RiverChoiceKindEnum.Ford : RiverChoiceKindEnum.Caulk;
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

        private static RiverChoiceKindEnum? ClassifyRiver(string label)
        {
            var l = label.ToLowerInvariant();
            if (l.Contains("ford")) return RiverChoiceKindEnum.Ford;
            if (l.Contains("caulk") || l.Contains("float")) return RiverChoiceKindEnum.Caulk;
            if (l.Contains("ferry")) return RiverChoiceKindEnum.Ferry;
            if (l.Contains("indian") || l.Contains("hire")) return RiverChoiceKindEnum.Indian;
            if (l.Contains("wait")) return RiverChoiceKindEnum.Wait;
            if (l.Contains("information")) return RiverChoiceKindEnum.MoreInfo;
            return null;
        }

        private static int ParseAfford(string screen)
        {
            var m = AffordRx.Match(screen);
            return m.Success && int.TryParse(m.Groups[1].Value.Replace(",", ""), out var n) ? n : 0;
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
