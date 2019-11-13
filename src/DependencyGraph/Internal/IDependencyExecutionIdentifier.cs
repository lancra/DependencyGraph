// -------------------------------------------------------------------------------------------------
// <copyright file="IDependencyExecutionIdentifier.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace LanceC.DependencyGraph.Internal
{
    /// <summary>
    /// Defines the interface for the identifier of an execution in a dependency graph.
    /// </summary>
    /// <typeparam name="TKey">The unique key type for the execution.</typeparam>
    public interface IDependencyExecutionIdentifier<TKey>
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
    }
}
