namespace org.maltparser.core.symbol
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	public interface Table
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int addSymbol(String symbol) throws org.maltparser.core.exception.MaltChainedException;
		int addSymbol(string symbol);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbolCodeToString(int code) throws org.maltparser.core.exception.MaltChainedException;
		string getSymbolCodeToString(int code);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSymbolStringToCode(String symbol) throws org.maltparser.core.exception.MaltChainedException;
		int getSymbolStringToCode(string symbol);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getSymbolStringToValue(String symbol) throws org.maltparser.core.exception.MaltChainedException;
		double getSymbolStringToValue(string symbol);
		string Name {get;}
		int size();
	}

}