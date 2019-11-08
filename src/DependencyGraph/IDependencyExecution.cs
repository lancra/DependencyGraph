// -------------------------------------------------------------------------------------------------
// <copyright file="IDependencyExecution.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Defines the interface for an execution in a depedency graph.
    /// </summary>
    /// <typeparam name="T">The unique key type for the execution.</typeparam>
    public interface IDependencyExecution<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the key for the execution.
        /// </summary>
        T Key { get; }

        /// <summary>
        /// Gets the keys for the executions that the <see cref="IDependencyExecution{T}"/> requires to be executed.
        /// </summary>
        IReadOnlyCollection<T> DependentKeys { get; }

        /// <summary>
        /// Executes the dependent action.
        /// </summary>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task Execute();
    }
}
