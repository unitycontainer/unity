// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using StopLight.ServiceInterfaces;

namespace StopLight.ServiceImplementations
{
	internal class TraceLogger : ILogger
	{
		#region ILogger Members

		public void Write(string message)
		{
			Trace.WriteLine(string.Format("{0}: {1}",
			                              DateTime.Now,
			                              message));
		}

		#endregion
	}
}
