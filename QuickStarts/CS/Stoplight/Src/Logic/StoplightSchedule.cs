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
using Microsoft.Practices.Unity;
using StopLight.ServiceImplementations;
using StopLight.ServiceInterfaces;

namespace StopLight.Logic
{
	public class StoplightSchedule
	{
		private IStoplightTimer timer;
		private ILogger logger = new NullLogger();
		private TimeSpan[] lightTimes = new TimeSpan[3];
		private int currentLight = 0;

		public event EventHandler ChangeLight;

		public StoplightSchedule(IStoplightTimer timer)
		{
			this.timer = timer;
			timer.Expired += OnTimerExpired;
		}

		[Dependency]
		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		public void Start()
		{
			timer.Start();
		}

		public void Update(TimeSpan green, TimeSpan yellow, TimeSpan red)
		{
			lightTimes[0] = green;
			lightTimes[1] = yellow;
			lightTimes[2] = red;

			logger.Write(string.Format("UPDATE SCHEDULE: {0} {1} {2}", green, yellow, red));
		}

		public void ForceChange()
		{
			OnTimerExpired(this, EventArgs.Empty);
			logger.Write(string.Format("FORCED CHANGE"));
		}

		private void OnTimerExpired(object sender, EventArgs e)
		{
			EventHandler handlers = ChangeLight;
			if(handlers != null)
			{
				handlers(this, EventArgs.Empty);
			}
			currentLight = ( currentLight + 1 ) % 3;
			timer.Duration = lightTimes[currentLight];
			timer.Start();
		}
	}
}
