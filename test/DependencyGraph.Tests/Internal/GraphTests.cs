// -------------------------------------------------------------------------------------------------
// <copyright file="GraphTests.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using LanceC.DependencyGraph.Internal;
using LanceC.DependencyGraph.Internal.Abstractions;
using LanceC.DependencyGraph.Tests.Testing;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Tests.Internal
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
            var mocker = new AutoMocker(MockBehavior.Loose);
            var node = "foo";
            var sut = mocker.CreateInstance<Graph<string>>();

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
            var mocker = new AutoMocker(MockBehavior.Loose);
            var node = "foo";

            var sut = mocker.CreateInstance<Graph<string>>();
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
            var mocker = new AutoMocker(MockBehavior.Loose);
            var source = "foo";

            var sut = mocker.CreateInstance<Graph<string>>();
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
            var mocker = new AutoMocker(MockBehavior.Loose);
            var source = "foo";
            var sut = mocker.CreateInstance<Graph<string>>();

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
            var mocker = new AutoMocker(MockBehavior.Loose);
            var source = "foo";
            var destination = "bar";

            var sut = mocker.CreateInstance<Graph<string>>();
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
            var mocker = new AutoMocker(MockBehavior.Loose);
            var source = "foo";
            var destination = "bar";

            var sut = mocker.CreateInstance<Graph<string>>();
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
            var mocker = new AutoMocker(MockBehavior.Loose);
            var source = "foo";
            var destination = "bar";

            var sut = mocker.CreateInstance<Graph<string>>();

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
        public void HasCycle_GraphContainsCycle_ReturnsTrue()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var sut = mocker.CreateInstance<Graph<string>>();
            sut.AddEdge("foo", "bar");
            sut.AddEdge("bar", "baz");
            sut.AddEdge("baz", "foo");

            // Act
            var hasCycle = sut.HasCycle();

            // Assert
            Assert.True(hasCycle);
        }

        [Fact]
        public void HasCycle_GraphDoesNotContainCycle_ReturnsFalse()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var sut = mocker.CreateInstance<Graph<string>>();
            sut.AddEdge("foo", "bar");
            sut.AddEdge("foo", "baz");
            sut.AddEdge("bar", "baz");

            // Act
            var hasCycle = sut.HasCycle();

            // Assert
            Assert.False(hasCycle);
        }

        [Fact]
        public void GetCycles_ForCycleDetectorOutput_ReturnsDetectedCycles()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var expectedCycles = new[]
            {
                new Cycle<string>(
                    new[]
                    {
                        new DummyNode<string>("1"),
                        new DummyNode<string>("2"),
                    }),
            };
            mocker.GetMock<ICycleDetector<string>>()
                .Setup(cycleDetector => cycleDetector.DetectCycles(It.IsAny<IEnumerable<INode<string>>>()))
                .Returns(expectedCycles);

            var sut = mocker.CreateInstance<Graph<string>>();

            // Act
            var actualCycles = sut.GetCycles();

            // Assert
            Assert.Equal(expectedCycles, actualCycles);
        }

        [Fact]
        public void TopologicalSort_GraphHasCycle_ThrowsInvalidOperationException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var node1 = "foo";
            var node2 = "bar";
            var node3 = "baz";

            var sut = mocker.CreateInstance<Graph<string>>();
            sut.GetOrAddNode(node1);
            sut.GetOrAddNode(node2);
            sut.GetOrAddNode(node3);
            sut.AddEdge(node1, node2);
            sut.AddEdge(node2, node3);
            sut.AddEdge(node3, node1);

            // Act
            var exception = Record.Exception(() => sut.TopologicalSort());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void TopologicalSort_SimpleGraphDoesNotHaveCycle_ReturnsSortedNodes()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var node1 = "foo";
            var node2 = "bar";
            var node3 = "baz";

            var sut = mocker.CreateInstance<Graph<string>>();
            sut.GetOrAddNode(node1);
            sut.GetOrAddNode(node2);
            sut.GetOrAddNode(node3);
            sut.AddEdge(node1, node2);
            sut.AddEdge(node1, node3);
            sut.AddEdge(node2, node3);

            // Act
            var sortedNodes = sut.TopologicalSort();

            // Assert
            Assert.Equal(3, sortedNodes.Count);
            Assert.Equal(node3, sortedNodes.ElementAt(0));
            Assert.Equal(node2, sortedNodes.ElementAt(1));
            Assert.Equal(node1, sortedNodes.ElementAt(2));
        }

        [Fact]
        public void TopologicalSort_ComplexGraphDoesNotHaveCycle_ReturnsSortedNodes()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var node1 = 1;
            var node2 = 2;
            var node3 = 3;
            var node4 = 4;
            var node5 = 5;
            var node6 = 6;
            var node7 = 7;

            var sut = mocker.CreateInstance<Graph<int>>();
            sut.GetOrAddNode(node1);
            sut.GetOrAddNode(node2);
            sut.GetOrAddNode(node3);
            sut.GetOrAddNode(node4);
            sut.GetOrAddNode(node5);
            sut.GetOrAddNode(node6);
            sut.GetOrAddNode(node7);
            sut.AddEdge(node1, node2);
            sut.AddEdge(node1, node3);
            sut.AddEdge(node1, node4);
            sut.AddEdge(node2, node4);
            sut.AddEdge(node2, node5);
            sut.AddEdge(node3, node6);
            sut.AddEdge(node4, node3);
            sut.AddEdge(node4, node6);
            sut.AddEdge(node4, node7);
            sut.AddEdge(node5, node4);
            sut.AddEdge(node5, node7);
            sut.AddEdge(node7, node6);

            // Act
            var sortedNodes = sut.TopologicalSort();

            // Assert
            Assert.Equal(7, sortedNodes.Count);
            Assert.Equal(node6, sortedNodes.ElementAt(0));
            Assert.Equal(node7, sortedNodes.ElementAt(1));
            Assert.Equal(node3, sortedNodes.ElementAt(2));
            Assert.Equal(node4, sortedNodes.ElementAt(3));
            Assert.Equal(node5, sortedNodes.ElementAt(4));
            Assert.Equal(node2, sortedNodes.ElementAt(5));
            Assert.Equal(node1, sortedNodes.ElementAt(6));
        }
    }
}
