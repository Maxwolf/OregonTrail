using System;
using System.Text;
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
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            var storeHelp = new StringBuilder();
            storeHelp.Append("Matt's General Store is the first place in the game to buy supplies.\n");
            storeHelp.Append("This is where you stock up with everything you need to start your journey.\n");
            storeHelp.Append("Stock up early if you can!\n\n");

            storeHelp.Append("Matt offers you some free pieces of advice:\n");
            storeHelp.Append("'I recommend at least six oxen to pull your wagon. And you'll need plenty of\n");
            storeHelp.Append("flour, sugar, bacon, coffee, and other types of food. I suggest you start out\n");
            storeHelp.Append("with at least 200 pounds for each person in your party'\n\n");

            storeHelp.Append("'You'll need good, warm clothing, especially for the mountains. I recommend\n");
            storeHelp.Append("taking at least 2 sets of clothing per person. You'll need ammunition, too.\n");
            storeHelp.Append("Each box of ammunition contains 20 bullets.'\n\n");

            storeHelp.Append("'Finally, you might want to take along some spare wagon parts.  Wagon Wheels,\n");
            storeHelp.Append("axles, and tongues are liable to break along the way. If you're unable to\n");
            storeHelp.Append("repair a broken wagon, you'll be in big trouble!'\n\n");

            storeHelp.Append("Press RETURN key to enter the store.\n");
            return storeHelp.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Change the game mode to be a store which can work with this data.
            GameSimulationApp.Instance.AddMode(ModeType.Store);
        }
    }
}