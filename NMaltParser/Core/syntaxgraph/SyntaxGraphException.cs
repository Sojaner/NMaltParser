using NMaltParser.Core.Exception;

namespace NMaltParser.Core.SyntaxGraph
{
    /// <summary>
	///  GraphException extends the MaltChainedException class and is thrown by classes
	///  within the graph package.
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public class SyntaxGraphException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;

		/// <summary>
		/// Creates a GraphException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public SyntaxGraphException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a GraphException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public SyntaxGraphException(string message, System.Exception cause) : base(message, cause)
		{
		}
	}
}