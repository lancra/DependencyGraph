using System;
using LanceC.DependencyGraph.Internal;
using Xunit;

namespace LanceC.DependencyGraph.Tests.Internal
{
    public class NodeFacts
    {
        public class TheAddAdjacentNodeMethod : NodeFacts
        {
            [Fact]
            public void AddsAdjacentNodeForNewNode()
            {
                // Arrange
                var node = new Node<string>("bar");
                var sut = new Node<string>("foo");

                // Act
                sut.AddAdjacentNode(node);

                // Assert
                var adjacentNode = Assert.Single(sut.AdjacentNodes);
                Assert.Equal(node.Value, adjacentNode.Value);
            }

            [Fact]
            public void DoesNotAddSecondAdjacentNodeForExistingNode()
            {
                // Arrange
                var node = new Node<string>("bar");

                var sut = new Node<string>("foo");
                sut.AddAdjacentNode(new Node<string>("bar"));

                // Act
                sut.AddAdjacentNode(node);

                // Assert
                var adjacentNode = Assert.Single(sut.AdjacentNodes);
                Assert.Equal(node.Value, adjacentNode.Value);
            }

            [Fact]
            public void ThrowsInvalidOperationExceptionWhenValueOfDestinationNodeIsEqualToValueOfSourceNode()
            {
                // Arrange
                var node = new Node<string>("foo");
                var sut = new Node<string>("foo");

                // Act
                var exception = Record.Exception(() => sut.AddAdjacentNode(node));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void IncrementsInDegreeOfDestinationNodeForNewNode()
            {
                // Arrange
                var node = new Node<string>("bar");
                var sut = new Node<string>("foo");

                // Act
                sut.AddAdjacentNode(node);

                // Assert
                Assert.Equal(1, node.InDegree);
            }

            [Fact]
            public void DoesNotIncrementInDegreeOfDestinationNodeForExistingNode()
            {
                // Arrange
                var node = new Node<string>("bar");

                var sut = new Node<string>("foo");
                sut.AddAdjacentNode(new Node<string>("bar"));

                var inDegree = node.InDegree;

                // Act
                sut.AddAdjacentNode(node);

                // Assert
                Assert.Equal(inDegree, node.InDegree);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionForNullNode()
            {
                // Arrange
                var node = default(Node<string>);
                var sut = new Node<string>("foo");

                // Act
                var exception = Record.Exception(() => sut.AddAdjacentNode(node));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
            }
        }
    }
}
