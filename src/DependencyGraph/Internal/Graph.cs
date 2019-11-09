// -------------------------------------------------------------------------------------------------
// <copyright file="Graph.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using LanceC.DependencyGraph.Internal.Abstractions;

namespace LanceC.DependencyGraph.Internal
{
    internal class Graph<T> : IGraph<T>
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

        public IReadOnlyCollection<T> TopologicalSort()
        {
            if (HasCycle())
            {
                var cycleTexts = GetCycles()
                    .Select(cycle => cycle.ToString())
                    .ToArray();
                throw new CircularDependenciesException(cycleTexts);
            }

            var sortQueue = new Queue<INode<T>>();
            foreach (var node in _nodes.Values)
            {
                if (node.InDegree == 0)
                {
                    sortQueue.Enqueue(node);
                }
            }

            var sortedNodes = new List<T>();
            var inDegrees = _nodes.Values.ToDictionary(node => node.Value, node => node.InDegree);
            while (sortQueue.Any())
            {
                var node = sortQueue.Dequeue();
                sortedNodes.Add(node.Value);

                foreach (var adjacentNode in node.AdjacentNodes)
                {
                    inDegrees[adjacentNode.Value]--;

                    if (inDegrees[adjacentNode.Value] == 0)
                    {
                        sortQueue.Enqueue(adjacentNode);
                    }
                }
            }

            sortedNodes.Reverse();
            return sortedNodes;
        }

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
