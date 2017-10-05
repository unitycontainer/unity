## Summary
Unity is a lightweight, extensible dependency injection container with support for instance and type interception.

## Overview
Unity is a lightweight, extensible dependency injection container with support for constructor, property, and method call injection. It facilitates building loosely coupled applications and provides developers with the following advantages:
- Simplified object creation, especially for hierarchical object structures and dependencies.
- Abstraction of requirements; this allows developers to specify dependencies at run time or in configuration and simplify management of crosscutting concerns.
- Increased flexibility by deferring component configuration to the container.
- Service location capability, which allows clients to store or cache the container.
- Instance and type interception.
- Support for Windows Store apps.
- Registration by convention feature to ease the task of configuring Unity.

Unity is a general-purpose container for use in any type of Microsoft.NET Framework-based application. It provides all of the features commonly found in dependency injection mechanisms, including methods to register type mappings and object instances, resolve objects, manage object lifetimes, and inject dependent objects into the parameters of constructors and methods, and as the value of properties of objects it resolves.

In addition, Unity is extensible. You can write container extensions that change the behavior of the container, or add new capabilities. For example, the interception feature provided by Unity, which you can use to capture calls to objects and add additional functionality and policies to the target objects, is implemented as a container extension.

### Audience Requirements
These reusable components and guidance are intended primarily for software developers and software architects. To get the most benefit from this guidance, you should a working knowledge of .NET programming.

### Contents of This Release
Microsoft Unity 3 contains the following:
- **Binaries**. The release includes pre-compiled, strong-named assemblies for all the source code.
- **Source code**. The release includes the source code for the Unity container, the Unity Interception Mechanism, Unity configuration as well as the source code for the integration packages with ASP.NET MVC and ASP.NET Web API.
- **Unit tests**. The release includes the unit tests that were created while Unity 3 was being developed.
- **Documentation**. A [separate download](http://go.microsoft.com/fwlink/p/?LinkID=290902) for Unity includes documentation, which includes guidance about how to use Unity and a class library reference.
- **Hands-on Labs**. The [Unity Hands-on Labs](https://www.microsoft.com/en-us/download/details.aspx?id=40287) help you learn about the Unity injection container and Unity interception. Step by step instructions and before and after source code are provided for each lab.

### System Requirements
- Supported architectures: x86 and x64.
- Operating systems: Microsoft Windows 8, Microsoft Windows 7, Windows Server 2008 R2, Windows Server 2012.
- .NET Framework: Microsoft .NET Framework 4.5, .NET for Windows Store Apps (previously known as Windows Runtime)

For a *rich development environment*, the following are recommended:
- Microsoft Visual Studio 2012 (Professional, Ultimate, or Express editions).

### Design Goals
Unity was designed to achieve the following goals:
- To promote the principles of modular design through aggressive decoupling of components, business objects, and services.
- To raise awareness of the need to maximize testability when designing applications.
- To provide a fast and lightweight dependency injection container mechanism for creating new object instances and managing existing object instances.
- To expose a compact and intuitive API for developers to use with the container.
- To support a wide range of programming languages, with method overrides that accept generic parameters where the language supports these.
- To implement attribute-driven injection for constructors, property setters, and methods of target objects.
- To provide extensibility through custom and third-party container extensions.
- To provide the performance required in line-of-business applications.

## What's New
This major release of Unity includes the following new features:
- Registration by convention.
- Support for NetCore (Windows Store apps).
- Resolving objects of type [`Lazy<T>`](https://msdn.microsoft.com/en-us/library/dd642331.aspx) by Unity.
- The Unity assembly is now Security Transparent.
- Support for ASP.NET MVC and ASP.NET Web API.

The detailed list of all changes is included in the [Release Notes](http://aka.ms/unity3release).

## Getting Started
For an introduction to dependency injection, see the article [Inversion of Control Containers and the Dependency Injection pattern](http://www.martinfowler.com/articles/injection.html) by Martin Fowler.

For information about getting started with Unity, see the [Unity documentation](http://go.microsoft.com/fwlink/p/?LinkID=290902) and the new [Unity Developer’s Guide](http://go.microsoft.com/fwlink/p/?LinkID=290913).
