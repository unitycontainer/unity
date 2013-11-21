// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
