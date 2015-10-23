using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Spawns a new game mode in the game simulation while maintaining the state of previous one so when we bounce back we
    ///     can move from here to next state.
    /// </summary>
    public sealed class BuyInitialItemsState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one.
        /// </summary>
        public BuyInitialItemsState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Complain if there is no players to add to the vehicle.
            if (userData.PlayerNames.Count <= 0)
                throw new InvalidOperationException("Cannot create vehicle with no people in new game info user data!");

            // Clear out any data amount items, monies, people that might have been in the vehicle.
            // NOTE: Sets starting monies, which was determined by player profession selection.
            GameSimulationApp.Instance.Vehicle.ResetVehicle(userData.StartingMonies);

            // Add all the player data we collected from attached game mode states.
            var crewNumber = 1;
            foreach (var name in userData.PlayerNames)
            {
                // First name in list is always the leader.
                var isLeader = userData.PlayerNames.IndexOf(name) == 0 && crewNumber == 1;
                GameSimulationApp.Instance.Vehicle.AddPerson(new Person(userData.PlayerProfession, name, isLeader));
                crewNumber++;
            }

            // Change the game mode to be a store which can work with this data.
            GameSimulationApp.Instance.AddMode(ModeType.Store);
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return "Creating store...";
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Nothing to see here, move along...
        }
    }
}