using System;

namespace org.maltparser.parser.guide
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	/// <summary>
	///  GuideException extends the MaltChainedException class and is thrown by classes
	///  within the guide package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class GuideException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a GuideException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public GuideException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a GuideException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public GuideException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}