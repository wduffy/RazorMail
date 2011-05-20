using System;
using System.Linq;
using System.Collections.Generic;

namespace RazorMail.Extensions
{
    internal static class CollectionExtensions
    {

        /// <summary>
        /// Performs the specified action on each element of the source
        /// </summary>
        /// <remarks></remarks>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) where T : class
        {
           if (source != null)
               foreach (T element in source)
                   action(element);
        }

        /// <summary>
        /// Checks the source to see if it is empty
        /// </summary>
        /// <returns>A boolean value indicating the if the source is empty</returns>
        /// <remarks></remarks>
        internal static bool IsEmpty<T>(this IEnumerable<T> s)
        {
            return s == null ? true : !s.Any();
        }

    }
}
