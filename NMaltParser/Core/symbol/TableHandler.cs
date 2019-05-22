namespace org.maltparser.core.symbol
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	public interface TableHandler
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Table getSymbolTable(String tableName) throws org.maltparser.core.exception.MaltChainedException;
		Table getSymbolTable(string tableName);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Table addSymbolTable(String tableName) throws org.maltparser.core.exception.MaltChainedException;
		Table addSymbolTable(string tableName);
	}

}