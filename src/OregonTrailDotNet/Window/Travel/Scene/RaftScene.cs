using System;
using System.Collections.Generic;
using System.Linq;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The Columbia run, exactly as FLOAT plays it: eighteen lanes down a diagonal river, momentum steering
    ///     (the arrows nudge a drift, they do not move the raft), rocks that live and die in their own lanes, three
    ///     direction signs going by on the bank, and a twenty-tick landing window after the third. The simulation is
    ///     the workbench's validated <see cref="RaftGame" />; what this scene adds is the stakes — every collision
    ///     runs the original's loss routines against the real party and wagon through <see cref="RaftDamage" />,
    ///     and the far shore is the last leg to Oregon City.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class RaftScene : SceneForm<TravelInfo>
    {
        private RaftGame _game;
        private SpriteScene _scene;
        private Sprite _raft;
        private Sprite _sign;
        private Sprite _landing;
        private Sprite[] _rocks;
        private RaftSteerEnum _pending = RaftSteerEnum.None;
        private Placement _wasRaft;
        private Placement[] _wasRocks;
        private Placement _wasSign, _wasLanding;
        private int _shoreHitsSeen;
        private int _rockHitsSeen;
        private readonly List<string> _losses = new();
        private readonly Random _damageRandom = new();
        private bool _destroyed;
        private int _endBeat;

        /// <summary>Initializes a new instance of the <see cref="RaftScene" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public RaftScene(IWindow window) : base(window)
        {
        }

        /// <summary>Only the game's two status lines and the frame's newline sit outside the picture.</summary>
        protected override int ReservedRows => 4;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <summary>A tick is a whole lane; nine a second puts the 225-tick run at the length the signs were cut for.</summary>
        protected override int DefaultTicksPerSecond => 9;

        /// <summary>Four frames a step, easing everything from its previous position — motion instead of a grid.</summary>
        protected override int FramesPerStep => 4;

        /// <inheritdoc />
        protected override void Build()
        {
            _game = new RaftGame();
            _scene = new SpriteScene(RaftArt.BuildRiver());

            // Draw order is list order: bank scenery first, then the rocks, then the raft over both.
            var markers = Art.Dos("float", RaftArt.MarkerBlock);
            _sign = new Sprite(RaftArt.Marker(markers, false)) { Visible = false };
            _landing = new Sprite(RaftArt.Marker(markers, true)) { Visible = false };
            _rocks =
            [
                new Sprite(Art.Dos("float", RaftArt.Rock)) { Visible = false },
                new Sprite(Art.Dos("float", RaftArt.Rock)) { Visible = false }
            ];
            _raft = new Sprite(Art.Dos("float", RaftArt.SmallRaft));

            _scene.Sprites.Add(_sign);
            _scene.Sprites.Add(_landing);
            foreach (var rock in _rocks)
                _scene.Sprites.Add(rock);
            _scene.Sprites.Add(_raft);

            Rewind();
            SyncSprites();
        }

        /// <inheritdoc />
        protected override void Advance()
        {
            // The run is over; hold the last picture for a second so the ending reads, then reckon up. The
            // interpolation is collapsed first — without it the frames keep easing from the previous step's
            // positions and snapping back, which shook the whole picture against the bank.
            if (_destroyed || _game.Outcome != RaftOutcomeEnum.Running)
            {
                Rewind();
                if (++_endBeat >= 9)
                    GoToResult();
                return;
            }

            var steer = _pending;
            _pending = RaftSteerEnum.None;

            Rewind();
            _game.Step(steer);
            SyncSprites();

            // The stakes: each fresh collision runs the original's loss routines against the real party. A single
            // catastrophic hit can destroy the raft outright, which ends the run where it happened.
            var collided = false;
            while (_shoreHitsSeen < _game.ShoreHits && !_destroyed)
            {
                _shoreHitsSeen++;
                collided = true;
                Suffer(RaftDamage.ShoreHit(_damageRandom));
            }

            while (_rockHitsSeen < _game.RockHits && !_destroyed)
            {
                _rockHitsSeen++;
                collided = true;
                Suffer(RaftDamage.RockHit(_damageRandom));
            }

            // Every collision sounded the DOS port's crash — rock and shore alike, raft-destroyed included —
            // while a clean landing and even a missed one stayed silent in both originals
            // (docs/legacy-sounds.md §§1.2, 2).
            if (collided)
                Sfx.Crash();

            // Missing the landing is survivable: supplies only, and the raft comes ashore regardless (:950/:960).
            if (!_destroyed && _game.Outcome == RaftOutcomeEnum.Missed)
                Suffer(RaftDamage.MissedLanding(_damageRandom));
        }

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            switch (key)
            {
                // Up and left are one direction, down and right the other, because the river runs diagonally. The
                // 1985 BASIC and the 1990 Pascal fold the keys the same way, five years and two languages apart.
                case ConsoleKey.LeftArrow:
                case ConsoleKey.UpArrow:
                    _pending = RaftSteerEnum.Far;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.DownArrow:
                    _pending = RaftSteerEnum.Near;
                    break;
            }
        }

        /// <summary>There is no stepping off a raft midstream — ESC does nothing until the river is done.</summary>
        protected override void OnEscape()
        {
        }

        /// <summary>The whole output is the river itself; the reckoning waits for the result screen.</summary>
        protected override string Compose()
        {
            // Every frame, not just every step: this is where the in-between positions are laid down.
            SyncSprites();
            return _scene.ToAnsi(PictureOptions());
        }

        /// <summary>Books one collision's losses and notices a destroyed raft.</summary>
        private void Suffer(RaftDamage.Report report)
        {
            _losses.AddRange(report.Lines);
            if (report.Destroyed)
                _destroyed = true;
        }

        /// <summary>Hands the run to the result screen: the outcome, and everything the river took.</summary>
        private void GoToResult()
        {
            UserData.RaftReport = new RaftReport(
                _losses.ToList(), _destroyed, _game.Outcome == RaftOutcomeEnum.Missed);
            SetForm(typeof(RaftResult));
        }

        private void Rewind()
        {
            _wasRaft = RaftNow();
            _wasRocks = _game.Rocks.Select(Where).ToArray();
            _wasSign = Where(_game.Sign);
            _wasLanding = Where(_game.Landing);
        }

        private void SyncSprites()
        {
            Place(_raft, _wasRaft, RaftNow(), 0, 0);

            for (var i = 0; i < _rocks.Length; i++)
                Place(_rocks[i], _wasRocks[i], Where(_game.Rocks[i]),
                    RaftGame.RockDriftX, RaftGame.RockDriftY);

            Place(_sign, _wasSign, Where(_game.Sign), RaftGame.SceneryDriftX, RaftGame.SceneryDriftY);
            Place(_landing, _wasLanding, Where(_game.Landing), RaftGame.SceneryDriftX, RaftGame.SceneryDriftY);
        }

        /// <summary>
        ///     Puts one sprite somewhere between where it was and where it is. A freshly filled slot has its missing
        ///     previous position reconstructed by stepping the spawn point back along the drift, so a new rock slides
        ///     in from the edge instead of sweeping across the water in front of the player.
        /// </summary>
        private void Place(Sprite sprite, Placement was, Placement now, double driftX, double driftY)
        {
            sprite.Visible = now.Visible;
            if (!now.Visible)
                return;

            var from = was.Visible ? was : new Placement(now.X - driftX, now.Y - driftY, true);
            sprite.X = SceneX(from.X + (now.X - from.X) * FrameProgress);
            sprite.Y = SceneY(from.Y + (now.Y - from.Y) * FrameProgress);
        }

        private Placement RaftNow() => new(RaftGame.LaneX(_game.Lane), RaftGame.LaneY(_game.Lane), true);

        private static Placement Where(RaftGame.Drifter drifter) => new(drifter.X, drifter.Y, drifter.Active);

        /// <summary>Where something stood at one instant, in FLOAT's own screen space.</summary>
        /// <param name="X">Left edge.</param>
        /// <param name="Y">Top edge.</param>
        /// <param name="Visible">Whether it was on screen at all.</param>
        private readonly record struct Placement(double X, double Y, bool Visible);

        private static int SceneX(double x) => (int) Math.Round(x * Art.DosWidth / RaftGame.ScreenWidth);

        private static int SceneY(double y) => (int) Math.Round(y * Art.DosHeight / RaftGame.ScreenHeight);
    }

    /// <summary>How the Columbia run ended, handed from the scene to its result screen.</summary>
    /// <param name="Losses">Everything the river took, in the order it took it.</param>
    /// <param name="Destroyed">Whether one catastrophic hit destroyed the raft and the party with it.</param>
    /// <param name="MissedLanding">Whether the landing window closed before the raft reached the shore.</param>
    public sealed record RaftReport(List<string> Losses, bool Destroyed, bool MissedLanding);
}
