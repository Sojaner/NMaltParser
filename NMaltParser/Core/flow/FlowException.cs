using System;

namespace org.maltparser.core.flow
{
	using  org.maltparser.core.exception;
	/// <summary>
	///  FlowException extends the MaltChainedException class and is thrown by classes
	///  within the flow package.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class FlowException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a FlowException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public FlowException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a FlowException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public FlowException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}