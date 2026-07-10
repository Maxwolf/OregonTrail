namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A tiny fixed-topology multilayer perceptron: inputs → one tanh hidden layer → linear outputs. All weights (and per
    ///     neuron biases) come from a flat vector, so an evolutionary optimizer can search them directly. Used by
    ///     <see cref="NeuralPolicy" /> to make state-dependent tactical decisions.
    /// </summary>
    public sealed class Mlp
    {
        public const int Inputs = 11;
        public const int Hidden = 8;
        public const int Outputs = 6;

        /// <summary>Total weights: each hidden neuron has a bias + one weight per input; each output a bias + one per hidden.</summary>
        public const int WeightCount = Hidden * (Inputs + 1) + Outputs * (Hidden + 1); // 96 + 54 = 150

        private readonly double[] _weights;
        private readonly int _offset;

        public Mlp(double[] weights, int offset = 0)
        {
            _weights = weights;
            _offset = offset;
        }

        /// <summary>Runs the network. <paramref name="inputs" /> must have length <see cref="Inputs" />.</summary>
        public double[] Forward(double[] inputs)
        {
            var w = _offset;

            var hidden = new double[Hidden];
            for (var j = 0; j < Hidden; j++)
            {
                var sum = _weights[w++]; // bias
                for (var i = 0; i < Inputs; i++)
                    sum += _weights[w++] * inputs[i];
                hidden[j] = Math.Tanh(sum);
            }

            var outputs = new double[Outputs];
            for (var k = 0; k < Outputs; k++)
            {
                var sum = _weights[w++]; // bias
                for (var j = 0; j < Hidden; j++)
                    sum += _weights[w++] * hidden[j];
                outputs[k] = sum;
            }

            return outputs;
        }
    }
}
