// -------------------------------------------------------------------------------------------------
// <copyright file="DependencyExecutionEngine_2Tests.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Exceptions;
using LanceC.DependencyGraph.Internal.Abstractions;
using LanceC.DependencyGraph.Tests.Testing;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Tests
{
    public class DependencyExecutionEngine_2Tests
    {
        [Fact]
        public async Task ExecuteAll_ForProvidedExecutions_RunsExecutionsInOrder()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

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
            mocker.GetMock<IDependencyExecutionSorter<string>>()
                .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                .Returns(keys);

            var sut = mocker.CreateInstance<DependencyExecutionEngine<string, string>>();

            // Act
            await sut.ExecuteAll(executions, default);

            // Assert
            executionMock1.Verify(execution => execution.Execute(default));
            executionMock2.Verify(execution => execution.Execute(default));
            executionMock3.Verify(execution => execution.Execute(default));
            sequenceVerifier.VerifyAll();
        }

        [Fact]
        public async Task ExecuteAll_ForProvidedExecutions_ReturnsExpectedResults()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

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
            mocker.GetMock<IDependencyExecutionSorter<string>>()
                .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                .Returns(keys);

            var sut = mocker.CreateInstance<DependencyExecutionEngine<string, string>>();

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
        public async Task ExecuteAll_ExecutionCollectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);
            var sut = mocker.CreateInstance<DependencyExecutionEngine<string, string>>();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll(default, default));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task ExecuteAll_DependencyExecutionSorterThrowsDuplicateKeyException_ThrowsDuplicateKeyException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var executions = Array.Empty<IDependencyExecution<string, string>>();

            mocker.GetMock<IDependencyExecutionSorter<string>>()
                .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                .Throws(new DuplicateKeyException<string>(Array.Empty<string>()));

            var sut = mocker.CreateInstance<DependencyExecutionEngine<string, string>>();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll(executions, default));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<DuplicateKeyException<string>>(exception);
        }

        [Fact]
        public async Task ExecuteAll_DependencyExecutionSorterThrowsCircularDependenciesException_ThrowsCircularDependenciesException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var executions = Array.Empty<IDependencyExecution<string, string>>();

            mocker.GetMock<IDependencyExecutionSorter<string>>()
                .Setup(dependencyExecutionSorter => dependencyExecutionSorter.Sort(executions))
                .Throws(new CircularDependenciesException(Array.Empty<string>()));

            var sut = mocker.CreateInstance<DependencyExecutionEngine<string, string>>();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll(executions, default));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<CircularDependenciesException>(exception);
        }

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
    }
}
