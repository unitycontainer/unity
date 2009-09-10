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
using System.Text;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// The exception thrown by the Unity container when
    /// an attempt to resolve a dependency fails.
    /// </summary>
    // FxCop suppression: The standard constructors don't make sense for this exception,
    // as calling them will leave out the information that makes the exception useful
    // in the first place.
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    public partial class ResolutionFailedException : Exception
    {
        private readonly string typeRequested;
        private readonly string nameRequested;

        /// <summary>
        /// Create a new <see cref="ResolutionFailedException"/> that records
        /// the exception for the given type and name.
        /// </summary>
        /// <param name="typeRequested">Type requested from the container.</param>
        /// <param name="nameRequested">Name requested from the container.</param>
        /// <param name="innerException">The actual exception that caused the failure of the build.</param>
        /// <param name="context">The build context representing the failed operation.</param>
        public ResolutionFailedException(Type typeRequested, string nameRequested, Exception innerException, IBuilderContext context)
            : base(CreateMessage(typeRequested, nameRequested, innerException, context), innerException)
        {
            if (typeRequested != null)
            {
                this.typeRequested = typeRequested.Name;
            }
            this.nameRequested = nameRequested;
        }

        /// <summary>
        /// The type that was being requested from the container at the time of failure.
        /// </summary>
        public string TypeRequested
        {
            get { return typeRequested; }
        }

        /// <summary>
        /// The name that was being requested from the container at the time of failure.
        /// </summary>
        public string NameRequested
        {
            get { return nameRequested; }
        }

        private static string CreateMessage(Type typeRequested, string nameRequested, Exception innerException, IBuilderContext context)
        {
            var builder = new StringBuilder();

            builder.AppendFormat(
                CultureInfo.CurrentCulture,
                Resources.ResolutionFailed,
                typeRequested,
                FormatName(nameRequested),
                ExceptionReason(context),
                innerException != null ? innerException.GetType().Name : "ResolutionFailedException",
                innerException != null ? innerException.Message : null);
            builder.AppendLine();

            AddContextDetails(builder, context, 1);

            return builder.ToString();
        }

        private static void AddContextDetails(StringBuilder builder, IBuilderContext context, int depth)
        {
            if (context != null)
            {
                var indentation = new string(' ', depth * 2);
                var key = (NamedTypeBuildKey)context.BuildKey;
                var originalKey = (NamedTypeBuildKey)context.OriginalBuildKey;

                builder.Append(indentation);

                if (key == originalKey)
                {
                    builder.AppendFormat(
                        CultureInfo.CurrentCulture,
                        Resources.ResolutionTraceDetail,
                        key.Type, FormatName(key.Name));
                }
                else
                {
                    builder.AppendFormat(
                        CultureInfo.CurrentCulture,
                        Resources.ResolutionWithMappingTraceDetail,
                        key.Type, FormatName(key.Name),
                        originalKey.Type, FormatName(originalKey.Name));
                }

                builder.AppendLine();

                if (context.CurrentOperation != null)
                {
                    builder.Append(indentation);
                    builder.AppendFormat(
                        CultureInfo.CurrentCulture,
                        context.CurrentOperation.ToString());
                    builder.AppendLine();
                }

                AddContextDetails(builder, context.ChildContext, depth + 1);
            }
        }

        private static string FormatName(string name)
        {
            return string.IsNullOrEmpty(name) ? "(none)" : '"' + name + '"';
        }

        private static string ExceptionReason(IBuilderContext context)
        {
            var deepestContext = context;
            while (deepestContext.ChildContext != null)
            {
                deepestContext = deepestContext.ChildContext;
            }

            if (deepestContext.CurrentOperation != null)
            {
                return deepestContext.CurrentOperation.ToString();
            }
            return Resources.NoOperationExceptionReason;
        }
    }
}
