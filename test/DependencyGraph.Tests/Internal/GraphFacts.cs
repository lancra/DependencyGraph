using System;
using System.Collections.Generic;
using System.Linq;
using LanceC.DependencyGraph.Exceptions;
using LanceC.DependencyGraph.Facts.Testing;
using LanceC.DependencyGraph.Internal;
using LanceC.DependencyGraph.Internal.Abstractions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Facts.Internal
{
    public class GraphFacts
    {
        private readonly AutoMocker _mocker = new AutoMocker();

        private Graph<string> CreateSystemUnderTest()
            => CreateSystemUnderTest<string>();

        private Graph<T> CreateSystemUnderTest<T>()
            where T : IEquatable<T>
            => _mocker.CreateInstance<Graph<T>>();

        public class TheGetOrAddNodeMethod : GraphFacts
        {
            [Fact]
            public void AddsNodeForNewValue()
            {
                // Arrange
                var node = "foo";
                var sut = CreateSystemUnderTest();

                // Act
                var newNode = sut.GetOrAddNode(node);

                // Assert
                Assert.Equal(node, newNode);
                var graphNode = Assert.Single(sut.Nodes);
                Assert.Equal(node, graphNode);
            }

            [Fact]
            public void DoesNotAddNodeForExistingValue()
            {
                // Arrange
                var node = "foo";

                var sut = CreateSystemUnderTest();
                sut.GetOrAddNode("foo");

                // Act
                var newNode = sut.GetOrAddNode(node);

                // Assert
                Assert.Equal(node, newNode);
                var graphNode = Assert.Single(sut.Nodes);
                Assert.Equal(node, graphNode);
            }
        }

        public class TheGetAdjacentNodesMethod : GraphFacts
        {
            public static TheoryData<IReadOnlyCollection<string>> ReturnsAdjacentNodesWhenSourceNodeIsInGraph_Data
                => new TheoryData<IReadOnlyCollection<string>>
                {
                    { Array.Empty<string>() },
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

            [Theory]
            [MemberData(nameof(ReturnsAdjacentNodesWhenSourceNodeIsInGraph_Data))]
            public void ReturnsAdjacentNodesWhenSourceNodeIsInGraph(IReadOnlyCollection<string> destinations)
            {
                // Arrange
                var source = "foo";

                var sut = CreateSystemUnderTest();
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
            public void ThrowsInvalidOperationExceptionWhenSourceNodeIsNotInGraph()
            {
                // Arrange
                var source = "foo";
                var sut = CreateSystemUnderTest();

                // Act
                var exception = Record.Exception(() => sut.GetAdjacentNodes(source));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<InvalidOperationException>(exception);
            }
        }

        public class TheAddEdgeMethod : GraphFacts
        {
            [Fact]
            public void AddsSourceNodeAndEdgeWhenSourceNodeIsNotInGraph()
            {
                // Arrange
                var source = "foo";
                var destination = "bar";

                var sut = CreateSystemUnderTest();
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
            public void AddsDestinationNodeAndEdgeWhenDestinationNodeIsNotInGraph()
            {
                // Arrange
                var source = "foo";
                var destination = "bar";

                var sut = CreateSystemUnderTest();
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
            public void AddsBothNodesAndEdgeWhenBothNodesAreNotInGraph()
            {
                // Arrange
                var source = "foo";
                var destination = "bar";

                var sut = CreateSystemUnderTest();

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

        public class TheHasCycleMethod : GraphFacts
        {
            [Fact]
            public void ReturnsTrueWhenGraphContainsCycle()
            {
                // Arrange
                var sut = CreateSystemUnderTest();
                sut.AddEdge("foo", "bar");
                sut.AddEdge("bar", "baz");
                sut.AddEdge("baz", "foo");

                // Act
                var hasCycle = sut.HasCycle();

                // Assert
                Assert.True(hasCycle);
            }

            [Fact]
            public void ReturnsFalseWhenGraphDoesNotContainCycle()
            {
                // Arrange
                var sut = CreateSystemUnderTest();
                sut.AddEdge("foo", "bar");
                sut.AddEdge("foo", "baz");
                sut.AddEdge("bar", "baz");

                // Act
                var hasCycle = sut.HasCycle();

                // Assert
                Assert.False(hasCycle);
            }
        }

        public class TheGetCyclesMethod : GraphFacts
        {
            [Fact]
            public void ReturnsDetectedCyclesFromCycleDetectorOutput()
            {
                // Arrange
                var expectedCycles = new[]
                {
                    new Cycle<string>(
                        new[]
                        {
                            new DummyNode<string>("1"),
                            new DummyNode<string>("2"),
                        }),
                };
                _mocker.GetMock<ICycleDetector<string>>()
                    .Setup(cycleDetector => cycleDetector.DetectCycles(It.IsAny<IEnumerable<INode<string>>>()))
                    .Returns(expectedCycles);

                var sut = CreateSystemUnderTest();

                // Act
                var actualCycles = sut.GetCycles();

                // Assert
                Assert.Equal(expectedCycles, actualCycles);
            }
        }

        public class TheTopologicalSortMethod : GraphFacts
        {
            [Fact]
            public void ThrowsCircularDependenciesExceptionWhenGraphHasCycle()
            {
                // Arrange
                var node1 = "foo";
                var node2 = "bar";
                var node3 = "baz";

                var cycle = new Cycle<string>(
                    new[]
                    {
                        new DummyNode<string>("baz"),
                        new DummyNode<string>("bar"),
                        new DummyNode<string>("foo"),
                    });
                _mocker.GetMock<ICycleDetector<string>>()
                    .Setup(cycleDetector => cycleDetector.DetectCycles(It.IsAny<IEnumerable<INode<string>>>()))
                    .Returns(new[] { cycle, });

                var sut = CreateSystemUnderTest();
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
                var circularDependenciesException = Assert.IsType<CircularDependenciesException>(exception);
                var dependencyChain = Assert.Single(circularDependenciesException.DependencyChains);
                Assert.Equal("'baz' -> 'bar' -> 'foo'", dependencyChain);
            }

            [Fact]
            public void ReturnsSortedNodesForSimpleGraphThatDoesNotHaveAnyCycles()
            {
                // Arrange
                var node1 = "foo";
                var node2 = "bar";
                var node3 = "baz";

                var sut = CreateSystemUnderTest();
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
            public void ReturnsSortedNodesForComplexGraphThatDoesNotHaveAnyCycles()
            {
                // Arrange
                var node1 = 1;
                var node2 = 2;
                var node3 = 3;
                var node4 = 4;
                var node5 = 5;
                var node6 = 6;
                var node7 = 7;

                var sut = CreateSystemUnderTest<int>();
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
}
