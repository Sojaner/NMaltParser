using System.IO;

namespace NMaltParser.Core.Symbol
{
    public interface SymbolTable : Table
	{
		// Categories

		// Types


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(java.io.BufferedWriter out) throws org.maltparser.core.exception.MaltChainedException;
		void save(StreamWriter @out);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.io.BufferedReader in) throws org.maltparser.core.exception.MaltChainedException;
		void load(StreamReader @in);
		int ValueCounter {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getNullValueCode(org.maltparser.core.symbol.nullvalue.NullValues.NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException;
		int getNullValueCode(NullValueId nullValueIdentifier);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNullValueSymbol(org.maltparser.core.symbol.nullvalue.NullValues.NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException;
		string getNullValueSymbol(NullValueId nullValueIdentifier);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isNullValue(String value) throws org.maltparser.core.exception.MaltChainedException;
		bool isNullValue(string value);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isNullValue(int code) throws org.maltparser.core.exception.MaltChainedException;
		bool isNullValue(int code);
	}

	public static class SymbolTable_Fields
	{
		public const int NA = -1;
		public const int INPUT = 1;
		public const int OUTPUT = 3;
		public static readonly string[] categories = new string[] {"", "INPUT", "", "OUTPUT"};
		public const int STRING = 1;
		public const int INTEGER = 2;
		public const int BOOLEAN = 3;
		public const int REAL = 4;
		public static readonly string[] types = new string[] {"", "STRING", "INTEGER", "BOOLEAN", "REAL"};
	}

}