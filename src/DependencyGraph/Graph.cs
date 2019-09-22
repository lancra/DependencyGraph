// -------------------------------------------------------------------------------------------------
// <copyright file="Graph.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DependencyGraph.Abstractions;

namespace DependencyGraph
{
    public class Graph<T> : IGraph<T>
        where T : IEquatable<T>
    {
        private readonly IDictionary<T, INode<T>> _nodes = new Dictionary<T, INode<T>>();
        private readonly ICycleDetector<T> _cycleDetector;

        public Graph(ICycleDetector<T> cycleDetector)
        {
            _cycleDetector = cycleDetector;
        }

        public IReadOnlyCollection<T> Nodes => _nodes.Keys.ToArray();

        public T GetOrAddNode(T node)
        {
            var storedNode = InternalGetOrAddNode(node);
            return storedNode.Value;
        }

        public IReadOnlyCollection<T> GetAdjacentNodes(T source)
        {
            if (!_nodes.TryGetValue(source, out var node))
            {
                throw new InvalidOperationException($"The source node is not present in the graph.");
            }

            return node.AdjacentNodes
                .Select(n => n.Value)
                .ToArray();
        }

        public void AddEdge(T source, T destination)
        {
            var sourceNode = InternalGetOrAddNode(source);
            var destinationNode = InternalGetOrAddNode(destination);

            sourceNode.AddAdjacentNode(destinationNode);
        }

        public bool HasCycle()
            => !_nodes.Values.Any(n => n.InDegree == 0);

        public IReadOnlyCollection<Cycle<T>> GetCycles()
            => _cycleDetector.DetectCycles(_nodes.Values);

        private INode<T> InternalGetOrAddNode(T value)
        {
            if (!_nodes.TryGetValue(value, out var node))
            {
                node = new Node<T>(value);
                _nodes.Add(value, node);
            }

            return node;
        }
    }
}
