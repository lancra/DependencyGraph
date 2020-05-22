using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Internal;
using LanceC.DependencyGraph.Internal.Abstractions;

namespace LanceC.DependencyGraph
{
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "Allow multiple instances of the same interface with different type parameters.")]
    internal class DependencyExecutionEngine<TKey> : IDependencyExecutionEngine<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IDependencyExecutionSorter<TKey> _dependencyExecutionSorter;

        public DependencyExecutionEngine(IDependencyExecutionSorter<TKey> dependencyExecutionSorter)
        {
            _dependencyExecutionSorter = dependencyExecutionSorter;
        }

        public async Task ExecuteAll(IEnumerable<IDependencyExecution<TKey>> executions, CancellationToken cancellationToken = default)
        {
            Guard.NotNull(executions, nameof(executions));

            var sortedExecutionKeys = _dependencyExecutionSorter.Sort(executions);

            foreach (var executionKey in sortedExecutionKeys)
            {
                var execution = executions.Single(exec => exec.Key.Equals(executionKey));
                await execution
                    .Execute(cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
