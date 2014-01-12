using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlServer.Replication.Core
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, object> evaluator)
        {
            return first.Except(second, new LambdaEqualityComparer<T>(evaluator));
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, object> evaluator)
        {
            return first.Union(second, new LambdaEqualityComparer<T>(evaluator));
        }

        /// <summary>
        /// Produces the set intersection of two sequences, based on a applying a function to the objects within the sequences
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An IEnumerable&lt;T&gt; whose distinct elements that also appear in second will be returned.</param>
        /// <param name="second">An IEnumerable&lt;T&gt; whose distinct elements that also appear in the first sequence will be returned.</param>
        /// <param name="evaluator"></param>
        /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
        /// <remarks>
        /// This method is used when instead of comparing the objects themselves, the intent is to compare something about the objects.
        /// For example, using the "Id" property to intersect the sequences
        /// </remarks>
        /// <example>
        /// The following example will find employees that are both active and managers, matching employees based on their employee ID.
        /// <code>
        /// class Employee
        /// {
        ///     public int Id { get; set; }
        /// 
        ///     public string Name { get; set; }
        /// }
        /// 
        /// var activeEmployees = new Employee[] { new Employee { Id = 1, Name = "John Smith", new Employee { Id = 2, Name = "Mary Walker" } };
        /// var managerEmployees = new Employee[] { new Employee { Id = 1, Name = "John Smith", new Employee { Id = 3, Name = "Scott James" } };
        /// var activeManagerEmployees = activeEmployees.Intersect(managerEmployees, x => x.Id);
        /// </code>
        /// </example>
        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, object> evaluator)
        {
            return first.Intersect(second, new LambdaEqualityComparer<T>(evaluator));
        }
    }
}
