using NMaltParser.Core.Exception;

namespace NMaltParser.Core.Symbol
{
    /// <summary>
	///  SymbolException extends the MaltChainedException class and is thrown by classes
	///  within the symbol package.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class SymbolException : MaltChainedException
	{
		public new const long serialVersionUID = 8045568022124816379L;
		/// <summary>
		/// Creates a SymbolException object with a message
		/// </summary>
		/// <param name="message">	the message </param>
		public SymbolException(string message) : base(message)
		{
		}
		/// <summary>
		/// Creates a SymbolException object with a message and a cause to the exception.
		/// </summary>
		/// <param name="message">	the message </param>
		/// <param name="cause">		the cause to the exception </param>
		public SymbolException(string message, System.Exception cause) : base(message, cause)
		{
		}
	}

}