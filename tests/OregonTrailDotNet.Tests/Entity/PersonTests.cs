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
            // Health starts at Good (500); each band is a 100 point window.
            var person = MakePerson();

            person.Damage(100);
            Assert.Equal(HealthStatusEnum.Fair, person.HealthStatus);

            person.Damage(100);
            Assert.Equal(HealthStatusEnum.Poor, person.HealthStatus);

            person.Damage(100);
            Assert.Equal(HealthStatusEnum.VeryPoor, person.HealthStatus);

            person.Damage(200);
            Assert.Equal(HealthStatusEnum.Dead, person.HealthStatus);
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
        public void Damage_BeyondMinimumHealth_KillsPerson()
        {
            var person = MakePerson();
            person.Damage(9999);

            Assert.Equal(HealthStatusEnum.Dead, person.HealthStatus);
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
            person.Damage(300);
            person.HealEntirely();

            Assert.Equal(HealthStatusEnum.Good, person.HealthStatus);
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
