// -------------------------------------------------------------------------------------------------
// <copyright file="IDependencyExecutionSorter.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using LanceC.DependencyGraph.Exceptions;

namespace LanceC.DependencyGraph.Internal.Abstractions
{
    /// <summary>
    /// Defines the interface for sorting a series of executions with inter-dependencies.
    /// </summary>
    /// <typeparam name="TKey">The unique key type for the executions.</typeparam>
    internal interface IDependencyExecutionSorter<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Sorts the <paramref name="executions"/> by their dependencies.
        /// </summary>
        /// <param name="executions">The identifiers for executions with inter-dependencies.</param>
        /// <returns>The sorted execution keys.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="executions"/> is <c>null</c>.</exception>
        /// <exception cref="CircularDependenciesException">
        /// Thrown when the <paramref name="executions"/> contain one or more dependency chains.
        /// </exception>
        /// <exception cref="DuplicateKeyException{TKey}">
        /// Thrown when the <paramref name="executions"/> contain one or more duplicate keys.
        /// </exception>
        IReadOnlyCollection<TKey> Sort(IEnumerable<IDependencyExecutionIdentifier<TKey>> executions);
    }
}
