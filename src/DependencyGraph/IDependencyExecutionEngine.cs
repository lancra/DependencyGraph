//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IDependencyExecutionEngine.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Defines the interface for an engine used to run a series of executions with inter-dependencies.
    /// </summary>
    /// <typeparam name="TKey">The unique key type for the executions.</typeparam>
    public interface IDependencyExecutionEngine<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Executes all provided <paramref name="executions"/> in a valid order.
        /// </summary>
        /// <param name="executions">The executions with inter-dependencies.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="executions"/> is <c>null</c>.</exception>
        /// <exception cref="CircularDependenciesException">
        /// Thrown when the <paramref name="executions"/> contain one or more dependency chains.
        /// </exception>
        /// <exception cref="DuplicateKeyException{TKey}">
        /// Thrown when the <paramref name="executions"/> contain one or more duplicate keys.
        /// </exception>
        Task ExecuteAll(IEnumerable<IDependencyExecution<TKey>> executions, CancellationToken cancellationToken = default);
    }
}
