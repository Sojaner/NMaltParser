using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Symbol;

namespace NMaltParser.Parser.History.Container
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class CombinedTableContainer : TableContainer, Table
	{
		private readonly TableHandler tableHandler;
		private readonly char separator;
		private readonly TableContainer[] containers;
		private readonly StringBuilder[] cachedSymbols;
		private readonly int[] cachedCodes;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CombinedTableContainer(org.maltparser.core.symbol.TableHandler _tableHandler, String _separator, java.util.List<TableContainer> _containers, char decisionSeparator) throws org.maltparser.core.exception.MaltChainedException
		public CombinedTableContainer(TableHandler _tableHandler, string _separator, IList<TableContainer> _containers, char decisionSeparator) : base(null, null, decisionSeparator)
		{
			tableHandler = _tableHandler;
			if (_separator.Length > 0)
			{
				separator = _separator[0];
			}
			else
			{
				separator = '~';
			};
			containers = new TableContainer[_containers.Count];
			for (int i = 0; i < _containers.Count; i++)
			{
				containers[i] = _containers[i];
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < containers.Length; i++)
			{
				sb.Append(containers[i].TableContainerName);
				sb.Append('+');
			}
			sb.Length = sb.Length - 1;
			Table = (Table)tableHandler.addSymbolTable(sb.ToString());
			Name = sb.ToString();

			cachedSymbols = new StringBuilder[containers.Length];
			cachedCodes = new int[containers.Length];
			for (int i = 0; i < containers.Length; i++)
			{
				cachedCodes[i] = -1;
				cachedSymbols[i] = new StringBuilder();
			};
		}

		public override void clearCache()
		{
			base.clearCache();
			for (int i = 0; i < cachedCodes.Length; i++)
			{
				cachedCodes[i] = -1;
			}
			for (int i = 0; i < cachedSymbols.Length; i++)
			{
				cachedSymbols[i].Length = 0;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int addSymbol(String value) throws org.maltparser.core.exception.MaltChainedException
		public virtual int addSymbol(string value)
		{
			return table.addSymbol(value);
		}

		public virtual string Name
		{
			get
			{
				return table.Name;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbolCodeToString(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getSymbolCodeToString(int code)
		{
			return table.getSymbolCodeToString(code);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSymbolStringToCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getSymbolStringToCode(string symbol)
		{
			return table.getSymbolStringToCode(symbol);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getSymbolStringToValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual double getSymbolStringToValue(string symbol)
		{
			return table.getSymbolStringToCode(symbol);
		}

		public virtual int NumberContainers
		{
			get
			{
				return containers.Length;
			}
		}


		/* override TableContainer */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbol(int code) throws org.maltparser.core.exception.MaltChainedException
		public override string getSymbol(int code)
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
				split();
			}
			return cachedSymbol.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public override int getCode(string symbol)
		{
			if (cachedSymbol == null || !cachedSymbol.Equals(symbol))
			{
				clearCache();
				cachedSymbol.Append(symbol);
				cachedCode = table.getSymbolStringToCode(symbol);
				split();
			}
			return cachedCode;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean containCode(int code) throws org.maltparser.core.exception.MaltChainedException
		public override bool containCode(int code)
		{
			if (cachedCode != code)
			{
				clearCache();
				cachedSymbol.Append(table.getSymbolCodeToString(code));
				if (cachedSymbol == null && cachedSymbol.Length == 0)
				{
					return false;
				}
				cachedCode = code;
				split();
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean containSymbol(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public override bool containSymbol(string symbol)
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
				split();
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCombinedCode(java.util.List<ActionContainer> codesToCombine) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getCombinedCode(IList<ActionContainer> codesToCombine)
		{
			bool cachedUsed = true;
			if (containers.Length != codesToCombine.Count)
			{
				clearCache();
				return -1;
			}

			for (int i = 0; i < containers.Length; i++)
			{
				if (codesToCombine[i].ActionCode != cachedCodes[i])
				{
					cachedUsed = false;
					if (codesToCombine[i].ActionCode >= 0 && containers[i].containCode(codesToCombine[i].ActionCode))
					{
						cachedSymbols[i].Length = 0;
						cachedSymbols[i].Append(containers[i].getSymbol(codesToCombine[i].ActionCode));
						cachedCodes[i] = codesToCombine[i].ActionCode;
					}
					else
					{
						cachedSymbols[i].Length = 0;
						cachedCodes[i] = -1;
					}
				}
			}

			if (!cachedUsed)
			{
				cachedSymbol.Length = 0;
				for (int i = 0; i < containers.Length; i++)
				{
					if (cachedSymbols[i].Length != 0)
					{
						cachedSymbol.Append(cachedSymbols[i]);
						cachedSymbol.Append(separator);
					}
				}
				if (cachedSymbol.Length > 0)
				{
					cachedSymbol.Length = cachedSymbol.Length - 1;
				}
				if (cachedSymbol.Length > 0)
				{
					cachedCode = table.addSymbol(cachedSymbol.ToString());
				}
				else
				{
					cachedCode = -1;
				}
			}
			return cachedCode;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCombinedCode(ActionContainer[] codesToCombine, int start) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getCombinedCode(ActionContainer[] codesToCombine, int start)
		{
			bool cachedUsed = true;
			if (start < 0 || containers.Length > (codesToCombine.Length - start))
			{
				clearCache();
				return -1;
			}

			for (int i = 0; i < containers.Length; i++)
			{
				int code = codesToCombine[i + start].ActionCode;
				if (code != cachedCodes[i])
				{
					cachedUsed = false;
					if (code >= 0 && containers[i].containCode(code))
					{
						cachedSymbols[i].Length = 0;
						cachedSymbols[i].Append(containers[i].getSymbol(code));
						cachedCodes[i] = code;
					}
					else
					{
						cachedSymbols[i].Length = 0;
						cachedCodes[i] = -1;
					}
				}
			}

			if (!cachedUsed)
			{
				cachedSymbol.Length = 0;
				for (int i = 0; i < containers.Length; i++)
				{
					if (cachedSymbols[i].Length != 0)
					{
						cachedSymbol.Append(cachedSymbols[i]);
						cachedSymbol.Append(separator);
					}
				}
				if (cachedSymbol.Length > 0)
				{
					cachedSymbol.Length = cachedSymbol.Length - 1;
				}
				if (cachedSymbol.Length > 0)
				{
					cachedCode = table.addSymbol(cachedSymbol.ToString());
				}
				else
				{
					cachedCode = -1;
				}
			}
			return cachedCode;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setActionContainer(java.util.List<ActionContainer> actionContainers, int decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setActionContainer(IList<ActionContainer> actionContainers, int decision)
		{
			if (decision != cachedCode)
			{
				clearCache();
				if (decision != -1)
				{
					cachedSymbol.Append(table.getSymbolCodeToString(decision));
					cachedCode = decision;
				}
				split();
			}

			for (int i = 0; i < containers.Length; i++)
			{
				if (cachedSymbols[i].Length != 0)
				{
					cachedCodes[i] = actionContainers[i].setAction(cachedSymbols[i].ToString());
				}
				else
				{
					cachedCodes[i] = actionContainers[i].setAction(null);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setActionContainer(ActionContainer[] actionContainers, int start, int decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setActionContainer(ActionContainer[] actionContainers, int start, int decision)
		{
			if (decision != cachedCode)
			{
				clearCache();
				if (decision != -1)
				{
					cachedSymbol.Append(table.getSymbolCodeToString(decision));
					cachedCode = decision;
				}
				split();
			}

			for (int i = 0; i < containers.Length; i++)
			{
				if (cachedSymbols[i].Length != 0)
				{
					cachedCodes[i] = actionContainers[i + start].setAction(cachedSymbols[i].ToString());
				}
				else
				{
					cachedCodes[i] = actionContainers[i + start].setAction(null);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void split() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void split()
		{
			int j = 0;
			for (int i = 0; i < containers.Length; i++)
			{
				cachedSymbols[i].Length = 0;
			}
			for (int i = 0; i < cachedSymbol.Length; i++)
			{
				if (cachedSymbol[i] == separator)
				{
					j++;
				}
				else
				{
					cachedSymbols[j].Append(cachedSymbol[i]);
				}
			}
			for (int i = j + 1; i < containers.Length; i++)
			{
				cachedSymbols[i].Length = 0;
			}
			for (int i = 0; i < containers.Length; i++)
			{
				if (cachedSymbols[i].Length != 0)
				{
					cachedCodes[i] = containers[i].getCode(cachedSymbols[i].ToString());
				}
				else
				{
					cachedCodes[i] = -1;
				}
			}
		}

		public virtual char Separator
		{
			get
			{
				return separator;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initSymbolTable() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void initSymbolTable()
		{

		}
	}

}