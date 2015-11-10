using System;
using System.Collections.Generic;
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
    }
}