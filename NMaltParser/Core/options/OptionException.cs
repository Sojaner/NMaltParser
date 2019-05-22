using System;

namespace org.maltparser.core.options
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	/// <summary>
	///  OptionException extends the MaltChainedException class and is thrown by classes
	///  within the options package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class OptionException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;

		/// <summary>
		/// Creates an OptionException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public OptionException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates an OptionException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public OptionException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}