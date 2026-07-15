using System.Text;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;

namespace OregonTrailDotNet.Bot.Testing
{
    /// <summary>Running tally for one training model across an automated-testing session.</summary>
    public sealed class ModelStats
    {
        public ModelStats(string key, string displayName)
        {
            Key = key;
            DisplayName = displayName;
        }

        public string Key { get; }
        public string DisplayName { get; }
        public int Games { get; set; }
        public int Wins { get; set; }
        public int Deaths { get; set; }
        public int Timeouts { get; set; }
        public int Problems { get; set; }
    }
}
