[![Build status](https://ci.appveyor.com/api/projects/status/nv00dk4lax6oqd00/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/unity/branch/master)
[![codecov](https://codecov.io/gh/unitycontainer/unity/branch/master/graph/badge.svg)](https://codecov.io/gh/unitycontainer/unity)
[![License](https://img.shields.io/badge/license-apache%202.0-60C060.svg)](https://github.com/IoC-Unity/Unity/blob/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/dt/Unity.svg)](https://www.nuget.org/packages/Unity)
[![NuGet](https://img.shields.io/nuget/v/Unity.svg)](https://www.nuget.org/packages/Unity)


# Overview

The Unity Container (Unity) is a lightweight, extensible dependency injection container. It facilitates building loosely coupled applications and provides developers with the following advantages:

* Simplified object creation, especially for hierarchical object structures and dependencies
* Abstraction of requirements; this allows developers to specify dependencies at run time or in configuration and simplify management of crosscutting concerns
* Increased flexibility by deferring component configuration to the container
* Service location capability; this allows clients to store or cache the container
* Instance and type interception
* Registration by convention


# New Features
[**Suggest**](https://feathub.com/unitycontainer/unity/features/new) new features or vote for the proposals you like, [**ADD**](https://feathub.com/unitycontainer/unity/features/new) your comments:

[![Feature Requests](http://feathub.com/unitycontainer/unity?format=svg)](http://feathub.com/unitycontainer/unity)


## Packages & Status
Unity library consists of multiple packages. For information about each package please follow the links

---
Package  | Build status | NuGet 
-------- | :------------ | :------------ 
Unity (Composite)    | [![Build status](https://ci.appveyor.com/api/projects/status/nv00dk4lax6oqd00/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/unity/branch/master)   | [![NuGet](https://img.shields.io/nuget/v/Unity.svg)](https://www.nuget.org/packages/Unity)
[Unity.Abstractions](https://github.com/unitycontainer/abstractions)  | [![Build status](https://ci.appveyor.com/api/projects/status/l3bwjwm7q10nrdus/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/abstractions/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Abstractions.svg)](https://www.nuget.org/packages/Unity.Abstractions) 
[Unity.Container](https://github.com/unitycontainer/container)  | [![Build status](https://ci.appveyor.com/api/projects/status/s7s905q6xd6b2503/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/container/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Container.svg)](https://www.nuget.org/packages/Unity.Container)
[Unity.Configuration](https://github.com/unitycontainer/configuration)  | [![Build status](https://ci.appveyor.com/api/projects/status/89jo5cuum6839j3b/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/configuration/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Configuration.svg)](https://www.nuget.org/packages/Unity.Configuration)
[Unity.Interception](https://github.com/unitycontainer/interception)  | [![Build status](https://ci.appveyor.com/api/projects/status/xb5tbuxxqb381kxc/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/interception/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Interception.svg)](https://www.nuget.org/packages/Unity.Interception)
[Unity.Interception.Configuration](https://github.com/unitycontainer/interception-configuration)  | [![Build status](https://ci.appveyor.com/api/projects/status/wh7x0lml55c483st/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/interception-configuration/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Interception.Configuration.svg)](https://www.nuget.org/packages/Unity.Interception.Configuration) 
[Unity.RegistrationByConvention]  |  [![Build status](https://ci.appveyor.com/api/projects/status/xv7bkc6v62g4w7n4/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/registration-by-convention/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.RegistrationByConvention.svg)](https://www.nuget.org/packages/Unity.RegistrationByConvention) 
[Unity.log4net](https://github.com/unitycontainer/log4net)  | [![Build status](https://ci.appveyor.com/api/projects/status/3x9gf21l6qqxo9rn/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/log4net/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.log4net.svg)](https://www.nuget.org/packages/Unity.log4net)
[Unity.NLog](https://github.com/unitycontainer/NLog)  | Unavailable | [![NuGet](https://img.shields.io/nuget/v/Unity.NLog.svg)](https://www.nuget.org/packages/Unity.NLog)
[Unity.Microsoft.Logging](https://github.com/unitycontainer/microsoft-logging)  | Unavailable |  [![NuGet](https://img.shields.io/nuget/v/Unity.Microsoft.Logging.svg)](https://www.nuget.org/packages/Unity.Microsoft.Logging)
[Unity.Microsoft.DependencyInjection](https://github.com/unitycontainer/microsoft-dependency-injection)  | [![Build status](https://ci.appveyor.com/api/projects/status/sevk2yb2jokf8ltr/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/microsoft-dependency-injection/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Microsoft.DependencyInjection.svg)](https://www.nuget.org/packages/Unity.Microsoft.DependencyInjection)
[Unity.AspNet.WebApi](https://github.com/unitycontainer/aspnet-webapi)  | [![Build status](https://ci.appveyor.com/api/projects/status/rn0ohbxtv6c0q726/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/aspnet-webapi/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.AspNet.WebApi.svg)](https://www.nuget.org/packages/Unity.AspNet.WebApi)
[Unity.Mvc](https://github.com/unitycontainer/aspnet-mvc)  | [![Build status](https://ci.appveyor.com/api/projects/status/ed670lsbm4sx95f0/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/aspnet-mvc/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.Mvc.svg)](https://www.nuget.org/packages/Unity.Mvc) 
[Unity.ServiceLocation](https://github.com/unitycontainer/service-location)  | [![Build status](https://ci.appveyor.com/api/projects/status/5q5129q417rg7xe2/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/service-location/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Unity.ServiceLocation.svg)](https://www.nuget.org/packages/Unity.ServiceLocation) 
[CommonServiceLocator](https://github.com/unitycontainer/commonservicelocator)  | [![Build status](https://ci.appveyor.com/api/projects/status/dax8w8u3d5c6kv0a/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/commonservicelocator/branch/master) | [![NuGet](https://img.shields.io/nuget/v/commonservicelocator.svg)](https://www.nuget.org/packages/CommonServiceLocator)




# Roadmap

### [v4.0.1](https://github.com/unitycontainer/unity/tree/a370e3cd8c0f9aa5f505e896ef5225f42711d361)

Version 4.x is dead. Loss of original signing certificate made it impossible to release anything compatible with v4.0.1 release. To give original developers a credit only about 60 issues were found during two years in production. To move on and enable further development version v5 has been created.

### [v5.x](https://github.com/unitycontainer/unity/tree/v5.x)

Version 5.x is created as replacement for v4.0.1. Assemblies and namespaces are renamed and refactored but otherwise it is compatible with the original. v5.0.0 release fixes most of the issues found in v4.0.1 and implements several optimizations but the accent was on compatibility and if optimization would break API it was ommited. Once stabilized, this version will enter LTS status and will be patched and fixed for the next few years. There will be no significant development in this line.

### v6.x

This is where all new development will be done. The plan for next release is:
- Optimize performance
- Add support for Microsoft.Extensions.DependencyInjection.2.0.0 
- Improve how constructors and dependencies are selected 
- etc.

The compatibilty would not be a driving factor so better performance and functionality could be acheived. 




## Release schedule and Long Time Support (LTS) <sup>1</sup>

| Release |  LTS Status   | Active LTS Start | Maintenance Start | Maintenance End |
|   :--:  |    :---:      |       :---:      |       :---:       |      :---:      |
|  v3.x   |    No LTS     |         -        |         -         |      2012       |
|  v4.x   |**End-of-Life**|         -        |         -         |      2015       |
|  v5.x   |**Active**     |    2017-10-18    |    October 2017   |  December 2019  |
| [6.x]   |**Pending**    |    2018-01-01    |    January 2018   |                 |

* <sup>1</sup>: All scheduled dates are subject to change.




## Documentation

The documentation is a work in progress project. Some info is available [here](https://unitycontainer.github.io) but more is coming...


## Issues and Contributions

- If something is broken and you know how to fix it, send a pull request. 
- If you have no idea what is wrong, create an issue

### Any feedback and contributions are welcome

If you have something you'd like to improve do not hesitate to send a Pull Request

