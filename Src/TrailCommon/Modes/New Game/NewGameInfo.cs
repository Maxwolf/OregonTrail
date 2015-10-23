using System.Collections.Generic;

namespace TrailCommon
{
    /// <summary>
    ///     Holds all of the information required to kick-start a running game simulation onto a trail path with people,
    ///     professions, vehicle, starting items, and all stats related to luck and repair skill.
    /// </summary>
    public sealed class NewGameInfo
    {
        private List<string> _playerNames;
        private Profession _playerProfession;
        private List<IItem> _startingInventory;
        private uint _startingMonies;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.NewGameInfo" /> class.
        /// </summary>
        public NewGameInfo(List<string> playerNames, Profession playerProfession,
            List<IItem> startingInventory)
        {
            _playerNames = playerNames;
            _playerProfession = playerProfession;
            _startingInventory = startingInventory;
            _startingMonies = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.NewGameInfo" /> class.
        /// </summary>
        public NewGameInfo()
        {
            _playerNames = new List<string>();
            _playerProfession = Profession.Banker;
            _startingInventory = new List<IItem>();
            _startingMonies = 0;
        }

        /// <summary>
        ///     Holds all of the player names as strings until they are added to the running game simulation when start game is
        ///     called at end of new game mode lifespan.
        /// </summary>
        public List<string> PlayerNames
        {
            get { return _playerNames; }
            set { _playerNames = value; }
        }

        /// <summary>
        ///     Determines what profession the player character is, this information is applied to the entire party as the group
        ///     leader affects every players stats.
        /// </summary>
        public Profession PlayerProfession
        {
            get { return _playerProfession; }
            set { _playerProfession = value; }
        }

        /// <summary>
        ///     References all of the starting items that the player decided to purchase from the first store interface they are
        ///     shown.
        /// </summary>
        public List<IItem> StartingInventory
        {
            get { return _startingInventory; }
            set { _startingInventory = value; }
        }

        /// <summary>
        ///     Starting amount of credits or monies the player has to spend on items in stores.
        /// </summary>
        public uint StartingMonies
        {
            get { return _startingMonies; }
            set { _startingMonies = value; }
        }
    }
}