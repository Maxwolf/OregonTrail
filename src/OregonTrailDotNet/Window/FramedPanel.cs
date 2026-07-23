// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using WolfCurses.Window.Control;

namespace OregonTrailDotNet.Window
{
    /// <summary>
    ///     Central factory for the game's titled status panels. Every framed "how am I doing" readout in the UI — the
    ///     on-the-trail and location status blocks, the store and river-crossing headers, and the hunting HUD and its
    ///     results — is drawn with the same WolfCurses <see cref="Box" /> styling (double border, centered title, one row of
    ///     padding). Routing them all through here keeps that styling in one place instead of re-specifying the box at every
    ///     call site, and replaces the hand-ruled "--------------------------------" dividers those panels used to draw by
    ///     hand.
    /// </summary>
    public static class FramedPanel
    {
        /// <summary>
        ///     Frames <paramref name="body" /> in a double-bordered box with <paramref name="title" /> centered in the top
        ///     border. The returned string has no trailing newline (matching <see cref="Box.Render" />), so a caller that
        ///     appends a menu or a call-to-action beneath the panel should append it with a line break in between — e.g. via
        ///     <c>StringBuilder.AppendLine(FramedPanel.Render(...))</c>.
        /// </summary>
        /// <param name="title">Heading centered in the panel's top border, e.g. "ON THE TRAIL".</param>
        /// <param name="body">The panel's contents; individual lines are laid out as-is inside the border.</param>
        public static string Render(string title, string body)
        {
            return new Box
            {
                Border = BoxBorderEnum.Double,
                Title = title,
                TitleAlignment = BoxAlignmentEnum.Center,
                Padding = 1
            }.Render(body);
        }
    }
}
