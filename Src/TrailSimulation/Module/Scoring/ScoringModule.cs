// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation
{
    using System.Collections.Generic;
    using System.Linq;
    using SimUnit;

    /// <summary>
    ///     Keeps track of all the high scores, loads them from a default set that can always be reset to. If there are no
    ///     custom scores to be loaded then the defaults will be used, the high-score should not be reset when the simulation
    ///     is reset instead only when manually reset from he manager module for it which can be accessed by the main menu
    ///     under options.
    /// </summary>
    public sealed class ScoringModule : Module
    {
        /// <summary>
        ///     Keeps track of the total number of points the player has earned through the course of the game.
        /// </summary>
        private List<Highscore> _highScores;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScoringModule" /> class.
        ///     Scoring tracker and tabulator for end game results from current simulation state.
        /// </summary>
        public ScoringModule()
        {
            Reset();
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
        public static IEnumerable<Highscore> DefaultTopTen
        {
            get
            {
                return new List<Highscore>
                {
                    new Highscore("Stephen Meek", 7650),
                    new Highscore("Celinda Hines", 5694),
                    new Highscore("Andrew Sublette", 4138),
                    new Highscore("David Hastings", 2945),
                    new Highscore("Ezra Meeker", 2052),
                    new Highscore("Willian Vaughn", 1401),
                    new Highscore("Mary Bartlett", 937),
                    new Highscore("Willian Wiggins", 615),
                    new Highscore("Charles Hopper", 396),
                    new Highscore("Elijah White", 250)
                };
            }
        }

        /// <summary>Adds a new high-score to the list.</summary>
        /// <param name="score"></param>
        public void Add(Highscore score)
        {
            _highScores.Add(score);
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();

            // TODO: Save the high score list as JSON before it is destroyed.

            // Destroyed the high score list.
            _highScores = null;
        }

        /// <summary>
        ///     Makes the top ten list reset to the original top ten hard-coded defaults.
        /// </summary>
        public void Reset()
        {
            _highScores = new List<Highscore>(DefaultTopTen);

            // TODO: Load custom list from JSON with user high scores altered from defaults.
        }
    }
}