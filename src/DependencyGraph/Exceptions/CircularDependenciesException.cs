using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LanceC.DependencyGraph.Exceptions
{
    /// <summary>
    /// Represents that one or more executions cannot occur due to a circular reference in their defined depenencies.
    /// </summary>
    [SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "This exception will only be instantiated from within this assembly.")]
    public class CircularDependenciesException : Exception
    {
        private const string DefaultMessage = "One or more circular dependencies were found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependenciesException"/> class.
        /// </summary>
        /// <param name="dependencyChains">The text representations of the circular dependency chains.</param>
        internal CircularDependenciesException(IReadOnlyCollection<string> dependencyChains)
            : base(DefaultMessage)
        {
            DependencyChains = dependencyChains;
        }

        /// <summary>
        /// Gets the text representations of the circular dependency chains.
        /// </summary>
        public IReadOnlyCollection<string> DependencyChains { get; } = Array.Empty<string>();
    }
}
