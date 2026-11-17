using Xunit.Sdk;
using Xunit.v3;

// The game simulation is a process-wide singleton (GameSimulationApp.Instance), so tests
// from different classes must never run at the same time.
[assembly: Parallelization(Mode = ParallelMode.None)]
