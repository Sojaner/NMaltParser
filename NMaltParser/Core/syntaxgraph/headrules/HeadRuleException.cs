using System;

namespace org.maltparser.core.syntaxgraph.headrules
{
	using  exception;

	/// <summary>
	///  HeadRuleException extends the MaltChainedException class and is thrown by classes
	///  within the headrule package.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class HeadRuleException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;

		/// <summary>
		/// Creates a HeadRuleException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public HeadRuleException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a HeadRuleException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public HeadRuleException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}