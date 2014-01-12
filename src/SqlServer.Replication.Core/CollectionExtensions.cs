using System;
using System.Collections.Generic;

namespace SqlServer.Replication.Core
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds a collection of items to another collection, returning the same collection (provides fluent API equivalent of AddRange)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ICollection<T> Add<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (items == null) throw new ArgumentNullException("items");

            foreach (var item in items)
                collection.Add(item);

            return collection;
        }
    }
}
