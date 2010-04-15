//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Reflection;
using System.Security;
[assembly: CLSCompliant(true)]
[assembly: AssemblyProduct("Microsoft Unity Application Block")]
[assembly: AssemblyCompany("Microsoft Corporation")]

[assembly: SecurityTransparent]
[assembly: AllowPartiallyTrustedCallers]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCopyright("Copyright © 2010 Microsoft Corporation.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("2.0.414.0")]
[assembly: AssemblyFileVersion("2.0.414.0")]
