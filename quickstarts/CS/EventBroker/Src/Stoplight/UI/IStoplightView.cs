// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Drawing;

namespace StopLight.UI
{
	public interface IStoplightView : INotifyPropertyChanged
	{
		Color CurrentColor { get; set; }

		string GreenDuration { get; set; }
		string YellowDuration { get; set; }
		string RedDuration { get; set; }

		event EventHandler UpdateClicked;
		event EventHandler ForceChangeClicked;

		void SetError(string propertyName, string errorMessage);
	}

	public static class StoplightViewProperties
	{
		public const string CurrentColor = "CurrentColor";
		public const string GreenDuration = "GreenDuration";
		public const string YellowDuration = "YellowDuration";
		public const string RedDuration = "RedDuration";
	}
}
