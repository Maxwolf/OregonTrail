// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    using System.Collections.Generic;
    using WolfCurses;

    /// <summary>
    ///     Holds all of the information required to kick-start a running game simulation onto a trail path with people,
    ///     professions, vehicle, starting items, and all stats related to luck and repair skill.
    /// </summary>
    public sealed class NewGameInfo : WindowData
    {
        /// <summary>
        ///     The _player names.
        /// </summary>
        private List<string> _playerNames;

        /// <summary>
        ///     The _player profession.
        /// </summary>
        private Profession _playerProfession;

        /// <summary>
        ///     The _starting inventory.
        /// </summary>
        private List<SimItem> _startingInventory;

        /// <summary>
        ///     The _starting monies.
        /// </summary>
        private int _startingMonies;

        /// <summary>
        ///     The _starting month.
        /// </summary>
        private Month _startingMonth;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.NewGameInfo" /> class.
        /// </summary>
        public NewGameInfo()
        {
            PlayerNameIndex = 0;
            _playerNames = new List<string>();
            _playerProfession = Profession.Banker;
            _startingInventory = new List<SimItem>();
            _startingMonies = 0;
            _startingMonth = Month.March;
            Modified = false;
        }

        /// <summary>
        ///     Index in the list of player names we are going to be inserting into.
        /// </summary>
        public int PlayerNameIndex { get; set; }

        /// <summary>
        ///     Holds all of the player names as strings until they are added to the running game simulation when start game is
        ///     called at end of new game Windows lifespan.
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
        public Month StartingMonth
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