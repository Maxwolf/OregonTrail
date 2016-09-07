// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

using System;
using System.Text;

namespace OregonTrailDotNet.WolfCurses.Window.Control
{
    /// <summary>
    ///     Progress bar that is drawn in characters and is a ping-pong marquee action bouncing back and fourth.
    /// </summary>
    public sealed class MarqueeBar
    {
        /// <summary>
        ///     The bar.
        /// </summary>
        private string bar;

        /// <summary>
        ///     The blank pointer.
        /// </summary>
        private string blankPointer;

        /// <summary>
        ///     The counter.
        /// </summary>
        private int counter;

        /// <summary>
        ///     The current directory.
        /// </summary>
        private Direction currdir;

        /// <summary>
        ///     The pointer.
        /// </summary>
        private string pointer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarqueeBar" /> class.
        /// </summary>
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

        /// <summary>remove the previous pointer and place it in a new position</summary>
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
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
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

            return bar + Environment.NewLine;
        }

        /// <summary>
        ///     The direction.
        /// </summary>
        private enum Direction
        {
            /// <summary>
            ///     The right.
            /// </summary>
            Right,

            /// <summary>
            ///     The left.
            /// </summary>
            Left
        };
    }
}