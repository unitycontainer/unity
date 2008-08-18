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

using StopLight.ServiceInterfaces;

namespace StopLightTests.TestDoubles
{
	internal class MockLogger : ILogger
	{
		private string lastMessage;

		public string LastMessage
		{
			get { return lastMessage; }
			private set { lastMessage = value; }
		}

		#region ILogger Members

		public void Write(string message)
		{
			LastMessage = message;
		}

		#endregion
	}
}
