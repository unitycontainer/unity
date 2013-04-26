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


using System.Diagnostics.CodeAnalysis;
namespace Microsoft.Practices.Unity.ObjectBuilder
{
    /// <summary>
    /// The build stages we use in the Unity container
    /// strategy pipeline.
    /// </summary>
    public enum UnityBuildStage
    {
        /// <summary>
        /// First stage. By default, nothing happens here.
        /// </summary>
        Setup,

        /// <summary>
        /// Second stage. Type mapping occurs here.
        /// </summary>
        TypeMapping,

        /// <summary>
        /// Third stage. lifetime managers are checked here,
        /// and if they're available the rest of the pipeline is skipped.
        /// </summary>
        Lifetime,

        /// <summary>
        /// Fourth stage. Reflection over constructors, properties, etc. is
        /// performed here.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PreCreation",
            Justification = "Kept for backward compatibility with ObjectBuilder")]
        PreCreation,

        /// <summary>
        /// Fifth stage. Instance creation happens here.
        /// </summary>
        Creation,

        /// <summary>
        /// Sixth stage. Property sets and method injection happens here.
        /// </summary>
        Initialization,

        /// <summary>
        /// Seventh and final stage. By default, nothing happens here.
        /// </summary>
        PostInitialization
    }
}
