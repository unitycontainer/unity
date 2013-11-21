// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;
using StopLight.ServiceInterfaces;

namespace StopLight.ServiceImplementations
{
	internal class RealTimeTimer : IStoplightTimer
	{
		private Timer timer;

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
