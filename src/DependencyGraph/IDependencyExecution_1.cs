// -------------------------------------------------------------------------------------------------
// <copyright file="IDependencyExecution_1.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Internal;

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Defines the interface for an execution in a dependency graph.
    /// </summary>
    /// <typeparam name="TKey">The unique key type for the execution.</typeparam>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "Allow multiple instances of the same interface with different type parameters.")]
    public interface IDependencyExecution<TKey> : IDependencyExecutionIdentifier<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Executes the dependent action.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task Execute(CancellationToken cancellationToken = default);
    }
}
