using LanceC.DependencyGraph.Internal;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Facts.Internal
{
    public class GraphFactoryFacts
    {
        private readonly AutoMocker _mocker = new AutoMocker();

        private GraphFactory<string> CreateSystemUnderTest()
            => _mocker.CreateInstance<GraphFactory<string>>();

        public class TheCreateMethod : GraphFactoryFacts
        {
            [Fact]
            public void ReturnsNewGraph()
            {
                // Arrange
                var sut = CreateSystemUnderTest();

                // Act
                var graph = sut.Create();

                // Assert
                Assert.Empty(graph.Nodes);
            }
        }
    }
}
