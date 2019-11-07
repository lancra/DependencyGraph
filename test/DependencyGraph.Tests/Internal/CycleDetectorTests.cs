// -------------------------------------------------------------------------------------------------
// <copyright file="CycleDetectorTests.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using LanceC.DependencyGraph.Internal;
using LanceC.DependencyGraph.Internal.Abstractions;
using LanceC.DependencyGraph.Tests.Testing;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Tests.Internal
{
    public class CycleDetectorTests
    {
        [Fact]
        public void DetectCycles_NodesIsNull_ThrowArgumentNullException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var sut = mocker.CreateInstance<CycleDetector<string>>();

            // Act
            var exception = Record.Exception(() => sut.DetectCycles(default));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void DetectCycles_NodesIsEmpty_ReturnsEmptyCycles()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var sut = mocker.CreateInstance<CycleDetector<string>>();

            // Act
            var cycles = sut.DetectCycles(new DummyNode<string>[0]);

            // Assert
            Assert.Empty(cycles);
        }

        [Fact]
        public void DetectCycles_HasCycleOfNodes_ReturnsCycles()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

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

            var sut = mocker.CreateInstance<CycleDetector<string>>();

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
        public void DetectCycles_HasMultipleCyclesOfNodes_ReturnsCycles()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

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

            var sut = mocker.CreateInstance<CycleDetector<string>>();

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
        public void DetectCycles_HasOverlappingCyclesOfNodes_ReturnsStronglyConnectedCycles()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

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

            var sut = mocker.CreateInstance<CycleDetector<string>>();

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
        public void DetectCycles_DoesNotHaveCycleOfNodes_ReturnsEmptyCycles()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var node1 = new DummyNode<string>("1");
            var node2 = new DummyNode<string>("2");
            var node3 = new DummyNode<string>("3");
            var node4 = new DummyNode<string>("4");

            node1.AdjacentNodes = new[] { node2, };
            node2.AdjacentNodes = new[] { node3, };
            node3.AdjacentNodes = new[] { node4, };
            node4.AdjacentNodes = new INode<string>[0];

            var nodes = new[]
            {
                node1,
                node2,
                node3,
                node4,
            };

            var sut = mocker.CreateInstance<CycleDetector<string>>();

            // Act
            var cycles = sut.DetectCycles(nodes);

            // Assert
            Assert.Empty(cycles);
        }
    }
}
