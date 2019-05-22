using NMaltParser.Core.Helper;
using NMaltParser.Core.Symbol;

namespace NMaltParser.Parser.Transition
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class TransitionTableHandler : TableHandler
	{
		private readonly HashMap<string, TransitionTable> transitionTables;

		public TransitionTableHandler()
		{
			transitionTables = new HashMap<string, TransitionTable>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.Table addSymbolTable(String tableName) throws org.maltparser.core.exception.MaltChainedException
		public virtual Table addSymbolTable(string tableName)
		{
			TransitionTable table = transitionTables[tableName];
			if (table == null)
			{
				table = new TransitionTable(tableName);
				transitionTables[tableName] = table;
			}
			return table;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.Table getSymbolTable(String tableName) throws org.maltparser.core.exception.MaltChainedException
		public virtual Table getSymbolTable(string tableName)
		{
			return transitionTables[tableName];
		}
	}

}