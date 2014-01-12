using System;

namespace SqlServer.Replication.Core
{
    public static class StringExtensions
    {
        public static T AsEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof (T), value);
        }
    }
}
