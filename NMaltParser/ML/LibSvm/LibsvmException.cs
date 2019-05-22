using System;
using NMaltParser.Core.Exception;

namespace NMaltParser.ML.LibSvm
{
    /// <summary>
	///  LibsvmException extends the MaltChainedException class and is thrown by classes
	///  within the libsvm package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class LibsvmException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a LibsvmException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public LibsvmException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a LibsvmException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public LibsvmException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}