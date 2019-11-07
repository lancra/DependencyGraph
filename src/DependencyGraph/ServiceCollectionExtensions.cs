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
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection DependencyGraph(this IServiceCollection services)
            => services
            .AddTransient(typeof(ICycleDetector<>), typeof(CycleDetector<>))
            .AddTransient(typeof(IGraphFactory<>), typeof(GraphFactory<>))
            .AddTransient(typeof(IDependencyExecutionEngine<>), typeof(DependencyExecutionEngine<>));
    }
}
