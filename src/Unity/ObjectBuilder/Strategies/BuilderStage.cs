// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace ObjectBuilder2
{
    /// <summary>
    /// Enumeration to represent the object builder stages.
    /// </summary>
    /// <remarks>
    /// The order of the values in the enumeration is the order in which the stages are run.
    /// </remarks>
    public enum BuilderStage
    {
        /// <summary>
        /// Strategies in this stage run before creation. Typical work done in this stage might
        /// include strategies that use reflection to set policies into the context that other
        /// strategies would later use.
        /// </summary>
        PreCreation,

        /// <summary>
        /// Strategies in this stage create objects. Typically you will only have a single policy-driven
        /// creation strategy in this stage.
        /// </summary>
        Creation,

        /// <summary>
        /// Strategies in this stage work on created objects. Typical work done in this stage might
        /// include setter injection and method calls.
        /// </summary>
        Initialization,

        /// <summary>
        /// Strategies in this stage work on objects that are already initialized. Typical work done in
        /// this stage might include looking to see if the object implements some notification interface
        /// to discover when its initialization stage has been completed.
        /// </summary>
        PostInitialization,
    }
}
