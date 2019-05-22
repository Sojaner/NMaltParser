namespace org.maltparser.core.syntaxgraph
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface Structure
	{
		/// <summary>
		/// Resets the structure.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException;
		void clear();
	}

}