// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/21/2015@11:24 PM

namespace TrailSimulation.Game
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Core;
    using Entity;

    /// <summary>
    ///     Shows point tabulation based on current simulation statistics. This way if the player dies or finishes the game we
    ///     just attach this state to the travel mode and it will show the final score and reset the game and return to main
    ///     menu when the player is done.
    /// </summary>
    [ParentWindow(GameWindow.GameOver)]
    public sealed class FinalPoints : InputForm<GameOverInfo>
    {
        /// <summary>
        ///     Holds the final point tabulation for the player to see.
        /// </summary>
        private StringBuilder _pointsPrompt;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FinalPoints" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public FinalPoints(IWindow window) : base(window)
        {
            _pointsPrompt = new StringBuilder();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Build up a representation of the current points the player has.
            _pointsPrompt.AppendLine($"{Environment.NewLine}Points for arriving in Oregon{Environment.NewLine}");

            // Shortcut to the game simulation instance to make code easier to read.
            var game = GameSimulationApp.Instance;

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

            // Calculates the average health just once because we need it many times.
            var avgHealth = game.Vehicle.PassengerHealth;

            // Figures out who the leader is among the vehicle passengers.
            var leaderPerson = game.Vehicle.PassengerLeader;

            // Builds up a list of tuples that represent quantity, description, and total points.
            var tuplePoints = new List<Tuple<int, string, int>>
            {
                // HealthLevel of vehicle passengers that are still alive.
                new Tuple<int, string, int>(
                    game.Vehicle.Passengers.Count,
                    $"people in {avgHealth.ToDescriptionAttribute().ToLowerInvariant()} health",
                    game.Vehicle.PassengerLivingCount*(int) avgHealth),
                new Tuple<int, string, int>(1, "wagon", Resources.Vehicle.Points),
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Animal].Quantity,
                    "oxen",
                    game.Vehicle.Inventory[Entities.Animal].Points),
                spareParts,
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Clothes].Quantity,
                    "sets of clothing",
                    game.Vehicle.Inventory[Entities.Clothes].Points),
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Ammo].Quantity,
                    "bullets",
                    game.Vehicle.Inventory[Entities.Ammo].Points),
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Food].Quantity,
                    "pounds of food",
                    game.Vehicle.Inventory[Entities.Food].Points),
                new Tuple<int, string, int>(
                    game.Vehicle.Inventory[Entities.Cash].Quantity,
                    "cash",
                    game.Vehicle.Inventory[Entities.Cash].Points)
            };

            // Create the actual points table from the tuple list data we created above from game simulation state.
            var locationTable = tuplePoints.ToStringTable(
                new[] {"Quantity", "Description", "Points"},
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

            // Add the total with the bonus so player can see the difference.
            Debug.Assert(leaderPerson != null, "leaderPerson != null");
            var totalPointsWithBonus = totalPoints*(int) leaderPerson.Profession;
            switch (leaderPerson.Profession)
            {
                case Profession.Banker:
                    break;
                case Profession.Carpenter:
                    _pointsPrompt.AppendLine($"Bonus Total: {totalPointsWithBonus}");
                    break;
                case Profession.Farmer:
                    _pointsPrompt.AppendLine($"Bonus Total: {totalPointsWithBonus}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // When building up the bonus text we will change the message about point multiplier so it makes sense.
            _pointsPrompt.AppendLine(
                $"{Environment.NewLine}For going as a {leaderPerson.Profession.ToString().ToLowerInvariant()}, your");
            switch (leaderPerson.Profession)
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

            // Add the score to the current listing that will get saved.
            GameSimulationApp.Instance.Scoring.Add(new Highscore(leaderPerson.Name, totalPointsWithBonus));

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