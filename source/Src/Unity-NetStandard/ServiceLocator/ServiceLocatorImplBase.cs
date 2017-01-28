using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.ServiceLocator.Properties;

namespace Unity.ServiceLocator
{
	/// <summary>
	/// This class is a helper that provides a default implementation
	/// for most of the methods of <see cref="T:Unity.ServiceLocator.IServiceLocator" />.
	/// </summary>
	public abstract class ServiceLocatorImplBase : IServiceLocator, IServiceProvider
	{
		protected ServiceLocatorImplBase()
		{
		}

		/// <summary>
		/// When implemented by inheriting classes, this method will do the actual work of
		/// resolving all the requested service instances.
		/// </summary>
		/// <param name="serviceType">Type of service requested.</param>
		/// <returns>Sequence of service instance objects.</returns>
		protected abstract IEnumerable<object> DoGetAllInstances(Type serviceType);

		/// <summary>
		/// When implemented by inheriting classes, this method will do the actual work of resolving
		/// the requested service instance.
		/// </summary>
		/// <param name="serviceType">Type of instance requested.</param>
		/// <param name="key">Name of registered service you want. May be null.</param>
		/// <returns>The requested service instance.</returns>
		protected abstract object DoGetInstance(Type serviceType, string key);

		/// <summary>
		/// Format the exception message for use in an <see cref="T:Unity.ServiceLocator.ActivationException" />
		/// that occurs while resolving multiple service instances.
		/// </summary>
		/// <param name="actualException">The actual exception thrown by the implementation.</param>
		/// <param name="serviceType">Type of service requested.</param>
		/// <returns>The formatted exception message string.</returns>
		protected virtual string FormatActivateAllExceptionMessage(Exception actualException, Type serviceType)
		{
			return string.Format(CultureInfo.CurrentUICulture, Resources.ActivateAllExceptionMessage, serviceType.Name);
		}

		/// <summary>
		/// Format the exception message for use in an <see cref="T:Unity.ServiceLocator.ActivationException" />
		/// that occurs while resolving a single service.
		/// </summary>
		/// <param name="actualException">The actual exception thrown by the implementation.</param>
		/// <param name="serviceType">Type of service requested.</param>
		/// <param name="key">Name requested.</param>
		/// <returns>The formatted exception message string.</returns>
		protected virtual string FormatActivationExceptionMessage(Exception actualException, Type serviceType, string key)
		{
			return string.Format(CultureInfo.CurrentUICulture, Resources.ActivationExceptionMessage, serviceType.Name, key);
		}

		/// <summary>
		/// Get all instances of the given <paramref name="serviceType" /> currently
		/// registered in the container.
		/// </summary>
		/// <param name="serviceType">Type of object requested.</param>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is are errors resolving
		/// the service instance.</exception>
		/// <returns>A sequence of instances of the requested <paramref name="serviceType" />.</returns>
		public virtual IEnumerable<object> GetAllInstances(Type serviceType)
		{
			IEnumerable<object> enumerable;
			try
			{
				enumerable = this.DoGetAllInstances(serviceType);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new ActivationException(this.FormatActivateAllExceptionMessage(exception, serviceType), exception);
			}
			return enumerable;
		}

		/// <summary>
		/// Get all instances of the given <typeparamref name="TService" /> currently
		/// registered in the container.
		/// </summary>
		/// <typeparam name="TService">Type of object requested.</typeparam>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is are errors resolving
		/// the service instance.</exception>
		/// <returns>A sequence of instances of the requested <typeparamref name="TService" />.</returns>
		public virtual IEnumerable<TService> GetAllInstances<TService>()
		{
			foreach (object allInstance in this.GetAllInstances(typeof(TService)))
			{
				yield return (TService)allInstance;
			}
		}

		/// <summary>
		/// Get an instance of the given <paramref name="serviceType" />.
		/// </summary>
		/// <param name="serviceType">Type of object requested.</param>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is an error resolving
		/// the service instance.</exception>
		/// <returns>The requested service instance.</returns>
		public virtual object GetInstance(Type serviceType)
		{
			return this.GetInstance(serviceType, null);
		}

		/// <summary>
		/// Get an instance of the given named <paramref name="serviceType" />.
		/// </summary>
		/// <param name="serviceType">Type of object requested.</param>
		/// <param name="key">Name the object was registered with.</param>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is an error resolving
		/// the service instance.</exception>
		/// <returns>The requested service instance.</returns>
		public virtual object GetInstance(Type serviceType, string key)
		{
			object obj;
			try
			{
				obj = this.DoGetInstance(serviceType, key);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new ActivationException(this.FormatActivationExceptionMessage(exception, serviceType, key), exception);
			}
			return obj;
		}

		/// <summary>
		/// Get an instance of the given <typeparamref name="TService" />.
		/// </summary>
		/// <typeparam name="TService">Type of object requested.</typeparam>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is are errors resolving
		/// the service instance.</exception>
		/// <returns>The requested service instance.</returns>
		public virtual TService GetInstance<TService>()
		{
			return (TService)this.GetInstance(typeof(TService), null);
		}

		/// <summary>
		/// Get an instance of the given named <typeparamref name="TService" />.
		/// </summary>
		/// <typeparam name="TService">Type of object requested.</typeparam>
		/// <param name="key">Name the object was registered with.</param>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is are errors resolving
		/// the service instance.</exception>
		/// <returns>The requested service instance.</returns>
		public virtual TService GetInstance<TService>(string key)
		{
			return (TService)this.GetInstance(typeof(TService), key);
		}

		/// <summary>
		/// Implementation of <see cref="M:System.IServiceProvider.GetService(System.Type)" />.
		/// </summary>
		/// <param name="serviceType">The requested service.</param>
		/// <exception cref="T:Unity.ServiceLocator.ActivationException">if there is an error in resolving the service instance.</exception>
		/// <returns>The requested object.</returns>
		public virtual object GetService(Type serviceType)
		{
			return this.GetInstance(serviceType, null);
		}
	}
}