using System;

namespace org.maltparser.transform.pseudo
{
	using  org.maltparser.core.exception;

	/// <summary>
	///  TransformationException extends the MaltChainedException class and is thrown by classes
	///  within the transform package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class TransformationException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a TransformationException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public TransformationException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a TransformationException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public TransformationException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}