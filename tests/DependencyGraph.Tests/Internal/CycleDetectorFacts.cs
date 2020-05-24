using System;
using System.Linq;
using LanceC.DependencyGraph.Facts.Testing;
using LanceC.DependencyGraph.Internal;
using LanceC.DependencyGraph.Internal.Abstractions;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Facts.Internal
{
    public class CycleDetectorFacts
    {
        private readonly AutoMocker _mocker = new AutoMocker();

        private CycleDetector<string> CreateSystemUnderTest()
            => _mocker.CreateInstance<CycleDetector<string>>();

        public class TheDetectCyclesMethod : CycleDetectorFacts
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenNodeEnumerableIsNull()
            {
                // Arrange
                var sut = CreateSystemUnderTest();

                // Act
                var exception = Record.Exception(() => sut.DetectCycles(default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
            }

            [Fact]
            public void ReturnsEmptyCycleCollectionWhenNodeEnumerableIsEmpty()
            {
                // Arrange
                var sut = CreateSystemUnderTest();

                // Act
                var cycles = sut.DetectCycles(Array.Empty<DummyNode<string>>());

                // Assert
                Assert.Empty(cycles);
            }

            [Fact]
            public void ReturnsCycleWhenCycleOfNodesExists()
            {
                // Arrange
                var node1 = new DummyNode<string>("1");
                var node2 = new DummyNode<string>("2");
                var node3 = new DummyNode<string>("3");
                var node4 = new DummyNode<string>("4");

                node1.AdjacentNodes = new[] { node2, };
                node2.AdjacentNodes = new[] { node3, };
                node3.AdjacentNodes = new[] { node1, };
                node4.AdjacentNodes = new[] { node2, };

                var nodes = new[]
                {
                    node1,
                    node2,
                    node3,
                    node4,
                };

                var sut = CreateSystemUnderTest();

                // Act
                var cycles = sut.DetectCycles(nodes);

                // Assert
                var cycle = Assert.Single(cycles);
                Assert.Equal(3, cycle.Nodes.Count);
                Assert.Single(cycle.Nodes, n => n == node1.Value);
                Assert.Single(cycle.Nodes, n => n == node2.Value);
                Assert.Single(cycle.Nodes, n => n == node3.Value);
            }

            [Fact]
            public void ReturnsCyclesWhenMultipleCyclesOfNodesExist()
            {
                // Arrange
                var node1 = new DummyNode<string>("1");
                var node2 = new DummyNode<string>("2");
                var node3 = new DummyNode<string>("3");
                var node4 = new DummyNode<string>("4");

                node1.AdjacentNodes = new[] { node2, };
                node2.AdjacentNodes = new[] { node1, };
                node3.AdjacentNodes = new[] { node4, };
                node4.AdjacentNodes = new[] { node3, };

                var nodes = new[]
                {
                    node1,
                    node2,
                    node3,
                    node4,
                };

                var sut = CreateSystemUnderTest();

                // Act
                var cycles = sut.DetectCycles(nodes);

                // Assert
                Assert.Equal(2, cycles.Count);

                var cycle1 = cycles.First();
                Assert.Equal(2, cycle1.Nodes.Count);
                Assert.Single(cycle1.Nodes, n => n == node1.Value);
                Assert.Single(cycle1.Nodes, n => n == node2.Value);

                var cycle2 = cycles.Last();
                Assert.Equal(2, cycle2.Nodes.Count);
                Assert.Single(cycle2.Nodes, n => n == node3.Value);
                Assert.Single(cycle2.Nodes, n => n == node4.Value);
            }

            [Fact]
            public void ReturnsStronglyConnectedCyclesWhenOverlappingCyclesOfNodesExist()
            {
                // Arrange
                var node1 = new DummyNode<string>("1");
                var node2 = new DummyNode<string>("2");
                var node3 = new DummyNode<string>("3");
                var node4 = new DummyNode<string>("4");
                var node5 = new DummyNode<string>("5");
                var node6 = new DummyNode<string>("6");
                var node7 = new DummyNode<string>("7");
                var node8 = new DummyNode<string>("8");

                node1.AdjacentNodes = new[] { node2, };
                node2.AdjacentNodes = new[] { node3, };
                node3.AdjacentNodes = new[] { node1, };
                node4.AdjacentNodes = new[] { node2, node3, node5, };
                node5.AdjacentNodes = new[] { node4, node6, };
                node6.AdjacentNodes = new[] { node3, node7, };
                node7.AdjacentNodes = new[] { node6, };
                node8.AdjacentNodes = new[] { node5, node7, };

                var nodes = new[]
                {
                    node1,
                    node2,
                    node3,
                    node4,
                    node5,
                    node6,
                    node7,
                    node8,
                };

                var sut = CreateSystemUnderTest();

                // Act
                var cycles = sut.DetectCycles(nodes);

                // Assert
                Assert.Equal(3, cycles.Count);

                var cycle1 = cycles.ElementAt(0);
                Assert.Equal(3, cycle1.Nodes.Count);
                Assert.Single(cycle1.Nodes, n => n == node1.Value);
                Assert.Single(cycle1.Nodes, n => n == node2.Value);
                Assert.Single(cycle1.Nodes, n => n == node3.Value);

                var cycle2 = cycles.ElementAt(1);
                Assert.Equal(2, cycle2.Nodes.Count);
                Assert.Single(cycle2.Nodes, n => n == node6.Value);
                Assert.Single(cycle2.Nodes, n => n == node7.Value);

                var cycle3 = cycles.ElementAt(2);
                Assert.Equal(2, cycle3.Nodes.Count);
                Assert.Single(cycle3.Nodes, n => n == node4.Value);
                Assert.Single(cycle3.Nodes, n => n == node5.Value);
            }

            [Fact]
            public void ReturnsEmptyCycleCollectionWhenNoCycleOfNodesExists()
            {
                // Arrange
                var node1 = new DummyNode<string>("1");
                var node2 = new DummyNode<string>("2");
                var node3 = new DummyNode<string>("3");
                var node4 = new DummyNode<string>("4");

                node1.AdjacentNodes = new[] { node2, };
                node2.AdjacentNodes = new[] { node3, };
                node3.AdjacentNodes = new[] { node4, };
                node4.AdjacentNodes = Array.Empty<INode<string>>();

                var nodes = new[]
                {
                    node1,
                    node2,
                    node3,
                    node4,
                };

                var sut = CreateSystemUnderTest();

                // Act
                var cycles = sut.DetectCycles(nodes);

                // Assert
                Assert.Empty(cycles);
            }
        }
    }
}
