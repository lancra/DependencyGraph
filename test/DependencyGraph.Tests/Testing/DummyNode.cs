using System;
using System.Collections.Generic;
using System.Diagnostics;
using LanceC.DependencyGraph.Internal.Abstractions;

namespace LanceC.DependencyGraph.Tests.Testing
{
    [DebuggerDisplay("Value = {Value}")]
    internal class DummyNode<T> : INode<T>
        where T : IEquatable<T>
    {
        public DummyNode(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public int InDegree { get; set; }

        public IReadOnlyCollection<INode<T>> AdjacentNodes { get; set; }

        public void AddAdjacentNode(INode<T> node)
        {
            throw new NotImplementedException();
        }
    }
}
