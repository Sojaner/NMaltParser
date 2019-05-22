using System;

namespace org.maltparser.ml.liblinear
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	/// <summary>
	///  LiblinearException extends the MaltChainedException class and is thrown by classes
	///  within the libsvm package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class LiblinearException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a LiblinearException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public LiblinearException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a LiblinearException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public LiblinearException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}