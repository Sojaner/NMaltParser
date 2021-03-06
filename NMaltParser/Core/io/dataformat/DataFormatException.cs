﻿using NMaltParser.Core.Exception;

namespace NMaltParser.Core.IO.DataFormat
{
    /// <summary>
	///  DataFormatException extends the MaltChainedException class and is thrown by classes
	///  within the dataformat package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class DataFormatException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;

		/// <summary>
		/// Creates a DataFormatException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public DataFormatException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a DataFormatException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public DataFormatException(string message, System.Exception cause) : base(message, cause)
		{
		}
	}
}