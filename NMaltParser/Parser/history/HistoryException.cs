using System;
using NMaltParser.Core.Exception;

namespace NMaltParser.Parser.History
{
    /// <summary>
	///  HistoryException extends the MaltChainedException class and is thrown by classes
	///  within the history package.
	/// 
	/// @author Johan Hall
	/// @since 1.1
	/// 
	/// </summary>
	public class HistoryException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a HistoryException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public HistoryException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a HistoryException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public HistoryException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}