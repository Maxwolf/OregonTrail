using System.Collections.Generic;
using OregonTrailDotNet.Presentation;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the FLOAT fidelity contract the raft scene runs on — with the 1985 BASIC's numbers asserted as
    ///     literals, so a drifted constant fails rather than moving the expectation with it: the raft casts off in
    ///     lane 16 already sliding away, steering nudges a drift rather than moving the raft, the banks bounce
    ///     (:400 reverses the drift), lane-17 contact before tick 206 is a shore hit and after 225 a miss, rocks
    ///     strike (and only strike) off the safe lane, and everything the river carries stays in its lane.
    /// </summary>
    public class RaftGameTests
    {
        /// <summary>One press against the opening drift parks the raft in its rock-proof starting lane.</summary>
        private static RaftGame Parked(int seed)
        {
            var game = new RaftGame(seed);
            game.Step(RaftSteerEnum.Near);
            Assert.Equal(0, game.Drift);
            Assert.Equal(RaftGame.StartLane, game.Lane);
            return game;
        }

        [Fact]
        public void CastOff_StartsInLaneSixteen_AlreadySlidingAway()
        {
            var game = new RaftGame(seed: 1);

            // :1030 — the raft starts one lane off the landing bank with the current already carrying it away,
            // which is what makes the opening urgent.
            Assert.Equal(16, game.Lane);
            Assert.Equal(-1, game.Drift);
            Assert.Equal(RaftOutcomeEnum.Running, game.Outcome);
        }

        [Fact]
        public void TheFarBank_BouncesTheRaft_AndChargesAShoreHit()
        {
            var game = new RaftGame(seed: 1);

            // Unsteered, the opening drift carries the raft down a lane a tick until it grinds the far bank.
            for (var i = 0; i < 16; i++)
                game.Step(RaftSteerEnum.None);

            Assert.Equal(0, game.Lane);
            Assert.Equal(1, game.ShoreHits);
            Assert.Equal(1, game.Drift);
            Assert.Contains("shore", game.LastEvent);

            // :400 reversed the drift, so the next tick moves the raft back off the bank.
            game.Step(RaftSteerEnum.None);
            Assert.Equal(1, game.Lane);
        }

        [Fact]
        public void TheNearBank_BeforeTheWindowOpens_IsJustMoreShore()
        {
            var game = Parked(seed: 1);
            for (var i = 0; i < 3; i++)
                game.Step(RaftSteerEnum.None);

            // :1115 — before the window opens, the same lane-17 contact that will later mean landing means
            // hitting the bank: charged, bounced, and the run goes on.
            game.Step(RaftSteerEnum.Near);
            Assert.Equal(17, game.Lane);
            Assert.Equal(RaftOutcomeEnum.Running, game.Outcome);
            Assert.Equal(1, game.ShoreHits);
            Assert.Contains("shore", game.LastEvent);

            game.Step(RaftSteerEnum.None);
            Assert.Equal(16, game.Lane);
        }

        [Fact]
        public void TheLanding_CountsInsideTheWindow()
        {
            var game = Parked(seed: 1);

            // Ride out the river in the safe lane to the tick after the window opens (:1115's literal 205).
            while (game.Tick < 206)
            {
                game.Step(RaftSteerEnum.None);
                Assert.Equal(RaftOutcomeEnum.Running, game.Outcome);
            }

            // One press toward the near bank inside the window is the landing.
            Assert.True(game.LandingWindowOpen);
            game.Step(RaftSteerEnum.Near);
            Assert.Equal(17, game.Lane);
            Assert.Equal(RaftOutcomeEnum.Landed, game.Outcome);
            Assert.Equal(207, game.Tick);
        }

        [Fact]
        public void SleepingThroughTheWindow_MissesTheLanding()
        {
            var game = Parked(seed: 1);

            while (game.Outcome == RaftOutcomeEnum.Running)
                game.Step(RaftSteerEnum.None);

            // :1116's literal 225: the tick after the window closes, the river has carried the raft past.
            Assert.Equal(RaftOutcomeEnum.Missed, game.Outcome);
            Assert.Equal(226, game.Tick);
        }

        [Fact]
        public void ARock_InTheRaftsLane_Strikes()
        {
            // The positive control for the collision machinery: park in lane 10, then hand-place a rock one
            // drift-step (+8, -4) short of the raft's log-deck box so the next tick must strike it.
            var game = Parked(seed: 1);
            game.Step(RaftSteerEnum.Far);
            for (var i = 0; i < 5; i++)
                game.Step(RaftSteerEnum.None);
            game.Step(RaftSteerEnum.Near);
            Assert.Equal(10, game.Lane);
            Assert.Equal(0, game.Drift);

            foreach (var rock in game.Rocks)
                rock.Active = false;
            var hitsBefore = game.RockHits;

            // Raft box at lane 10: x 170..189, y 62..68. Post-drift rock box from (152, 66): x 171..190,
            // y 63..68 — overlapped on both axes.
            var placed = game.Rocks[0];
            placed.X = 152;
            placed.Y = 66;
            placed.Active = true;

            game.Step(RaftSteerEnum.None);

            Assert.Equal(hitsBefore + 1, game.RockHits);
            Assert.False(placed.Active);
            Assert.Contains("rock", game.LastEvent);
            Assert.Equal(RaftOutcomeEnum.Running, game.Outcome);
        }

        [Fact]
        public void TheSigns_RunOnTheOriginalSchedule()
        {
            var game = Parked(seed: 1);

            var appearances = new HashSet<int>();
            var clearances = new HashSet<int>();
            var landingSeenAt = -1;

            while (game.Tick < 225)
            {
                var wasActive = game.Sign.Active;
                game.Step(RaftSteerEnum.None);
                if (!wasActive && game.Sign.Active)
                    appearances.Add(game.Tick);
                if (wasActive && !game.Sign.Active)
                    clearances.Add(game.Tick);
                if (landingSeenAt < 0 && game.Landing.Active)
                    landingSeenAt = game.Tick;
            }

            // The literals from the listing: three signs at :60/:120/:170 (:1070), the first two taken away at
            // :97/:157, the third left to drift off on its own, and the landing marker rising into view at :175.
            Assert.Equal(new HashSet<int> { 60, 120, 170 }, appearances);
            Assert.Contains(97, clearances);
            Assert.Contains(157, clearances);
            Assert.Equal(175, landingSeenAt);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void LaneSixteen_IsRockProof(int seed)
        {
            // The property that makes the run fair (and non-fatal contact correct): :300 cannot spawn a rock past
            // lane 15.9 and a rock's drift preserves its lane, so the starting lane can never be struck — on any
            // seed, which is why the original is playable at all. (ARock_InTheRaftsLane is the positive control
            // proving rocks do strike off this lane.)
            var game = Parked(seed);

            while (game.Outcome == RaftOutcomeEnum.Running)
                game.Step(RaftSteerEnum.None);

            Assert.Equal(0, game.RockHits);
            Assert.Equal(0, game.ShoreHits);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(7)]
        [InlineData(42)]
        public void EverythingTheRiverCarries_StaysInItsLane(int seed)
        {
            // Both drift vectors — a rock's (+8, -4) and bank scenery's (+6, -3) — must preserve the cross-river
            // lane: that is what keeps rocks in their spawn lane and the signs riding the bank instead of sliding
            // into the water.
            var game = Parked(seed);
            var lanes = new Dictionary<RaftGame.Drifter, double>();
            var spawnsSeen = 0;

            while (game.Outcome == RaftOutcomeEnum.Running)
            {
                var carried = new Dictionary<RaftGame.Drifter, bool>();
                foreach (var rock in game.Rocks)
                    carried[rock] = rock.Active;
                carried[game.Sign] = game.Sign.Active;
                carried[game.Landing] = game.Landing.Active;

                game.Step(RaftSteerEnum.None);

                foreach (var rock in game.Rocks)
                {
                    if (!rock.Active)
                    {
                        lanes.Remove(rock);
                        continue;
                    }

                    var lane = RaftGame.LaneOf(rock.X, rock.Y);
                    if (carried[rock] && lanes.TryGetValue(rock, out var was))
                    {
                        Assert.Equal(was, lane, precision: 6);
                    }
                    else
                    {
                        spawnsSeen++;

                        // :300 spawns along the left edge and bottom-left corner, all short of lane 16.
                        Assert.True(lane < 16.0, $"rock spawned in lane {lane:0.00}");
                    }

                    lanes[rock] = lane;
                }

                foreach (var piece in new[] { game.Sign, game.Landing })
                {
                    if (!piece.Active)
                    {
                        lanes.Remove(piece);
                        continue;
                    }

                    var lane = RaftGame.LaneOf(piece.X, piece.Y);
                    if (carried[piece] && lanes.TryGetValue(piece, out var was))
                        Assert.Equal(was, lane, precision: 6);
                    lanes[piece] = lane;
                }
            }

            Assert.True(spawnsSeen > 0, "the run saw no rocks at all — the invariant went unexercised");
        }
    }
}
