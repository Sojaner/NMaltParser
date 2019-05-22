using System.Text;
using NMaltParser.Core.Symbol;

namespace NMaltParser.Parser.History.Container
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class TableContainer
	{
		public enum RelationToNextDecision
		{
			COMBINED,
			SEQUANTIAL,
			BRANCHED,
			SWITCHED,
			NONE
		}
		protected internal int cachedCode;
		protected internal readonly StringBuilder cachedSymbol;
		protected internal Table table;
		protected internal string name;
		private readonly RelationToNextDecision relationToNextDecision;

		public TableContainer(Table _table, string _name, char _decisionSeparator)
		{
			table = _table;
			name = _name;
			switch (_decisionSeparator)
			{
			case '+':
				relationToNextDecision = RelationToNextDecision.COMBINED;
				break;
			case ',':
				relationToNextDecision = RelationToNextDecision.SEQUANTIAL;
				break;
			case ';':
				relationToNextDecision = RelationToNextDecision.BRANCHED;
				break;
			case '#':
				relationToNextDecision = RelationToNextDecision.BRANCHED;
				break;
			case '?':
				relationToNextDecision = RelationToNextDecision.SWITCHED;
				break;
			default:
				relationToNextDecision = RelationToNextDecision.NONE;
			break;
			}
			cachedSymbol = new StringBuilder();
			cachedCode = -1;
		}


		public virtual void clearCache()
		{
			cachedCode = -1;
			cachedSymbol.Length = 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbol(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getSymbol(int code)
		{
			if (code < 0 && !containCode(code))
			{
				clearCache();
				return null;
			}
			if (cachedCode != code)
			{
				clearCache();
				cachedCode = code;
				cachedSymbol.Append(table.getSymbolCodeToString(cachedCode));
			}
			return cachedSymbol.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getCode(string symbol)
		{
			if (cachedSymbol == null || !cachedSymbol.Equals(symbol))
			{
				clearCache();
				cachedSymbol.Append(symbol);
				cachedCode = table.getSymbolStringToCode(symbol);
			}
			return cachedCode;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean containCode(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool containCode(int code)
		{
			if (cachedCode != code)
			{
				clearCache();
				cachedSymbol.Append(table.getSymbolCodeToString(code));
				if (cachedSymbol == null)
				{
					return false;
				}
				cachedCode = code;
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean containSymbol(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool containSymbol(string symbol)
		{
			if (cachedSymbol == null || !cachedSymbol.Equals(symbol))
			{
				clearCache();
				cachedCode = table.getSymbolStringToCode(symbol);
				if (cachedCode < 0)
				{
					return false;
				}
				cachedSymbol.Append(symbol);
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean continueWithNextDecision(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool continueWithNextDecision(int code)
		{
			if (table is DecisionPropertyTable)
			{
				return ((DecisionPropertyTable)table).continueWithNextDecision(code);
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean continueWithNextDecision(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool continueWithNextDecision(string symbol)
		{
			if (table is DecisionPropertyTable)
			{
				return ((DecisionPropertyTable)table).continueWithNextDecision(symbol);
			}
			return true;
		}

		public virtual Table Table
		{
			get
			{
				return table;
			}
			set
			{
				table = value;
			}
		}

		public virtual string TableName
		{
			get
			{
				return table != null?table.Name:null;
			}
		}

		public virtual string TableContainerName
		{
			get
			{
				return name;
			}
		}

		public virtual RelationToNextDecision getRelationToNextDecision()
		{
			return relationToNextDecision;
		}


		protected internal virtual string Name
		{
			set
			{
				name = value;
			}
		}

		public virtual int size()
		{
			return table.size();
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(name);
			sb.Append(" -> ");
			sb.Append(cachedSymbol);
			sb.Append(" = ");
			sb.Append(cachedCode);
			return sb.ToString();
		}
	}

}