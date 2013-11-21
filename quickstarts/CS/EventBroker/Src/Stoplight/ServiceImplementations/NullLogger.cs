// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using StopLight.ServiceInterfaces;

namespace StopLight.ServiceImplementations
{
	internal class NullLogger : ILogger
	{
		#region ILogger Members

		public void Write(string message)
		{
		}

		#endregion
	}
}
