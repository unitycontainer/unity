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
