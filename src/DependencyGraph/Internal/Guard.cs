// -------------------------------------------------------------------------------------------------
// <copyright file="Guard.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;

namespace DependencyGraph.Internal
{
    internal static class Guard
    {
        public static void NotEmpty(string value, string parameterName)
        {
            var e = default(Exception);
            if (value is null)
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException($"The string argument '{parameterName}' cannot be empty.");
            }

            if (!(e is null))
            {
                NotEmpty(parameterName, nameof(parameterName));
                throw e;
            }
        }

        public static void NotNull(object value, string parameterName)
        {
            if (value is null)
            {
                NotEmpty(parameterName, nameof(parameterName));
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
