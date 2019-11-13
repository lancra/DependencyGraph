// -------------------------------------------------------------------------------------------------
// <copyright file="DependencyExecutionEngine_2.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Exceptions;
using LanceC.DependencyGraph.Internal;
using LanceC.DependencyGraph.Internal.Abstractions;

namespace LanceC.DependencyGraph
{
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "Allow multiple instances of the same interface with different type parameters.")]
    internal class DependencyExecutionEngine<TKey, TResult> : IDependencyExecutionEngine<TKey, TResult>
        where TKey : IEquatable<TKey>
    {
        private readonly IGraphFactory<TKey> _graphFactory;

        public DependencyExecutionEngine(IGraphFactory<TKey> graphFactory)
        {
            _graphFactory = graphFactory;
        }

        public async Task<IReadOnlyCollection<ExecutionResult<TKey, TResult>>> ExecuteAll(
            IEnumerable<IDependencyExecution<TKey, TResult>> executions,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(executions, nameof(executions));
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

            var results = new List<ExecutionResult<TKey, TResult>>();
            var sortedExecutionKeys = graph.TopologicalSort();
            foreach (var executionKey in sortedExecutionKeys)
            {
                var execution = executions.Single(exec => exec.Key.Equals(executionKey));
                var result = await execution
                    .Execute(cancellationToken)
                    .ConfigureAwait(false);

                results.Add(result);
            }

            return results;
        }

        private void VerifyUniqueKeys(IEnumerable<IDependencyExecution<TKey, TResult>> executions)
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
