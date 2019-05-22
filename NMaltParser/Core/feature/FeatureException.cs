using System;

namespace org.maltparser.core.feature
{
	using  exception;

	/// <summary>
	///  FeatureException extends the MaltChainedException class and is thrown by classes
	///  within the feature package.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class FeatureException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a FeatureException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public FeatureException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a FeatureException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public FeatureException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}