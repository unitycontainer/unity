using System;
using Unity.ServiceLocator.Properties;

namespace Unity.ServiceLocator
{
	/// <summary>
	/// This class provides the ambient container for this application. If your
	/// framework defines such an ambient container, use ServiceLocator.Current
	/// to get it.
	/// </summary>
	public static class ServiceLocator
	{
		private static ServiceLocatorProvider currentProvider;

		/// <summary>
		/// The current ambient container.
		/// </summary>
		public static IServiceLocator Current
		{
			get
			{
				if (!Unity.ServiceLocator.ServiceLocator.IsLocationProviderSet)
				{
					throw new InvalidOperationException(Resources.ServiceLocationProviderNotSetMessage);
				}
				return Unity.ServiceLocator.ServiceLocator.currentProvider();
			}
		}

		public static bool IsLocationProviderSet
		{
			get
			{
				return (object)Unity.ServiceLocator.ServiceLocator.currentProvider != (object)null;
			}
		}

		/// <summary>
		/// Set the delegate that is used to retrieve the current container.
		/// </summary>
		/// <param name="newProvider">Delegate that, when called, will return
		/// the current ambient container.</param>
		public static void SetLocatorProvider(ServiceLocatorProvider newProvider)
		{
			Unity.ServiceLocator.ServiceLocator.currentProvider = newProvider;
		}
	}
}