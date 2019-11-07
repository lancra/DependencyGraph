// -------------------------------------------------------------------------------------------------
// <copyright file="DependencyExecutionEngine.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Internal.Abstractions;

namespace LanceC.DependencyGraph
{
    internal class DependencyExecutionEngine<T> : IDependencyExecutionEngine<T>
        where T : IEquatable<T>
    {
        private readonly IGraphFactory<T> _graphFactory;

        public DependencyExecutionEngine(IGraphFactory<T> graphFactory)
        {
            _graphFactory = graphFactory;
        }

        public async Task ExecuteAll(IEnumerable<IDependencyExecution<T>> executions)
        {
            var graph = _graphFactory.Create();
            foreach (var execution in executions)
            {
                graph.GetOrAddNode(execution.Key);
                foreach (var dependency in execution.Dependencies)
                {
                    graph.GetOrAddNode(dependency.Key);
                    graph.AddEdge(execution.Key, dependency.Key);
                }
            }

            var sortedExecutionKeys = graph.TopologicalSort();
            foreach (var executionKey in sortedExecutionKeys)
            {
                var execution = executions.Single(exec => exec.Key.Equals(executionKey));
                await execution.Execute();
            }
        }
    }
}
