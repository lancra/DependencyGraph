// -------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using LanceC.DependencyGraph.Internal;
using LanceC.DependencyGraph.Internal.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LanceC.DependencyGraph
{
    /// <summary>
    /// Provides extensions for registering the dependency graph in a <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the dependency graph to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to modify.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddDependencyGraph(this IServiceCollection services)
            => services
            .AddTransient(typeof(ICycleDetector<>), typeof(CycleDetector<>))
            .AddTransient(typeof(IGraphFactory<>), typeof(GraphFactory<>))
            .AddTransient(typeof(IDependencyExecutionEngine<>), typeof(DependencyExecutionEngine<>));
    }
}
