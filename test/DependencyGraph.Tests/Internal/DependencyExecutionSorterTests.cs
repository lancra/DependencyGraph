using System;
using System.Linq;
using LanceC.DependencyGraph.Exceptions;
using LanceC.DependencyGraph.Internal;
using LanceC.DependencyGraph.Internal.Abstractions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Tests.Internal
{
    public class DependencyExecutionSorterTests
    {
        [Fact]
        public void Sort_ExecutionsWithDependencies_AddsEdgesToGraph()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var executionMock1 = MockDependencyExecution("1");
            var executionMock2 = MockDependencyExecution("2");
            var executionMock3 = MockDependencyExecution("3");

            executionMock1
                .SetupGet(execution => execution.DependentKeys)
                .Returns(new[] { executionMock2.Object.Key, executionMock3.Object.Key, });
            executionMock2
                .SetupGet(execution => execution.DependentKeys)
                .Returns(new[] { executionMock3.Object.Key, });

            var graphMock = mocker.GetMock<IGraph<string>>();
            graphMock
                .Setup(graph => graph.TopologicalSort())
                .Returns(new[]
                {
                    executionMock3.Object.Key,
                    executionMock2.Object.Key,
                    executionMock1.Object.Key,
                });

            mocker.GetMock<IGraphFactory<string>>()
                .Setup(graphFactory => graphFactory.Create())
                .Returns(graphMock.Object);

            var executions = new[]
            {
                executionMock1.Object,
                executionMock2.Object,
                executionMock3.Object,
            };
            var sut = mocker.CreateInstance<DependencyExecutionSorter<string>>();

            // Act
            sut.Sort(executions);

            // Assert
            graphMock.Verify(graph => graph.AddEdge(executionMock1.Object.Key, executionMock2.Object.Key));
            graphMock.Verify(graph => graph.AddEdge(executionMock1.Object.Key, executionMock3.Object.Key));
            graphMock.Verify(graph => graph.AddEdge(executionMock2.Object.Key, executionMock3.Object.Key));
        }

        [Fact]
        public void Sort_ExecutionsWithDependencies_ReturnsExecutionKeysInOrder()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var executionMock1 = MockDependencyExecution("1");
            var executionMock2 = MockDependencyExecution("2");
            var executionMock3 = MockDependencyExecution("3");

            executionMock1
                .SetupGet(execution => execution.DependentKeys)
                .Returns(new[] { executionMock2.Object.Key, executionMock3.Object.Key, });
            executionMock2
                .SetupGet(execution => execution.DependentKeys)
                .Returns(new[] { executionMock3.Object.Key, });

            var graphMock = mocker.GetMock<IGraph<string>>();
            graphMock
                .Setup(graph => graph.TopologicalSort())
                .Returns(new[]
                {
                    executionMock3.Object.Key,
                    executionMock2.Object.Key,
                    executionMock1.Object.Key,
                });

            mocker.GetMock<IGraphFactory<string>>()
                .Setup(graphFactory => graphFactory.Create())
                .Returns(graphMock.Object);

            var executions = new[]
            {
                executionMock1.Object,
                executionMock2.Object,
                executionMock3.Object,
            };
            var sut = mocker.CreateInstance<DependencyExecutionSorter<string>>();

            // Act
            var keys = sut.Sort(executions);

            // Assert
            Assert.Equal(3, keys.Count);

            var firstKey = keys.ElementAt(0);
            Assert.Equal(executionMock3.Object.Key, firstKey);

            var secondKey = keys.ElementAt(1);
            Assert.Equal(executionMock2.Object.Key, secondKey);

            var thirdKey = keys.ElementAt(2);
            Assert.Equal(executionMock1.Object.Key, thirdKey);
        }

        [Fact]
        public void Sort_ExecutionsWithoutDependencies_RunsExecutionsInOrder()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var executionMock1 = MockDependencyExecution("1");
            var executionMock2 = MockDependencyExecution("2");
            var executionMock3 = MockDependencyExecution("3");

            var graphMock = mocker.GetMock<IGraph<string>>();
            graphMock
                .Setup(graph => graph.TopologicalSort())
                .Returns(new[]
                {
                    executionMock3.Object.Key,
                    executionMock2.Object.Key,
                    executionMock1.Object.Key,
                });

            mocker.GetMock<IGraphFactory<string>>()
                .Setup(graphFactory => graphFactory.Create())
                .Returns(graphMock.Object);

            var executions = new[]
            {
                executionMock1.Object,
                executionMock2.Object,
                executionMock3.Object,
            };
            var sut = mocker.CreateInstance<DependencyExecutionSorter<string>>();

            // Act
            var keys = sut.Sort(executions);

            // Assert
            Assert.Equal(3, keys.Count);

            var firstKey = keys.ElementAt(0);
            Assert.Equal(executionMock3.Object.Key, firstKey);

            var secondKey = keys.ElementAt(1);
            Assert.Equal(executionMock2.Object.Key, secondKey);

            var thirdKey = keys.ElementAt(2);
            Assert.Equal(executionMock1.Object.Key, thirdKey);
        }

        [Fact]
        public void Sort_ExecutionCollectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);
            var sut = mocker.CreateInstance<DependencyExecutionSorter<string>>();

            // Act
            var exception = Record.Exception(() => sut.Sort(default));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void Sort_ExecutionsHasDuplicateKeys_ThrowsDuplicateKeyException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var key = "1";
            var executions = new[]
            {
                MockDependencyExecution(key).Object,
                MockDependencyExecution(key).Object,
            };

            var sut = mocker.CreateInstance<DependencyExecutionSorter<string>>();

            // Act
            var exception = Record.Exception(() => sut.Sort(executions));

            // Assert
            Assert.NotNull(exception);
            var duplicateKeyException = Assert.IsType<DuplicateKeyException<string>>(exception);
            var duplicateKey = Assert.Single(duplicateKeyException.Keys);
            Assert.Equal(key, duplicateKey);
        }

        [Fact]
        public void Sort_TopologicalSortThrowsCircularDependenciesException_ThrowsCircularDependenciesException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var executions = Array.Empty<IDependencyExecution<string>>();

            var graphMock = mocker.GetMock<IGraph<string>>();
            graphMock
                .Setup(graph => graph.TopologicalSort())
                .Throws(new CircularDependenciesException(Array.Empty<string>()));

            mocker.GetMock<IGraphFactory<string>>()
                .Setup(graphFactory => graphFactory.Create())
                .Returns(graphMock.Object);

            var sut = mocker.CreateInstance<DependencyExecutionSorter<string>>();

            // Act
            var exception = Record.Exception(() => sut.Sort(executions));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<CircularDependenciesException>(exception);
        }

        private Mock<IDependencyExecution<TKey>> MockDependencyExecution<TKey>(TKey key)
            where TKey : IEquatable<TKey>
        {
            var dependencyExecutionMock = new Mock<IDependencyExecution<TKey>>();
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.Key)
                .Returns(key);
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.DependentKeys)
                .Returns(Array.Empty<TKey>());

            return dependencyExecutionMock;
        }
    }
}
