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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Practices.ObjectBuilder2
{
    [Serializable]
    public partial class BuildFailedException
    {
        /// <summary>
        /// Create a new <see cref="BuildFailedException"/> from the serialized information.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        protected BuildFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Guard.ArgumentNotNull(info, "info");
            executingStrategyTypeName = info.GetString("ExecutingStrategy");
            executingStrategyIndex = info.GetInt32("ExecutingStrategyIndex");
            buildKey = info.GetString("BuildKey");
        }

        ///<summary>
        ///When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with information about the exception.
        ///</summary>
        ///
        ///<param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination. </param>
        ///<param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown. </param>
        ///<exception cref="T:System.ArgumentNullException">The info parameter is a null reference (Nothing in Visual Basic). </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" /></PermissionSet>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(info==null) throw new ArgumentNullException("info");
            base.GetObjectData(info, context);
            info.AddValue("ExecutingStrategy", ExecutingStrategyTypeName, typeof (string));
            info.AddValue("ExecutingStrategyIndex", ExecutingStrategyIndex, typeof (int));
            info.AddValue("BuildKey", BuildKey, typeof (string));
        }
    }
}
