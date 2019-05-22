using System;

namespace org.maltparser.parser
{
	using  org.maltparser.core.exception;

	/// <summary>
	///  ParsingException extends the MaltChainedException class and is thrown by classes
	///  within the parser.algorithm package.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ParsingException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a ParsingException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public ParsingException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a ParsingException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public ParsingException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}