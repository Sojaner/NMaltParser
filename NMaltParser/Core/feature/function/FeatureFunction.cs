namespace org.maltparser.core.feature.function
{
	using  exception;
	using  value;
	using  symbol;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface FeatureFunction : Function
	{
		/// <summary>
		/// Returns the string representation of the integer <code>code</code> according to the feature function. 
		/// </summary>
		/// <param name="code"> the integer representation of the symbol </param>
		/// <returns> the string representation of the integer <code>code</code> according to the feature function. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract String getSymbol(int code) throws org.maltparser.core.exception.MaltChainedException;
		string getSymbol(int code);
		/// <summary>
		/// Returns the integer representation of the string <code>symbol</code> according to the feature function.
		/// </summary>
		/// <param name="symbol"> the string representation of the symbol </param>
		/// <returns> the integer representation of the string <code>symbol</code> according to the feature function. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int getCode(String symbol) throws org.maltparser.core.exception.MaltChainedException;
		int getCode(string symbol);
		/// <summary>
		/// Returns the symbol table used by the feature function.
		/// </summary>
		/// <returns> the symbol table used by the feature function. </returns>
		SymbolTable SymbolTable {get;}
		/// <summary>
		/// Returns the feature value
		/// </summary>
		/// <returns> the feature value </returns>
		FeatureValue FeatureValue {get;}

		int Type {get;}
		string MapIdentifier {get;}
	}

}