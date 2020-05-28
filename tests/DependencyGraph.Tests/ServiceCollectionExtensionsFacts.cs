using LanceC.DependencyGraph.Internal.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LanceC.DependencyGraph.Facts
{
    public class ServiceCollectionExtensionsFacts
    {
        public class TheAddDependencyGraphMethod : ServiceCollectionExtensionsFacts
        {
            [Fact]
            public void RegistersCycleDetector()
            {
                // Arrange
                var services = new ServiceCollection();

                // Act
                services.AddDependencyGraph();

                // Assert
                var serviceProvider = services.BuildServiceProvider();
                var cycleDetector = serviceProvider.GetService<ICycleDetector<string>>();
                Assert.NotNull(cycleDetector);
            }

            [Fact]
            public void RegistersDependencyExecutionSorter()
            {
                // Arrange
                var services = new ServiceCollection();

                // Act
                services.AddDependencyGraph();

                // Assert
                var serviceProvider = services.BuildServiceProvider();
                var dependencyExecutionSorter = serviceProvider.GetService<IDependencyExecutionSorter<string>>();
                Assert.NotNull(dependencyExecutionSorter);
            }

            [Fact]
            public void RegistersGraphFactory()
            {
                // Arrange
                var services = new ServiceCollection();

                // Act
                services.AddDependencyGraph();

                // Assert
                var serviceProvider = services.BuildServiceProvider();
                var graphFactory = serviceProvider.GetService<IGraphFactory<string>>();
                Assert.NotNull(graphFactory);
            }

            [Fact]
            public void RegistersDependencyExecutionEngineOfOneType()
            {
                // Arrange
                var services = new ServiceCollection();

                // Act
                services.AddDependencyGraph();

                // Assert
                var serviceProvider = services.BuildServiceProvider();
                var dependencyExecutionEngine = serviceProvider.GetService<IDependencyExecutionEngine<string>>();
                Assert.NotNull(dependencyExecutionEngine);
            }

            [Fact]
            public void RegistersDependencyExecutionEngineOfTwoTypes()
            {
                // Arrange
                var services = new ServiceCollection();

                // Act
                services.AddDependencyGraph();

                // Assert
                var serviceProvider = services.BuildServiceProvider();
                var dependencyExecutionEngine = serviceProvider.GetService<IDependencyExecutionEngine<string, string>>();
                Assert.NotNull(dependencyExecutionEngine);
            }

            [Fact]
            public void RegistersDependencyExecutionEngineOfThreeTypes()
            {
                // Arrange
                var services = new ServiceCollection();

                // Act
                services.AddDependencyGraph();

                // Assert
                var serviceProvider = services.BuildServiceProvider();
                var dependencyExecutionEngine = serviceProvider.GetService<IDependencyExecutionEngine<string, string, string>>();
                Assert.NotNull(dependencyExecutionEngine);
            }
        }
    }
}
