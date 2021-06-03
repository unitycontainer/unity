
# Overview

The Unity Container (Unity) is a full featured, extensible dependency injection container. It facilitates building loosely coupled applications and provides developers with host of other useful features. For more information see [documentation](http://unitycontainer.org/articles/introduction.html).

## Announcements

Date       | Announcement
:--------  | :------------
01/06/2021 | While developing Unity v6 several bugs were uncovered by the team. Since v6 is still in early beta it has been decided to release [Unity v5.12](https://github.com/orgs/unitycontainer/projects/5) to fix these issues. For more information follow this [project](https://github.com/orgs/unitycontainer/projects/5).
01/05/2020 | Unity team is starting work on [Unity v6 release](https://github.com/unitycontainer/unity/tree/release/6.0.0). To track progress follow [this project](https://github.com/orgs/unitycontainer/projects/3).
14/05/2020 | Starting May 2020 Unity Container is a [.NET Foundation](https://dotnetfoundation.org/projects/unitycontainer) project.


## Packages & Status

Unity library consists of multiple packages. For information about each package please follow the links

---
Package  | License       | Version       | Downloads
-------- | :------------ | :------------ | :------------
Unity (Composite) | [![License](https://img.shields.io/github/license/unitycontainer/unity.svg)](https://github.com/unitycontainer/unity/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.svg)](https://www.nuget.org/packages/Unity) | [![NuGet](https://img.shields.io/nuget/dt/Unity.svg)](https://www.nuget.org/packages/Unity)
[Unity.Abstractions](https://github.com/unitycontainer/abstractions) | [![License](https://img.shields.io/github/license/unitycontainer/abstractions.svg)](https://github.com/unitycontainer/abstractions/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.Abstractions.svg)](https://www.nuget.org/packages/Unity.Abstractions) | [![NuGet](https://img.shields.io/nuget/dt/Unity.Abstractions.svg)](https://www.nuget.org/packages/Unity.Abstractions)
[Unity.Container](https://github.com/unitycontainer/container) | [![License](https://img.shields.io/github/license/unitycontainer/container.svg)](https://github.com/unitycontainer/container/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.Container.svg)](https://www.nuget.org/packages/Unity.Container) | [![NuGet](https://img.shields.io/nuget/dt/Unity.Container.svg)](https://www.nuget.org/packages/Unity.Container)
[Unity.Configuration](https://github.com/unitycontainer/configuration) | [![License](https://img.shields.io/github/license/unitycontainer/configuration.svg)](https://github.com/unitycontainer/configuration/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.Configuration.svg)](https://www.nuget.org/packages/Unity.Configuration) | [![NuGet](https://img.shields.io/nuget/dt/Unity.Configuration.svg)](https://www.nuget.org/packages/Unity.Configuration)
[Unity.Interception](https://github.com/unitycontainer/interception) | [![License](https://img.shields.io/github/license/unitycontainer/interception.svg)](https://github.com/unitycontainer/interception/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.Interception.svg)](https://www.nuget.org/packages/Unity.Interception) | [![NuGet](https://img.shields.io/nuget/dt/Unity.Interception.svg)](https://www.nuget.org/packages/Unity.Interception)
[Unity.Interception.Configuration](https://github.com/unitycontainer/interception-configuration) | [![License](https://img.shields.io/github/license/unitycontainer/interception-configuration.svg)](https://github.com/unitycontainer/interception-configuration/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.Interception.Configuration.svg)](https://www.nuget.org/packages/Unity.Interception.Configuration) | [![NuGet](https://img.shields.io/nuget/dt/Unity.Interception.Configuration.svg)](https://www.nuget.org/packages/Unity.Interception.Configuration)
[Unity.RegistrationByConvention](https://github.com/unitycontainer/registration-by-convention) | [![License](https://img.shields.io/github/license/unitycontainer/registration-by-convention.svg)](https://github.com/unitycontainer/registration-by-convention/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.RegistrationByConvention.svg)](https://www.nuget.org/packages/Unity.RegistrationByConvention) | [![NuGet](https://img.shields.io/nuget/dt/Unity.RegistrationByConvention.svg)](https://www.nuget.org/packages/Unity.RegistrationByConvention)
[Unity.log4net](https://github.com/unitycontainer/log4net) | [![License](https://img.shields.io/github/license/unitycontainer/log4net.svg)](https://github.com/unitycontainer/log4net/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.log4net.svg)](https://www.nuget.org/packages/Unity.log4net) | [![NuGet](https://img.shields.io/nuget/dt/Unity.log4net.svg)](https://www.nuget.org/packages/Unity.log4net)
[Unity.NLog](https://github.com/unitycontainer/NLog) | [![License](https://img.shields.io/github/license/unitycontainer/NLog.svg)](https://github.com/unitycontainer/NLog/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.NLog.svg)](https://www.nuget.org/packages/Unity.NLog) | [![NuGet](https://img.shields.io/nuget/dt/Unity.NLog.svg)](https://www.nuget.org/packages/Unity.NLog)
[Unity.Microsoft.Logging](https://github.com/unitycontainer/microsoft-logging) | [![License](https://img.shields.io/github/license/unitycontainer/microsoft-logging.svg)](https://github.com/unitycontainer/microsoft-logging/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.Microsoft.Logging.svg)](https://www.nuget.org/packages/Unity.Microsoft.Logging) | [![NuGet](https://img.shields.io/nuget/dt/Unity.Microsoft.Logging.svg)](https://www.nuget.org/packages/Unity.Microsoft.Logging)
[Unity.Microsoft.DependencyInjection](https://github.com/unitycontainer/microsoft-dependency-injection) | [![License](https://img.shields.io/github/license/unitycontainer/microsoft-dependency-injection.svg)](https://github.com/unitycontainer/microsoft-dependency-injection/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.Microsoft.DependencyInjection.svg)](https://www.nuget.org/packages/Unity.Microsoft.DependencyInjection)| [![NuGet](https://img.shields.io/nuget/dt/Unity.Microsoft.DependencyInjection.svg)](https://www.nuget.org/packages/Unity.Microsoft.DependencyInjection)
[Unity.AspNet.WebApi](https://github.com/unitycontainer/aspnet-webapi) | [![License](https://img.shields.io/github/license/unitycontainer/aspnet-webapi.svg)](https://github.com/unitycontainer/aspnet-webapi/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.AspNet.WebApi.svg)](https://www.nuget.org/packages/Unity.AspNet.WebApi) | [![NuGet](https://img.shields.io/nuget/dt/Unity.AspNet.WebApi.svg)](https://www.nuget.org/packages/Unity.AspNet.WebApi)
[Unity.Mvc](https://github.com/unitycontainer/aspnet-mvc) | [![License](https://img.shields.io/github/license/unitycontainer/aspnet-mvc.svg)](https://github.com/unitycontainer/aspnet-mvc/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.Mvc.svg)](https://www.nuget.org/packages/Unity.Mvc) | [![NuGet](https://img.shields.io/nuget/dt/Unity.Mvc.svg)](https://www.nuget.org/packages/Unity.Mvc)
[Unity.WCF](https://github.com/unitycontainer/wcf) | [![License](https://img.shields.io/github/license/unitycontainer/wcf.svg)](https://github.com/unitycontainer/wcf/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.wcf.svg)](https://www.nuget.org/packages/Unity.wcf) | [![NuGet](https://img.shields.io/nuget/dt/Unity.wcf.svg)](https://www.nuget.org/packages/Unity.wcf)
[Unity.ServiceLocation](https://github.com/unitycontainer/service-location) | [![License](https://img.shields.io/github/license/unitycontainer/service-location.svg)](https://github.com/unitycontainer/service-location/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/Unity.ServiceLocation.svg)](https://www.nuget.org/packages/Unity.ServiceLocation) | [![NuGet](https://img.shields.io/nuget/dt/Unity.ServiceLocation.svg)](https://www.nuget.org/packages/Unity.ServiceLocation)
[CommonServiceLocator](https://github.com/unitycontainer/commonservicelocator) | [![License](https://img.shields.io/github/license/unitycontainer/commonservicelocator.svg)](https://github.com/unitycontainer/commonservicelocator/blob/master/LICENSE) | [![NuGet](https://img.shields.io/nuget/v/commonservicelocator.svg)](https://www.nuget.org/packages/CommonServiceLocator) | [![NuGet](https://img.shields.io/nuget/dt/commonservicelocator.svg)](https://www.nuget.org/packages/CommonServiceLocator)

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](https://www.contributor-covenant.org/) to clarify expected behavior in our community. For more information, see the [.NET Foundation Code of Conduct](https://www.dotnetfoundation.org/code-of-conduct)

## Contributing

See the [Contributing guide](https://github.com/unitycontainer/unity/blob/master/CONTRIBUTING.md) for more information.

## .NET Foundation

Unity Container is a [.NET Foundation](https://dotnetfoundation.org/projects/unitycontainer) project.
