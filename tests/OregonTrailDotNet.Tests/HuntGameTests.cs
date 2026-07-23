using System.Collections.Generic;
using System.Linq;
using OregonTrailDotNet.Presentation;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the hunt simulation the game's field hunt runs on — pure, seedable logic that until now was only
    ///     ever validated by playing it. The rules that matter to the wagon: exactly one bullet per trigger pull,
    ///     raw pounds accumulating per kill with the dressing applied once at hunt's end, the roster gated by the
    ///     country, rotation paced at one step per three ticks the shorter way round, and the spawn block once four
    ///     carcasses litter the field.
    /// </summary>
    public class HuntGameTests
    {
        /// <summary>Blocks all spawning by littering the field, so a scripted shot meets no surprise animals.</summary>
        private static void BlockSpawns(HuntGame game)
        {
            for (var i = 0; i < HuntGame.CarcassSpawnBlock; i++)
                game.Carcasses.Add(new HuntGame.Carcass());
        }

        [Fact]
        public void Fire_CostsExactlyOneBullet_AndOnlyOneCanFly()
        {
            var game = new HuntGame(seed: 1);

            game.Fire();
            Assert.Equal(19, game.Bullets);
            Assert.Equal(1, game.ShotsFired);
            Assert.True(game.Shot.Active);

            // A second pull while the bullet is out costs nothing — the original allows one in the air.
            game.Fire();
            Assert.Equal(19, game.Bullets);
            Assert.Equal("A bullet is already in flight.", game.LastEvent);
        }

        [Fact]
        public void Fire_WithNothingLeft_DoesNothing()
        {
            var game = new HuntGame(seed: 1, bullets: 1);
            BlockSpawns(game);

            game.Fire();
            while (game.Shot.Active)
                game.Step();

            game.Fire();
            Assert.Equal(0, game.Bullets);
            Assert.Equal(1, game.ShotsFired);
            Assert.Equal("Out of bullets.", game.LastEvent);
        }

        [Fact]
        public void Rotation_TakesThreeTicksAStep_TheShorterWayRound()
        {
            var game = new HuntGame(seed: 1);
            BlockSpawns(game);

            // Two steps clockwise: nothing after two ticks, one step after three, arrived after six.
            game.AimAt(2);
            game.Step();
            game.Step();
            Assert.Equal(0, game.Aim);
            game.Step();
            Assert.Equal(1, game.Aim);
            game.Step();
            game.Step();
            game.Step();
            Assert.Equal(2, game.Aim);

            // Seven steps clockwise is one counter-clockwise; the rifle takes the short way.
            game.AimAt(1);
            game.Step();
            game.Step();
            game.Step();
            Assert.Equal(1, game.Aim);
        }

        [Fact]
        public void AScriptedShot_BagsRawPounds_AndCountsTheKill()
        {
            var game = new HuntGame(seed: 1);
            BlockSpawns(game);

            // Swing east first, with the field empty, so the shot geometry is exact.
            game.AimAt(2);
            while (game.Aim != 2)
                game.Step();

            // A bison walking in from the right, straddling the bullet's row: the bullet leaves at
            // (HunterX+10, HunterY+12) moving +4/tick, the bison closes at -1/tick, and its 28-pixel width
            // guarantees the overlap lands on an integer tick.
            var bison = game.Animals[0];
            bison.Species = 2;
            bison.X = game.HunterX + 90;
            bison.Y = game.HunterY + 12 - 8;
            bison.Facing = -1;
            bison.Active = true;

            game.Fire();
            var guard = 0;
            while (game.Shot.Active && guard++ < 60)
                game.Step();

            Assert.Equal(1, game.Kills);
            Assert.InRange(game.Pounds, 1700, 2000);
            Assert.False(bison.Active);
            Assert.Equal(HuntGame.CarcassSpawnBlock + 1, game.Carcasses.Count);
            Assert.Contains("bison", game.LastEvent);
        }

        [Fact]
        public void Bag_HalvesFromThreePoundsUp_ExactlyAsTheBasicDid()
        {
            // HUNT.LIB:50011 — L = INT(L / (2 - (L < 3))): one- and two-pound kills pass whole.
            Assert.Equal(0, HuntGame.Bag(0));
            Assert.Equal(1, HuntGame.Bag(1));
            Assert.Equal(2, HuntGame.Bag(2));
            Assert.Equal(1, HuntGame.Bag(3));
            Assert.Equal(2, HuntGame.Bag(4));
            Assert.Equal(50, HuntGame.Bag(100));
            Assert.Equal(1000, HuntGame.Bag(2000));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Roster_GatesTheBigSpecies_ByTheCountry(int seed)
        {
            // No deer, bear or bison on the roster: a whole hunt spawns only doe, rabbit and squirrel.
            var game = new HuntGame(seed, antleredDeer: false, bear: false, bison: false);
            var seen = new HashSet<int>();

            for (var tick = 0; tick < HuntGame.TimeLimit; tick++)
            {
                game.Step();
                foreach (var animal in game.Animals.Where(a => a.Active))
                    seen.Add(animal.Species);
            }

            Assert.NotEmpty(seen);
            Assert.All(seen, species => Assert.InRange(species, 3, 5));
        }

        [Fact]
        public void Roster_OnThePlains_HasBisonButNoBear()
        {
            // A single hunt only sees a dozen-odd spawns, so one seed can legitimately roll no bison; a fixed
            // handful of seeds keeps the run deterministic while making a bison-free result impossible short of
            // the roster actually losing species 2.
            var seen = new HashSet<int>();
            foreach (var seed in new[] { 4, 5, 6, 7, 8 })
            {
                var game = new HuntGame(seed, antleredDeer: true, bear: false, bison: true);
                for (var tick = 0; tick < HuntGame.TimeLimit; tick++)
                {
                    game.Step();
                    foreach (var animal in game.Animals.Where(a => a.Active))
                        seen.Add(animal.Species);
                }
            }

            // Both halves of the name: bison actually roam (dropping species 2 from the roster must fail here,
            // not pass silently), and bear never do.
            Assert.NotEmpty(seen);
            Assert.Contains(2, seen);
            Assert.DoesNotContain(1, seen);
        }

        [Fact]
        public void FourCarcasses_BlockAllFurtherSpawns()
        {
            var game = new HuntGame(seed: 1);
            BlockSpawns(game);

            for (var tick = 0; tick < 500; tick++)
            {
                game.Step();
                Assert.All(game.Animals, animal => Assert.False(animal.Active));
            }
        }

        [Fact]
        public void TheCountdown_EndsTheHunt_AndFreezesIt()
        {
            var game = new HuntGame(seed: 1);
            BlockSpawns(game);

            for (var tick = 0; tick < HuntGame.TimeLimit; tick++)
                game.Step();

            Assert.True(game.Finished);
            Assert.Equal(HuntGame.TimeLimit, game.Tick);

            // A finished hunt no longer ticks.
            game.Step();
            Assert.Equal(HuntGame.TimeLimit, game.Tick);
        }
    }
}
