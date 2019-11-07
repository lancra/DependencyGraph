// -------------------------------------------------------------------------------------------------
// <copyright file="Node.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using LanceC.DependencyGraph.Internal.Abstractions;

namespace LanceC.DependencyGraph.Internal
{
    internal class Node<T> : INode<T>
        where T : IEquatable<T>
    {
        private readonly IDictionary<T, INode<T>> _adjacentNodes = new Dictionary<T, INode<T>>();

        public Node(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public int InDegree { get; set; }

        public IReadOnlyCollection<INode<T>> AdjacentNodes => _adjacentNodes.Values.ToArray();

        public void AddAdjacentNode(INode<T> node)
        {
            Guard.NotNull(node, nameof(node));

            if (node.Value.Equals(Value))
            {
                throw new InvalidOperationException("A node cannot be adjacent to itself.");
            }

            if (!_adjacentNodes.ContainsKey(node.Value))
            {
                _adjacentNodes.Add(node.Value, node);
                node.InDegree++;
            }
        }
    }
}
