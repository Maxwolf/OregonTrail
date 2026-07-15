using System.Text.Json;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Freezes the wire format of the persisted learning genome. <see cref="StrategyGenome" /> serializes ONLY its raw
    ///     double vector, as a bare JSON array (positional — no property names), so the BestGenomeJson / GenomeJson columns in
    ///     the bot database carry no C# member names and are immune to renaming the genome's decode members. This test fails if
    ///     serialization ever starts emitting a named object, which would break every genome saved by an earlier build.
    /// </summary>
    public sealed class GenomeSerializationFreezeTests
    {
        [Fact]
        public void Genome_Serializes_As_A_Bare_Positional_Double_Array()
        {
            var json = StrategyGenome.Default().ToJson();

            using var doc = JsonDocument.Parse(json);
            // A positional array, NOT a named object: the wire carries values by index, never by member name.
            Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
            Assert.Equal(StrategyGenome.Length, doc.RootElement.GetArrayLength());
        }

        [Fact]
        public void Genome_Round_Trips_Through_Json_Unchanged()
        {
            var original = StrategyGenome.Default();
            var restored = StrategyGenome.FromJson(original.ToJson());
            Assert.Equal(original.Raw, restored.Raw);
        }
    }
}
