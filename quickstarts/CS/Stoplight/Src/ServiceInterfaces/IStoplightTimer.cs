// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace StopLight.ServiceInterfaces
{
	public interface IStoplightTimer
	{
		TimeSpan Duration { get; set; }
		void Start();
		event EventHandler Expired;
	}
}
