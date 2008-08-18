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
using EventBrokerExtension;
using Microsoft.Practices.Unity;
using StopLight.ServiceImplementations;
using StopLight.ServiceInterfaces;
using StopLight.UI;

namespace StopLight
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			IUnityContainer container = new UnityContainer()
                .AddNewExtension<SimpleEventBrokerExtension>()
				.RegisterType<ILogger, TraceLogger>()
				.RegisterType<IStoplightTimer, RealTimeTimer>();

			Application.Run(container.Resolve<StoplightForm>());
		}
	}
}
