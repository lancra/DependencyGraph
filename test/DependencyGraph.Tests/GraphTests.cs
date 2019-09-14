// -------------------------------------------------------------------------------------------------
// <copyright file="GraphTests.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Xunit;

namespace DependencyGraph.Tests
{
    public class GraphTests
    {
        public static TheoryData<IReadOnlyCollection<string>> GetAdjacentNodes_SourceNodeInGraph_Data
            => new TheoryData<IReadOnlyCollection<string>>
            {
                { new string[0] },
                {
                    new[]
                    {
                        "bar",
                    }
                },
                {
                    new[]
                    {
                        "bar",
                        "baz",
                    }
                },
            };

        [Fact]
        public void GetOrAddNode_NewNode_AddsNode()
        {
            // Arrange
            var node = "foo";
            var sut = new Graph<string>();

            // Act
            var newNode = sut.GetOrAddNode(node);

            // Assert
            Assert.Equal(node, newNode);
            var graphNode = Assert.Single(sut.Nodes);
            Assert.Equal(node, graphNode);
        }

        [Fact]
        public void GetOrAddNode_ExistingNode_DoesNotAddNode()
        {
            // Arrange
            var node = "foo";

            var sut = new Graph<string>();
            sut.GetOrAddNode("foo");

            // Act
            var newNode = sut.GetOrAddNode(node);

            // Assert
            Assert.Equal(node, newNode);
            var graphNode = Assert.Single(sut.Nodes);
            Assert.Equal(node, graphNode);
        }

        [Theory]
        [MemberData(nameof(GetAdjacentNodes_SourceNodeInGraph_Data))]
        public void GetAdjacentNodes_SourceNodeInGraph_ReturnsAdjacentNodes(IReadOnlyCollection<string> destinations)
        {
            // Arrange
            var source = "foo";

            var sut = new Graph<string>();
            sut.GetOrAddNode(source);
            foreach (var destination in destinations)
            {
                sut.AddEdge(source, destination);
            }

            // Act
            var adjacentNodes = sut.GetAdjacentNodes(source);

            // Assert
            Assert.Equal(destinations.Count, adjacentNodes.Count);
        }

        [Fact]
        public void GetAdjacentNodes_SourceNodeNotInGraph_ThrowsInvalidOperationException()
        {
            // Arrange
            var source = "foo";
            var sut = new Graph<string>();

            // Act
            var exception = Record.Exception(() => sut.GetAdjacentNodes(source));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void AddEdge_SourceNodeNotInGraph_AddsSourceNodeAndEdge()
        {
            // Arrange
            var source = "foo";
            var destination = "bar";

            var sut = new Graph<string>();
            sut.GetOrAddNode(destination);

            // Act
            sut.AddEdge(source, destination);

            // Assert
            Assert.Equal(2, sut.Nodes.Count);
            Assert.Single(sut.Nodes, source);
            Assert.Single(sut.Nodes, destination);

            var adjacentNodes = sut.GetAdjacentNodes(source);
            var adjacentNode = Assert.Single(adjacentNodes);
            Assert.Equal(destination, adjacentNode);
        }

        [Fact]
        public void AddEdge_DestinationNodeNotInGraph_AddsDestinationNodeAndEdge()
        {
            // Arrange
            var source = "foo";
            var destination = "bar";

            var sut = new Graph<string>();
            sut.GetOrAddNode(source);

            // Act
            sut.AddEdge(source, destination);

            // Assert
            Assert.Equal(2, sut.Nodes.Count);
            Assert.Single(sut.Nodes, source);
            Assert.Single(sut.Nodes, destination);

            var adjacentNodes = sut.GetAdjacentNodes(source);
            var adjacentNode = Assert.Single(adjacentNodes);
            Assert.Equal(destination, adjacentNode);
        }

        [Fact]
        public void AddEdge_BothNodesNotInGraph_AddsBothNodesAndEdge()
        {
            // Arrange
            var source = "foo";
            var destination = "bar";

            var sut = new Graph<string>();

            // Act
            sut.AddEdge(source, destination);

            // Assert
            Assert.Equal(2, sut.Nodes.Count);
            Assert.Single(sut.Nodes, source);
            Assert.Single(sut.Nodes, destination);

            var adjacentNodes = sut.GetAdjacentNodes(source);
            var adjacentNode = Assert.Single(adjacentNodes);
            Assert.Equal(destination, adjacentNode);
        }
    }
}
