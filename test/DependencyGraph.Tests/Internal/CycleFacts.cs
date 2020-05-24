using System.Collections.Generic;
using LanceC.DependencyGraph.Internal;
using LanceC.DependencyGraph.Internal.Abstractions;
using LanceC.DependencyGraph.Tests.Testing;
using Xunit;

namespace LanceC.DependencyGraph.Tests.Internal
{
    public class CycleFacts
    {
        public class TheToStringMethod : CycleFacts
        {
            public static IEnumerable<object[]> ReturnsTextForNodes_Data
                => new TheoryData<IEnumerable<INode<string>>, string>
                {
                    {
                        new[] { new DummyNode<string>("1"), },
                        "'1'"
                    },
                    {
                        new[]
                        {
                            new DummyNode<string>("1"),
                            new DummyNode<string>("2"),
                            new DummyNode<string>("3"),
                        },
                        "'1' -> '2' -> '3'"
                    },
                    {
                        new[]
                        {
                            new DummyNode<string>("3"),
                            new DummyNode<string>("2"),
                            new DummyNode<string>("1"),
                        },
                        "'3' -> '2' -> '1'"
                    },
                };

            [Theory]
            [MemberData(nameof(ReturnsTextForNodes_Data))]
            internal void ReturnsTextForNodes(IEnumerable<INode<string>> nodes, string expected)
            {
                // Arrange
                var sut = new Cycle<string>(nodes);

                // Act
                var actual = sut.ToString();

                // Assert
                Assert.Equal(expected, actual);
            }
        }
    }
}
