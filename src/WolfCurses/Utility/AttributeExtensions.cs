// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OregonTrailDotNet.WolfCurses.Utility
{
    /// <summary>
    ///     Meant for dealing with attributes and grabbing all the available classes of a given type with specified attribute
    ///     using generics.
    /// </summary>
    public static class AttributeExtensions
    {
        /// <summary>
        ///     Find all the classes which have a custom attribute I've defined on them, and I want to be able to find them
        ///     on-the-fly when an application is using my library.
        /// </summary>
        /// <param name="inherit">The inherit.</param>
        /// <remarks>http://stackoverflow.com/a/720171</remarks>
        public static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
            where TAttribute : Attribute
        {
            return from a in typeof(Program).GetTypeInfo().Assembly.DefinedTypes
                   where a.IsDefined(typeof(TAttribute), inherit)
                   select a.AsType();
        }

        /// <summary>Determine if a type implements a specific generic interface type.</summary>
        /// <param name="baseType">The base Type.</param>
        /// <param name="interfaceType">The interface Type.</param>
        /// <remarks>http://stackoverflow.com/a/503359</remarks>
        /// <returns>The <see cref="bool" />.</returns>
        public static bool IsImplementationOf(this Type baseType, Type interfaceType)
        {
            return baseType.GetInterfaces().Any(interfaceType.Equals);
        }

        /// <summary>Find the fields in an enum that have a specific attribute with a specific value.</summary>
        /// <param name="source">The source.</param>
        /// <param name="inherit">The inherit.</param>
        /// <returns>The <see cref="IEnumerable" />.</returns>
        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider source, bool inherit)
            where T : Attribute
        {
            var attrs = source.GetCustomAttributes(typeof (T), inherit);
            return (attrs != null) ? (T[]) attrs : Enumerable.Empty<T>();
        }

        /// <summary>Extension method for enum's that helps with getting custom attributes off of enum values</summary>
        /// <param name="enumValue">The enum Value.</param>
        /// <returns>The <see cref="T" />.</returns>
        public static T GetEnumAttribute<T>(this Enum enumValue)
            where T : Attribute
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            var attribs = field.GetCustomAttributes(typeof (T), false);
            var result = default(T);

            var attributes = attribs as IList<Attribute> ?? attribs.ToList();
            if (attributes.Any())
            {
                result = attributes.FirstOrDefault() as T;
            }

            return result;
        }

        /// <summary>Grabs first attribute from a given object and returns the first one in the enumeration.</summary>
        /// <typeparam name="T">Role of attribute that we should be looking for.</typeparam>
        /// <param name="value">Object that will have attribute tag specified in generic parameter..</param>
        /// <returns>Attribute of the specified type from inputted object.</returns>
        private static T GetAttribute<T>(this object value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo.FirstOrDefault()?.GetCustomAttributes(typeof (T), false);
            return (T) attributes?.FirstOrDefault();
        }

        /// <summary>Attempts to grab description attribute from any object.</summary>
        /// <param name="value">Object that should have description attribute.</param>
        /// <returns>Description attribute text, if null then type name without name space.</returns>
        public static string ToDescriptionAttribute(this object value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}