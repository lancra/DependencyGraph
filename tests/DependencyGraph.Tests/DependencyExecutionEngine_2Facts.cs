using System;
using System.Linq;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Exceptions;
using LanceC.DependencyGraph.Facts.Testing;
using LanceC.DependencyGraph.Internal.Abstractions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Facts
{
    public class DependencyExecutionEngine_2Facts
    {
        private readonly AutoMocker _mocker = new AutoMocker();

        private DependencyExecutionEngine<string, string> CreateSystemUnderTest()
            => _mocker.CreateInstance<DependencyExecutionEngine<string, string>>();

        private Mock<IDependencyExecution<TKey, TResult>> MockDependencyExecution<TKey, TResult>(TKey key, TResult result)
            where TKey : IEquatable<TKey>
        {
            var dependencyExecutionMock = new Mock<IDependencyExecution<TKey, TResult>>();
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.Key)
                .Returns(key);
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.DependentKeys)
                .Returns(Array.Empty<TKey>());
            dependencyExecutionMock
                .Setup(dependencyExecution => dependencyExecution.Execute(default))
                .ReturnsAsync(result);

            return dependencyExecutionMock;
        }

        public class TheExecuteAllMethod : DependencyExecutionEngine_2Facts
        {
            [Fact]
            public async Task RunsExecutionsInOrder()
            {
                // Arrange
                var executionMock1 = MockDependencyExecution("1", "One");
                var executionMock2 = MockDependencyExecution("2", "Two");
                var executionMock3 = MockDependencyExecution("3", "Three");

                var sequenceVerifier = new SequenceVerifier();
                executionMock3
                    .InSequence(sequenceVerifier.Sequence)
                    .Setup(execution => execution.Execute(default))
                    .ReturnsAsync("Three")
                    .Callback(sequenceVerifier.NextCallback());
                executionMock2
                    .InSequence(sequenceVerifier.Sequence)
                    .Setup(execution => execution.Execute(default))
                    .ReturnsAsync("Two")
                    .Callback(sequenceVerifier.NextCallback());
                executionMock1
                    .InSequence(sequenceVerifier.Sequence)
                    .Setup(execution => execution.Execute(default))
                    .ReturnsAsync("One")
                    .Callback(sequenceVerifier.NextCallback());

                var executions = new[]
                {
                    executionMock3.Object,
                    executionMock2.Object,
                    executionMock1.Object,
                };

                var keys = new[]
                {
                    executionMock3.Object.Key,
                    executionMock2.Object.Key,
                    executionMock1.Object.Key,
                };
                _mocker.GetMock<IDependencyExecutionSorter<string>>()
                    .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                    .Returns(keys);

                var sut = CreateSystemUnderTest();

                // Act
                await sut.ExecuteAll(executions, default);

                // Assert
                executionMock1.Verify(execution => execution.Execute(default));
                executionMock2.Verify(execution => execution.Execute(default));
                executionMock3.Verify(execution => execution.Execute(default));
                sequenceVerifier.VerifyAll();
            }

            [Fact]
            public async Task ReturnsExpectedExecutionResults()
            {
                // Arrange
                var executionMock1 = MockDependencyExecution("1", "One");
                var executionMock2 = MockDependencyExecution("2", "Two");
                var executionMock3 = MockDependencyExecution("3", "Three");

                var executions = new[]
                {
                    executionMock3.Object,
                    executionMock2.Object,
                    executionMock1.Object,
                };

                var keys = new[]
                {
                    executionMock3.Object.Key,
                    executionMock2.Object.Key,
                    executionMock1.Object.Key,
                };
                _mocker.GetMock<IDependencyExecutionSorter<string>>()
                    .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                    .Returns(keys);

                var sut = CreateSystemUnderTest();

                // Act
                var results = await sut.ExecuteAll(executions, default);

                // Assert
                Assert.Equal(3, results.Count);

                var firstResult = results.ElementAt(0);
                Assert.Equal(executionMock3.Object.Key, firstResult.Key);
                Assert.Equal("Three", firstResult.Result);

                var secondResult = results.ElementAt(1);
                Assert.Equal(executionMock2.Object.Key, secondResult.Key);
                Assert.Equal("Two", secondResult.Result);

                var thirdResult = results.ElementAt(2);
                Assert.Equal(executionMock1.Object.Key, thirdResult.Key);
                Assert.Equal("One", thirdResult.Result);
            }

            [Fact]
            public async Task ThrowsArgumentNullExceptionWhenExecutionCollectionIsNull()
            {
                // Arrange
                var sut = CreateSystemUnderTest();

                // Act
                var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll(default, default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<ArgumentNullException>(exception);
            }

            [Fact]
            public async Task ThrowsDuplicateKeyExceptionWhenDependencyExecutionSorterThrowsDuplicateKeyException()
            {
                // Arrange
                var executions = Array.Empty<IDependencyExecution<string, string>>();

                _mocker.GetMock<IDependencyExecutionSorter<string>>()
                    .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                    .Throws(new DuplicateKeyException<string>(Array.Empty<string>()));

                var sut = CreateSystemUnderTest();

                // Act
                var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll(executions, default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<DuplicateKeyException<string>>(exception);
            }

            [Fact]
            public async Task ThrowsCircularDependenciesExceptionWhenDependencyExecutionSorterThrowsCircularDependenciesException()
            {
                // Arrange
                var executions = Array.Empty<IDependencyExecution<string, string>>();

                _mocker.GetMock<IDependencyExecutionSorter<string>>()
                    .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                    .Throws(new CircularDependenciesException(Array.Empty<string>()));

                var sut = CreateSystemUnderTest();

                // Act
                var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll(executions, default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<CircularDependenciesException>(exception);
            }
        }
    }
}
