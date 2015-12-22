// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Landmark.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Scenic area along the trail where the player can stop and rest, mostly used to signify total progress along the
    ///     entire trail with clear visual markers for the player to recognize.
    /// </summary>
    public sealed class Landmark : Location
    {
        /// <summary>Initializes a new instance of the <see cref="Landmark"/> class. Initializes a new instance of the <see cref="T:TrailSimulation.Entity.Location"/> class.</summary>
        /// <param name="name">The name.</param>
        /// <param name="climateType">The climate Type.</param>
        public Landmark(string name, Climate climateType) : base(name, climateType)
        {
        }

        /// <summary>
        ///     Determines if the location allows the player to chat to other NPC's in the area which can offer up advice about the
        ///     trail ahead.
        /// </summary>
        public override bool ChattingAllowed
        {
            get { return false; }
        }

        /// <summary>
        ///     Defines the type of location this is, the game simulation will trigger and load different states depending on this
        ///     value. Defaults to default value which is a normal location with nothing special happening.
        /// </summary>
        public override LocationCategory Category
        {
            get { return LocationCategory.Landmark; }
        }

        /// <summary>
        ///     Determines if this location has a store which the player can buy items from using their monies.
        /// </summary>
        public override bool ShoppingAllowed
        {
            get { return false; }
        }
    }
}