// -------------------------------------------------------------------------------------------------
// <copyright file="ExecutionResult.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Represents the return result from an execution.
    /// </summary>
    /// <typeparam name="TKey">The unique key type for the execution.</typeparam>
    /// <typeparam name="TResult">The result type for the execution.</typeparam>
    public class ExecutionResult<TKey, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionResult{TKey, TResult}"/> class.
        /// </summary>
        /// <param name="key">The key for the execution.</param>
        /// <param name="result">The execution result.</param>
        internal ExecutionResult(TKey key, TResult result)
        {
            Key = key;
            Result = result;
        }

        /// <summary>
        /// Gets the key for the execution.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Gets the execution result.
        /// </summary>
        public TResult Result { get; }
    }
}
