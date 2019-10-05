// -------------------------------------------------------------------------------------------------
// <copyright file="GraphFactory.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using DependencyGraph.Internal.Abstractions;

namespace DependencyGraph.Internal
{
    internal class GraphFactory<T> : IGraphFactory<T>
        where T : IEquatable<T>
    {
        private readonly ICycleDetector<T> _cycleDetector;

        public GraphFactory(ICycleDetector<T> cycleDetector)
        {
            _cycleDetector = cycleDetector;
        }

        public IGraph<T> Create()
            => new Graph<T>(_cycleDetector);
    }
}
