// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@2:38 PM

using System;

namespace OregonTrailDotNet.WolfCurses.Core
{
    /// <summary>
    ///     Used for rolling the virtual dice in the simulation to determine the outcome of various events.
    /// </summary>
    public sealed class Randomizer : Module.Module
    {
        /// <summary>
        ///     Game logic objects.
        /// </summary>
        private Random _random;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Randomizer" /> class.
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.ModuleProduct" /> class.
        /// </summary>
        public Randomizer()
        {
            // Create a unique random seed based on current system tick.
            RandomSeed = (int) DateTime.Now.Ticks & 0x0000FFF;
            _random = new Random(RandomSeed);
        }

        /// <summary>
        ///     Number used to seed the random number generator.
        /// </summary>
        private int RandomSeed { get; }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            _random = null;
        }

        /// <summary>
        ///     C64 style RND with 0 would return clock timer 0 - 60 number so we do the same here for simulation.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int Next()
        {
            return _random.Next(60);
        }

        /// <summary>Returns a random number within a specified range.</summary>
        /// <returns>
        ///     A 32-bit signed integer greater than or equal to <paramref name="minValue" /> and less than
        ///     <paramref name="maxValue" />; that is, the range of return values includes <paramref name="minValue" /> but not
        ///     <paramref name="maxValue" />. If <paramref name="minValue" /> equals <paramref name="maxValue" />,
        ///     <paramref name="minValue" /> is returned.
        /// </returns>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">
        ///     The exclusive upper bound of the random number returned. <paramref name="maxValue" /> must be
        ///     greater than or equal to <paramref name="minValue" />.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="minValue" /> is greater than
        ///     <paramref name="maxValue" />.
        /// </exception>
        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        /// <summary>Returns a nonnegative random number less than the specified maximum.</summary>
        /// <returns>
        ///     A 32-bit signed integer greater than or equal to zero, and less than <paramref name="maxValue" />; that is, the
        ///     range of return values ordinarily includes zero but not <paramref name="maxValue" />. However, if
        ///     <paramref name="maxValue" /> equals zero, <paramref name="maxValue" /> is returned.
        /// </returns>
        /// <param name="maxValue">
        ///     The exclusive upper bound of the random number to be generated. <paramref name="maxValue" />
        ///     must be greater than or equal to zero.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="maxValue" /> is less than zero.</exception>
        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        /// <summary>
        ///     Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        ///     A double-precision floating point number greater than or equal to 0.0, and less than 1.0.
        /// </returns>
        public double NextDouble()
        {
            return _random.NextDouble();
        }

        /// <summary>Fills the elements of a specified array of bytes with random numbers.</summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null.</exception>
        public void NextBytes(byte[] buffer)
        {
            _random.NextBytes(buffer);
        }

        /// <summary>
        ///     Returns a random Boolean value.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool NextBool()
        {
            return _random.Next(100)%2 == 0;
        }
    }
}