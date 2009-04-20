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
using Microsoft.Practices.ObjectBuilder2.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a simple class for validating parameters and throwing exceptions.
    /// </summary>
	public static class Guard
	{
        /// <summary>
        /// Validates <paramref name="argumentValue"/> is not null and throws <see cref="ArgumentNullException"/> if it is null.
        /// </summary>
        /// <param name="argumentValue">The value to validate.</param>
        /// <param name="argumentName">The name of <paramref name="argumentValue"/>.</param>
		public static void ArgumentNotNull(object argumentValue,
		                                   string argumentName)
		{
			if (argumentValue == null) throw new ArgumentNullException(argumentName);
		}

        /// <summary>
        /// Validates <paramref name="argumentValue"/> is not null or an empty string and throws <see cref="ArgumentNullException"/> if it is null or an empty string .
        /// </summary>
        /// <param name="argumentValue">The value to validate.</param>
        /// <param name="argumentName">The name of <paramref name="argumentValue"/>.</param>
		public static void ArgumentNotNullOrEmpty(string argumentValue,
		                                          string argumentName)
		{
			if (argumentValue == null) throw new ArgumentNullException(argumentName);
			if (argumentValue.Length == 0) throw new ArgumentException(Resources.ProvidedStringArgMustNotBeEmpty, 
                argumentName);
		}
	}
}
