using System;

namespace Unity.ServiceLocator
{
	/// <summary>
	/// This delegate type is used to provide a method that will
	/// return the current container. Used with the <see cref="T:Unity.ServiceLocator.ServiceLocator" />
	/// static accessor class.
	/// </summary>
	/// <returns>An <see cref="T:Unity.ServiceLocator.IServiceLocator" />.</returns>
	public delegate IServiceLocator ServiceLocatorProvider();
}