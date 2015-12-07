using System;
using System.Text;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Progress bar that is drawn in characters and is a ping-pong marquee action bouncing back and fourth.
    /// </summary>
    internal sealed class MarqueeBar
    {
        private string bar;
        private string blankPointer;
        private int counter;
        private Direction currdir;
        private string pointer;

        public MarqueeBar()
        {
            bar = "|                         |";
            pointer = "***";
            blankPointer = BlankPointer();
            currdir = Direction.Right;
            counter = 1;
        }

        /// <summary>
        ///     sets the attribute blankPointer with a empty string the same length that the pointer
        /// </summary>
        /// <returns>A string filled with space characters</returns>
        private string BlankPointer()
        {
            var blank = new StringBuilder();
            for (var cont = 0; cont < pointer.Length; cont++)
                blank.Append(" ");
            return blank.ToString();
        }

        /// <summary>
        ///     reset the bar to its original state
        /// </summary>
        private void ClearBar()
        {
            bar = bar.Replace(pointer, blankPointer);
        }

        /// <summary>
        ///     remove the previous pointer and place it in a new position
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        private void PlacePointer(int start, int end)
        {
            ClearBar();
            bar = bar.Remove(start, end);
            bar = bar.Insert(start, pointer);
        }

        /// <summary>
        ///     prints the progress bar according to pointers and current Direction
        /// </summary>
        public string Step()
        {
            if (currdir == Direction.Right)
            {
                PlacePointer(counter, pointer.Length);
                counter++;
                if (counter + pointer.Length == bar.Length)
                    currdir = Direction.Left;
            }
            else
            {
                PlacePointer(counter - pointer.Length, pointer.Length);
                counter--;
                if (counter == pointer.Length)
                    currdir = Direction.Right;
            }

            return (bar + Environment.NewLine);
        }

        private enum Direction
        {
            Right,
            Left
        };
    }
}