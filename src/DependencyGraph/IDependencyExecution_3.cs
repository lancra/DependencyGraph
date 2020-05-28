using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Internal;

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Defines the interface for an execution in a dependency graph.
    /// </summary>
    /// <typeparam name="TKey">The unique key type for the execution.</typeparam>
    /// <typeparam name="TContext">The context type for the execution.</typeparam>
    /// <typeparam name="TResult">The result type for the execution.</typeparam>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "Allow multiple instances of the same interface with different type parameters.")]
    public interface IDependencyExecution<TKey, TContext, TResult> : IDependencyExecutionIdentifier<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Executes the dependent action.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="priorExecutionResults">The execution results for all previously-run actions.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation,
        /// containing the execution result.
        /// </returns>
        Task<TResult> Execute(
            TContext context,
            ExecutionResultCollection<TKey, TResult> priorExecutionResults,
            CancellationToken cancellationToken = default);
    }
}
