// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using StopLight.ServiceInterfaces;

namespace StopLightTests.TestDoubles
{
	internal class MockStoplightTimer : IStoplightTimer
	{
		private TimeSpan duration;

		#region IStoplightTimer Members

		public TimeSpan Duration
		{
			get { return duration; }
			set { duration = value; }
		}

		public void Start()
		{
		}

		public event EventHandler Expired;

		#endregion

		public void Expire()
		{
			if(Expired != null)
			{
				Expired(this, EventArgs.Empty);
			}
		}
	}
}
