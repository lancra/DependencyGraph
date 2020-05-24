using System;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Exceptions;
using LanceC.DependencyGraph.Facts.Testing;
using LanceC.DependencyGraph.Internal.Abstractions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Facts
{
    public class DependencyExecutionEngine_1Facts
    {
        private readonly AutoMocker _mocker = new AutoMocker();

        private DependencyExecutionEngine<string> CreateSystemUnderTest()
            => _mocker.CreateInstance<DependencyExecutionEngine<string>>();

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

        public class TheExecuteAllMethod : DependencyExecutionEngine_1Facts
        {
            [Fact]
            public async Task RunsExecutionsInOrder()
            {
                // Arrange
                var executionMock1 = MockDependencyExecution("1");
                var executionMock2 = MockDependencyExecution("2");
                var executionMock3 = MockDependencyExecution("3");

                var sequenceVerifier = new SequenceVerifier();
                executionMock3
                    .InSequence(sequenceVerifier.Sequence)
                    .Setup(execution => execution.Execute(default))
                    .Returns(Task.CompletedTask)
                    .Callback(sequenceVerifier.NextCallback());
                executionMock2
                    .InSequence(sequenceVerifier.Sequence)
                    .Setup(execution => execution.Execute(default))
                    .Returns(Task.CompletedTask)
                    .Callback(sequenceVerifier.NextCallback());
                executionMock1
                    .InSequence(sequenceVerifier.Sequence)
                    .Setup(execution => execution.Execute(default))
                    .Returns(Task.CompletedTask)
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
                var executions = Array.Empty<IDependencyExecution<string>>();

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
                var executions = Array.Empty<IDependencyExecution<string>>();

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
