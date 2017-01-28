using System;
using System.Collections.Generic;

namespace Unity.ServiceLocator
{
	/// <summary>
	/// The generic Service Locator interface. This interface is used
	/// to retrieve services (instances identified by type and optional
	/// name) from a container.
	/// </summary>
	public interface IServiceLocator : IServiceProvider
	{
		/// <summary>
		/// Get all instances of the given <paramref name="serviceType" /> currently
		/// registered in the container.
		/// </summary>
		/// <param name="serviceType">Type of object requested.</param>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is are errors resolving
		/// the service instance.</exception>
		/// <returns>A sequence of instances of the requested <paramref name="serviceType" />.</returns>
		IEnumerable<object> GetAllInstances(Type serviceType);

		/// <summary>
		/// Get all instances of the given <typeparamref name="TService" /> currently
		/// registered in the container.
		/// </summary>
		/// <typeparam name="TService">Type of object requested.</typeparam>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is are errors resolving
		/// the service instance.</exception>
		/// <returns>A sequence of instances of the requested <typeparamref name="TService" />.</returns>
		IEnumerable<TService> GetAllInstances<TService>();

		/// <summary>
		/// Get an instance of the given <paramref name="serviceType" />.
		/// </summary>
		/// <param name="serviceType">Type of object requested.</param>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is an error resolving
		/// the service instance.</exception>
		/// <returns>The requested service instance.</returns>
		object GetInstance(Type serviceType);

		/// <summary>
		/// Get an instance of the given named <paramref name="serviceType" />.
		/// </summary>
		/// <param name="serviceType">Type of object requested.</param>
		/// <param name="key">Name the object was registered with.</param>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is an error resolving
		/// the service instance.</exception>
		/// <returns>The requested service instance.</returns>
		object GetInstance(Type serviceType, string key);

		/// <summary>
		/// Get an instance of the given <typeparamref name="TService" />.
		/// </summary>
		/// <typeparam name="TService">Type of object requested.</typeparam>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is are errors resolving
		/// the service instance.</exception>
		/// <returns>The requested service instance.</returns>
		TService GetInstance<TService>();

		/// <summary>
		/// Get an instance of the given named <typeparamref name="TService" />.
		/// </summary>
		/// <typeparam name="TService">Type of object requested.</typeparam>
		/// <param name="key">Name the object was registered with.</param>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is are errors resolving
		/// the service instance.</exception>
		/// <returns>The requested service instance.</returns>
		TService GetInstance<TService>(string key);
	}
}