// -------------------------------------------------------------------------------------------------
// <copyright file="CycleDetector.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DependencyGraph.Internal.Abstractions;

namespace DependencyGraph.Internal
{
    internal class CycleDetector<T> : ICycleDetector<T>
        where T : IEquatable<T>
    {
        public IReadOnlyCollection<Cycle<T>> DetectCycles(IEnumerable<INode<T>> nodes)
        {
            Guard.NotNull(nodes, nameof(nodes));

            var cycles = new List<Cycle<T>>();

            var cycleTrackers = nodes.ToDictionary(node => node.Value, _ => new CycleTracker());
            var executionStack = new Stack<INode<T>>();
            var cycleStack = new Stack<INode<T>>();
            var index = 0;

            foreach (var node in nodes)
            {
                executionStack.Push(node);

                while (executionStack.Any())
                {
                    var currentNode = executionStack.Peek();
                    var currentTracker = cycleTrackers[currentNode.Value];

                    if (!currentTracker.Index.HasValue)
                    {
                        currentTracker.Index = index;
                        currentTracker.LowLink = index;
                        index++;

                        cycleStack.Push(currentNode);
                    }

                    var isUnvisited = false;
                    foreach (var adjacentNode in currentNode.AdjacentNodes)
                    {
                        var adjacentTracker = cycleTrackers[adjacentNode.Value];
                        if (!adjacentTracker.Index.HasValue)
                        {
                            executionStack.Push(adjacentNode);
                            isUnvisited = true;
                        }
                        else if (cycleStack.Any(n => n.Value.Equals(adjacentNode.Value)))
                        {
                            currentTracker.LowLink = Math.Min(currentTracker.LowLink.Value, adjacentTracker.LowLink.Value);
                        }
                    }

                    if (isUnvisited)
                    {
                        isUnvisited = false;
                        continue;
                    }
                    else
                    {
                        executionStack.Pop();
                    }

                    if (currentTracker.LowLink.Value == currentTracker.Index.Value &&
                        cycleStack.Any())
                    {
                        var cycleList = new List<INode<T>>();
                        INode<T> poppedNode;

                        do
                        {
                            poppedNode = cycleStack.Pop();
                            cycleList.Add(poppedNode);
                        }
                        while (!currentNode.Value.Equals(poppedNode.Value));

                        if (cycleList.Count > 1)
                        {
                            var cycle = new Cycle<T>(cycleList);
                            cycles.Add(cycle);
                        }
                    }
                }
            }

            return cycles;
        }

        private class CycleTracker
        {
            public int? Index { get; set; }

            public int? LowLink { get; set; }
        }
    }
}
