using System.Collections.Generic;
using System.IO;

namespace NMaltParser.Core.Symbol
{
    public interface SymbolTableHandler : TableHandler
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SymbolTable addSymbolTable(String tableName) throws org.maltparser.core.exception.MaltChainedException;
		SymbolTable addSymbolTable(string tableName);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SymbolTable addSymbolTable(String tableName, SymbolTable parentTable) throws org.maltparser.core.exception.MaltChainedException;
		SymbolTable addSymbolTable(string tableName, SymbolTable parentTable);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SymbolTable addSymbolTable(String tableName, int columnCategory, int columnType, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException;
		SymbolTable addSymbolTable(string tableName, int columnCategory, int columnType, string nullValueStrategy);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SymbolTable getSymbolTable(String tableName) throws org.maltparser.core.exception.MaltChainedException;
		SymbolTable getSymbolTable(string tableName);
		ISet<string> SymbolTableNames {get;}
		void cleanUp();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(java.io.OutputStreamWriter osw) throws org.maltparser.core.exception.MaltChainedException;
		void save(StreamWriter osw);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(String fileName, String charSet) throws org.maltparser.core.exception.MaltChainedException;
		void save(string fileName, string charSet);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.io.InputStreamReader isr) throws org.maltparser.core.exception.MaltChainedException;
		void load(StreamReader isr);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(String fileName, String charSet) throws org.maltparser.core.exception.MaltChainedException;
		void load(string fileName, string charSet);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SymbolTable loadTagset(String fileName, String tableName, String charSet, int columnCategory, int columnType, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException;
		SymbolTable loadTagset(string fileName, string tableName, string charSet, int columnCategory, int columnType, string nullValueStrategy);
	}

}