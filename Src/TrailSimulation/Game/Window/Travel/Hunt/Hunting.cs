// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/25/2015@7:56 PM

namespace TrailSimulation.Game
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Entity;

    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class Hunting : Form<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Hunting" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public Hunting(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Reference dictionary for all the animals in the game, used to help hunting mode determine what types of animals
        ///     will spawn when the player is out looking for them.
        /// </summary>
        internal static IDictionary<Entities, SimItem> DefaultAnimals
        {
            get
            {
                // Create inventory of items with default starting amounts.
                var defaultAnimals = new Dictionary<Entities, SimItem>
                {
                    {Entities.Animal, Animals.Bear},
                    {Entities.Animal, Animals.Buffalo},
                    {Entities.Animal, Animals.Caribou},
                    {Entities.Animal, Animals.Deer},
                    {Entities.Animal, Animals.Duck},
                    {Entities.Animal, Animals.Goose},
                    {Entities.Animal, Animals.Rabbit},
                    {Entities.Animal, Animals.Squirrel}
                };

                // Zero out all of the quantities by removing their max quantity.
                foreach (var animal in defaultAnimals)
                {
                    animal.Value.ReduceQuantity(animal.Value.MaxQuantity);
                }

                // Now we have default inventory of a store with all quantities zeroed out.
                return defaultAnimals;
            }
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string OnRenderForm()
        {
            throw new NotImplementedException();
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            throw new NotImplementedException();
        }
    }
}