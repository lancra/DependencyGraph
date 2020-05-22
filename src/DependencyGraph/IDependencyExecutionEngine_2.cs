using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Exceptions;

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Defines the interface for an engine used to run a series of executions with inter-dependencies.
    /// </summary>
    /// <typeparam name="TKey">The unique key type for the executions.</typeparam>
    /// <typeparam name="TResult">The result type for the executions.</typeparam>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "Allow multiple instances of the same interface with different type parameters.")]
    public interface IDependencyExecutionEngine<TKey, TResult>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Executes all provided <paramref name="executions"/> in a valid order.
        /// </summary>
        /// <param name="executions">The executions with inter-dependencies.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation,
        /// containing the results of the executions.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="executions"/> is <c>null</c>.</exception>
        /// <exception cref="CircularDependenciesException">
        /// Thrown when the <paramref name="executions"/> contain one or more dependency chains.
        /// </exception>
        /// <exception cref="DuplicateKeyException{TKey}">
        /// Thrown when the <paramref name="executions"/> contain one or more duplicate keys.
        /// </exception>
        Task<IReadOnlyCollection<ExecutionResult<TKey, TResult>>> ExecuteAll(
            IEnumerable<IDependencyExecution<TKey, TResult>> executions,
            CancellationToken cancellationToken = default);
    }
}
