// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Collections.Generic;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Module.Time;
using WolfCurses.Window;

namespace OregonTrailDotNet.Window.MainMenu
{
    /// <summary>
    ///     Holds all of the information required to kick-start a running game simulation onto a trail path with people,
    ///     professions, vehicle, starting items, and all stats related to luck and repair skill.
    /// </summary>
    public sealed class NewGameInfo : WindowData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrailDotNet.Window.MainMenu.NewGameInfo" /> class.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public NewGameInfo()
        {
            PlayerNameIndex = 0;
            PlayerNames = new List<string>();
            PlayerProfession = Entity.Person.Profession.Banker;
            StartingInventory = new List<SimItem>();
            StartingMonies = 0;
            StartingMonth = Month.March;
        }

        /// <summary>
        ///     Index in the list of player names we are going to be inserting into.
        /// </summary>
        public int PlayerNameIndex { get; set; }

        /// <summary>
        ///     Holds all of the player names as strings until they are added to the running game simulation when start game is
        ///     called at end of new game Windows lifespan.
        /// </summary>
        public List<string> PlayerNames { get; set; }

        /// <summary>
        ///     Determines what profession the player character is, this information is applied to the entire party as the group
        ///     leader affects every players stats.
        /// </summary>
        public Entity.Person.Profession PlayerProfession { get; set; }

        /// <summary>
        ///     References all of the starting items that the player decided to purchase from the first store interface they are
        ///     shown.
        /// </summary>
        public List<SimItem> StartingInventory { get; set; }

        /// <summary>
        ///     Starting amount of credits or monies the player has to spend on items in stores.
        /// </summary>
        public int StartingMonies { get; set; }

        /// <summary>
        ///     Starting month of the simulation, this helps determine the amount of grass for grazing, temperature, chance for
        ///     failure or random event, etc.
        /// </summary>
        public Month StartingMonth { get; set; }
    }
}