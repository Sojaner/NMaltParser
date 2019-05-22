using System;

namespace org.maltparser.ml.cheater
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	/// <summary>
	///  CheaterException extends the MaltChainedException class and is thrown by classes
	///  within the cheater package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class CheaterException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a CheaterException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public CheaterException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a CheaterException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public CheaterException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}