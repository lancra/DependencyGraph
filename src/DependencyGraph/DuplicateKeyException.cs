// -------------------------------------------------------------------------------------------------
// <copyright file="DuplicateKeyException.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Represents that the provided executions contain multiple instances of the same key.
    /// </summary>
    /// <typeparam name="T">The execution key type.</typeparam>
    [SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "This exception will only be instantiated from within this assembly.")]
    public class DuplicateKeyException<T> : Exception
    {
        private const string DefaultMessage = "One or more duplicate execution keys were provided.";

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyException{T}"/> class.
        /// </summary>
        /// <param name="keys">The duplicate keys.</param>
        internal DuplicateKeyException(IEnumerable<T> keys)
            : base(DefaultMessage)
        {
            Keys = keys.ToArray();
        }

        /// <summary>
        /// Gets the duplicate keys.
        /// </summary>
        public IReadOnlyCollection<T> Keys { get; }
    }
}
