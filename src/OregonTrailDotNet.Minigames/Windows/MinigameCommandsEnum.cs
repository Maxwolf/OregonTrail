using WolfCurses.Utility;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     The workbench menu. Every value becomes a numbered choice automatically, described by its attribute.
    /// </summary>
    public enum MinigameCommandsEnum
    {
        /// <summary>The Columbia River raft, ported from the original's <c>FLOAT</c> BASIC.</summary>
        [Description("Columbia River raft  (FLOAT)")] ColumbiaRaft = 1,

        /// <summary>The hunt, rebuilt from the disassembled <c>&amp; HUNT</c> machine code.</summary>
        [Description("Hunting              (& HUNT)")] Hunt = 2,

        /// <summary>The tombstone card, for dialling in where the epitaph letters sit on the stone.</summary>
        [Description("Tombstone            (letter positioning)")] Tombstone = 3,

        /// <summary>Contact sheets of the extracted sprites, for checking frame order and mirroring.</summary>
        [Description("Sprite sheets        (verify frames)")] Sprites = 4,

        /// <summary>Every trail stop's card, in order, named from the original's own tables.</summary>
        [Description("Trail stops          (all 18 location cards)")] Landmarks = 5,

        /// <summary>The walking team, the sliding world, and the ground colour weather paints it.</summary>
        [Description("Travel screen        (ox walk, scrolling world)")] Travel = 6,

        /// <summary>Close the workbench.</summary>
        [Description("Quit")] Quit = 7
    }
}
