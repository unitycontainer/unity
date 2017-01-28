using System;

namespace Unity.ServiceLocator.Properties
{
	/// <summary>
	/// Resources
	/// </summary>
	public static class Resources
	{
		/// <summary>
		/// Message shown if an exception occurs during a GetAllInstances call
		/// </summary>
		public static string ActivateAllExceptionMessage
		{
			get
			{
				return "Activation error occurred while trying to get all instances of type {0}";
			}
		}

		/// <summary>
		/// Message shown on exception in GetInstance method
		/// </summary>
		public static string ActivationExceptionMessage
		{
			get
			{
				return "Activation error occurred while trying to get instance of type {0}, key \"{ 1} \"";
			}
		}

		/// <summary>
		/// Message shown if ServiceLocator. Current called before Unity.ServiceLocatorProvider is set.
		/// </summary>
		public static string ServiceLocationProviderNotSetMessage
		{
			get
			{
				return "ServiceLocationProvider must be set.";
			}
		}
	}
}