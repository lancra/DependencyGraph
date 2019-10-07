// -------------------------------------------------------------------------------------------------
// <copyright file="DependencyExecutionEngineTests.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using DependencyGraph.Internal.Abstractions;
using DependencyGraph.Tests.Testing;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace DependencyGraph.Tests
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
                .SetupGet(execution => execution.Dependencies)
                .Returns(new[] { executionMock2.Object, executionMock3.Object, });
            executionMock2
                .SetupGet(execution => execution.Dependencies)
                .Returns(new[] { executionMock3.Object, });

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
            await sut.ExecuteAll(executions);

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
                .SetupGet(execution => execution.Dependencies)
                .Returns(new[] { executionMock2.Object, executionMock3.Object, });
            executionMock2
                .SetupGet(execution => execution.Dependencies)
                .Returns(new[] { executionMock3.Object, });

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
                .Setup(execution => execution.Execute())
                .Returns(Task.CompletedTask)
                .Callback(sequenceVerifier.NextCallback());
            executionMock2
                .InSequence(sequenceVerifier.Sequence)
                .Setup(execution => execution.Execute())
                .Returns(Task.CompletedTask)
                .Callback(sequenceVerifier.NextCallback());
            executionMock1
                .InSequence(sequenceVerifier.Sequence)
                .Setup(execution => execution.Execute())
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
            await sut.ExecuteAll(executions);

            // Assert
            executionMock1.Verify(execution => execution.Execute());
            executionMock2.Verify(execution => execution.Execute());
            executionMock3.Verify(execution => execution.Execute());
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
                .Setup(execution => execution.Execute())
                .Returns(Task.CompletedTask)
                .Callback(sequenceVerifier.NextCallback());
            executionMock2
                .InSequence(sequenceVerifier.Sequence)
                .Setup(execution => execution.Execute())
                .Returns(Task.CompletedTask)
                .Callback(sequenceVerifier.NextCallback());
            executionMock1
                .InSequence(sequenceVerifier.Sequence)
                .Setup(execution => execution.Execute())
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
            await sut.ExecuteAll(executions);

            // Assert
            executionMock1.Verify(execution => execution.Execute());
            executionMock2.Verify(execution => execution.Execute());
            executionMock3.Verify(execution => execution.Execute());
            sequenceVerifier.VerifyAll();
        }

        private Mock<IDependencyExecution<T>> MockDependencyExecution<T>(T key)
            where T : IEquatable<T>
        {
            var dependencyExecutionMock = new Mock<IDependencyExecution<T>>();
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.Key)
                .Returns(key);
            dependencyExecutionMock
                .SetupGet(dependencyExecution => dependencyExecution.Dependencies)
                .Returns(new IDependencyExecution<T>[0]);

            return dependencyExecutionMock;
        }
    }
}
