namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     A named training model the user can pick when creating a profile. It pairs a <em>representation</em> (how a search
    ///     vector becomes an <see cref="IPolicy" />) with a <em>search algorithm</em> (<see cref="IOptimizer" />). A profile is
    ///     bound to one model for its life, so its saved learning state is always interpreted by the same model.
    /// </summary>
    public interface ITrainingModel
    {
        /// <summary>Stable identifier stored in the profile's <c>policy_kind</c>.</summary>
        string Key { get; }

        string DisplayName { get; }
        string Description { get; }

        /// <summary>Length of the vector the optimizer searches.</summary>
        int VectorLength { get; }

        double[] InitialMean();
        double[] InitialStd();

        /// <summary>Turns a search vector into a concrete playing policy.</summary>
        IPolicy Decode(double[] vector, string leaderName);

        /// <summary>Creates the search algorithm this model uses, seeded with the model's initial distribution.</summary>
        IOptimizer CreateOptimizer(int populationSize);
    }
}
