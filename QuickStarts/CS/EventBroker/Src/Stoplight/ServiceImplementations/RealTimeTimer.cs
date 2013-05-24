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
using System.Windows.Forms;
using SimpleEventBroker;
using StopLight.ServiceInterfaces;

namespace StopLight.ServiceImplementations
{
	internal class RealTimeTimer : IStoplightTimer
	{
		private Timer timer;

        [Publishes("TimerTick")]
		public event EventHandler Expired;

		public RealTimeTimer()
		{
			timer = new Timer();
			timer.Tick += OnTick;
		}

		public TimeSpan Duration
		{
			get { return TimeSpan.FromMilliseconds(timer.Interval); }
			set { timer.Interval = (int)value.TotalMilliseconds; }
		}

		public void Start()
		{
			timer.Start();
		}

		private void OnTick(object sender, EventArgs e)
		{
			timer.Stop();
			OnExpired(this);
		}

		protected virtual void OnExpired(object sender)
		{
			EventHandler handlers = Expired;
			if(handlers != null)
			{
				handlers(this, EventArgs.Empty);
			}
		}
	}
}
