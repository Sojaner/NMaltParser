using System;
using NMaltParser.Core.Exception;

namespace NMaltParser.ML.Lib
{
    /// <summary>
	///  LibException extends the MaltChainedException class and is thrown by classes
	///  within the lib package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class LibException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a LibException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public LibException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a LibException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public LibException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}