using Xunit;

// Both the bot control-panel app and the game app are process-wide singletons, so no two tests from
// different classes may run concurrently.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
