// -------------------------------------------------------------------------------------------------
// <copyright file="GraphTests.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DependencyGraph.Abstractions;
using DependencyGraph.Tests.Testing;
using Moq;
using Moq.AutoMock;
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

        public static TheoryData<IReadOnlyCollection<Cycle<string>>> GetCycles_ForCycleDetectorOutput_Data
            => new TheoryData<IReadOnlyCollection<Cycle<string>>>
            {
                { new Cycle<string>[0] },
                {
                    new[]
                    {
                        new Cycle<string>(
                            new[]
                            {
                                new DummyNode<string>("1"),
                                new DummyNode<string>("2"),
                            }),
                    }
                },
                {
                    new[]
                    {
                        new Cycle<string>(
                            new[]
                            {
                                new DummyNode<string>("1"),
                                new DummyNode<string>("2"),
                            }),
                        new Cycle<string>(
                            new[]
                            {
                                new DummyNode<string>("3"),
                                new DummyNode<string>("4"),
                            }),
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

        [Theory]
        [MemberData(nameof(GetCycles_ForCycleDetectorOutput_Data))]
        public void GetCycles_ForCycleDetectorOutput_ReturnsDetectedCycles(IReadOnlyCollection<Cycle<string>> expecteds)
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            mocker.GetMock<ICycleDetector<string>>()
                .Setup(cycleDetector => cycleDetector.DetectCycles(It.IsAny<IEnumerable<INode<string>>>()))
                .Returns(expecteds);

            var sut = mocker.CreateInstance<Graph<string>>();

            // Act
            var actuals = sut.GetCycles();

            // Assert
            Assert.Equal(expecteds, actuals);
        }
    }
}
