// -------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="LanceC">
// Copyright (c) LanceC. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------------------------

using DependencyGraph.Internal;
using DependencyGraph.Internal.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyGraph
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyGraph(this IServiceCollection services)
            => services
            .AddTransient(typeof(ICycleDetector<>), typeof(CycleDetector<>))
            .AddTransient(typeof(IGraphFactory<>), typeof(GraphFactory<>))
            .AddTransient(typeof(IDependencyExecutionEngine<>), typeof(DependencyExecutionEngine<>));
    }
}
