// -------------------------------------------------------------------------------------------------
// <copyright file="Cycle.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DependencyGraph.Internal.Abstractions;

namespace DependencyGraph.Internal
{
    internal class Cycle<T>
        where T : IEquatable<T>
    {
        public Cycle(IEnumerable<INode<T>> nodes)
        {
            Nodes = nodes
                .Select(node => node.Value)
                .ToArray();
        }

        public IReadOnlyCollection<T> Nodes { get; }

        public override string ToString()
            => "\"" + string.Join("\" -> \"", Nodes) + "\"";
    }
}
