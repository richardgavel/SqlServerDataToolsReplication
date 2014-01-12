using System;
using System.Collections.Generic;
using System.Data;

namespace SqlServer.Replication.Core
{
    /// <summary>
    /// Extensions on IDataReader to reduce repetitive calls and increase readability
    /// </summary>
    internal static class DataReaderExtensions
    {
        internal static string GetBoolean(this IDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);

            return reader.IsDBNull(ordinal) ? string.Empty : (reader.GetBoolean(ordinal)) ? "True" : "False";
        }

        internal static string GetEnum<T>(this IDataReader reader, string columnName) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            return Enum.GetName(typeof(T), reader.GetValue(reader.GetOrdinal(columnName)));
        }

        internal static int? GetInt32(this IDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);

            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(reader.GetOrdinal(columnName));
        }

        internal static long GetInt64(this IDataReader reader, string columnName)
        {
            return reader.GetInt64(reader.GetOrdinal(columnName));
        }

        internal static string GetString(this IDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);

            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }

        internal static string GetByte(this IDataReader reader, string columnName, IDictionary<int, string> translation)
        {
            var value = reader.GetByte(reader.GetOrdinal(columnName));

            if (!translation.ContainsKey(value))
                throw new ArgumentOutOfRangeException();

            return translation[value];
        }

        internal static string GetInt32(this IDataReader reader, string columnName, IDictionary<int, string> translation)
        {
            var value = reader.GetInt32(reader.GetOrdinal(columnName));

            if (!translation.ContainsKey(value))
                throw new ArgumentOutOfRangeException();

            return translation[value];
        }
    }
}
