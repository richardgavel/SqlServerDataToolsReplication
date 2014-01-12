using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlServer.Replication.Core
{
    internal class LambdaEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object>[] _evaluationFunctions;

        internal LambdaEqualityComparer(params Func<T, object>[] evaluationFunctions)
        {
            _evaluationFunctions = evaluationFunctions;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return (from function in _evaluationFunctions let xValue = function(x) let yValue = function(y) where (xValue != null) || (yValue != null) select xValue.Equals(yValue)).All(result => result);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            var builder = new StringBuilder();

            foreach (var objValue in _evaluationFunctions.Select(function => function(obj)))
            {
                builder.Append(objValue + ":");
            }

            return builder.ToString().GetHashCode();
        }
    }
}
