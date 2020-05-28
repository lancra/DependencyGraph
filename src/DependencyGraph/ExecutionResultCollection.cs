using System.Collections.Generic;
using System.Linq;

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Represents a collection of executions results.
    /// </summary>
    /// <typeparam name="TKey">The unique key type for the execution.</typeparam>
    /// <typeparam name="TResult">The result type for the execution.</typeparam>
    public class ExecutionResultCollection<TKey, TResult>
    {
        private readonly IDictionary<TKey, ExecutionResult<TKey, TResult>> _results =
            new Dictionary<TKey, ExecutionResult<TKey, TResult>>();

        /// <summary>
        /// Gets the execution result values.
        /// </summary>
        public IReadOnlyCollection<ExecutionResult<TKey, TResult>> Values
            => _results.Values.ToArray();

        /// <summary>
        /// Gets the execution result by key.
        /// </summary>
        /// <param name="key">The unique key for the execution.</param>
        /// <returns>The matching execution result if found. Otherwise, <see langword="null"/>.</returns>
        public ExecutionResult<TKey, TResult> Get(TKey key)
        {
            _results.TryGetValue(key, out var result);
            return result;
        }

        internal void Add(ExecutionResult<TKey, TResult> result)
            => _results.Add(result.Key, result);
    }
}
