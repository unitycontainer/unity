using System;

namespace Unity.ServiceLocator
{
	/// <summary>
	/// The standard exception thrown when a ServiceLocator has an error in resolving an object.
	/// </summary>
	public class ActivationException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception" /> class.
		/// </summary>
		public ActivationException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error. 
		///  </param>
		public ActivationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception. 
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. 
		/// </param>
		public ActivationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}