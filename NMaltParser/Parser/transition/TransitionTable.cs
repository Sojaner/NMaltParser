using System.Collections.Generic;

namespace org.maltparser.parser.transition
{
    using  core.helper;
	using  core.symbol;
	using  history.container;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class TransitionTable : Table, DecisionPropertyTable
	{
		private readonly string name;
		private readonly SortedDictionary<int, Transition> code2transitionMap;
		private readonly HashMap<string, Transition> symbol2transitionMap;
		private readonly HashMap<Transition, TransitionTable> childrenTables;

		public TransitionTable(string tableName)
		{
			name = tableName;
			code2transitionMap = new SortedDictionary<int, Transition>();
			symbol2transitionMap = new HashMap<string, Transition>();
			childrenTables = new HashMap<Transition, TransitionTable>();
		}

		public virtual void addTransition(int code, string symbol, bool labeled, TransitionTable childrenTable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Transition transition = new Transition(code, symbol, labeled);
			Transition transition = new Transition(code, symbol, labeled);
			code2transitionMap[code] = transition;
			symbol2transitionMap[symbol] = transition;
			childrenTables[transition] = childrenTable;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean continueWithNextDecision(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool continueWithNextDecision(int code)
		{
			if (code2transitionMap.ContainsKey(code))
			{
				return code2transitionMap[code].Labeled;
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean continueWithNextDecision(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool continueWithNextDecision(string symbol)
		{
			if (symbol2transitionMap.ContainsKey(symbol))
			{
				return symbol2transitionMap[symbol].Labeled;
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.Table getTableForNextDecision(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual Table getTableForNextDecision(int code)
		{
			if (code2transitionMap.ContainsKey(code))
			{
				return childrenTables[code2transitionMap[code]];
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.Table getTableForNextDecision(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual Table getTableForNextDecision(string symbol)
		{
			if (symbol2transitionMap.ContainsKey(symbol))
			{
				return childrenTables[symbol2transitionMap[symbol]];
			}
			return null;
		}

		public virtual Transition getTransition(string symbol)
		{
			return symbol2transitionMap[symbol];
		}

		public virtual Transition getTransition(int code)
		{
			return code2transitionMap[code];
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int addSymbol(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual int addSymbol(string symbol)
		{
			return -1;
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbolCodeToString(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getSymbolCodeToString(int code)
		{
			if (code < 0)
			{
				return null;
			}
			return code2transitionMap[code].Symbol;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSymbolStringToCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getSymbolStringToCode(string symbol)
		{
			if (ReferenceEquals(symbol, null))
			{
				return -1;
			}
			return symbol2transitionMap[symbol].Code;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getSymbolStringToValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual double getSymbolStringToValue(string symbol)
		{
			return 1.0;
		}

		public virtual int size()
		{
			return code2transitionMap.Count;
		}

	}

}