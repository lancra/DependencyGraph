using System;
using System.Collections.Generic;
using LanceC.DependencyGraph.Exceptions;

namespace LanceC.DependencyGraph.Internal.Abstractions
{
    /// <summary>
    /// Defines a graph.
    /// </summary>
    /// <typeparam name="T">The value type of the child nodes.</typeparam>
    internal interface IGraph<T>
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

        /// <summary>
        /// Determines whether the nodes in the <see cref="IGraph{T}"/> for a cycle.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="IGraph{T}"/> contains a cycle; otherwise, <c>false</c>.</returns>
        bool HasCycle();

        /// <summary>
        /// Gets the strongly-connected cycles in the <see cref="IGraph{T}"/>.
        /// </summary>
        /// <returns>The <see cref="Cycle{T}"/> collection.</returns>
        IReadOnlyCollection<Cycle<T>> GetCycles();

        /// <summary>
        /// Performs a topological sort on the nodes in the <see cref="IGraph{T}"/>.
        /// </summary>
        /// <returns>The sorted nodes.</returns>
        /// <exception cref="CircularDependenciesException">Thrown when the graph contains one or more cycles.</exception>
        IReadOnlyCollection<T> TopologicalSort();
    }
}
