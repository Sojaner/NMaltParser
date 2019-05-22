using System.Text;
using NMaltParser.Core.Symbol;

namespace NMaltParser.Parser.History.Container
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ActionContainer
	{
		private readonly Table table;
		private readonly string name;
		private int actionCode;
		private string actionSymbol;


		public ActionContainer(TableContainer tableContainer)
		{
			table = tableContainer.Table;
			name = tableContainer.TableContainerName;
			clear();
		}

		public virtual void clear()
		{
			actionCode = -1;
			actionSymbol = null;
		}

		public virtual string ActionSymbol
		{
			get
			{
				return actionSymbol;
			}
		}

		public virtual int ActionCode
		{
			get
			{
				return actionCode;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String setAction(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual string setAction(int code)
		{
			if (actionCode != code)
			{
				if (code < 0)
				{
					clear();
				}
				else
				{
					actionSymbol = table.getSymbolCodeToString(code);
					if (ReferenceEquals(actionSymbol, null))
					{
						clear();
					}
					else
					{
						actionCode = code;
					}
				}
			}
			return actionSymbol;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int setAction(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual int setAction(string symbol)
		{
			if (ReferenceEquals(symbol, null))
			{
				clear();
			}
			else
			{
				actionCode = table.getSymbolStringToCode(symbol);
				if (actionCode == -1)
				{
					clear();
				}
				else
				{
					actionSymbol = symbol;
				}
			}
			return actionCode;
		}

		public virtual Table Table
		{
			get
			{
				return table;
			}
		}

		public virtual string TableName
		{
			get
			{
				if (table == null)
				{
					return null;
				}
				return table.Name;
			}
		}

		public virtual string TableContainerName
		{
			get
			{
				return name;
			}
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(name);
			sb.Append(" -> ");
			sb.Append(actionSymbol);
			sb.Append(" = ");
			sb.Append(actionCode);
			return sb.ToString();
		}
	}

}