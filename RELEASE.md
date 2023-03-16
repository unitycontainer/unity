# Release 6.0.0

During the development more that 100 bugs and issues were discovered and [reported](https://github.com/unitycontainer/container/issues). This [project](https://github.com/orgs/unitycontainer/projects/3) was created and updated to keep track of the progress and development of this release.

## New engine

Unity 6.0.0 will be released with a new and improved engine. The old implementation is no [longer compatible](https://github.com/unitycontainer/container/issues/312) with current version of .NET and does not allow further progress of the container.

## Legacy Support

During the development a lot of attention went into creating implementation compatible with v5 as well as v4. To demonstrate differences in baseline and show proper use of different versions more than 2000 unit tests were created in [Regression Tests](https://github.com/unitycontainer?q=regression) project.

## Performance

Container's performance was one of the biggest points of attention for the team. A lot of re-engendering of code was done to conserve CPU cycles and memory resources. As you can see from this brief test the performance was improved significantly.

``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen Threadripper 2970WX, 1 CPU, 48 logical and 24 physical cores
.NET Core SDK=5.0.300
  [Host]              : .NET Core 5.0.6 (CoreCLR 5.0.621.22011, CoreFX 5.0.621.22011), X64 RyuJIT
  .NET 4.6.2          : .NET Framework 4.8 (4.8.4341.0), X64 RyuJIT
  .NET 4.7.2          : .NET Framework 4.8 (4.8.4341.0), X64 RyuJIT
  .NET Core 5.0       : .NET Core 5.0.6 (CoreCLR 5.0.621.22011, CoreFX 5.0.621.22011), X64 RyuJIT
  ShortRun-.NET 4.6.2 : .NET Framework 4.8 (4.8.4341.0), X64 RyuJIT
  ShortRun-.NET 4.8   : .NET Framework 4.8 (4.8.4341.0), X64 RyuJIT
```

| Namespace |                           Method |      Mean |      Error |    StdDev |
|---------- |--------------------------------- |----------:|-----------:|----------:|
|  Unity.v4 |    `Resolve<IUnityContainer>( )` | 623.62 ns |   3.583 ns |  3.176 ns |
|  Unity.v5 |    `Resolve<IUnityContainer>( )` | 103.41 ns |   0.417 ns |  0.390 ns |
|  Unity.v6 |    `Resolve<IUnityContainer>( )` |  35.58 ns |   0.367 ns |  0.344 ns |
