using Xunit;

// The game simulation is a process-wide singleton (GameSimulationApp.Instance), so tests
// from different classes must never run at the same time.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
