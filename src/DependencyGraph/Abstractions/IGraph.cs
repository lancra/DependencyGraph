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
        /// Gets the child nodes in the <see cref="IGraph{T}"/>.
        /// </summary>
        IReadOnlyCollection<INode<T>> Nodes { get; }

        /// <summary>
        /// Adds a node to the <see cref="IGraph{T}"/> if it does not already exist.
        /// </summary>
        /// <param name="value">The value of the <see cref="INode{T}"/> to add.</param>
        /// <returns>
        /// The <see cref="INode{T}"/>.
        /// This will be either the existing <see cref="INode{T}"/> if it is already in the <see cref="IGraph{T}"/>, or
        /// the new <see cref="INode{T}"/> if it was not in the <see cref="IGraph{T}"/>.
        /// </returns>
        INode<T> GetOrAddNode(T value);

        /// <summary>
        /// Gets the nodes adjacent to the node with the provided <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value of the source <see cref="INode{T}"/>.</param>
        /// <returns>The adjacent nodes.</returns>
        IReadOnlyCollection<INode<T>> GetAdjacentNodes(T value);

        /// <summary>
        /// Adds a directed edge between two nodes.
        /// </summary>
        /// <param name="source">The source <see cref="INode{T}"/>.</param>
        /// <param name="destination">The destination <see cref="INode{T}"/>.</param>
        void AddEdge(T source, T destination);
    }
}
