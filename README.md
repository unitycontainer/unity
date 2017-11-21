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



# Issues and Contributions

- If something is broken and you know how to fix it, send a pull request. 
- If you have no idea what is wrong, create an issue

## Any feedback and contributions are welcome
