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
    public class DependencyExecutionEngine_3Facts
    {
        private readonly AutoMocker _mocker = new AutoMocker();

        private DependencyExecutionEngine<string, string, string> CreateSystemUnderTest()
            => _mocker.CreateInstance<DependencyExecutionEngine<string, string, string>>();

        private Mock<IDependencyExecution<TKey, TContext, TResult>> MockDependencyExecution<TKey, TContext, TResult>(
            TKey key,
            TContext context,
            TResult result)
            where TKey : IEquatable<TKey>
        {
            var dependencyExecutionMock = new Mock<IDependencyExecution<TKey, TContext, TResult>>();
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.Key)
                .Returns(key);
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.DependentKeys)
                .Returns(Array.Empty<TKey>());
            dependencyExecutionMock
                .Setup(dependencyExecution => dependencyExecution.Execute(
                    context,
                    It.IsAny<ExecutionResultCollection<TKey, TResult>>(),
                    default))
                .ReturnsAsync(result);

            return dependencyExecutionMock;
        }

        public class TheExecuteAllMethod : DependencyExecutionEngine_3Facts
        {
            [Fact]
            public async Task RunsExecutionsInOrder()
            {
                // Arrange
                var context = "foo";
                var executionMock1 = MockDependencyExecution("1", context, "One");
                var executionMock2 = MockDependencyExecution("2", context, "Two");
                var executionMock3 = MockDependencyExecution("3", context, "Three");

                var sequenceVerifier = new SequenceVerifier();
                executionMock3
                    .InSequence(sequenceVerifier.Sequence)
                    .Setup(execution => execution.Execute(
                        context,
                        It.Is<ExecutionResultCollection<string, string>>(results => results.Values.Count == 0),
                        default))
                    .ReturnsAsync("Three")
                    .Callback(sequenceVerifier.NextCallback());
                executionMock2
                    .InSequence(sequenceVerifier.Sequence)
                    .Setup(execution => execution.Execute(
                        context,
                        It.Is<ExecutionResultCollection<string, string>>(results =>
                            results.Values.Count == 1 &&
                            results.Values.Any(result => result.Key == "3")),
                        default))
                    .ReturnsAsync("Two")
                    .Callback(sequenceVerifier.NextCallback());
                executionMock1
                    .InSequence(sequenceVerifier.Sequence)
                    .Setup(execution => execution.Execute(
                        context,
                        It.Is<ExecutionResultCollection<string, string>>(results =>
                            results.Values.Count == 2 &&
                            results.Values.Any(result => result.Key == "3") &&
                            results.Values.Any(result => result.Key == "2")),
                        default))
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
                await sut.ExecuteAll(context, executions, default);

                // Assert
                executionMock1.Verify(execution => execution.Execute(
                    context,
                    It.IsAny<ExecutionResultCollection<string, string>>(),
                    default));
                executionMock2.Verify(execution => execution.Execute(
                    context,
                    It.IsAny<ExecutionResultCollection<string, string>>(),
                    default));
                executionMock3.Verify(execution => execution.Execute(
                    context,
                    It.IsAny<ExecutionResultCollection<string, string>>(),
                    default));
                sequenceVerifier.VerifyAll();
            }

            [Fact]
            public async Task ReturnsExpectedExecutionResults()
            {
                // Arrange
                var context = "foo";
                var executionMock1 = MockDependencyExecution("1", context, "One");
                var executionMock2 = MockDependencyExecution("2", context, "Two");
                var executionMock3 = MockDependencyExecution("3", context, "Three");

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
                var results = await sut.ExecuteAll(context, executions, default);

                // Assert
                Assert.Equal(3, results.Values.Count);

                var firstResult = results.Values.ElementAt(0);
                Assert.Equal(executionMock3.Object.Key, firstResult.Key);
                Assert.Equal("Three", firstResult.Result);

                var secondResult = results.Values.ElementAt(1);
                Assert.Equal(executionMock2.Object.Key, secondResult.Key);
                Assert.Equal("Two", secondResult.Result);

                var thirdResult = results.Values.ElementAt(2);
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
                var executions = Array.Empty<IDependencyExecution<string, string, string>>();

                _mocker.GetMock<IDependencyExecutionSorter<string>>()
                    .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                    .Throws(new DuplicateKeyException<string>(Array.Empty<string>()));

                var sut = CreateSystemUnderTest();

                // Act
                var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll("foo", executions, default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<DuplicateKeyException<string>>(exception);
            }

            [Fact]
            public async Task ThrowsCircularDependenciesExceptionWhenDependencyExecutionSorterThrowsCircularDependenciesException()
            {
                // Arrange
                var executions = Array.Empty<IDependencyExecution<string, string, string>>();

                _mocker.GetMock<IDependencyExecutionSorter<string>>()
                    .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                    .Throws(new CircularDependenciesException(Array.Empty<string>()));

                var sut = CreateSystemUnderTest();

                // Act
                var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll("foo", executions, default));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType<CircularDependenciesException>(exception);
            }
        }
    }
}
