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
    public class DependencyExecutionSorterFacts
    {
        private readonly AutoMocker _mocker = new AutoMocker();

        private DependencyExecutionSorter<string> CreateSystemUnderTest()
            => _mocker.CreateInstance<DependencyExecutionSorter<string>>();

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

        public class TheSortMethod : DependencyExecutionSorterFacts
        {
            [Fact]
            public void AddsEdgesToGraphForExecutionsWithDependencies()
            {
                // Arrange
                var executionMock1 = MockDependencyExecution("1");
                var executionMock2 = MockDependencyExecution("2");
                var executionMock3 = MockDependencyExecution("3");

                executionMock1
                    .SetupGet(execution => execution.DependentKeys)
                    .Returns(new[] { executionMock2.Object.Key, executionMock3.Object.Key, });
                executionMock2
                    .SetupGet(execution => execution.DependentKeys)
                    .Returns(new[] { executionMock3.Object.Key, });

                var graphMock = _mocker.GetMock<IGraph<string>>();
                graphMock
                    .Setup(graph => graph.TopologicalSort())
                    .Returns(new[]
                    {
                    executionMock3.Object.Key,
                    executionMock2.Object.Key,
                    executionMock1.Object.Key,
                    });

                _mocker.GetMock<IGraphFactory<string>>()
                    .Setup(graphFactory => graphFactory.Create())
                    .Returns(graphMock.Object);

                var executions = new[]
                {
                    executionMock1.Object,
                    executionMock2.Object,
                    executionMock3.Object,
                };
                var sut = CreateSystemUnderTest();

                // Act
                sut.Sort(executions);

                // Assert
                graphMock.Verify(graph => graph.AddEdge(executionMock1.Object.Key, executionMock2.Object.Key));
                graphMock.Verify(graph => graph.AddEdge(executionMock1.Object.Key, executionMock3.Object.Key));
                graphMock.Verify(graph => graph.AddEdge(executionMock2.Object.Key, executionMock3.Object.Key));
            }

            [Fact]
            public void ReturnsExecutionKeysInOrderForExecutionsWithDependencies()
            {
                // Arrange
                var executionMock1 = MockDependencyExecution("1");
                var executionMock2 = MockDependencyExecution("2");
                var executionMock3 = MockDependencyExecution("3");

                executionMock1
                    .SetupGet(execution => execution.DependentKeys)
                    .Returns(new[] { executionMock2.Object.Key, executionMock3.Object.Key, });
                executionMock2
                    .SetupGet(execution => execution.DependentKeys)
                    .Returns(new[] { executionMock3.Object.Key, });

                var graphMock = _mocker.GetMock<IGraph<string>>();
                graphMock
                    .Setup(graph => graph.TopologicalSort())
                    .Returns(new[]
                    {
                    executionMock3.Object.Key,
                    executionMock2.Object.Key,
                    executionMock1.Object.Key,
                    });

                _mocker.GetMock<IGraphFactory<string>>()
                    .Setup(graphFactory => graphFactory.Create())
                    .Returns(graphMock.Object);

                var executions = new[]
                {
                    executionMock1.Object,
                    executionMock2.Object,
                    executionMock3.Object,
                };
                var sut = CreateSystemUnderTest();

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
            public void RunsExecutionsInOrderForExecutionsWithoutDependencies()
            {
                // Arrange
                var executionMock1 = MockDependencyExecution("1");
                var executionMock2 = MockDependencyExecution("2");
                var executionMock3 = MockDependencyExecution("3");

                var graphMock = _mocker.GetMock<IGraph<string>>();
                graphMock
                    .Setup(graph => graph.TopologicalSort())
                    .Returns(new[]
                    {
                    executionMock3.Object.Key,
                    executionMock2.Object.Key,
                    executionMock1.Object.Key,
                    });

                _mocker.GetMock<IGraphFactory<string>>()
                    .Setup(graphFactory => graphFactory.Create())
                    .Returns(graphMock.Object);

                var executions = new[]
                {
                    executionMock1.Object,
                    executionMock2.Object,
                    executionMock3.Object,
                };
                var sut = CreateSystemUnderTest();

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
            public void ThrowsArgumentNullExceptionWhenExecutionCollectionIsNull()
            {
                // Arrange
                var sut = CreateSystemUnderTest();

                // Act
                var exception = Record.Exception(() => sut.Sort(default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
            }

            [Fact]
            public void ThrowsDuplicateKeyExceptionWhenExecutionsHasDuplicateKeys()
            {
                // Arrange
                var key = "1";
                var executions = new[]
                {
                    MockDependencyExecution(key).Object,
                    MockDependencyExecution(key).Object,
                };

                var sut = CreateSystemUnderTest();

                // Act
                var exception = Record.Exception(() => sut.Sort(executions));

                // Assert
                Assert.NotNull(exception);
                var duplicateKeyException = Assert.IsType<DuplicateKeyException<string>>(exception);
                var duplicateKey = Assert.Single(duplicateKeyException.Keys);
                Assert.Equal(key, duplicateKey);
            }

            [Fact]
            public void ThrowsCircularDependenciesExceptionWhenTopologicalSortThrowsCircularDependenciesException()
            {
                // Arrange
                var executions = Array.Empty<IDependencyExecution<string>>();

                var graphMock = _mocker.GetMock<IGraph<string>>();
                graphMock
                    .Setup(graph => graph.TopologicalSort())
                    .Throws(new CircularDependenciesException(Array.Empty<string>()));

                _mocker.GetMock<IGraphFactory<string>>()
                    .Setup(graphFactory => graphFactory.Create())
                    .Returns(graphMock.Object);

                var sut = CreateSystemUnderTest();

                // Act
                var exception = Record.Exception(() => sut.Sort(executions));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<CircularDependenciesException>(exception);
            }
        }
    }
}
