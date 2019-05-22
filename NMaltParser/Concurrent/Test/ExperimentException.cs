using System;

namespace org.maltparser.concurrent.test
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	/// <summary>
	///  ExperimentException extends the MaltChainedException class and is thrown by classes
	///  within the graph package.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ExperimentException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;

		/// <summary>
		/// Creates a ExperimentException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public ExperimentException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a ExperimentException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public ExperimentException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}