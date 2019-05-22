using System;

namespace org.maltparser.concurrent.graph
{
	using  org.maltparser.core.exception;

	/// <summary>
	///  ConcurrentGraphException extends the MaltChainedException class and is thrown by classes
	///  within the graph package.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ConcurrentGraphException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;

		/// <summary>
		/// Creates a ConcurrentGraphException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public ConcurrentGraphException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a ConcurrentGraphException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public ConcurrentGraphException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}