using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Timers;
using TrailCommon;
using Timer = System.Timers.Timer;

namespace TrailEntities
{
    public abstract class SimulationApp : ISimulation
    {
        private const string servername = ".";
        private const string ServerPipeName = "ToSrvPipe";
        private const string ClientPipeName = "FromSrvPipe";
        private readonly object _commandQueueLock = new object();
        private readonly Queue<Tuple<string, string>> _queuedCommands = new Queue<Tuple<string, string>>();

        private readonly Thread _receiver;
        private readonly Thread _sender;

        /// <summary>
        ///     To wait till a response is received for a request and THEN proceed
        /// </summary>
        private readonly AutoResetEvent _waitForResponse = new AutoResetEvent(false);

        /// <summary>
        ///     Equivalent to receiving a "quit" on the console
        /// </summary>
        private bool _cancelRequested;

        private string _currentId;
        private List<IMode> _modes;
        private Randomizer _random;
        private Timer _tickTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationApp" /> class.
        /// </summary>
        protected SimulationApp()
        {
            _random = new Randomizer((int) DateTime.Now.Ticks & 0x0000FFF);
            TotalTicks = 0;
            TickPhase = "*";
            _modes = new List<IMode>();

            // Create timer for every second, enabled by default, hook elapsed event.
            _tickTimer = new Timer(1000);
            _tickTimer.Elapsed += OnTickTimerFired;

            // Do not allow timer to automatically tick, this prevents it spawning multiple threads, enable the timer.
            _tickTimer.AutoReset = false;
            _tickTimer.Enabled = true;

            _sender = new Thread(syncClientServer =>
            {
                // Body of thread
                var waitForResponse = (AutoResetEvent) syncClientServer;

                using (
                    var pipeStream = new NamedPipeClientStream(servername, ServerPipeName, PipeDirection.Out,
                        PipeOptions.None)
                    )
                {
                    pipeStream.Connect();

                    using (var sw = new StreamWriter(pipeStream) {AutoFlush = true})
                        // Do this till Cancel() is called
                        while (!_cancelRequested)
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

            _receiver = new Thread(syncClientServer =>
            {
                var waitForResponse = (AutoResetEvent) syncClientServer;

                using (
                    var pipeStream = new NamedPipeClientStream(servername, ClientPipeName, PipeDirection.In,
                        PipeOptions.None)
                    )
                {
                    pipeStream.Connect();

                    using (var sr = new StreamReader(pipeStream))
                        // Do this till Cancel() is called
                        // Again, this is a tight loop, perhaps a Thread.Yield or something?
                        while (!_cancelRequested)
                            // If there's anything in the stream
                            if (!sr.EndOfStream)
                            {
                                // Read it
                                var response = sr.ReadLine();
                                // Raise the event for processing
                                // Note that this event is being raised from the
                                // receiver thread and you can't access UI here
                                // You will need to Control.BeginInvoke or some such
                                RaiseResponseReceived(_currentId, response);

                                // Proceed with sending subsequent commands
                                waitForResponse.Set();
                            }
                }
            });
        }

        public string TickPhase { get; private set; }

        public void RemoveMode(ModeType mode)
        {
            throw new NotImplementedException();
        }

        public void StartGame()
        {
            NewgameEvent?.Invoke();
        }

        public bool IsClosing { get; private set; }

        public IMode ActiveMode
        {
            get
            {
                if (_modes.Count <= 0)
                    return null;

                var lastMode = _modes[_modes.Count - 1];
                return lastMode;
            }
        }

        public string ActiveModeName
        {
            get
            {
                if (_modes.Count <= 0)
                    return "Starting";

                var lastMode = _modes[_modes.Count - 1];
                return lastMode.Mode.ToString();
            }
        }

        public ReadOnlyCollection<IMode> Modes
        {
            get { return new ReadOnlyCollection<IMode>(_modes); }
        }

        public Randomizer Random
        {
            get { return _random; }
        }

        public uint TotalTicks { get; private set; }

        public event NewGame NewgameEvent;
        public event EndGame EndgameEvent;
        public event TickSim TickEvent;
        public event ModeChanged ModeChangedEvent;

        public void AddMode(ModeType mode)
        {
            // Create new mode, check if it is in mode list.
            var changeMode = OnModeChanging(mode);
            if (!_modes.Contains(changeMode))
            {
                _modes.Add(changeMode);
                ModeChangedEvent?.Invoke(changeMode.Mode);
            }
        }

        public void CloseSimulation()
        {
            // Allow any data structures to save themselves.
            Console.WriteLine("Closing...");
            IsClosing = true;
            OnDestroy();
        }

        /// <summary>
        ///     Raises an event when a response is received
        /// </summary>
        private void RaiseResponseReceived(string id, string message)
        {
            ResponseReceived?.Invoke(this, new ResponseReceivedEventArgs(id, message));
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

        public event ResponseReceived ResponseReceived;

        protected abstract GameMode OnModeChanging(ModeType mode);

        private void OnTickTimerFired(object Sender, ElapsedEventArgs e)
        {
            Tick();

            // Allow the timer to tick again now that we have finished working.
            _tickTimer.Enabled = true;
        }

        private void Tick()
        {
            // Increase the tick count.
            TotalTicks++;

            if (TotalTicks == 1)
            {
                OnFirstTick();
            }

            TickPhase = TickVisualizer(TickPhase);

            // Fire tick event for any subscribers to see and overrides for inheriting classes.
            TickEvent?.Invoke(TotalTicks);
            OnTick();
        }

        protected virtual void OnFirstTick()
        {
            _sender.Start(_waitForResponse);
            _receiver.Start(_waitForResponse);
        }

        /// <summary>
        ///     Used for showing player that simulation is ticking on main view.
        /// </summary>
        private static string TickVisualizer(string currentPhase)
        {
            switch (currentPhase)
            {
                case @"*":
                    return @"|";
                case @"|":
                    return @"/";
                case @"/":
                    return @"-";
                case @"-":
                    return @"\";
                case @"\":
                    return @"|";
                default:
                    return "*";
            }
        }

        public virtual void OnDestroy()
        {
            _cancelRequested = true;
            _modes.Clear();
            EndgameEvent?.Invoke();
        }

        protected virtual void OnTick()
        {
            // Only tick if there are modes to tick.
            if (_modes.Count <= 0)
                return;

            // Only top-most game mode gets ticking action.
            var lastMode = _modes[_modes.Count - 1];
            lastMode?.TickMode();
        }
    }
}