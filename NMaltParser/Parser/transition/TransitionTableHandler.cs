﻿namespace org.maltparser.parser.transition
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.helper;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.symbol;
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