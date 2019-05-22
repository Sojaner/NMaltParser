﻿using System;

namespace org.maltparser.core.propagation
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	/// <summary>
	///  PropagationException extends the MaltChainedException class and is thrown by classes
	///  within the propagation package.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class PropagationException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a PropagationException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public PropagationException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a PropagationException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public PropagationException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}