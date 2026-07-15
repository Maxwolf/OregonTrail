// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Collections.Generic;
using System.Linq;
using OregonTrailDotNet.Persistence;

namespace OregonTrailDotNet.Module.Scoring
{
    /// <summary>
    ///     Keeps track of all the high scores, loads them from a default set that can always be reset to. If there are no
    ///     custom scores to be loaded then the defaults will be used, the high-score should not be reset when the simulation
    ///     is reset instead only when manually reset from he manager module for it which can be accessed by the main menu
    ///     under options.
    /// </summary>
    public sealed class ScoringModule : WolfCurses.Module.Module
    {
        /// <summary>
        ///     Keeps track of the total number of points the player has earned through the course of the game.
        /// </summary>
        private List<Highscore> _highScores;

        /// <summary>Optional persistence for player-earned scores; null keeps the module purely in-memory (bot/tests).</summary>
        private readonly HighScoreStore _store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScoringModule" /> class.
        ///     Scoring tracker and tabulator for end game results from current simulation state.
        /// </summary>
        /// <param name="store">High-score persistence, or null to run without saving (in-memory only).</param>
        public ScoringModule(HighScoreStore store = null)
        {
            _store = store;
            Load();
        }

        /// <summary>
        ///     Keeps track of the total number of points the player has earned through the course of the game.
        /// </summary>
        public IEnumerable<Highscore> TopTen
        {
            get { return _highScores.OrderByDescending(x => x.Points).Take(10); }
        }

        /// <summary>
        ///     Original high scores from Apple II version of the game.
        /// </summary>
        public static IEnumerable<Highscore> DefaultTopTen => new List<Highscore>
        {
            new Highscore("Stephen Meek", 7650),
            new Highscore("Celinda Hines", 5694),
            new Highscore("Andrew Sublette", 4138),
            new Highscore("David Hastings", 2945),
            new Highscore("Ezra Meeker", 2052),
            new Highscore("William Vaughn", 1401),
            new Highscore("Mary Bartlett", 937),
            new Highscore("William Wiggins", 615),
            new Highscore("Charles Hopper", 396),
            new Highscore("Elijah White", 250)
        };

        /// <summary>Adds a new high-score to the list, persisting it to the game database when persistence is enabled.</summary>
        /// <param name="score"></param>
        public void Add(Highscore score)
        {
            _highScores.Add(score);
            _store?.Insert(score.Name, score.Points);
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly. Scores are already saved on the way in (see <see cref="Add" />), so there is nothing to flush here.
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();

            // Destroyed the high score list.
            _highScores = null;
        }

        /// <summary>
        ///     Makes the top ten list reset to the original hard-coded defaults, discarding any player-earned scores. When
        ///     persistence is on this also clears the saved scores from the game database.
        /// </summary>
        public void Reset()
        {
            _store?.Clear();
            _highScores = new List<Highscore>(DefaultTopTen);
        }

        /// <summary>
        ///     Seeds the working list from the original defaults, then layers any player-earned scores loaded from the game
        ///     database on top (the top ten is computed on read). Without a database this is just the defaults, exactly as before.
        /// </summary>
        private void Load()
        {
            _highScores = new List<Highscore>(DefaultTopTen);

            if (_store == null)
                return;

            foreach (var (name, points) in _store.All())
                _highScores.Add(new Highscore(name, points));
        }
    }
}