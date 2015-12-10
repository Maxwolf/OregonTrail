using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shows point tabulation based on current simulation statistics. This way if the player dies or finishes the game we
    ///     just attach this state to the travel mode and it will show the final score and reset the game and return to main
    ///     menu when the player is done.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class FinalPoints : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Holds the final point tabulation for the player to see.
        /// </summary>
        private StringBuilder _pointsPrompt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public FinalPoints(IWindow window) : base(window)
        {
            _pointsPrompt = new StringBuilder();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Build up a representation of the current points the player has.
            _pointsPrompt.AppendLine($"{Environment.NewLine}Points for arriving in Oregon");

            // Shortcut to the game simulation instance to make code easier to read.
            var game = GameSimulationApp.Instance;

            // Builds up a list of enumeration health values for living passengers.
            var alivePersonsHealth = new List<Health>();
            foreach (var person in game.Vehicle.Passengers)
            {
                // Only add the health to average calculation if person is not dead.
                if (!person.IsDead)
                    alivePersonsHealth.Add(person.Health);
            }

            // Casts all the enumeration health values to integers and averages them.
            var averageHealthValue = (int)alivePersonsHealth.Cast<int>().Average();

            // If we fail to parse the health value we averaged then we set health to very poor as default.
            Health averageHealth;
            if (!Enum.TryParse(averageHealthValue.ToString(CultureInfo.InvariantCulture), true, out averageHealth))
                averageHealth = Health.VeryPoor;

            // Calculate the total points of all spare parts for the tuple list below ahead of time.
            var spareAxles = new Tuple<int, string, int>(
                game.Vehicle.Inventory[Entities.Axle].Quantity,
                game.Vehicle.Inventory[Entities.Axle].PluralForm,
                game.Vehicle.Inventory[Entities.Axle].Points);

            var spareTongues = new Tuple<int, string, int>(
                game.Vehicle.Inventory[Entities.Tongue].Quantity,
                game.Vehicle.Inventory[Entities.Tongue].PluralForm,
                game.Vehicle.Inventory[Entities.Tongue].Points);

            var spareWheels = new Tuple<int, string, int>(
                game.Vehicle.Inventory[Entities.Wheel].Quantity,
                game.Vehicle.Inventory[Entities.Wheel].PluralForm,
                game.Vehicle.Inventory[Entities.Wheel].Points);

            var spareParts = new Tuple<int, string, int>(
                spareAxles.Item1 + spareTongues.Item1 + spareWheels.Item1,
                "spare wagon parts",
                spareAxles.Item3 + spareTongues.Item3 + spareWheels.Item3);

            // Builds up a list of tuples that represent quantity, description, and total points.
            var tuplePoints = new List<Tuple<int, string, int>>
            {
                // Health of vehicle passengers.
                new Tuple<int, string, int>(
                    game.Vehicle.Passengers.Count(),
                    $"people in {((Health) averageHealth).ToDescriptionAttribute().ToLowerInvariant()} health",
                    Resources.Person.Points*game.Vehicle.Passengers.Count()),
                // Vehicle existence counts for some points.
                new Tuple<int, string, int>(1, "wagon", Resources.Vehicle.Points),
                // Number of oxen still alive pulling vehicle.
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Animal].Quantity,
                    game.Vehicle.Inventory[Entities.Animal].PluralForm,
                    game.Vehicle.Inventory[Entities.Animal].Points),
                // Spare vehicle parts.
                spareParts,
                // Clothing
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Clothes].Quantity,
                    game.Vehicle.Inventory[Entities.Clothes].PluralForm,
                    game.Vehicle.Inventory[Entities.Clothes].Points),
                // Bullets
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Ammo].Quantity,
                    game.Vehicle.Inventory[Entities.Ammo].PluralForm,
                    game.Vehicle.Inventory[Entities.Ammo].Points),
                // Food
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Food].Quantity,
                    game.Vehicle.Inventory[Entities.Food].PluralForm,
                    game.Vehicle.Inventory[Entities.Food].Points),
                // Cash
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Cash].Quantity,
                    game.Vehicle.Inventory[Entities.Cash].PluralForm,
                    game.Vehicle.Inventory[Entities.Cash].Points)
            };

            // Create the actual points table from the tuple list data we created above from game simulation state.
            var locationTable = tuplePoints.ToStringTable(
                new[] { "Quantity", "Description", "Points" },
                u => u.Item1,
                u => u.Item2,
                u => u.Item3
                );
            _pointsPrompt.AppendLine(locationTable);

            // Calculate total points for all entities and items.
            var totalPoints = 0;
            foreach (var tuplePoint in tuplePoints)
            {
                totalPoints += tuplePoint.Item3;
            }
            _pointsPrompt.AppendLine($"Total: {totalPoints}");

            // Grab the leaders profession, used to determine points multiplier at end.
            var leaderProfession = Profession.Banker;
            foreach (var person in game.Vehicle.Passengers)
            {
                // Add leader position when we come by it.
                if (person.IsLeader)
                    leaderProfession = person.Profession;
            }

            // Add the total with the bonus so player can see the difference.
            switch (leaderProfession)
            {
                case Profession.Banker:
                    // Banker doesn't get this print out since he gets no bonus.
                    break;
                case Profession.Carpenter:
                    _pointsPrompt.AppendLine($"Total with bonus: {totalPoints * (int)leaderProfession}");
                    break;
                case Profession.Farmer:
                    _pointsPrompt.AppendLine($"Total with bonus: {totalPoints * (int)leaderProfession}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // When building up the bonus text we will change the message about point multiplier so it makes sense.
            _pointsPrompt.AppendLine($"For going as a {leaderProfession.ToString().ToLowerInvariant()}, your");
            switch (leaderProfession)
            {
                case Profession.Banker:
                    _pointsPrompt.AppendLine($"points are normal, no bonus!{Environment.NewLine}");
                    break;
                case Profession.Carpenter:
                    _pointsPrompt.AppendLine($"points are doubled.{Environment.NewLine}");
                    break;
                case Profession.Farmer:
                    _pointsPrompt.AppendLine($"points are tripled.{Environment.NewLine}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _pointsPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Completely resets the game to default state it was in when it first started.
            GameSimulationApp.Instance.Restart();
        }
    }
}