using System.Reflection;
using System.Text.RegularExpressions;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Hunt;
using Xunit;
using PersonEntity = OregonTrailDotNet.Entity.Person.Person;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Guards the redesigned hunting screen's contract with the headless training bot: no matter how the human-facing
    ///     layout changes, while an animal is being aimed at the body must still carry the exact "Type the word '&lt;word&gt;'"
    ///     token that <c>ScreenRecognizer.TypeWordRx</c> scrapes to learn which word to type. If this breaks, the bot
    ///     silently stops shooting. Also locks in the per-animal meat yields and the ammunition meters — the
    ///     bullets-remaining bar shown while hunting and the running bullets-fired tally reported afterward.
    /// </summary>
    public class HuntScreenTests : SimulationTestBase
    {
        // Mirrors bot/OregonTrailDotNet.Bot/Game/ScreenRecognizer.cs:38 exactly.
        private static readonly Regex TypeWordRx = new(@"Type the word '([A-Za-z]+)'");

        /// <summary>
        ///     Prey spawns and target selection are random, so keep starting fresh hunts and ticking each toward the clock
        ///     running out until one puts an animal in the player's sights. Returns NULL only if that never happened, which
        ///     across this many attempts is astronomically unlikely.
        /// </summary>
        private static HuntManager HuntWithTarget()
        {
            for (var attempt = 0; attempt < 500; attempt++)
            {
                var hunt = new HuntManager();
                for (var t = 0; t < HuntManager.HUNTINGTIME && !hunt.PreyAvailable; t++)
                    hunt.OnTick(false, false);

                if (hunt.PreyAvailable)
                    return hunt;
            }

            return null;
        }

        [Fact]
        public void HuntBody_WhileAiming_KeepsTheWordTokenTheBotScrapes()
        {
            var hunt = HuntWithTarget();
            Assert.NotNull(hunt);

            var screen = hunt!.HuntInfo;

            var match = TypeWordRx.Match(screen);
            Assert.True(match.Success, $"bot word token missing from hunt body:\n{screen}");
            Assert.Equal(hunt.ShootingWord.ToString(), match.Groups[1].Value, ignoreCase: true);
        }

        [Fact]
        public void AnimalYields_AreIntentional_NotTheDefaultOnePound()
        {
            // Weight is per-unit meat yield; each prey is created with quantity 1, so TotalWeight of a single copy is the
            // yield. Bear/Duck/Squirrel once omitted the yield argument and silently gave 1 lb each — lock in real values.
            Assert.InRange(YieldOf(Animals.Bear), 100, 199);
            Assert.Equal(2, YieldOf(Animals.Duck));
            Assert.Equal(1, YieldOf(Animals.Squirrel));

            // Sanity on the ones that were always set explicitly.
            Assert.Equal(50, YieldOf(Animals.Deer));
            Assert.Equal(2, YieldOf(Animals.Goose));
            Assert.Equal(2, YieldOf(Animals.Rabbit));
            Assert.InRange(YieldOf(Animals.Buffalo), 350, 499);
            Assert.InRange(YieldOf(Animals.Caribou), 300, 349);
        }

        private static int YieldOf(SimItem animal) => new SimItem(animal, 1).TotalWeight;

        /// <summary>
        ///     Zeroes the wagon's ammunition and refills it to an exact known amount, so tests that assert against the
        ///     bullets-remaining meter and the bullets-fired tally have a deterministic starting figure to compare with.
        /// </summary>
        private static void SetAmmo(int bullets)
        {
            var ammo = GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Ammo];
            ammo.ReduceQuantity(ammo.Quantity);
            ammo.AddQuantity(bullets);
        }

        [Fact]
        public void HuntHud_ShowsBulletsRemainingMeter_AgainstWhatThePartySetOutWith()
        {
            // Hunting is only ever reached with ammunition on hand; pin it to a known amount so the meter is deterministic.
            SetAmmo(40);

            var hunt = new HuntManager();

            Assert.Equal(40, hunt.StartingBullets);
            Assert.Equal(40, hunt.BulletsRemaining);

            var screen = hunt.HuntInfo;
            Assert.Contains("Bullets", screen);         // the new resource meter's label
            Assert.Contains("40 / 40 bullets", screen); // remaining / started-with, drains as the player fires
        }

        [Fact]
        public void LandingAShot_TalliesBulletsFired_EqualToTheAmmunitionActuallySpent()
        {
            var vehicle = GameSimulationApp.Instance.Vehicle;
            vehicle.ResetVehicle();

            // TryShoot's miss roll reads the party leader's profession, so the wagon needs a leader aboard.
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Hunter", true));

            // Fix the ammunition before the hunt begins so StartingBullets (captured in the constructor) is a known value.
            SetAmmo(60);
            var hunt = new HuntManager();

            // A freshly generated prey has TargetTime 0, so it passes both the miss roll and the on-time-shot gate,
            // deterministically reaching the ammunition-deduction path. The target field is private; set it directly.
            typeof(HuntManager)
                .GetField("_target", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(hunt, new PreyItem());

            Assert.True(hunt.TryShoot());
            Assert.Equal(1, hunt.KillCount);
            Assert.Equal(60, hunt.StartingBullets);
            Assert.InRange(hunt.BulletsFired, 10, 13);                   // each kill costs 10-13 bullets
            Assert.Equal(60 - hunt.BulletsFired, hunt.BulletsRemaining); // tally equals the ammunition genuinely removed
        }

        [Fact]
        public void LandingAShot_WithFewerBulletsThanTheShotCosts_TalliesOnlyWhatWasActuallySpent()
        {
            var vehicle = GameSimulationApp.Instance.Vehicle;
            vehicle.ResetVehicle();
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Hunter", true));

            // Only 5 bullets on hand, but a kill costs 10-13. ReduceQuantity floors at zero, so the tally must record
            // the 5 genuinely spent, never the nominal cost — otherwise "bullets fired" could exceed what the party
            // ever carried. This pins the delta accumulation (ammoBefore - ammo.Quantity) against that floor.
            SetAmmo(5);
            var hunt = new HuntManager();
            typeof(HuntManager)
                .GetField("_target", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(hunt, new PreyItem());

            Assert.True(hunt.TryShoot());
            Assert.Equal(5, hunt.StartingBullets);
            Assert.Equal(5, hunt.BulletsFired);     // floored: only the 5 actually on hand left the wagon
            Assert.Equal(0, hunt.BulletsRemaining);
        }

        [Fact]
        public void ResultsScreen_ReportsBulletsFired_FromTheRunningTally_NotLiveAmmo()
        {
            var vehicle = GameSimulationApp.Instance.Vehicle;
            vehicle.ResetVehicle();
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Hunter", true));
            SetAmmo(60);

            var (window, data, form) = StartHuntSession();

            // One guaranteed kill (fresh target's TargetTime 0 never misses) so the tally is non-zero.
            typeof(HuntManager)
                .GetField("_target", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(data.Hunt, new PreyItem());
            Assert.True(data.Hunt.TryShoot());
            var fired = data.Hunt.BulletsFired;
            Assert.InRange(fired, 10, 13);

            // Leaving the hunt attaches the results form; its OnFormPostCreate runs the post-hunt turn.
            form.StopHunting();
            Assert.IsType<HuntingResult>(window.CurrentForm);

            // Simulate a post-hunt event (Thief/BanditsAttack/Snakebite) stealing ammunition BEFORE the results screen
            // renders. A live-ammo calculation (StartingBullets - BulletsRemaining) would now over-report the outing's
            // cost; the results screen must instead echo the accumulated tally, which this theft cannot move.
            vehicle.Inventory[EntitiesEnum.Ammo].ReduceQuantity(20);

            var screen = window.OnRenderWindow();
            Assert.Contains($"Bullets fired: {fired}", screen);
        }

        [Fact]
        public void ResultsScreen_WhenEmptyHanded_ReportsZeroBulletsFired()
        {
            var vehicle = GameSimulationApp.Instance.Vehicle;
            vehicle.ResetVehicle();
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Hunter", true));
            SetAmmo(60);

            // Quit without firing a shot: no bullets ever leave the wagon.
            var (window, _, form) = StartHuntSession();
            form.StopHunting();
            Assert.IsType<HuntingResult>(window.CurrentForm);

            var screen = window.OnRenderWindow();
            Assert.Contains("Bullets fired: 0", screen);
        }

        /// <summary>
        ///     Spins up a live Travel window with a hunt session in progress and its Hunting form, mirroring how the game
        ///     enters a hunt. The window's shared <see cref="TravelInfo" /> is protected, so it is reached by reflection —
        ///     the same handle the game's forms use to read <see cref="TravelInfo.Hunt" />.
        /// </summary>
        private static (Travel Window, TravelInfo Data, Hunting Form) StartHuntSession()
        {
            var window = new Travel(GameSimulationApp.Instance);
            var data = (TravelInfo) window.GetType().BaseType!
                .GetProperty("UserData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                .GetValue(window)!;
            data.GenerateHunt();
            var form = new Hunting(window);
            return (window, data, form);
        }
    }
}
