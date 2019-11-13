// -------------------------------------------------------------------------------------------------
// <copyright file="IDependencyExecution_2.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Defines the interface for an execution in a dependency graph.
    /// </summary>
    /// <typeparam name="TKey">The unique key type for the execution.</typeparam>
    /// <typeparam name="TResult">The result type for the execution.</typeparam>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "Allow multiple instances of the same interface with different type parameters.")]
    public interface IDependencyExecution<TKey, TResult>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets the key for the execution.
        /// </summary>
        TKey Key { get; }

        /// <summary>
        /// Gets the keys for the executions that the <see cref="IDependencyExecution{TKey}"/> requires to be executed.
        /// </summary>
        IReadOnlyCollection<TKey> DependentKeys { get; }

        /// <summary>
        /// Executes the dependent action.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation,
        /// containing the execution result.
        /// </returns>
        Task<ExecutionResult<TKey, TResult>> Execute(CancellationToken cancellationToken = default);
    }
}
