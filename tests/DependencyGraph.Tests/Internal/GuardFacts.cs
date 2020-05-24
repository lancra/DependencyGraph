using System;
using LanceC.DependencyGraph.Internal;
using Xunit;

namespace LanceC.DependencyGraph.Facts.Internal
{
    public class GuardFacts
    {
        public class TheNotEmptyMethod : GuardFacts
        {
            [Theory]
            [InlineData("", "", typeof(ArgumentException))]
            [InlineData("", "value", typeof(ArgumentException))]
            [InlineData(null, null, typeof(ArgumentNullException))]
            [InlineData(null, "value", typeof(ArgumentNullException))]
            public void ThrowsExceptionOfProvidedTypeForInvalidParameters(string value, string parameterName, Type exceptionType)
            {
                // Act
                var exception = Record.Exception(() => Guard.NotEmpty(value, parameterName));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType(exceptionType, exception);
            }

            [Theory]
            [InlineData("foo", "value")]
            [InlineData("foo", null)]
            [InlineData("foo", "")]
            public void DoesNotThrowExceptionForValidParameters(string value, string parameterName)
            {
                // Act
                var exception = Record.Exception(() => Guard.NotEmpty(value, parameterName));

                // Assert
                Assert.Null(exception);
            }
        }

        public class TheNotNullMethod : GuardFacts
        {
            [Theory]
            [InlineData(null, "", typeof(ArgumentException))]
            [InlineData(null, null, typeof(ArgumentNullException))]
            [InlineData(null, "value", typeof(ArgumentNullException))]
            public void ThrowsExceptionOfProvidedTypeForInvalidParameters(object value, string parameterName, Type exceptionType)
            {
                // Act
                var exception = Record.Exception(() => Guard.NotNull(value, parameterName));

                // Assert
                Assert.NotNull(exception);
                Assert.IsType(exceptionType, exception);
            }

            [Theory]
            [InlineData("foo", "value")]
            [InlineData("foo", null)]
            [InlineData("foo", "")]
            [InlineData(1, "value")]
            [InlineData(1, null)]
            [InlineData(1, "")]
            [InlineData(1.2, "value")]
            [InlineData(1.2, null)]
            [InlineData(1.2, "")]
            [InlineData('x', "value")]
            [InlineData('x', null)]
            [InlineData('x', "")]
            public void DoesNotThrowExceptionForValidParameters(object value, string parameterName)
            {
                // Act
                var exception = Record.Exception(() => Guard.NotNull(value, parameterName));

                // Assert
                Assert.Null(exception);
            }
        }
    }
}
