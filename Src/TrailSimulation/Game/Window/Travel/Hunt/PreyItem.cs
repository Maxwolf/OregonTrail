// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/26/2015@11:47 PM

namespace TrailSimulation.Game
{
    using Core;
    using Entity;

    /// <summary>
    ///     Defines a given animal the player can shoot and kill for it's meat. Depending on weather and current conditions the
    ///     type of animal created may vary.
    /// </summary>
    public sealed class PreyItem : ITick
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.PreyItem" /> class.
        /// </summary>
        public PreyItem()
        {
            // Starts the shoot time at zero.
            ShootTime = 0;

            // Randomly generate a shooting time up to total hunting time.
            ShootTimeMax = GameSimulationApp.Instance.Random.Next(Hunting.HUNTINGTIME);

            // Randomly select a animal for this prey to be from default animals.
            var preyIndex = GameSimulationApp.Instance.Random.Next(Hunting.DefaultAnimals.Count);

            // Select a random animal that we can be from default animals.
            Animal = new SimItem(Hunting.DefaultAnimals[preyIndex], 1);
        }

        /// <summary>
        ///     Determines what type of animal this prey will be if the player kills it.
        /// </summary>
        public SimItem Animal { get; }

        /// <summary>
        ///     Keeps track of the total number of seconds this prey has been visible. Once it reaches it's maximum shoot time it
        ///     will be removed from the list of possible animals the player can hunt.
        /// </summary>
        public int ShootTime { get; private set; }

        /// <summary>
        ///     Maximum amount of time this animal will be in the list of things that can be killed and are available on the field.
        /// </summary>
        public int ShootTimeMax { get; }


        /// <summary>
        ///     Determines if the animal is currently on the field and able to be shot by the player.
        /// </summary>
        public bool Visible { get; private set; }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public void OnTick(bool systemTick, bool skipDay)
        {
            // No work is done on system ticks.
            if (systemTick)
                return;

            // No work is done if force ticked.
            if (skipDay)
                return;

            // Increment the total shot time until maximum.
            if (ShootTime < ShootTimeMax)
            {
                ShootTime++;
                Visible = true;
            }
            else
            {
                ShootTime = ShootTimeMax;
                Visible = false;
            }
        }
    }
}