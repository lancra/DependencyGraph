using Xunit;

namespace LanceC.DependencyGraph.Facts
{
    public class ExecutionResultCollectionFacts
    {
        private ExecutionResultCollection<string, string> CreateSystemUnderTest()
            => new ExecutionResultCollection<string, string>();

        public class TheGetMethod : ExecutionResultCollectionFacts
        {
            [Fact]
            public void ReturnsExecutionResultWithSpecifiedKey()
            {
                // Arrange
                var sut = CreateSystemUnderTest();

                var expectedExecutionResult = new ExecutionResult<string, string>("1", "1");
                sut.Add(expectedExecutionResult);

                var notExpectedExecutionResult = new ExecutionResult<string, string>("2", "2");
                sut.Add(notExpectedExecutionResult);

                // Act
                var actualExecutionResult = sut.Get("1");

                // Assert
                Assert.Equal(expectedExecutionResult, actualExecutionResult);
            }

            [Fact]
            public void ReturnsNullWhenNoExecutionResultExistsForTheSpecifiedKey()
            {
                // Arrange
                var sut = CreateSystemUnderTest();

                sut.Add(new ExecutionResult<string, string>("1", "1"));
                sut.Add(new ExecutionResult<string, string>("2", "2"));

                // Act
                var executionResult = sut.Get("3");

                // Assert
                Assert.Null(executionResult);
            }
        }
    }
}
