using System.Collections.Generic;
using TrailEntities.Entity;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Holds all of the information required to kick-start a running game simulation onto a trail path with people,
    ///     professions, vehicle, starting items, and all stats related to luck and repair skill.
    /// </summary>
    public sealed class MainMenuInfo : IModeInfo
    {
        private List<string> _playerNames;
        private Profession _playerProfession;
        private List<SimItem> _startingInventory;
        private int _startingMonies;
        private Months _startingMonth;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Game.MainMenuInfo" /> class.
        /// </summary>
        public MainMenuInfo()
        {
            _playerNames = new List<string>();
            _playerProfession = Profession.Banker;
            _startingInventory = new List<SimItem>();
            _startingMonies = 0;
            _startingMonth = Months.March;
            Modified = false;
        }

        /// <summary>
        ///     Holds all of the player names as strings until they are added to the running game simulation when start game is
        ///     called at end of new game mode lifespan.
        /// </summary>
        public List<string> PlayerNames
        {
            get { return _playerNames; }
            set
            {
                _playerNames = value;
                Modified = true;
            }
        }

        /// <summary>
        ///     Determines what profession the player character is, this information is applied to the entire party as the group
        ///     leader affects every players stats.
        /// </summary>
        public Profession PlayerProfession
        {
            get { return _playerProfession; }
            set
            {
                _playerProfession = value;
                Modified = true;
            }
        }

        /// <summary>
        ///     References all of the starting items that the player decided to purchase from the first store interface they are
        ///     shown.
        /// </summary>
        public List<SimItem> StartingInventory
        {
            get { return _startingInventory; }
            set
            {
                _startingInventory = value;
                Modified = true;
            }
        }

        /// <summary>
        ///     Starting amount of credits or monies the player has to spend on items in stores.
        /// </summary>
        public int StartingMonies
        {
            get { return _startingMonies; }
            set
            {
                _startingMonies = value;
                Modified = true;
            }
        }

        /// <summary>
        ///     Starting month of the simulation, this helps determine the amount of grass for grazing, temperature, chance for
        ///     failure or random event, etc.
        /// </summary>
        public Months StartingMonth
        {
            get { return _startingMonth; }
            set
            {
                _startingMonth = value;
                Modified = true;
            }
        }

        /// <summary>
        ///     Determines if the initial new game info object has been altered from the defaults in any way by calling sets on
        ///     other properties.
        /// </summary>
        private bool Modified { get; set; }
    }
}