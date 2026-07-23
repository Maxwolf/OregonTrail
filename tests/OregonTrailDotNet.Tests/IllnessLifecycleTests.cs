using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Event.Person;
using OregonTrailDotNet.Module.Director;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Window.MainMenu;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the illness lifecycle the weather revival gave teeth: the ten-day countdown ends in an announced
    ///     WellAgain recovery whose heal actually zeroes the wear, and the turn for the worse both fires on the
    ///     half-worn (the positive control) and never strikes below that line — from both sides, because a test
    ///     that only watches a region where no strike can land proves nothing about the guard.
    /// </summary>
    public class IllnessLifecycleTests : SimulationTestBase
    {
        private void BootParty()
        {
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> { "Alice", "Bob", "Carol", "Dave" },
                PlayerProfession = ProfessionEnum.Banker,
                StartingMonies = 1600,
                StartingMonth = MonthEnum.April
            });

            // Enough food that a lone ticked person never starves through either phase, and a set of clothes a
            // head so the cold never bites — which is what keeps the daily wear low in the below-half phase.
            Game.Vehicle.Inventory[EntitiesEnum.Food].AddQuantity(5000);
            Game.Vehicle.Inventory[EntitiesEnum.Clothes].AddQuantity(20);
        }

        private static double AilmentMax => (double) typeof(Person)
            .GetField("AilmentMax", BindingFlags.NonPublic | BindingFlags.Static)!
            .GetValue(null)!;

        [Fact]
        public void TheCountdown_EndsInAnAnnouncedRecovery_ThatActuallyHeals()
        {
            BootParty();
            var person = Game.Vehicle.Passengers.First(p => !p.Leader);

            person.Infect();
            person.Damage(40);
            Assert.True(person.IsSick);
            Assert.True(person.Ailment >= 40);

            // Ten clean days shake it off unaided; the generous bound rides out the small chance of the daily
            // water-disease roll re-infecting mid-count (which only restarts the countdown).
            var guard = 0;
            while (person.IsSick && guard++ < 200)
                person.OnTick(false, false);

            Assert.False(person.IsSick);
            Assert.NotEqual(HealthStatusEnum.Dead, person.HealthStatus);

            // The recovery is announced by the event, and the event's heal is real: the 40-point wear is gone,
            // not just the sickness flag (at most one day's ordinary wear has landed since the heal).
            Assert.Equal("WellAgain", SceneEvents.LastEventName);
            Assert.True(person.Ailment < 15,
                $"recovered with ailment {person.Ailment:0.0} — WellAgain announced but did not heal");
        }

        [Fact]
        public void WellAgain_HealsEntirely()
        {
            BootParty();
            var person = Game.Vehicle.Passengers.First(p => !p.Leader);

            person.Infect();
            person.Damage(50);
            Assert.True(person.Ailment >= 50);

            Game.EventDirector.TriggerEvent(person, typeof(WellAgain));

            Assert.False(person.IsSick);
            Assert.Equal(0.0, person.Ailment);
        }

        [Fact]
        public void TheTurnForTheWorse_StrikesTheHalfWorn_AndOnlyThem()
        {
            BootParty();
            var person = Game.Vehicle.Passengers.First(p => !p.Leader);
            var half = AilmentMax / 2;

            // Phase one: sick but lightly worn. In this scenario the daily wear converges well below half (fed,
            // clothed, stopped wagon), so with the guard intact no crash can ever land; with the guard gone the
            // 5%/day roll strikes almost surely inside 150 days. The per-day sanity assert keeps the phase
            // honest — if a future balance change pushes wear over the line, the test says so instead of
            // silently testing nothing.
            person.Infect();
            var previous = SceneEvents.LastEventName;
            for (var day = 0; day < 150; day++)
            {
                if (!person.IsSick)
                    person.Infect();

                Assert.True(person.Ailment < half,
                    $"the below-half phase drifted above the line (ailment {person.Ailment:0.0}) — re-tune the scenario");
                person.OnTick(false, false);

                Assert.False(SceneEvents.LastEventName == "TurnForWorse" && previous != "TurnForWorse",
                    $"a turn for the worse struck at low ailment on day {day}");
                previous = SceneEvents.LastEventName;
            }

            // Phase two, the positive control: hold the same person above the half-worn line and the 5%/day
            // crash must actually land — proving the mechanism exists — and every strike must have found them
            // at or past half. Top-ups stay modest so a strike's own 28-point damage can never stack to the
            // ailment cap (which would kill through StrikeDownWithIllness).
            var strikes = 0;
            for (var day = 0; day < 300 && person.HealthStatus != HealthStatusEnum.Dead; day++)
            {
                if (!person.IsSick)
                    person.Infect();
                while (person.Ailment < half + 10 && person.HealthStatus != HealthStatusEnum.Dead)
                    person.Damage(20);

                var wornBefore = person.Ailment;
                person.OnTick(false, false);

                if (SceneEvents.LastEventName == "TurnForWorse" && previous != "TurnForWorse")
                {
                    strikes++;
                    Assert.True(wornBefore >= half,
                        $"a turn for the worse struck at ailment {wornBefore:0.0}, below half of {AilmentMax}");
                }

                previous = SceneEvents.LastEventName;
            }

            Assert.True(strikes > 0, "300 half-worn sick days never crashed — the turn-for-the-worse roll is dead");
        }
    }
}
