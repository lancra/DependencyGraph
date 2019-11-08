// -------------------------------------------------------------------------------------------------
// <copyright file="CircularDependenciesException.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LanceC.DependencyGraph
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
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependenciesException"/> class.
        /// </summary>
        /// <param name="dependencyChains">The text representations of the circular dependency chains.</param>
        internal CircularDependenciesException(IReadOnlyCollection<string> dependencyChains)
            : base()
        {
            DependencyChains = dependencyChains;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependenciesException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="dependencyChains">The text representations of the circular dependency chains.</param>
        internal CircularDependenciesException(string message, IReadOnlyCollection<string> dependencyChains)
            : base(message)
        {
            DependencyChains = dependencyChains;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependenciesException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception is specified.
        /// </param>
        /// <param name="dependencyChains">The text representations of the circular dependency chains.</param>
        internal CircularDependenciesException(string message, Exception innerException, IReadOnlyCollection<string> dependencyChains)
            : base(message)
        {
            DependencyChains = dependencyChains;
        }

        /// <summary>
        /// Gets the text representations of the circular dependency chains.
        /// </summary>
        public IReadOnlyCollection<string> DependencyChains { get; } = Array.Empty<string>();
    }
}
