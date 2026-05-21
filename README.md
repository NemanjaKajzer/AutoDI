# AutoDI

![CI](https://github.com/NemanjaKajzer/AutoDI/actions/workflows/ci.yml/badge.svg)
![NuGet](https://img.shields.io/nuget/v/AutoDI)
![AOT Compatible](https://img.shields.io/badge/AOT-compatible-brightgreen)

Compile-time dependency injection registration for .NET. No runtime reflection, no assembly scanning — services are discovered and registered at compile time via a Roslyn source generator.

## Why AutoDI?

Most DI registration libraries (like Scrutor) scan assemblies at runtime using reflection. This has two costs:

- **Startup latency** — reflection is slow, especially with large assemblies
- **AOT incompatibility** — reflection-based scanning breaks Native AOT publishing

AutoDI solves both by moving service discovery to compile time.

## Installation

```
dotnet add package AutoDI
```

## Usage

Decorate your service classes with one of three attributes:

```csharp
using AutoDI;

[RegisterScoped]
public class UserService : IUserService { }

[RegisterSingleton]
public class ConfigService : IConfigService { }

[RegisterTransient]
public class EmailSender : IEmailSender { }
```

Then call the generated extension method in your startup code:

```csharp
builder.Services.AddAutoRegisteredServices();
```

That's it. No manual registration, no assembly scanning, no reflection.

## Supported Patterns

```csharp
// Multiple interfaces
[RegisterScoped]
public class FooBarService : IFoo, IBar { }
// → AddScoped<IFoo, FooBarService>()
// → AddScoped<IBar, FooBarService>()

// Open generics
[RegisterScoped]
public class Repository<T> : IRepository<T> { }
// → AddScoped(typeof(IRepository<>), typeof(Repository<>))

// Self-registration (no interface)
[RegisterScoped]
public class StandaloneService { }
// → AddScoped<StandaloneService>()

// Internal classes
[RegisterScoped]
internal sealed class InternalService : IInternalService { }
// → AddScoped<IInternalService, InternalService>()
```

`IDisposable` is automatically excluded from interface registrations.

## Benchmark

Measured on .NET 10, 50 services, BenchmarkDotNet.

| Method                   | Mean       | Allocated |
|--------------------------|------------|-----------|
| Manual registration      | 2.1 μs     | 4.2 KB    |
| **AutoDI (compile-time)**| **2.3 μs** | **4.2 KB**|
| Scrutor assembly scan    | 812.4 μs   | 128.7 KB  |

AutoDI is effectively identical to manual registration because it **is** manual registration — just written by the compiler instead of you.

> Benchmark figures are placeholders — replace with actual BenchmarkDotNet output from P7.

## Native AOT

AutoDI is fully compatible with Native AOT. Because all registration code is generated at compile time, there is no reflection at startup:

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
</PropertyGroup>
```

```
dotnet publish -c Release -r linux-x64 --self-contained
```

No trim warnings. No reflection. Just fast startup.

## How It Works

AutoDI uses a Roslyn `IIncrementalGenerator` to scan your project at compile time. It finds every class decorated with `[RegisterScoped]`, `[RegisterSingleton]`, or `[RegisterTransient]` and emits a static extension method:

```csharp
// AutoDI.g.cs — generated at compile time
public static partial class AutoDIServiceCollectionExtensions
{
    public static IServiceCollection AddAutoRegisteredServices(
        this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IConfigService, ConfigService>();
        services.AddTransient<IEmailSender, EmailSender>();
        return services;
    }
}
```

The generated file is visible in your IDE under **Dependencies → Analyzers → AutoDI.SourceGenerator**.

## License

MIT
