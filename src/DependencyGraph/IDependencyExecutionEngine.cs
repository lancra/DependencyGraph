//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IDependencyExecutionEngine.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DependencyGraph
{
    /// <summary>
    /// Defines the interface for an engine used to run a series of executions with inter-dependencies.
    /// </summary>
    /// <typeparam name="T">The unique key type for the executions.</typeparam>
    public interface IDependencyExecutionEngine<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Executes all provided <paramref name="executions"/> in a valid order.
        /// </summary>
        /// <param name="executions">The executions with inter-dependencies.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task ExecuteAll(IEnumerable<IDependencyExecution<T>> executions);
    }
}
