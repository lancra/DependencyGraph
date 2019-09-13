// -------------------------------------------------------------------------------------------------
// <copyright file="INode.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DependencyGraph.Abstractions
{
    /// <summary>
    /// Defines a node on a <see cref="IGraph{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type of the <see cref="INode{T}"/>.</typeparam>
    public interface INode<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the value of the <see cref="INode{T}"/>.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets the nodes adjacent to the current <see cref="INode{T}"/>.
        /// </summary>
        IReadOnlyCollection<INode<T>> AdjacentNodes { get; }

        /// <summary>
        /// Adds an adjacent node to the source <see cref="INode{T}"/> if it does not already exist.
        /// </summary>
        /// <param name="node">The adjacent <see cref="INode{T}"/> to add.</param>
        /// <returns>
        /// The node. This will either be the existing node if it is already in the source <see cref="INode{T}"/>,
        /// or the new node if it was not in the source <see cref="INode{T}"/>.
        /// </returns>
        INode<T> GetOrAddAdjacentNode(INode<T> node);
    }
}
