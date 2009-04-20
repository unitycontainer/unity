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

// Strong name is supplied during automated build process, not in the code
using System.Diagnostics.CodeAnalysis;
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification="Strong name is supplied in build process.")]
[assembly: SuppressMessage("Microsoft.Usage", "CA2209:AssembliesShouldDeclareMinimumSecurity")]

[assembly: SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Scope = "member", Target = "Microsoft.Practices.ObjectBuilder2.BuildFailedException..ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)", Justification="Validation done by base class.")]
