//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ICycleDetector.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace LanceC.DependencyGraph.Internal.Abstractions
{
    /// <summary>
    /// Defines the detector used for finding cycles in a collection of nodes.
    /// </summary>
    /// <typeparam name="T">The value type of the nodes.</typeparam>
    /// <remarks>Uses Tarjan's strongly connected components algorithm to detect cycles.</remarks>
    internal interface ICycleDetector<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Detects the cycles in the <see cref="INode{T}"/> collection.
        /// </summary>
        /// <param name="nodes">The nodes to detect cycles in.</param>
        /// <returns>The <see cref="Cycle{T}"/> collection.</returns>
        IReadOnlyCollection<Cycle<T>> DetectCycles(IEnumerable<INode<T>> nodes);
    }
}
