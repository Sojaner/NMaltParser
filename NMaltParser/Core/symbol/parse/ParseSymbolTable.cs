using System;
using System.Collections.Generic;
using System.IO;

namespace org.maltparser.core.symbol.parse
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.helper;
	using  org.maltparser.core.symbol.nullvalue.NullValues;


	public class ParseSymbolTable : SymbolTable
	{
		private readonly string name;
		private readonly SymbolTable parentSymbolTable;
		private readonly int type;
		/// <summary>
		/// Special treatment during parsing </summary>
		private readonly IDictionary<string, int> symbolCodeMap;
		private readonly IDictionary<int, string> codeSymbolMap;
		private readonly IDictionary<string, double> symbolValueMap;
		private int valueCounter;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParseSymbolTable(String _name, int _category, int _type, String nullValueStrategy, org.maltparser.core.symbol.SymbolTableHandler parentSymbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public ParseSymbolTable(string _name, int _category, int _type, string nullValueStrategy, SymbolTableHandler parentSymbolTableHandler)
		{
			this.name = _name;
			this.type = _type;
			this.parentSymbolTable = parentSymbolTableHandler.addSymbolTable(name, _category, _type, nullValueStrategy);
			this.symbolCodeMap = new HashMap<string, int>();
			this.codeSymbolMap = new HashMap<int, string>();
			this.symbolValueMap = new HashMap<string, double>();
			this.valueCounter = -1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParseSymbolTable(String _name, org.maltparser.core.symbol.SymbolTable parentTable, org.maltparser.core.symbol.SymbolTableHandler parentSymbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public ParseSymbolTable(string _name, SymbolTable parentTable, SymbolTableHandler parentSymbolTableHandler)
		{
			this.name = _name;
			this.type = org.maltparser.core.symbol.SymbolTable_Fields.STRING;
			this.parentSymbolTable = parentSymbolTableHandler.addSymbolTable(name, parentTable);
			this.symbolCodeMap = new HashMap<string, int>();
			this.codeSymbolMap = new HashMap<int, string>();
			this.symbolValueMap = new HashMap<string, double>();
			this.valueCounter = -1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParseSymbolTable(String name, org.maltparser.core.symbol.SymbolTableHandler parentSymbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public ParseSymbolTable(string name, SymbolTableHandler parentSymbolTableHandler)
		{
			this.name = name;
			this.type = org.maltparser.core.symbol.SymbolTable_Fields.STRING;
			this.parentSymbolTable = parentSymbolTableHandler.addSymbolTable(name);
			this.symbolCodeMap = new HashMap<string, int>();
			this.codeSymbolMap = new HashMap<int, string>();
			this.symbolValueMap = new HashMap<string, double>();
			this.valueCounter = -1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int addSymbol(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual int addSymbol(string symbol)
		{
			if (!parentSymbolTable.isNullValue(symbol))
			{
				if (string.ReferenceEquals(symbol, null) || symbol.Length == 0)
				{
					throw new SymbolException("Symbol table error: empty string cannot be added to the symbol table");
				}

				int code = parentSymbolTable.getSymbolStringToCode(symbol);
				if (code > -1)
				{
					return code;
				}
				if (this.type == org.maltparser.core.symbol.SymbolTable_Fields.REAL)
				{
					addSymbolValue(symbol);
				}
				if (!symbolCodeMap.ContainsKey(symbol))
				{
	//				System.out.println("!symbolCodeMap.containsKey(symbol) : " + this.getName() + ": " + symbol.toString());
					if (valueCounter == -1)
					{
						valueCounter = parentSymbolTable.ValueCounter + 1;
					}
					else
					{
						valueCounter++;
					}
					symbolCodeMap[symbol] = valueCounter;
					codeSymbolMap[valueCounter] = symbol;
					return valueCounter;
				}
				else
				{
					return symbolCodeMap[symbol];
				}
			}
			else
			{
				return parentSymbolTable.getSymbolStringToCode(symbol);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double addSymbolValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual double addSymbolValue(string symbol)
		{
			if (!symbolValueMap.ContainsKey(symbol))
			{
				double? value = Convert.ToDouble(symbol);
				symbolValueMap[symbol] = value.Value;
				return value.Value;
			}
			else
			{
				return symbolValueMap[symbol];
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbolCodeToString(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getSymbolCodeToString(int code)
		{
			if (code < 0)
			{
				throw new SymbolException("The symbol code '" + code + "' cannot be found in the symbol table. ");
			}
			string symbol = parentSymbolTable.getSymbolCodeToString(code);
			if (!string.ReferenceEquals(symbol, null))
			{
				return symbol;
			}
			else
			{
				return codeSymbolMap[code];
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSymbolStringToCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getSymbolStringToCode(string symbol)
		{
			if (string.ReferenceEquals(symbol, null))
			{
				throw new SymbolException("The symbol code '" + symbol + "' cannot be found in the symbol table. ");
			}

			int code = parentSymbolTable.getSymbolStringToCode(symbol);
			if (code > -1)
			{
				return code;
			}

			int? item = symbolCodeMap[symbol];
			if (item == null)
			{
				throw new SymbolException("Could not find the symbol '" + symbol + "' in the symbol table. ");
			}
			return item.Value;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getSymbolStringToValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual double getSymbolStringToValue(string symbol)
		{
			if (string.ReferenceEquals(symbol, null))
			{
				throw new SymbolException("The symbol code '" + symbol + "' cannot be found in the symbol table. ");
			}
			double value = parentSymbolTable.getSymbolStringToValue(symbol);
			if (value != Double.NaN)
			{
				return value;
			}

			double? item = symbolValueMap[symbol];
			if (item == null)
			{
				throw new SymbolException("Could not find the symbol '" + symbol + "' in the symbol table. ");
			}
			return item.Value;
		}

		public virtual void clearTmpStorage()
		{
			symbolCodeMap.Clear();
			codeSymbolMap.Clear();
			symbolValueMap.Clear();
			valueCounter = -1;
		}

		public virtual int size()
		{
			return parentSymbolTable.size();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(java.io.BufferedWriter out) throws org.maltparser.core.exception.MaltChainedException
		public virtual void save(StreamWriter @out)
		{
			parentSymbolTable.save(@out);
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.io.BufferedReader in) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(StreamReader @in)
		{
			parentSymbolTable.load(@in);
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual int ValueCounter
		{
			get
			{
				return parentSymbolTable.ValueCounter;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getNullValueCode(org.maltparser.core.symbol.nullvalue.NullValues.NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getNullValueCode(NullValueId nullValueIdentifier)
		{
			return parentSymbolTable.getNullValueCode(nullValueIdentifier);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNullValueSymbol(org.maltparser.core.symbol.nullvalue.NullValues.NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getNullValueSymbol(NullValueId nullValueIdentifier)
		{
			return parentSymbolTable.getNullValueSymbol(nullValueIdentifier);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isNullValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool isNullValue(string symbol)
		{
			return parentSymbolTable.isNullValue(symbol);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isNullValue(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool isNullValue(int code)
		{
			return parentSymbolTable.isNullValue(code);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ParseSymbolTable other = (ParseSymbolTable)obj;
			ParseSymbolTable other = (ParseSymbolTable)obj;
			return ((string.ReferenceEquals(name, null)) ? string.ReferenceEquals(other.name, null) : name.Equals(other.name));
		}

		public override int GetHashCode()
		{
			return 217 + (null == name ? 0 : name.GetHashCode());
		}

		public override string ToString()
		{
			return parentSymbolTable.ToString();
		}
	}

}