using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>
    ///     Plays one live game for a given training model using a policy sampled from the model's own optimizer. With
    ///     <c>learn: false</c> (automated testing) it never folds results back, so it just keeps drawing varied candidates to
    ///     explore many game states. With <c>learn: true</c> (the benchmark) it accumulates each batch's fitness and folds it
    ///     back into the optimizer between batches, so the models genuinely improve — turning "time to first win" into a real
    ///     learning race rather than a measurement of the zero-shot warm-start prior (which was identical across models).
    /// </summary>
    public sealed class FuzzPlayer
    {
        // How many varied candidate vectors to draw from a model at a time before drawing another batch.
        private const int SampleBatch = 16;

        private readonly bool _learn;
        private readonly Dictionary<string, IOptimizer> _optimizers;
        private readonly Dictionary<string, Queue<double[]>> _queues;
        // When learning, the fitness of each drawn candidate, held until a full batch drains and is folded back in.
        private readonly Dictionary<string, List<(double[] Vector, double Fitness)>> _scored;

        public FuzzPlayer(IReadOnlyList<ITrainingModel> models, bool learn = false)
        {
            _learn = learn;
            _optimizers = models.ToDictionary(m => m.Key, m => m.CreateOptimizer(SampleBatch));
            _queues = models.ToDictionary(m => m.Key, _ => new Queue<double[]>());
            _scored = models.ToDictionary(m => m.Key, _ => new List<(double[], double)>());
        }

        public RunResult Play(ITrainingModel model)
        {
            var queue = _queues[model.Key];
            if (queue.Count == 0)
            {
                // A batch has drained. When learning, fold the batch's scored candidates back into the optimizer so the next
                // batch is drawn from an improved distribution; then refill from the (now-updated) optimizer.
                if (_learn && _scored[model.Key].Count > 0)
                {
                    _optimizers[model.Key].Update(_scored[model.Key]);
                    _scored[model.Key].Clear();
                }

                foreach (var vector in _optimizers[model.Key].Sample())
                    queue.Enqueue(vector);
            }

            var candidate = queue.Count > 0 ? queue.Dequeue() : model.InitialMean();
            var policy = model.Decode(candidate, $"{model.DisplayName} (test)");
            var result = GamePlayer.PlayOnce(policy);

            if (_learn)
                _scored[model.Key].Add((candidate, TrainingSession.Fitness(result)));

            return result;
        }
    }
}
