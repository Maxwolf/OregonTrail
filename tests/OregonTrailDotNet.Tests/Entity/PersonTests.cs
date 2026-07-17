using OregonTrailDotNet.Entity.Person;
using Xunit;
using PersonEntity = OregonTrailDotNet.Entity.Person.Person;

namespace OregonTrailDotNet.Tests.Entity
{
    /// <summary>
    ///     Covers direct health manipulation on party members. The fixed-amount Damage overload, Kill,
    ///     and HealEntirely never roll dice or trigger events, so no game simulation is required.
    /// </summary>
    public class PersonTests
    {
        private static PersonEntity MakePerson(bool leader = true)
        {
            return new PersonEntity(ProfessionEnum.Banker, "Alice", leader);
        }

        [Fact]
        public void Ctor_SetsIdentityAndStartsInGoodHealth()
        {
            var person = MakePerson();

            Assert.Equal("Alice", person.Name);
            Assert.Equal(ProfessionEnum.Banker, person.Profession);
            Assert.True(person.Leader);
            Assert.Equal(HealthStatusEnum.Good, person.HealthStatus);
        }

        [Fact]
        public void Ctor_NonLeaderIsNotLeader()
        {
            Assert.False(MakePerson(false).Leader);
        }

        [Fact]
        public void Damage_WalksHealthDownThroughEveryBand()
        {
            // Nobody starts the trail worn at all, and every 35 points of wear costs a band.
            var person = MakePerson();
            Assert.Equal(HealthStatusEnum.Good, person.HealthStatus);

            person.Damage(35);
            Assert.Equal(HealthStatusEnum.Fair, person.HealthStatus);

            person.Damage(35);
            Assert.Equal(HealthStatusEnum.Poor, person.HealthStatus);

            person.Damage(35);
            Assert.Equal(HealthStatusEnum.VeryPoor, person.HealthStatus);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void Damage_IgnoresNonPositiveAmounts(int amount)
        {
            var person = MakePerson();
            person.Damage(amount);

            Assert.Equal(HealthStatusEnum.Good, person.HealthStatus);
        }

        [Fact]
        public void Damage_BeyondWhatAPersonCanTake_MakesThemIllRatherThanKillingThemOutright()
        {
            // Nobody drops dead of hardship in one go: worn past all bearing they sicken, and it is being struck down a
            // second time while already ill that finishes them. This is the whole of how the trail kills.
            var person = MakePerson();
            person.Damage(9999);

            Assert.NotEqual(HealthStatusEnum.Dead, person.HealthStatus);
            Assert.True(person.IsSick);

            person.Damage(9999);
            Assert.Equal(HealthStatusEnum.Dead, person.HealthStatus);
            Assert.Equal(CauseOfDeathEnum.Illness, person.Cause);
        }

        [Fact]
        public void Infect_StartsTheRecoveryCountdown_SoIllnessIsNeverPermanent()
        {
            // Illness has to run out on its own, because being struck down while already ill is what kills. An infection
            // that never cleared - which is what happens if the countdown is left at zero - both wears on the whole party
            // for the rest of the journey and turns the next misfortune into a certain death.
            var person = MakePerson();
            person.Infect();
            Assert.True(person.IsSick);
            Assert.True(person.IllnessDaysRemaining > 0);
        }

        [Fact]
        public void Injure_LaysAPersonUpForLongerThanAnIllness()
        {
            var ill = MakePerson();
            ill.Infect();

            var hurt = MakePerson();
            hurt.Injure();

            Assert.True(hurt.IllnessDaysRemaining > ill.IllnessDaysRemaining);
        }

        [Fact]
        public void Kill_MakesPersonDead()
        {
            var person = MakePerson();
            person.Kill();

            Assert.Equal(HealthStatusEnum.Dead, person.HealthStatus);
        }

        [Fact]
        public void HealEntirely_RestoresFullHealth()
        {
            var person = MakePerson();
            person.Damage(100);
            person.HealEntirely();

            Assert.Equal(HealthStatusEnum.Good, person.HealthStatus);
            Assert.False(person.IsSick);
        }

        [Fact]
        public void HealEntirely_CannotResurrectTheDead()
        {
            var person = MakePerson();
            person.Kill();
            person.HealEntirely();

            Assert.Equal(HealthStatusEnum.Dead, person.HealthStatus);
        }
    }
}
