using System;

namespace org.maltparser.core.plugin
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	/// <summary>
	///  PluginException extends the MaltChainedException class and is thrown by classes
	///  within the plugin package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class PluginException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;

		/// <summary>
		/// Creates a PluginException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public PluginException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a PluginException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public PluginException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}