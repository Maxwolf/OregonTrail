using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Handles core interaction of the game, all other game types are inherited from this game mode. Deals with weather,
    ///     parties, random events, keeping track of beginning and end of the game.
    /// </summary>
    public sealed class GameSimulationApp : SimulationApp, IGameSimulation
    {
        private ClimateSimulation _climate;
        private TimeSimulation _time;
        private Vehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameSimulationApp" /> class.
        /// </summary>
        public GameSimulationApp()
        {
            _time = new TimeSimulation(1985, Months.May, 5, TravelPace.Paused);
            _time.DayEndEvent += TimeSimulation_DayEndEvent;
            _time.MonthEndEvent += TimeSimulation_MonthEndEvent;
            _time.YearEndEvent += TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent += TimeSimulation_SpeedChangeEvent;

            _climate = new ClimateSimulation(this, ClimateClassification.Moderate);
            TrailSimulation = new TrailSimulation();
            TotalTurns = 0;
            Vehicle = new Vehicle();
        }

        public TrailSimulation TrailSimulation { get; private set; }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
            private set { _vehicle = value as Vehicle; }
        }

        public uint TotalTurns { get; private set; }

        public void TakeTurn()
        {
            TotalTurns++;
            _time.TickTime();
        }

        public ITimeSimulation Time
        {
            get { return _time; }
        }

        public IClimateSimulation Climate
        {
            get { return _climate; }
        }

        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="mode">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        protected override GameMode OnModeChanging(ModeType mode)
        {
            switch (mode)
            {
                case ModeType.Travel:
                    return new TravelMode(this);
                case ModeType.ForkInRoad:
                    return new ForkInRoadMode(this);
                case ModeType.Hunt:
                    return new HuntMode(this);
                case ModeType.Landmark:
                    return new LandmarkMode(this);
                case ModeType.NewGame:
                    return new NewGameMode(this);
                case ModeType.RandomEvent:
                    return new RandomEventMode(this);
                case ModeType.RiverCrossing:
                    return new RiverCrossingMode(this);
                case ModeType.Settlement:
                    return new SettlementMode(this);
                case ModeType.Store:
                    return new StoreMode(this);
                case ModeType.Trade:
                    return new TradeMode(this);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public void ThreadSenderStartClient(object obj)
        {
            // Ensure that we only start the client after the server has created the pipe
            ManualResetEvent SyncClientServer = (ManualResetEvent)obj;

            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "ToSrvPipe", PipeDirection.Out, PipeOptions.None))
            {
                // The connect function will indefinitely wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                pipeStream.Connect();

                Console.WriteLine("[Client] Pipe connection established");
                using (StreamWriter sw = new StreamWriter(pipeStream))
                {
                    sw.AutoFlush = true;
                    string temp;
                    Console.WriteLine("Please type a message and press [Enter], or type 'quit' to exit the program");
                    while ((temp = Console.ReadLine()) != null)
                    {
                        if (temp == "quit") break;
                        sw.WriteLine(temp);
                    }
                }
            }
        }

        public void ThreadStartReceiverClient(object obj)
        {
            // Ensure that we only start the client after the server has created the pipe
            ManualResetEvent SyncClientServer = (ManualResetEvent)obj;

            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "FromSrvPipe", PipeDirection.In, PipeOptions.None))
            {
                // The connect function will indefinitely wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                pipeStream.Connect();

                Console.WriteLine("[ClientReceiver] Pipe connection established");

                using (StreamReader sr = new StreamReader(pipeStream))
                {
                    // Display the read text to the console
                    string temp;
                    while ((temp = sr.ReadLine()) != null)
                    {
                        Console.WriteLine("Received from server: {0}", temp);
                    }
                }
            }
        }

        protected override void OnFirstTick()
        {
            base.OnFirstTick();

            // TODO: FIX ME
            AddMode(ModeType.NewGame);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            // Unhook delegates from events.
            _time.DayEndEvent -= TimeSimulation_DayEndEvent;
            _time.MonthEndEvent -= TimeSimulation_MonthEndEvent;
            _time.YearEndEvent -= TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent -= TimeSimulation_SpeedChangeEvent;

            // Destroy all instances.
            _time = null;
            _climate = null;
            TrailSimulation = null;
            TotalTurns = 0;
            Vehicle = null;
        }

        private void TimeSimulation_SpeedChangeEvent()
        {
        }

        private void TimeSimulation_YearEndEvent(uint yearCount)
        {
        }

        private void TimeSimulation_DayEndEvent(uint dayCount)
        {
            _climate.TickClimate();
            Vehicle.UpdateVehicle();
            TrailSimulation.ReachedPointOfInterest();
            _vehicle.DistanceTraveled += (uint)Vehicle.Pace;
        }

        private void TimeSimulation_MonthEndEvent(uint monthCount)
        {
        }
    }
}