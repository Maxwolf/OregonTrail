using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace TrailEntities.Widget
{
    /// <summary>
    ///     Meant for dealing with attributes and grabbing all the available classes of a given type with specified attribute
    ///     using generics.
    /// </summary>
    public static class AttributeHelper
    {
        /// <summary>
        ///     Find all the classes which have a custom attribute I've defined on them, and I want to be able to find them
        ///     on-the-fly when an application is using my library.
        /// </summary>
        /// <remarks>http://stackoverflow.com/a/720171</remarks>
        public static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
            where TAttribute : Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                where t.IsDefined(typeof (TAttribute), inherit)
                select t;
        }

        /// <summary>
        ///     Find the fields in an enum that have a specific attribute with a specific value.
        /// </summary>
        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider source, bool inherit)
            where T : Attribute
        {
            var attrs = source.GetCustomAttributes(typeof (T), inherit);
            return (attrs != null) ? (T[]) attrs : Enumerable.Empty<T>();
        }

        /// <summary>
        ///     Inspects types for public, non-public constructors we can use or an instance of the given object.
        /// </summary>
        /// <param name="objectType">Type of object we would like to create instance of.</param>
        /// <returns>Activated instance of object type. Ready to be casted to whatever the developer wants.</returns>
        public static object GetInstanceByType(Type objectType)
        {
            // Check if the class is abstract base class, we don't want to add that.
            if (objectType.IsAbstract)
                return null;

            // Get the constructor and create an instance object type.
            var instantiatedType = Activator.CreateInstance(
                objectType,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] {objectType.Name}, // Constructor with 1 parameter...
                CultureInfo.InvariantCulture);

            return instantiatedType;
        }
    }
}