using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Old school spinning pixel progress, normally used to show the thread is not locked by some running process.
    /// </summary>
    internal sealed class SpinningPixel
    {
        private List<string> animation;
        private int counter;

        public SpinningPixel()
        {
            animation = new List<string> {"/", "-", @"\", "|"};
            counter = 0;
        }

        /// <summary>
        ///     prints the character found in the animation according to the current index
        /// </summary>
        public string Step()
        {
            var barText = animation[counter];
            counter++;
            if (counter == animation.Count)
                counter = 0;

            return barText;
        }
    }
}