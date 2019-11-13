// -------------------------------------------------------------------------------------------------
// <copyright file="DependencyExecutionSorter.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using LanceC.DependencyGraph.Exceptions;
using LanceC.DependencyGraph.Internal.Abstractions;

namespace LanceC.DependencyGraph.Internal
{
    internal class DependencyExecutionSorter<TKey> : IDependencyExecutionSorter<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IGraphFactory<TKey> _graphFactory;

        public DependencyExecutionSorter(IGraphFactory<TKey> graphFactory)
        {
            _graphFactory = graphFactory;
        }

        public IReadOnlyCollection<TKey> Sort(IEnumerable<IDependencyExecutionIdentifier<TKey>> executions)
        {
            VerifyUniqueKeys(executions);

            var graph = _graphFactory.Create();
            foreach (var execution in executions)
            {
                graph.GetOrAddNode(execution.Key);
                foreach (var dependentKey in execution.DependentKeys)
                {
                    graph.GetOrAddNode(dependentKey);
                    graph.AddEdge(execution.Key, dependentKey);
                }
            }

            var sortedExecutionKeys = graph
                .TopologicalSort()
                .ToArray();
            return sortedExecutionKeys;
        }

        private void VerifyUniqueKeys(IEnumerable<IDependencyExecutionIdentifier<TKey>> executions)
        {
            var duplicateKeys = executions
                .GroupBy(execution => execution.Key)
                .Where(executionGroup => executionGroup.Count() > 1)
                .Select(executionGroup => executionGroup.Key)
                .ToArray();
            if (duplicateKeys.Any())
            {
                throw new DuplicateKeyException<TKey>(duplicateKeys);
            }
        }
    }
}
