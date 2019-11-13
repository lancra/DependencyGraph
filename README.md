DependencyGraph
===

[nuget]: https://www.nuget.org/packages/LanceC.DependencyGraph/

This package is used to execute processes that are dependent on the execution of at least one other process. The engine builds a directed graph of provided executions, checks for cycles, performs a reverse topological sort, and executes the results in order.

## Install

Install the [NuGet package][nuget] into your project.

```
PM> Install-Package LanceC.DependencyGraph
```
```
$ dotnet add package LanceC.DependencyGraph
```

## Usage

- The dependency graph classes must first be registered for DI.
- Processes to be executed need to implement `IDependencyExecution<T>`.
    - The identifier type has to implement [`IEquatable<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1).
    - It is recommended to register each execution in the DI container and retrieve them as an `IEnumerable<IDependencyExecution<T>>`.

### Registration

```c#
services.AddDependencyGraph();
```

### Execution

The sample below uses `string` as the identifier type for the executions.

```c#
public class Foo
{
    private readonly IDependencyExecutionEngine<string> _dependencyExecutionEngine;
    private readonly IEnumerable<IDependencyExecution<string>> _executions;

    public Foo(
        IDependencyExecutionEngine<string> dependencyExecutionEngine,
        IEnumerable<IDependencyExecution<string>> executions)
    {
        _dependencyExecutionEngine = dependencyExecutionEngine;
        _executions = executions;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        await _dependencyExecutionEngine.ExecuteAll(executions, cancellationToken);
    }
}
```

### Errors

- In the case where a circular dependency chain is found (e.g. `A` has a dependency on `B` and `B` has a dependency on `A`), a `CircularDependenciesException` is thrown upon engine execution.
    - The exception contains the text representations of the [strongly-connected cycles](https://en.wikipedia.org/wiki/Tarjan%27s_strongly_connected_components_algorithm) (e.g. 'A' -> 'B').
- In the case where multiple executions contain the same key, a `DuplicateKeyException` is thrown upon engine execution.
- 
