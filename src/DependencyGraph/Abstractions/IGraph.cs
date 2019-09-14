// -------------------------------------------------------------------------------------------------
// <copyright file="IGraph.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DependencyGraph.Abstractions
{
    /// <summary>
    /// Defines a graph.
    /// </summary>
    /// <typeparam name="T">The value type of the child nodes.</typeparam>
    public interface IGraph<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the nodes in the <see cref="IGraph{T}"/>.
        /// </summary>
        IReadOnlyCollection<T> Nodes { get; }

        /// <summary>
        /// Adds a node to the <see cref="IGraph{T}"/> if it does not already exist.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <returns>
        /// The node. This will be either the existing node if it is already in the <see cref="IGraph{T}"/>,
        /// or the new node if it was not in the <see cref="IGraph{T}"/>.
        /// </returns>
        T GetOrAddNode(T node);

        /// <summary>
        /// Gets the nodes adjacent to the provided <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <returns>The adjacent nodes.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <paramref name="source"/> node is not present in the <see cref="IGraph{T}"/>.
        /// </exception>
        IReadOnlyCollection<T> GetAdjacentNodes(T source);

        /// <summary>
        /// Adds a directed edge between two nodes.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="destination">The destination node.</param>
        void AddEdge(T source, T destination);
    }
}
