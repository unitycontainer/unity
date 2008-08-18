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
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Practices.ObjectBuilder2.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// The exception that gets thrown if a build or teardown operation fails.
    /// </summary>
    [Serializable]
    public class BuildFailedException : Exception
    {
        private readonly string executingStrategyTypeName;
        private readonly int executingStrategyIndex;
        private readonly string buildKey;

        /// <summary>
        /// Create a new <see cref="BuildFailedException"/> instance containing
        /// the information about the currently executing strategy that caused
        /// the exception.
        /// </summary>
        /// <param name="executingStrategy">The <see cref="IBuilderStrategy"/> that was
        /// executing at the time the exception was thrown.</param>
        /// <param name="executingStrategyIndex">The index of the current strategy in its
        /// strategy chain.</param>
        /// <param name="buildKey">The build key being built up.</param>
        /// <param name="innerException">Underlying exception.</param>
        public BuildFailedException(
            IBuilderStrategy executingStrategy, 
            int executingStrategyIndex, 
            object buildKey, 
            Exception innerException)
            : base(null, innerException)
        {
            if(executingStrategy != null)
            {
                executingStrategyTypeName = executingStrategy.GetType().Name;
            }
            this.executingStrategyIndex = executingStrategyIndex;

            if (buildKey != null)
            {
                this.buildKey = buildKey.ToString();
            }
        }

        #region Standard constructors - do not use

        /// <summary>
        /// Create a new <see cref="BuildFailedException"/>. Do not use this constructor, it
        /// does not take any of the data that makes this type useful.
        /// </summary>
        public BuildFailedException()
        {
        }

        /// <summary>
        /// Create a new <see cref="BuildFailedException"/>. Do not use this constructor, it
        /// does not take any of the data that makes this type useful.
        /// </summary>
        /// <param name="message">Error message, ignored.</param>
        public BuildFailedException(string message) : base(message)
        {
            
        }

        /// <summary>
        /// Create a new <see cref="BuildFailedException"/>. Do not use this constructor, it
        /// does not take any of the data that makes this type useful.
        /// </summary>
        /// <param name="message">Error message, ignored.</param>
        /// <param name="innerException">Inner exception.</param>
        public BuildFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Create a new <see cref="BuildFailedException"/> from the serialized information.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", 
            Justification="Validation done by Guard class")]
        protected BuildFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Guard.ArgumentNotNull(info, "info");
            executingStrategyTypeName = info.GetString("ExecutingStrategy");
            executingStrategyIndex = info.GetInt32("ExecutingStrategyIndex");
            buildKey = info.GetString("BuildKey");
        }

        #endregion

        /// <summary>
        /// The strategy that was executing when the exception occurred.
        /// </summary>
        public string ExecutingStrategyTypeName
        {
            get { return executingStrategyTypeName; }
        }

        /// <summary>
        /// The index of the currently executing strategy in the build chain.
        /// </summary>
        public int ExecutingStrategyIndex
        {
            get { return executingStrategyIndex; }
        }

        /// <summary>
        /// The build key that was being built at the time of the exception.
        /// </summary>
        public string BuildKey
        {
            get { return buildKey; }
        }

        ///<summary>
        ///Gets a message that describes the current exception.
        ///</summary>
        ///
        ///<returns>
        ///The error message that explains the reason for the exception, or an empty string("").
        ///</returns>
        ///<filterpriority>1</filterpriority>
        public override string Message
        {
            get 
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.BuildFailedException,
                    ExecutingStrategyTypeName,
                    ExecutingStrategyIndex,
                    BuildKey,
                    InnerException != null ? InnerException.Message : null);
            }
        }

        ///<summary>
        ///When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with information about the exception.
        ///</summary>
        ///
        ///<param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination. </param>
        ///<param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown. </param>
        ///<exception cref="T:System.ArgumentNullException">The info parameter is a null reference (Nothing in Visual Basic). </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" /></PermissionSet>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Base class does the validation")]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ExecutingStrategy", ExecutingStrategyTypeName, typeof(string));
            info.AddValue("ExecutingStrategyIndex", ExecutingStrategyIndex, typeof(int));
            info.AddValue("BuildKey", BuildKey, typeof(string));
        }
    }
}
