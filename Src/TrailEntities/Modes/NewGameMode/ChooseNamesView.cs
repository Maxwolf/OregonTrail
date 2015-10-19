using System;
using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    public class ChooseNamesView : ConcreteCommand
    {
        private List<string> _playerNames;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ChooseNamesView" /> class.
        /// </summary>
        public ChooseNamesView(IGameSimulation game) : base(game)
        {
            _playerNames = new List<string>();
        }

        private string GetPlayerName()
        {
            var readLine = Console.ReadLine();
            if (readLine != null)
            {
                readLine = readLine.Trim();
                if (!string.IsNullOrEmpty(readLine) &&
                    !string.IsNullOrWhiteSpace(readLine))
                {
                    return readLine;
                }
            }

            // Just return a random name if there is invalid input.
            string[] names = {"Bob", "Joe", "Sally", "Tim", "Steve"};
            return names[Game.Random.Next(names.Length)];
        }

        private void ChooseNames()
        {
            Console.Clear();
            Console.WriteLine("Party leader name?");
            var playerName = GetPlayerName();
            _playerNames.Add(playerName);
            Console.WriteLine("Added: " + playerName);

            Console.WriteLine("Party member two name?");
            playerName = GetPlayerName();
            _playerNames.Add(playerName);
            Console.WriteLine("Added: " + playerName);

            Console.WriteLine("Party member three name?");
            playerName = GetPlayerName();
            _playerNames.Add(playerName);
            Console.WriteLine("Added: " + playerName);

            Console.WriteLine("Party member four name?");
            playerName = GetPlayerName();
            _playerNames.Add(playerName);
            Console.WriteLine("Added: " + playerName);

            Console.WriteLine("Your Party Members:");
            var crewNumber = 1;
            foreach (var name in _playerNames)
            {
                var isLeader = _playerNames.IndexOf(name) == 0;
                if (isLeader)
                {
                    Console.WriteLine(crewNumber + ")." + name + " (leader)");
                }
                else
                {
                    Console.WriteLine(crewNumber + ")." + name);
                }
                crewNumber++;
            }

            Console.WriteLine("Does this look correct? Y/N");
            var namesCorrectResponse = Console.ReadLine();
            if (!string.IsNullOrEmpty(namesCorrectResponse))
            {
                namesCorrectResponse = namesCorrectResponse.Trim().ToLowerInvariant();
                if (namesCorrectResponse.Equals("n"))
                {
                    _playerNames.Clear();
                    ChooseNames();
                }
            }
            else
            {
                _playerNames.Clear();
                ChooseNames();
            }
        }

        public override void Execute()
        {
            base.Execute();

            ChooseNames();
        }
    }
}