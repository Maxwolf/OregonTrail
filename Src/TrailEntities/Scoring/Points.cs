using System;

namespace TrailEntities
{
    /// <summary>
    ///     Represents a given type of point that can take in a object of a given type in the concrete handler and we will
    ///     process and calculate total points for this item and display the information in a to string override so it is easy
    ///     to get to and visualize to user as a list of objects (such as a table).
    /// </summary>
    public sealed class Points
    {
        /// <summary>
        ///     Default value that will be used in delineating how many points will be awarded per a particular object type. Used
        ///     as default value and also checking in overload for ToString.
        /// </summary>
        private const int DEFAULT_PER_AMOUNT = 1;

        /// <summary>
        ///     Since the string.empty property is computed and not static we have to make a empty string for a constant for
        ///     default display name.
        /// </summary>
        private const string DEFAULT_DISPLAY_NAME = "";

        /// <summary>
        ///     Some things like cash don't have any equivalent type other than being a base POCO object, this allows us to have
        ///     nice display name for it regardless.
        /// </summary>
        private readonly string _optionalDisplayName;

        /// <summary>
        ///     Defines the quantity of the type of item that must be located in inventory for points awarded to be returned.
        /// </summary>
        private readonly int _perAmount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Points" /> class.
        /// </summary>
        public Points(Item resource, int pointsAwarded,
            int perAmount = DEFAULT_PER_AMOUNT, string optionalDisplayName = DEFAULT_DISPLAY_NAME)
        {
            // Complain if the per amount is zero, the developer is doing it wrong.
            if (perAmount <= 0)
                throw new ArgumentException("Per amount is less than zero, default value is one for a reason!");

            // Setup point tabulator basics.
            Resource = resource;
            PointsAwarded = pointsAwarded;
            _optionalDisplayName = optionalDisplayName;
            _perAmount = perAmount;
        }

        /// <summary>
        ///     Total points the player will get for this item being in their inventory multiplied by the quantity owned.
        /// </summary>
        public int PointsAwarded { get; }

        /// <summary>
        ///     Represents the item we will be comparing with other items for, cost is not evaluated in any of these calculations.
        /// </summary>
        private Item Resource { get; }

        /// <summary>
        ///     Representation of the point scoring as a string that can be displayed visually to user so they understand the
        ///     scoring mechanism.
        /// </summary>
        public override string ToString()
        {
            // Check if optional display name is being used.
            var displayName = Resource.ToString();
            if (!string.IsNullOrEmpty(_optionalDisplayName) &&
                !string.IsNullOrWhiteSpace(_optionalDisplayName))
            {
                displayName = _optionalDisplayName;
            }

            // Check if per amount is default value of one.
            return _perAmount == DEFAULT_PER_AMOUNT
                ? $"{displayName}"
                : $"{displayName} (per {_perAmount})";
        }

        /// <summary>
        ///     Calculates the total points that should be given for inputted quantity of the object in question.
        /// </summary>
        /// <param name="quantity">Amount of the item found int he players inventory that needs to be calculated.</param>
        /// <returns>Points to be awarded for the given quantity of the item according to scoring rules.</returns>
        public int CalculatePointsForAmount(int quantity)
        {
            // Check quantity is above zero.
            if (quantity <= 0)
                return 0;

            // Check that quantity is above divisor for point calculation.
            if (quantity < _perAmount)
                return 0;

            // Figure out how many points for this quantity.
            var points = (quantity/_perAmount)*PointsAwarded;

            // Return the result to the caller.
            return points;
        }
    }
}