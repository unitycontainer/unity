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
