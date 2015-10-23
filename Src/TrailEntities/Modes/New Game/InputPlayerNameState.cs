using TrailCommon;

namespace TrailEntities
{
    public abstract class InputPlayerNameState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will create new state taking values from old state
        /// </summary>
        protected InputPlayerNameState(ModeState<NewGameInfo> state) : base(state)
        {
        }

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        protected InputPlayerNameState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
        }

        internal string GetPlayerName()
        {
            // Just return a random name if there is invalid input.
            string[] names = { "Bob", "Joe", "Sally", "Tim", "Steve" };
            return names[GameSimulationApp.Instance.Random.Next(names.Length)];
        }
    }
}