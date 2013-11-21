// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Windows.Forms;
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
				.RegisterType<ILogger, TraceLogger>()
				.RegisterType<IStoplightTimer, RealTimeTimer>();

			Application.Run(container.Resolve<StoplightForm>());
		}
	}
}
