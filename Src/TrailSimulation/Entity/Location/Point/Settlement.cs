// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settlement.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Civilized area where many other people from different vehicles congregate together and share resources.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Civilized area where many other people from different vehicles congregate together and share resources.
    /// </summary>
    public sealed class Settlement : Location
    {
        /// <summary>Initializes a new instance of the <see cref="Settlement"/> class. Initializes a new instance of the<see cref="T:TrailSimulation.Entity.Location"/> class.</summary>
        /// <param name="name">The name.</param>
        /// <param name="climateType">The climate Type.</param>
        public Settlement(string name, Climate climateType) : base(name, climateType)
        {
        }

        /// <summary>
        ///     Determines if the location allows the player to chat to other NPC's in the area which can offer up advice about the
        ///     trail ahead.
        /// </summary>
        public override bool ChattingAllowed
        {
            get { return true; }
        }

        /// <summary>
        ///     Defines the type of location this is, the game simulation will trigger and load different states depending on this
        ///     value. Defaults to default value which is a normal location with nothing special happening.
        /// </summary>
        public override LocationCategory Category
        {
            get { return LocationCategory.Settlement; }
        }

        /// <summary>
        ///     Determines if this location has a store which the player can buy items from using their monies.
        /// </summary>
        public override bool ShoppingAllowed
        {
            get { return true; }
        }
    }
}