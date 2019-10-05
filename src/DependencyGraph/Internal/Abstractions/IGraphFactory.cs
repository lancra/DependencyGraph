// -------------------------------------------------------------------------------------------------
// <copyright file="IGraphFactory.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;

namespace DependencyGraph.Internal.Abstractions
{
    /// <summary>
    /// Defines the interface for creating a <see cref="IGraph{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type of the child nodes.</typeparam>
    internal interface IGraphFactory<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Creates a <see cref="IGraph{T}"/>.
        /// </summary>
        /// <returns>The created <see cref="IGraph{T}"/>.</returns>
        IGraph<T> Create();
    }
}
