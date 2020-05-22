using LanceC.DependencyGraph.Internal;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Tests.Internal
{
    public class GraphFactoryTests
    {
        [Fact]
        public void Create_WithNoParameters_ReturnsNewGraph()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);
            var sut = mocker.CreateInstance<GraphFactory<string>>();

            // Act
            var graph = sut.Create();

            // Assert
            Assert.Empty(graph.Nodes);
        }
    }
}
