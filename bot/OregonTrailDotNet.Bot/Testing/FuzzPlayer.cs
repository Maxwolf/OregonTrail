using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>
    ///     Plays one live game for a given training model using a fresh, ever-varying policy sampled from the model's own
    ///     optimizer (no learning — it just keeps drawing candidates). Shared by the automated-testing and benchmark sessions
    ///     so both explore many different game states rather than replaying one fixed strategy.
    /// </summary>
    public sealed class FuzzPlayer
    {
        // How many varied candidate vectors to draw from a model at a time before drawing another batch.
        private const int SampleBatch = 16;

        private readonly Dictionary<string, IOptimizer> _optimizers;
        private readonly Dictionary<string, Queue<double[]>> _queues;

        public FuzzPlayer(IReadOnlyList<ITrainingModel> models)
        {
            _optimizers = models.ToDictionary(m => m.Key, m => m.CreateOptimizer(SampleBatch));
            _queues = models.ToDictionary(m => m.Key, _ => new Queue<double[]>());
        }

        public RunResult Play(ITrainingModel model)
        {
            var queue = _queues[model.Key];
            if (queue.Count == 0)
                foreach (var vector in _optimizers[model.Key].Sample())
                    queue.Enqueue(vector);

            var candidate = queue.Count > 0 ? queue.Dequeue() : model.InitialMean();
            var policy = model.Decode(candidate, $"{model.DisplayName} (test)");
            return GamePlayer.PlayOnce(policy);
        }
    }
}
