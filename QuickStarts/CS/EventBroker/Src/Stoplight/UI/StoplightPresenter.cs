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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Microsoft.Practices.Unity;
using SimpleEventBroker;
using StopLight.Logic;

namespace StopLight.UI
{
	public class StoplightPresenter
	{
		private Stoplight stoplight;
		private StoplightSchedule schedule;

		[Dependency]
		public Stoplight Stoplight
		{
			get { return stoplight; }
			set { stoplight = value; }
		}

		[Dependency]
		public StoplightSchedule Schedule
		{
			get { return schedule; }
			set { schedule = value; }
		}

		private IStoplightView view;

		public void SetView(IStoplightView view)
		{
			this.view = view;
			view.PropertyChanged += OnViewPropertyChanged;
			view.UpdateClicked += OnViewUpdateClicked;
			view.ForceChangeClicked += OnViewForceChangeClicked;

			Stoplight.Changed += OnStoplightChanged;

			view.GreenDuration = "3000";
			view.YellowDuration = "500";
			view.RedDuration = "5000";
			view.CurrentColor = Color.Green;

			Schedule.Update(TimeSpan.FromMilliseconds(3000),
						 TimeSpan.FromMilliseconds(500),
						 TimeSpan.FromMilliseconds(5000));
			Schedule.Start();
		}

        [SubscribesTo("ChangeLight")]
        public void OnScheduledLightChange(object sender, EventArgs e)
        {
            Stoplight.Next();
        }

		private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			string newDurationValue = null;
			int newDurationInMilliseconds;

			if (e.PropertyName == StoplightViewProperties.CurrentColor)
			{
				return;
			}

			ActOnProperty(e.PropertyName,
				delegate { newDurationValue = view.GreenDuration; },
				delegate { newDurationValue = view.YellowDuration; },
				delegate { newDurationValue = view.RedDuration; });

			if (!int.TryParse(newDurationValue, out newDurationInMilliseconds))
			{
				view.SetError(e.PropertyName, "Duration must be an integer");
			}
			else
			{
				view.SetError(e.PropertyName, null);
			}
		}

		private delegate void Action();

		private void ActOnProperty(
			string propertyName, Action greenAction, Action yellowAction, Action redAction)
		{
			switch (propertyName)
			{
			case StoplightViewProperties.GreenDuration:
				greenAction();
				break;

			case StoplightViewProperties.YellowDuration:
				yellowAction();
				break;

			case StoplightViewProperties.RedDuration:
				redAction();
				break;

			case StoplightViewProperties.CurrentColor:
				return;
			}
		}

		private void OnViewForceChangeClicked(object sender, EventArgs e)
		{
			Schedule.ForceChange();
		}

		private void OnViewUpdateClicked(object sender, EventArgs e)
		{
			string[] durationsAsString = new string[3];
			List<string> propNames = new List<string>(new string[]
			{
				StoplightViewProperties.GreenDuration,
				StoplightViewProperties.YellowDuration,
				StoplightViewProperties.RedDuration
			});

			foreach (string propName in propNames)
			{
				ActOnProperty(propName,
				delegate { durationsAsString[0] = view.GreenDuration; },
				delegate { durationsAsString[1] = view.YellowDuration; },
				delegate { durationsAsString[2] = view.RedDuration; }
				);
			}

			TimeSpan[] timeSpans = Array.ConvertAll<string, TimeSpan>(durationsAsString,
			delegate(string s)
			{
				return
				TimeSpan.FromMilliseconds(
				int.Parse(s));
			});
			Schedule.Update(timeSpans[0], timeSpans[1], timeSpans[2]);

		}

		private void OnStoplightChanged(object sender, LightChangedEventArgs e)
		{
			switch (e.CurrentColor)
			{
			case StoplightColors.Green:
				view.CurrentColor = Color.Green;
				break;

			case StoplightColors.Yellow:
				view.CurrentColor = Color.Yellow;
				break;

			case StoplightColors.Red:
				view.CurrentColor = Color.Red;
				break;
			}
		}
	}
}
