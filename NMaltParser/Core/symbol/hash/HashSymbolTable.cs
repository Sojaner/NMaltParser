using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.symbol.hash
{
    using  helper;
	using  nullvalue;


    public sealed class HashSymbolTable : SymbolTable
	{
		private readonly string name;
		private readonly IDictionary<string, int> symbolCodeMap;
		private readonly IDictionary<int, string> codeSymbolMap;
		private readonly IDictionary<string, double> symbolValueMap;
		private readonly NullValues nullValues;
		private readonly int category;
		private readonly int type;
		private int valueCounter;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HashSymbolTable(String _name, int _category, int _type, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException
		public HashSymbolTable(string _name, int _category, int _type, string nullValueStrategy)
		{
			name = _name;
			category = _category;
			type = _type;
			symbolCodeMap = new HashMap<string, int>();
			codeSymbolMap = new HashMap<int, string>();
			symbolValueMap = new HashMap<string, double>();
			if (category == SymbolTable_Fields.OUTPUT)
			{
				nullValues = new OutputNullValues(nullValueStrategy, this);
			}
			else
			{
				nullValues = new InputNullValues(nullValueStrategy, this);
			}
			valueCounter = nullValues.NextCode;
		}

		public HashSymbolTable(string _name)
		{
			name = _name;
			category = SymbolTable_Fields.NA;
			type = SymbolTable_Fields.STRING;
			symbolCodeMap = new HashMap<string, int>();
			codeSymbolMap = new HashMap<int, string>();
			symbolValueMap = new HashMap<string, double>();
			nullValues = new InputNullValues("one", this);
			valueCounter = 1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int addSymbol(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public int addSymbol(string symbol)
		{
			if (nullValues == null || !nullValues.isNullValue(symbol))
			{
				if (ReferenceEquals(symbol, null) || symbol.Length == 0)
				{
					throw new SymbolException("Symbol table error: empty string cannot be added to the symbol table");
				}

				if (type == SymbolTable_Fields.REAL)
				{
					addSymbolValue(symbol);
				}
				if (!symbolCodeMap.ContainsKey(symbol))
				{
					int code = valueCounter;
					symbolCodeMap[symbol] = code;
					codeSymbolMap[code] = symbol;
					valueCounter++;
					return code;
				}
				else
				{
					return symbolCodeMap[symbol];
				}
			}
			else
			{
				return nullValues.symbolToCode(symbol);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double addSymbolValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public double addSymbolValue(string symbol)
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
		public string getSymbolCodeToString(int code)
		{
			if (code >= 0)
			{
				if (nullValues == null || !nullValues.isNullValue(code))
				{
					return codeSymbolMap[code];
				}
				else
				{
					return nullValues.codeToSymbol(code);
				}
			}
			else
			{
				throw new SymbolException("The symbol code '" + code + "' cannot be found in the symbol table. ");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSymbolStringToCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public int getSymbolStringToCode(string symbol)
		{
			if (!ReferenceEquals(symbol, null))
			{
				if (nullValues == null || !nullValues.isNullValue(symbol))
				{
					int? value = symbolCodeMap[symbol];
					return (value != null) ? value.Value : -1;
				}
				else
				{
					return nullValues.symbolToCode(symbol);
				}
			}
			else
			{
				throw new SymbolException("The symbol code '" + symbol + "' cannot be found in the symbol table. ");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getSymbolStringToValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public double getSymbolStringToValue(string symbol)
		{
			if (!ReferenceEquals(symbol, null))
			{
				if (type == SymbolTable_Fields.REAL && nullValues == null || !nullValues.isNullValue(symbol))
				{
					double? value = symbolValueMap[symbol];
					return (value != null) ? value.Value : double.Parse(symbol);
				}
				else
				{
					return 1.0;
				}
			}
			else
			{
				throw new SymbolException("The symbol code '" + symbol + "' cannot be found in the symbol table. ");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void saveHeader(java.io.BufferedWriter out) throws org.maltparser.core.exception.MaltChainedException
		public void saveHeader(StreamWriter @out)
		{
			try
			{
				@out.append('\t');
				@out.append(Name);
				@out.append('\t');
				@out.append(Convert.ToString(Category));
				@out.append('\t');
				@out.append(Convert.ToString(Type));
				@out.append('\t');
				@out.append(NullValueStrategy);
				@out.append('\n');
			}
			catch (IOException e)
			{
				throw new SymbolException("Could not save the symbol table. ", e);
			}
		}

		public int Category
		{
			get
			{
				return category;
			}
		}

		public int Type
		{
			get
			{
				return type;
			}
		}

		public string NullValueStrategy
		{
			get
			{
				if (nullValues == null)
				{
					return null;
				}
				return nullValues.NullValueStrategy;
			}
		}

		public int size()
		{
			return symbolCodeMap.Count;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(java.io.BufferedWriter out) throws org.maltparser.core.exception.MaltChainedException
		public void save(StreamWriter @out)
		{
			try
			{
				@out.Write(name);
				@out.BaseStream.WriteByte('\n');
				if (type != SymbolTable_Fields.REAL)
				{
					// TODO sort codes before writing due to change from TreeMap to HashMap
					foreach (int? code in codeSymbolMap.Keys)
					{
						@out.Write(Convert.ToString(code));
						@out.BaseStream.WriteByte('\t');
						@out.Write(codeSymbolMap[code]);
						@out.BaseStream.WriteByte('\n');
					}
				}
				else
				{
					foreach (string symbol in symbolValueMap.Keys)
					{
						@out.BaseStream.WriteByte(1);
						@out.BaseStream.WriteByte('\t');
						@out.Write(symbol);
						@out.BaseStream.WriteByte('\n');
					}
				}
				@out.BaseStream.WriteByte('\n');
			}
			catch (IOException e)
			{
				throw new SymbolException("Could not save the symbol table. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.io.BufferedReader in) throws org.maltparser.core.exception.MaltChainedException
		public void load(StreamReader @in)
		{
			int max = 0;
			string fileLine;
			try
			{
				while (!ReferenceEquals((fileLine = @in.ReadLine()), null))
				{
					int index;
					if (fileLine.Length == 0 || (index = fileLine.IndexOf('\t')) == -1)
					{
						valueCounter = max + 1;
						break;
					}

					if (type != SymbolTable_Fields.REAL)
					{
						int code;
						try
						{
							code = int.Parse(fileLine.Substring(0,index));
						}
						catch (FormatException e)
						{
							throw new SymbolException("The symbol table file (.sym) contains a non-integer value in the first column. ", e);
						}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String symbol = fileLine.substring(index+1);
						string symbol = fileLine.Substring(index + 1);
						symbolCodeMap[symbol] = code;
						codeSymbolMap[code] = symbol;

						if (max < code)
						{
							max = code;
						}
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String symbol = fileLine.substring(index+1);
						string symbol = fileLine.Substring(index + 1);
						symbolValueMap[symbol] = double.Parse(symbol);

						max = 1;
					}
				}
			}
			catch (IOException e)
			{
				throw new SymbolException("Could not load the symbol table. ", e);
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public int ValueCounter
		{
			get
			{
				return valueCounter;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getNullValueCode(org.maltparser.core.symbol.nullvalue.NullValues.NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException
		public int getNullValueCode(NullValues.NullValueId nullValueIdentifier)
		{
			if (nullValues == null)
			{
				throw new SymbolException("The symbol table does not have any null-values. ");
			}
			return nullValues.nullvalueToCode(nullValueIdentifier);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNullValueSymbol(org.maltparser.core.symbol.nullvalue.NullValues.NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException
		public string getNullValueSymbol(NullValues.NullValueId nullValueIdentifier)
		{
			if (nullValues == null)
			{
				throw new SymbolException("The symbol table does not have any null-values. ");
			}
			return nullValues.nullvalueToSymbol(nullValueIdentifier);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isNullValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public bool isNullValue(string symbol)
		{
			if (nullValues != null)
			{
				return nullValues.isNullValue(symbol);
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isNullValue(int code) throws org.maltparser.core.exception.MaltChainedException
		public bool isNullValue(int code)
		{
			if (nullValues != null)
			{
				return nullValues.isNullValue(code);
			}
			return false;
		}

		public ISet<int> Codes
		{
			get
			{
				return codeSymbolMap.Keys;
			}
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
			if (GetType() != obj.GetType())
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final HashSymbolTable other = (HashSymbolTable)obj;
			HashSymbolTable other = (HashSymbolTable)obj;
			return ((ReferenceEquals(name, null)) ? ReferenceEquals(other.name, null) : name.Equals(other.name));
		}

		public override int GetHashCode()
		{
			return 217 + (null == name ? 0 : name.GetHashCode());
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(name);
			sb.Append(' ');
			sb.Append(valueCounter);
			return sb.ToString();
		}
	}

}