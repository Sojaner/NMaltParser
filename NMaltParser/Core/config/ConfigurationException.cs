using System;

namespace org.maltparser.core.config
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	/// <summary>
	///  ConfigurationException extends the MaltChainedException class and is thrown by classes
	///  within the configuration package.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ConfigurationException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;

		/// <summary>
		/// Creates a ConfigurationException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public ConfigurationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a ConfigurationException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public ConfigurationException(string message, Exception cause) : base(message, cause)
		{
		}
	}


}