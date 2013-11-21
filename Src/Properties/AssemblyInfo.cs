// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Microsoft.Practices.Unity.InterceptionExtension")]
[assembly: AssemblyDescription("Interception support for the Unity Application Block")]

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: AssemblyProduct("Microsoft Unity Application Block")]
[assembly: AssemblyCompany("Microsoft Corporation")]

#if !SILVERLIGHT
[assembly: AllowPartiallyTrustedCallers]
#endif

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCopyright("Copyright © 2011,2012 Microsoft Corporation.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.1311.0")]
[assembly: AssemblyInformationalVersion("3.0.1311-prerelease")]

[assembly: NeutralResourcesLanguage("en")]
