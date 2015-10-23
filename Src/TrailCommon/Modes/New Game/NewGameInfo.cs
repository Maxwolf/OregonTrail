using System.Collections.Generic;

namespace TrailCommon
{
    public sealed class NewGameInfo
    {
        private List<string> _playerNames;
        private Profession _playerProfession;
        private List<IItem> _startingInventory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.NewGameInfo" /> class.
        /// </summary>
        public NewGameInfo(List<string> playerNames, Profession playerProfession, List<IItem> startingInventory)
        {
            _playerNames = playerNames;
            _playerProfession = playerProfession;
            _startingInventory = startingInventory;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.NewGameInfo" /> class.
        /// </summary>
        public NewGameInfo()
        {
            _playerNames = new List<string>(4);
            _playerProfession = Profession.Banker;
            _startingInventory = new List<IItem>();
        }

        public List<string> PlayerNames
        {
            get { return _playerNames; }
            set { _playerNames = value; }
        }

        public Profession PlayerProfession
        {
            get { return _playerProfession; }
            set { _playerProfession = value; }
        }

        public List<IItem> StartingInventory
        {
            get { return _startingInventory; }
            set { _startingInventory = value; }
        }
    }
}