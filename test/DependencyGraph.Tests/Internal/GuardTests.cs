// -------------------------------------------------------------------------------------------------
// <copyright file="GuardTests.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using DependencyGraph.Internal;
using Xunit;

namespace DependencyGraph.Tests.Internal
{
    public class GuardTests
    {
        [Theory]
        [InlineData("", "", typeof(ArgumentException))]
        [InlineData("", "value", typeof(ArgumentException))]
        [InlineData(null, null, typeof(ArgumentNullException))]
        [InlineData(null, "value", typeof(ArgumentNullException))]
        public void NotEmpty_ForInvalidParameters_ThrowsExceptionOfProvidedType(string value, string parameterName, Type exceptionType)
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
        public void NotEmpty_ForValidParameters_DoesNotThrowException(string value, string parameterName)
        {
            // Act
            var exception = Record.Exception(() => Guard.NotEmpty(value, parameterName));

            // Assert
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null, "", typeof(ArgumentException))]
        [InlineData(null, null, typeof(ArgumentNullException))]
        [InlineData(null, "value", typeof(ArgumentNullException))]
        public void NotNull_ForInvalidParameters_ThrowsExceptionOfProvidedType(object value, string parameterName, Type exceptionType)
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
        public void NotNull_ForValidParameters_DoesNotThrowException(object value, string parameterName)
        {
            // Act
            var exception = Record.Exception(() => Guard.NotNull(value, parameterName));

            // Assert
            Assert.Null(exception);
        }
    }
}
