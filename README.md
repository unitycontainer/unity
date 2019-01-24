# Overview

The Unity Container (Unity) is a full featured, extensible dependency injection container. It facilitates building loosely coupled applications and provides developers with the following advantages:

* Simplified object creation, especially for hierarchical object structures and dependencies
* Abstraction of requirements; this allows developers to specify dependencies at run time or in configuration and simplify management of crosscutting concerns
* Increased flexibility by deferring component configuration to the container
* Service location capability; this allows clients to store or cache the container
* Instance and type interception
* Registration by convention

# Performance Stats for v5.9.0

This release is all about optimizing performance.  These are the latest benchmarks:

``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.523 (1803/April2018Update/Redstone4)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3914058 Hz, Resolution=255.4893 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3260.0
  Job-NIEANE : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3260.0

InvocationCount=100000  LaunchCount=1  RunStrategy=Throughput  

```
|       Method | Version |        Mean |      Error |     StdDev |
|------------- |-------- |------------:|-----------:|-----------:|
|    Singleton | 4.0.1   |   455.53 ns |  4.3610 ns |  4.0793 ns |
|    Singleton | 5.8.13  |   127.87 ns |  0.9838 ns |  0.9203 ns |
|    Singleton | 5.9.0   |    76.19 ns |  0.8752 ns |  0.8187 ns |
|              |         |             |            |            |
| Unregistered | 4.0.1   |   893.14 ns |  3.9070 ns |  3.6546 ns |
| Unregistered | 5.8.13  |   128.18 ns |  1.2329 ns |  1.1532 ns |
| Unregistered | 5.9.0   |    88.37 ns |  0.8162 ns |  0.7635 ns |
|              |         |             |            |            |
|    Transient | 4.0.1   |   906.03 ns |  4.0031 ns |  3.5487 ns |
|    Transient | 5.8.13  |   143.36 ns |  1.7001 ns |  1.5071 ns |
|    Transient | 5.9.0   |    96.90 ns |  1.3002 ns |  1.1526 ns |
|              |         |             |            |            |
|      Mapping | 4.0.1   |   776.70 ns |  2.3000 ns |  2.1514 ns |
|      Mapping | 5.8.13  |   141.33 ns |  1.4194 ns |  1.3278 ns |
|      Mapping | 5.9.0   |   122.58 ns |  2.4451 ns |  2.4014 ns |
|              |         |             |            |            |
|        Array | 4.0.1   | 8,725.32 ns | 36.1246 ns | 33.7909 ns |
|        Array | 5.8.13  |   642.21 ns |  4.4079 ns |  3.9075 ns |
|        Array | 5.9.0   |   605.97 ns |  5.6593 ns |  5.2937 ns |
|              |         |             |            |            |
|   Enumerable | 4.0.1   |          NA |         NA |         NA |
|   Enumerable | 5.8.13  |   739.89 ns |  3.5254 ns |  3.2977 ns |
|   Enumerable | 5.9.0   |   669.90 ns |  5.6207 ns |  4.6935 ns |




# New Features
[**Suggest**](https://feathub.com/unitycontainer/unity/features/new) new features or vote for the proposals you like, [**ADD**](https://feathub.com/unitycontainer/unity/features/new) your comments:

[![Feature Requests](http://feathub.com/unitycontainer/unity?format=svg)](http://feathub.com/unitycontainer/unity)


## Packages & Status
Unity library consists of multiple packages. For information about each package please follow the links

---
Package  | Build status | NuGet 
-------- | :------------ | :------------ 
Unity (Composite)    | [![Build status](https://ci.appveyor.com/api/projects/status/nv00dk4lax6oqd00/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/unity/branch/master)   | [![NuGet](https://img.shields.io/nuget/v/Unity.svg)](https://www.nuget.org/packages/Unity)
[Unity.Abstractions](https://github.com/unitycontainer/abstractions)  | [![Build status](https://ci.appveyor.com/api/projects/status/l3bwjwm7q10nrdus/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/abstractions/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Abstractions.svg)](https://www.nuget.org/packages/Unity.Abstractions) 
[Unity.Container](https://github.com/unitycontainer/container)  | [![Build status](https://ci.appveyor.com/api/projects/status/s7s905q6xd6b2503/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/container/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Container.svg)](https://www.nuget.org/packages/Unity.Container)
[Unity.Configuration](https://github.com/unitycontainer/configuration)  | [![Build status](https://ci.appveyor.com/api/projects/status/89jo5cuum6839j3b/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/configuration/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Configuration.svg)](https://www.nuget.org/packages/Unity.Configuration)
[Unity.Interception](https://github.com/unitycontainer/interception)  | [![Build status](https://ci.appveyor.com/api/projects/status/xb5tbuxxqb381kxc/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/interception/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Interception.svg)](https://www.nuget.org/packages/Unity.Interception)
[Unity.Interception.Configuration](https://github.com/unitycontainer/interception-configuration)  | [![Build status](https://ci.appveyor.com/api/projects/status/wh7x0lml55c483st/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/interception-configuration/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Interception.Configuration.svg)](https://www.nuget.org/packages/Unity.Interception.Configuration) 
[Unity.RegistrationByConvention](https://github.com/unitycontainer/registration-by-convention)  |  [![Build status](https://ci.appveyor.com/api/projects/status/xv7bkc6v62g4w7n4/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/registration-by-convention/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.RegistrationByConvention.svg)](https://www.nuget.org/packages/Unity.RegistrationByConvention) 
[Unity.log4net](https://github.com/unitycontainer/log4net)  | [![Build status](https://ci.appveyor.com/api/projects/status/3x9gf21l6qqxo9rn/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/log4net/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.log4net.svg)](https://www.nuget.org/packages/Unity.log4net)
[Unity.NLog](https://github.com/unitycontainer/NLog)  | [![Build status](https://ci.appveyor.com/api/projects/status/tr7ykk0g5jgieon2/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/nlog-9y7y3/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.NLog.svg)](https://www.nuget.org/packages/Unity.NLog)
[Unity.Microsoft.Logging](https://github.com/unitycontainer/microsoft-logging)  | [![Build status](https://ci.appveyor.com/api/projects/status/r97hcdjf377ty6kq/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/microsoft-logging/branch/master) |  [![NuGet](https://img.shields.io/nuget/v/Unity.Microsoft.Logging.svg)](https://www.nuget.org/packages/Unity.Microsoft.Logging)
[Unity.Microsoft.DependencyInjection](https://github.com/unitycontainer/microsoft-dependency-injection)  | [![Build status](https://ci.appveyor.com/api/projects/status/sevk2yb2jokf8ltr/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/microsoft-dependency-injection/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Microsoft.DependencyInjection.svg)](https://www.nuget.org/packages/Unity.Microsoft.DependencyInjection)
[Unity.AspNet.WebApi](https://github.com/unitycontainer/aspnet-webapi)  | [![Build status](https://ci.appveyor.com/api/projects/status/rn0ohbxtv6c0q726/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/aspnet-webapi/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.AspNet.WebApi.svg)](https://www.nuget.org/packages/Unity.AspNet.WebApi)
[Unity.Mvc](https://github.com/unitycontainer/aspnet-mvc)  | [![Build status](https://ci.appveyor.com/api/projects/status/ed670lsbm4sx95f0/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/aspnet-mvc/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Mvc.svg)](https://www.nuget.org/packages/Unity.Mvc) 
[Unity.ServiceLocation](https://github.com/unitycontainer/service-location)  | [![Build status](https://ci.appveyor.com/api/projects/status/5q5129q417rg7xe2/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/service-location/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.ServiceLocation.svg)](https://www.nuget.org/packages/Unity.ServiceLocation) 
[CommonServiceLocator](https://github.com/unitycontainer/commonservicelocator)  | [![Build status](https://ci.appveyor.com/api/projects/status/dax8w8u3d5c6kv0a/branch/master?svg=true)](https://ci.appveyor.com/project/unitycontainer/commonservicelocator/branch/master) | [![NuGet](https://img.shields.io/nuget/v/commonservicelocator.svg)](https://www.nuget.org/packages/CommonServiceLocator)




# Roadmap

### [v4.0.1](https://github.com/unitycontainer/unity/tree/a370e3cd8c0f9aa5f505e896ef5225f42711d361)

Version 4.x is dead. Loss of original signing certificate made it impossible to release anything compatible with v4.0.1 release. To give original developers a credit only about 60 issues were found during two years in production. To move on and enable further development version v5 has been created.

### [v5.x](https://github.com/unitycontainer/unity/tree/v5.x)

Version 5.x is created as replacement for v4.0.1. Assemblies and namespaces are renamed and refactored but otherwise it is compatible with the original. v5.0.0 release fixes most of the issues found in v4.0.1 and implements several optimizations but the accent was on compatibility and if optimization would break API it was omitted. Once stabilized, this version will enter LTS status and will be patched and fixed for the next few years. There will be no significant development in this line.

To build v5.x locally please follow these steps:
- `git clone https://github.com/unitycontainer/unity.git`
- `cd unity && git checkout v5.x && git submodule update --init --recursive`
- open `package.sln` in Visual Studio and build.



### [v6.x](https://github.com/unitycontainer/unity/tree/v6.x)

This is where all new development will be done. 
The compatibility would not be a driving factor so better performance and functionality could be achieved. 

To build v6.x locally please follow these steps:
- `git clone https://github.com/unitycontainer/unity.git`
- `cd unity && git checkout v6.x && git submodule update --init --recursive`
- open `package.sln` in Visual Studio and build.


## Release schedule and Long Time Support (LTS) <sup>1</sup>

| Release |  LTS Status   | Active LTS Start | Maintenance Start | Maintenance End |
|   :--:  |    :---:      |       :---:      |       :---:       |      :---:      |
|  v3.x   |    No LTS     |         -        |         -         |      2012       |
|  v4.x   |**End-of-Life**|         -        |         -         |      2015       |
|  v5.x   |**Active**     |    2018-10-18    |    March 2018     |  December 2019  |
| [6.x]   |**Pending**    |    2019-01-01    |  January 2019     |                 |

* <sup>1</sup>: All scheduled dates are subject to change.




## Documentation

The documentation is a work in progress. Some info is available [here](https://unitycontainer.github.io) but more is coming...
I am a bit busy working on v6.x engine and will not be able to dedicate any time to writing docs any time soon. **I could really use some help with it**. If you feel you could contribute, it would be very welcome.
I've considered an idea of crowd sourcing enough funds to hire a professional editor but this takes tame too, so...


## Issues and Contributions

- If something is broken and you know how to fix it, send a pull request. 
- If you have no idea what is wrong, create an issue

### Any feedback and contributions are welcome

If you have something you'd like to improve do not hesitate to send a Pull Request

