using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     Per-game supply decision state shared by every policy: when to walk into a fort store, when to browse emigrant
    ///     trades, and the loop guards that keep both bounded. Policies consult it first in ChooseTravel and fall back to
    ///     their own tactics when it returns null. One instance per policy per game — the counters ARE the loop guards.
    /// </summary>
    internal sealed class SupplyPlanner
    {
        private string? _restockedAt;
        private string? _opportunisticAt;
        private int _opportunisticAttempts;
        private int _desperateTradeAttempts;
        private bool _desperateStoreTried;

        public TravelCommandsEnum? Decide(GameSnapshot state, IReadOnlyCollection<TravelCommandsEnum> available,
            Func<EntitiesEnum, int> target)
        {
            if (SupplyTactics.IsDesperate(state))
            {
                // The fort store is a GUARANTEED seller of the item that un-strands the wagon — take it before gambling
                // on random emigrant offers, and bypass the once-per-stop guard (a party can strand at the very fort it
                // already shopped at). One try per stranding episode so an unaffordable store can't ping-pong forever.
                if (!_desperateStoreTried && available.Contains(TravelCommandsEnum.BuySupplies) && state.Cash >= 20)
                {
                    _desperateStoreTried = true;
                    _restockedAt = state.LocationName;
                    return TravelCommandsEnum.BuySupplies;
                }

                if (available.Contains(TravelCommandsEnum.AttemptToTrade) &&
                    _desperateTradeAttempts < SupplyTactics.MaxDesperateTradeAttempts)
                {
                    _desperateTradeAttempts++;
                    return TravelCommandsEnum.AttemptToTrade;
                }

                // Budget exhausted: let the policy fall through (ContinueOnTrail) so a hopeless stranding still ends.
                return null;
            }

            _desperateTradeAttempts = 0;
            _desperateStoreTried = false;

            // Restock at a fort store once per stop: top every tracked item back up toward its target (the store session
            // buys oxen and food first). This is the standing counter to mid-trail oxen loss.
            if (available.Contains(TravelCommandsEnum.BuySupplies) && state.LocationName != _restockedAt &&
                SupplyTactics.ShouldRestock(state, target))
            {
                _restockedAt = state.LocationName;
                return TravelCommandsEnum.BuySupplies;
            }

            // Opportunistic browsing: a team below the 6-ox full-speed floor or a missing spare part is one bad event
            // from stranding, and no store may be near. Check a few emigrant offers per stop; the policy's margin rule
            // (SupplyTactics.AcceptTrade) decides whether any of them is worth taking.
            if (available.Contains(TravelCommandsEnum.AttemptToTrade) && WantsOpportunisticTrade(state))
            {
                if (state.LocationName != _opportunisticAt)
                {
                    _opportunisticAt = state.LocationName;
                    _opportunisticAttempts = 0;
                }

                if (_opportunisticAttempts < 3)
                {
                    _opportunisticAttempts++;
                    return TravelCommandsEnum.AttemptToTrade;
                }
            }

            return null;
        }

        private static bool WantsOpportunisticTrade(GameSnapshot state) =>
            state.Oxen < 6 || state.Wheels <= 0 || state.Axles <= 0 || state.Tongues <= 0;
    }
}
