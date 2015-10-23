using TrailCommon;

namespace TrailEntities
{
    public sealed class StartGameState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public StartGameState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
        }

        public override void TickState()
        {
            foreach (var name in UserData.PlayerNames)
            {
                // First name in list in leader.
                var isLeader = UserData.PlayerNames.IndexOf(name) == 0;
                GameSimulationApp.Instance.Vehicle.AddPerson(new Person(UserData.PlayerProfession, name, isLeader));
                GameSimulationApp.Instance.StartGame();
            }
        }

        public override string GetStateTUI()
        {
            return "Adding " + UserData.PlayerNames.Count + " people to vehicle...";
        }

        public override void OnInputBufferReturned(string input)
        {
            // Nothing to see here, move along...
        }
    }
}
