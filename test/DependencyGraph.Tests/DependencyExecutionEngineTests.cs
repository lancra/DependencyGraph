// -------------------------------------------------------------------------------------------------
// <copyright file="DependencyExecutionEngineTests.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using LanceC.DependencyGraph.Internal.Abstractions;
using LanceC.DependencyGraph.Tests.Testing;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.DependencyGraph.Tests
{
    public class DependencyExecutionEngineTests
    {
        [Fact]
        public async Task ExecuteAll_ExecutionsWithDependencies_AddsEdgesToGraph()
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
            var sut = mocker.CreateInstance<DependencyExecutionEngine<string>>();

            // Act
            await sut.ExecuteAll(executions, default);

            // Assert
            graphMock.Verify(graph => graph.AddEdge(executionMock1.Object.Key, executionMock2.Object.Key));
            graphMock.Verify(graph => graph.AddEdge(executionMock1.Object.Key, executionMock3.Object.Key));
            graphMock.Verify(graph => graph.AddEdge(executionMock2.Object.Key, executionMock3.Object.Key));
        }

        [Fact]
        public async Task ExecuteAll_ExecutionsWithDependencies_RunsExecutionsInOrder()
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
                executionMock1.Object,
                executionMock2.Object,
                executionMock3.Object,
            };
            var sut = mocker.CreateInstance<DependencyExecutionEngine<string>>();

            // Act
            await sut.ExecuteAll(executions, default);

            // Assert
            executionMock1.Verify(execution => execution.Execute(default));
            executionMock2.Verify(execution => execution.Execute(default));
            executionMock3.Verify(execution => execution.Execute(default));
            sequenceVerifier.VerifyAll();
        }

        [Fact]
        public async Task ExecuteAll_ExecutionsWithoutDependencies_RunsExecutionsInOrder()
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
            var sut = mocker.CreateInstance<DependencyExecutionEngine<string>>();

            // Act
            await sut.ExecuteAll(executions, default);

            // Assert
            executionMock1.Verify(execution => execution.Execute(default));
            executionMock2.Verify(execution => execution.Execute(default));
            executionMock3.Verify(execution => execution.Execute(default));
            sequenceVerifier.VerifyAll();
        }

        [Fact]
        public async Task ExecuteAll_ExecutionCollectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);
            var sut = mocker.CreateInstance<DependencyExecutionEngine<string>>();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll(default, default));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task ExecuteAll_ExecutionsHasDuplicateKeys_ThrowsDuplicateKeyException()
        {
            // Arrange
            var mocker = new AutoMocker(MockBehavior.Loose);

            var key = "1";
            var executions = new[]
            {
                MockDependencyExecution(key).Object,
                MockDependencyExecution(key).Object,
            };

            var sut = mocker.CreateInstance<DependencyExecutionEngine<string>>();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll(executions, default));

            // Assert
            Assert.NotNull(exception);
            var duplicateKeyException = Assert.IsType<DuplicateKeyException<string>>(exception);
            var duplicateKey = Assert.Single(duplicateKeyException.Keys);
            Assert.Equal(key, duplicateKey);
        }

        [Fact]
        public async Task ExecuteAll_TopologicalSortThrowsCircularDependenciesException_ThrowsCircularDependenciesException()
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

            var sut = mocker.CreateInstance<DependencyExecutionEngine<string>>();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.ExecuteAll(executions, default));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<CircularDependenciesException>(exception);
        }

        private Mock<IDependencyExecution<T>> MockDependencyExecution<T>(T key)
            where T : IEquatable<T>
        {
            var dependencyExecutionMock = new Mock<IDependencyExecution<T>>();
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.Key)
                .Returns(key);
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.DependentKeys)
                .Returns(Array.Empty<T>());

            return dependencyExecutionMock;
        }
    }
}
