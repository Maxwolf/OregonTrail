// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Helper class that deals with activating classes without using the actual activator class because that requires and
    ///     empty parameterless constructor and we cannot always guarantee we will be able have one. Using these methods don't
    ///     require a constructor to be used and furthermore the use of expressions to generate them ensures caching so penalty
    ///     for type activation is only hit once on first instance creation.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     FormatterServices.GetUninitializedObject(t) will fail for string. Hence special handling for string is
        ///     in place to return empty string.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>The <see cref="bool" />.</returns>
        private static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        ///     Create expression is effectively cached and incurs penalty only the first time the type is loaded. Will handle
        ///     value types too in an efficient manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks>http://stackoverflow.com/a/16162475</remarks>
        /// <example>MyType me = New`MyType`.Instance</example>
        public static class New<T>
        {
            /// <summary>
            ///     The instance.
            /// </summary>
            public static readonly Func<T> Instance = Creator();

            /// <summary>
            ///     The creator.
            /// </summary>
            /// <returns>
            ///     The <see cref="Func" />.
            /// </returns>
            private static Func<T> Creator()
            {
                var t = typeof (T);
                if (t == typeof (string))
                    return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

                if (t.HasDefaultConstructor())
                    return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

                return () => (T) FormatterServices.GetUninitializedObject(t);
            }
        }
    }
}