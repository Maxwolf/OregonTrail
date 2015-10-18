using System;
using System.Collections.Generic;
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
    public sealed class GameServerApp : SimulationApp, IGameServer
    {
        private ClimateSimulation _climate;
        private TimeSimulation _time;
        private Vehicle _vehicle;
        private const string ServerPipeName = "ToSrvPipe";
        private readonly Thread _sender;
        private readonly Queue<Tuple<string, string>> _queuedCommands = new Queue<Tuple<string, string>>();
        private readonly object _commandQueueLock = new object();

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameServerApp" /> class.
        /// </summary>
        public GameServerApp()
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

            _sender = new Thread(syncClientServer =>
            {
                // Body of thread
                var waitForResponse = (AutoResetEvent)syncClientServer;

                using (
                    var pipeStream = new NamedPipeClientStream(SimulationName, ServerPipeName, PipeDirection.Out,
                        PipeOptions.None)
                    )
                {
                    pipeStream.Connect();

                    using (var sw = new StreamWriter(pipeStream) { AutoFlush = true })
                        // Do this till Cancel() is called
                        while (!IsClosing)
                        {
                            // No commands? Keep waiting
                            // This is a tight loop, perhaps a Thread.Yield or something?
                            if (_queuedCommands.Count == 0)
                                continue;

                            Tuple<string, string> _currentCommand = null;

                            // We're going to modify the command queue, lock it
                            lock (_commandQueueLock)
                                // Check to see if someone else stole our command
                                // before we got here
                                if (_queuedCommands.Count > 0)
                                    _currentCommand = _queuedCommands.Dequeue();

                            // Was a command dequeued above?
                            if (_currentCommand != null)
                            {
                                _currentId = _currentCommand.Item1;
                                sw.WriteLine(_currentCommand.Item2);

                                // Wait for the response to this command
                                waitForResponse.WaitOne();
                            }
                        }
                }
            });
        }

        protected override void OnTick()
        {
            base.OnTick();
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
            var SyncClientServer = (ManualResetEvent) obj;

            using (var pipeStream = new NamedPipeClientStream(".", "ToSrvPipe", PipeDirection.Out, PipeOptions.None))
            {
                // The connect function will indefinitely wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                pipeStream.Connect();

                Console.WriteLine("[Client] Pipe connection established");
                using (var sw = new StreamWriter(pipeStream))
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
            var SyncClientServer = (ManualResetEvent) obj;

            using (var pipeStream = new NamedPipeClientStream(".", "FromSrvPipe", PipeDirection.In, PipeOptions.None))
            {
                // The connect function will indefinitely wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                pipeStream.Connect();

                Console.WriteLine("[ClientReceiver] Pipe connection established");

                using (var sr = new StreamReader(pipeStream))
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

        /// <summary>
        ///     Add a command to queue of outgoing commands.
        /// </summary>
        /// <returns>ID of the enqueued command so the user can relate it with the corresponding response.</returns>
        public string EnqueueCommand(string command)
        {
            var resultId = Guid.NewGuid().ToString();
            lock (_commandQueueLock)
            {
                _queuedCommands.Enqueue(Tuple.Create(resultId, command));
            }
            return resultId;
        }

        protected override void OnFirstTick()
        {
            _sender.Start(_waitForResponse);

            // TODO: FIX ME
            //AddMode(ModeType.NewGame);
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
            _vehicle.DistanceTraveled += (uint) Vehicle.Pace;
        }

        private void TimeSimulation_MonthEndEvent(uint monthCount)
        {
        }
    }
}