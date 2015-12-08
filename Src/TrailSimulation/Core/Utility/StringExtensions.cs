namespace TrailSimulation.Core
{
    /// <summary>
    ///     Utility class that contains useful extension methods that make working with strings a little easier and less
    ///     repetitive.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Cuts off a string given a certain input amount, useful for ensuring the user never enters more than possible into a
        ///     given field.
        /// </summary>
        /// <param name="value">String that needs to be truncated to max length.</param>
        /// <param name="maxLength">Negative values will cause exception.</param>
        /// <returns>Truncated string.</returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}