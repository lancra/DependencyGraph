using System;
using System.Collections.Generic;

namespace LanceC.DependencyGraph.Internal.Abstractions
{
    /// <summary>
    /// Defines a node on a <see cref="IGraph{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type of the <see cref="INode{T}"/>.</typeparam>
    internal interface INode<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the value of the <see cref="INode{T}"/>.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets or sets the number of nodes coming into the <see cref="INode{T}"/>.
        /// </summary>
        int InDegree { get; set; }

        /// <summary>
        /// Gets the nodes adjacent to the current <see cref="INode{T}"/>.
        /// </summary>
        IReadOnlyCollection<INode<T>> AdjacentNodes { get; }

        /// <summary>
        /// Adds an adjacent node to the source <see cref="INode{T}"/> if it does not already exist.
        /// </summary>
        /// <param name="node">The adjacent <see cref="INode{T}"/> to add.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the value of the destination <paramref name="node"/>
        /// is equal to the value of the source <see cref="INode{T}"/>.
        /// </exception>
        void AddAdjacentNode(INode<T> node);
    }
}
