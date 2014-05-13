// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Security;

[assembly: CLSCompliant(true)]
[assembly: AssemblyProduct("Microsoft Unity Application Block")]
[assembly: AssemblyCompany("Microsoft Corporation")]

[assembly: SecurityTransparent]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCopyright("Copyright © Microsoft Corporation")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("3.5.1.0")]
[assembly: AssemblyFileVersion("3.5.1405.0")]
[assembly: AssemblyInformationalVersion("3.5.1405-prerelease")]
