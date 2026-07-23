using System;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Event;
using OregonTrailDotNet.Event.River;
using OregonTrailDotNet.Event.Vehicle;

namespace OregonTrailDotNet.Window.Travel.RiverCrossing
{
    /// <summary>
    ///     What crossing a river actually does, extracted verbatim from <see cref="CrossingTick" /> so the graphical
    ///     crossing scene runs the identical simulation: take the agreed payment, advance a random stretch of river
    ///     per tick, run the day's skip-turn, and roll the crossing's dangers — the midstream one-shot for a ford or
    ///     float, the per-tick category roll under a ferry or guide. Pinned by CrossingSimulationTests; nothing here
    ///     renders.
    /// </summary>
    internal sealed class CrossingSimulation
    {
        private readonly RiverGenerator _river;

        /// <summary>Initializes a new instance of the <see cref="CrossingSimulation" /> class.</summary>
        /// <param name="river">The river being crossed, with the chosen method and any agreed price on it.</param>
        internal CrossingSimulation(RiverGenerator river)
        {
            _river = river;
        }

        /// <summary>Feet of river behind the wagon, up to the river's width.</summary>
        internal int FeetCrossed { get; private set; }

        /// <summary>True once the far bank is reached; further ticks change nothing.</summary>
        internal bool Finished { get; private set; }

        /// <summary>
        ///     When true, a midstream disaster is recorded on <see cref="PendingDisaster" /> instead of executing
        ///     immediately. The graphical scene sets this so it can show the wagon in trouble BEFORE the event
        ///     window covers the picture with its message; the text form leaves it off and keeps the original
        ///     immediate order. The dice are rolled either way at the same moment — only the window is delayed.
        /// </summary>
        internal bool DeferMidstreamDisasters { get; set; }

        /// <summary>The deferred midstream event, waiting for the host to fire it; null when none is pending.</summary>
        internal Type PendingDisaster { get; set; }

        /// <summary>
        ///     Parks the vehicle and takes the agreed payment. The ferryman charges only a party with strictly more
        ///     cash than his fare; the Shoshoni guide takes an exact payment — greater-or-equal, matching the offer
        ///     gate in IndianGuidePrompt, since a strict compare let a party with exactly the price cross free.
        /// </summary>
        internal void Begin()
        {
            var game = GameSimulationApp.Instance;

            // Park the vehicle if it is not somehow by now.
            game.Vehicle.Status = VehicleStatusEnum.Stopped;

            // Check if ferry operator wants players monies for trip across river.
            if ((_river.FerryCost > 0) &&
                (game.Vehicle.Inventory[EntitiesEnum.Cash].TotalValue > _river.FerryCost))
            {
                game.Vehicle.Inventory[EntitiesEnum.Cash].ReduceQuantity((int) _river.FerryCost);

                // Clear out the cost for the ferry since it has been paid for now.
                _river.FerryCost = 0;
            }

            // Check if the Indian guide wants his clothes for the trip that you agreed to.
            if ((_river.IndianCost > 0) &&
                (game.Vehicle.Inventory[EntitiesEnum.Clothes].Quantity >= _river.IndianCost))
            {
                game.Vehicle.Inventory[EntitiesEnum.Clothes].ReduceQuantity(_river.IndianCost);

                // Clear out the cost for the guide since it has been paid for now.
                _river.IndianCost = 0;
            }
        }

        /// <summary>
        ///     One simulation tick of crossing. The order is the characterized original's: advance, and on the
        ///     finishing tick freeze without a turn or a danger roll; otherwise the skip-day turn runs first and the
        ///     crossing's dangers roll after it.
        /// </summary>
        internal void Tick()
        {
            if (Finished)
                return;

            var game = GameSimulationApp.Instance;

            // Increment the amount we have floated over the river.
            FeetCrossed += game.Random.Next(1, _river.RiverWidth/4);

            // Check to see if we will finish crossing river before crossing more.
            if (FeetCrossed >= _river.RiverWidth)
            {
                FeetCrossed = _river.RiverWidth;
                Finished = true;
                return;
            }

            // River crossing will allow ticking of people, vehicle, and other important events but others like
            // consuming food are disabled.
            game.TakeTurn(true);

            // Attempt to throw a random event related to some failure happening with river crossing.
            switch (_river.CrossingType)
            {
                case RiverCrossChoiceEnum.Ford:
                    // Whatever the river is going to do, it does it midstream, and only once.
                    if (!_river.DisasterHappened && (FeetCrossed >= _river.RiverWidth/2))
                        FordMidstream(game);
                    break;
                case RiverCrossChoiceEnum.Float:
                    if (!_river.DisasterHappened && (FeetCrossed >= _river.RiverWidth/2))
                        FloatMidstream(game);
                    break;
                case RiverCrossChoiceEnum.Ferry:
                case RiverCrossChoiceEnum.Indian:
                    game.EventDirector.TriggerEventByType(game.Vehicle, EventCategoryEnum.RiverCross);
                    break;
                case RiverCrossChoiceEnum.None:
                case RiverCrossChoiceEnum.WaitForWeather:
                case RiverCrossChoiceEnum.GetMoreInformation:
                    throw new InvalidOperationException(
                        $"Invalid river crossing result choice {_river.CrossingType}.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Works out what a ford costs the party, which is decided almost entirely by how deep the water is. Anything up
        ///     to two and a half feet is safe and the only question is what the riverbed does to you; up to three feet soaks
        ///     the supplies and costs a day; deeper than that and the river starts taking things, and the deeper it runs the
        ///     more it takes - it is never certain, but by five or six feet it very nearly is.
        /// </summary>
        /// <param name="game">Running simulation.</param>
        private void FordMidstream(GameSimulationApp game)
        {
            _river.DisasterHappened = true;

            // Shallow enough to walk across; only the going underfoot is in question.
            if (_river.RiverDepth < RiverGenerator.SafeFordDepth)
            {
                switch (_river.RiverBottom)
                {
                    case RiverBottomEnum.Muddy when game.Random.NextDouble() < 0.4:
                        Trigger(game, typeof(StuckInMud));
                        break;
                    case RiverBottomEnum.Rough when game.Random.NextDouble() < 0.16:
                        Trigger(game, typeof(VehicleFloods));
                        break;
                }

                return;
            }

            // Deep enough to come over the wagon bed and wet everything in it, but not to carry it off.
            if (_river.RiverDepth < RiverGenerator.DangerousFordDepth)
            {
                Trigger(game, typeof(SuppliesWet));
                return;
            }

            // Past that the river is genuinely taking things, and how much rides on how deep it is.
            if (game.Random.NextDouble() < _river.RiverDepth/10.0)
                Trigger(game, typeof(VehicleWashOut));
        }

        /// <summary>Fires a midstream disaster now, or parks it for the host when deferral is on.</summary>
        private void Trigger(GameSimulationApp game, Type eventType)
        {
            if (DeferMidstreamDisasters)
            {
                PendingDisaster = eventType;
                return;
            }

            game.EventDirector.TriggerEvent(game.Vehicle, eventType);
        }

        /// <summary>
        ///     Works out what floating the wagon costs. Depth is not the danger here - the wagon is already swimming - it is
        ///     how fast the water is moving, so a deep slow river is a better bet than a shallow fast one.
        /// </summary>
        /// <param name="game">Running simulation.</param>
        private void FloatMidstream(GameSimulationApp game)
        {
            _river.DisasterHappened = true;

            // Still water carries a caulked wagon across without complaint.
            if (_river.RiverDepth <= RiverGenerator.SafeFordDepth)
                return;

            if (game.Random.NextDouble() < _river.RiverSpeed/20.0)
                Trigger(game, typeof(VehicleFloods));
        }
    }
}
