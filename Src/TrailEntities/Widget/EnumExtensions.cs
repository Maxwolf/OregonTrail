using System;
using System.ComponentModel;

namespace TrailEntities.Widget
{
    /// <summary>
    ///     Extension methods intended for use with enumerations and description attributes and specifically converting values
    ///     to and from those states.
    /// </summary>
    public static class EnumExtensions
    {
        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof (T), false);
            return (T) attributes[0];
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string ToName(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        ///     Converts a description attribute from a given enum type into value again.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof (T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof (DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T) field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T) field.GetValue(null);
                }
            }

            throw new ArgumentException(
                "Could not convert string description back into enumeration value from " + typeof (T).Name,
                nameof(description));
        }

        /// <summary>
        ///     Specialized extension methods that feed of it to get the description.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetAttributeDescription(this Enum enumValue)
        {
            var attribute = enumValue.GetAttributeOfType<DescriptionAttribute>();
            return attribute == null ? string.Empty : attribute.Description;
        }

        /// <summary>
        ///     Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;</example>
        /// <remarks>http://stackoverflow.com/a/9276348</remarks>
        private static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof (T), false);
            return (attributes.Length > 0) ? (T) attributes[0] : null;
        }
    }
}