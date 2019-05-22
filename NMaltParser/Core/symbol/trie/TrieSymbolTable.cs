using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.symbol.trie
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.symbol.nullvalue;
	using  org.maltparser.core.symbol.nullvalue;
	using  org.maltparser.core.symbol.nullvalue;
	using  org.maltparser.core.symbol.nullvalue.NullValues;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.0
	/// </summary>
	public class TrieSymbolTable : SymbolTable
	{
		private readonly string name;
		private readonly Trie trie;
		private readonly SortedDictionary<int, TrieNode> codeTable;
		private int category;
		private readonly NullValues nullValues;
		private int valueCounter;
		/// <summary>
		/// Cache the hash code for the symbol table </summary>
		private int cachedHash;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TrieSymbolTable(String _name, Trie _trie, int _category, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException
		public TrieSymbolTable(string _name, Trie _trie, int _category, string nullValueStrategy)
		{
			this.name = _name;
			this.trie = _trie;
			this.category = _category;
			codeTable = new SortedDictionary<int, TrieNode>();
			if (this.category != org.maltparser.core.symbol.SymbolTable_Fields.OUTPUT)
			{
				nullValues = new OutputNullValues(nullValueStrategy, this);
			}
			else
			{
				nullValues = new InputNullValues(nullValueStrategy, this);
			}
			valueCounter = nullValues.NextCode;
		}

		public TrieSymbolTable(string _name, Trie trie)
		{
			this.name = _name;
			this.trie = trie;
			codeTable = new SortedDictionary<int, TrieNode>();
			nullValues = new InputNullValues("one", this);
			valueCounter = 1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int addSymbol(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual int addSymbol(string symbol)
		{
			if (nullValues == null || !nullValues.isNullValue(symbol))
			{
				if (string.ReferenceEquals(symbol, null) || symbol.Length == 0)
				{
					throw new SymbolException("Symbol table error: empty string cannot be added to the symbol table");
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TrieNode node = trie.addValue(symbol, this, -1);
				TrieNode node = trie.addValue(symbol, this, -1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int code = node.getEntry(this);
				int code = node.getEntry(this).Value;
				if (!codeTable.ContainsKey(code))
				{
					codeTable[code] = node;
				}
				return code;
			}
			else
			{
				return nullValues.symbolToCode(symbol);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbolCodeToString(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getSymbolCodeToString(int code)
		{
			if (code >= 0)
			{
				if (nullValues == null || !nullValues.isNullValue(code))
				{
					TrieNode node = codeTable[code];
					if (node != null)
					{
						return trie.getValue(node, this);
					}
					else
					{
						return null;
					}
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
		public virtual int getSymbolStringToCode(string symbol)
		{
			if (!string.ReferenceEquals(symbol, null))
			{
				if (nullValues == null || !nullValues.isNullValue(symbol))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final System.Nullable<int> entry = trie.getEntry(symbol, this);
					int? entry = trie.getEntry(symbol, this);
					if (entry != null)
					{
						return entry.Value;
					}
					else
					{
						return -1;
					}
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
		public virtual double getSymbolStringToValue(string symbol)
		{
			if (string.ReferenceEquals(symbol, null))
			{
				throw new SymbolException("The symbol code '" + symbol + "' cannot be found in the symbol table. ");
			}

			return 1.0;
		}
		public virtual void clearTmpStorage()
		{

		}

		public virtual string NullValueStrategy
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


		public virtual int Category
		{
			get
			{
				return category;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void saveHeader(java.io.BufferedWriter out) throws org.maltparser.core.exception.MaltChainedException
		public virtual void saveHeader(StreamWriter @out)
		{
			try
			{
				@out.append('\t');
				@out.append(Name);
				@out.append('\t');
				@out.append(Convert.ToString(Category));
				@out.append('\t');
				@out.append(Convert.ToString(org.maltparser.core.symbol.SymbolTable_Fields.STRING));
				@out.append('\t');
				@out.append(NullValueStrategy);
				@out.append('\n');
			}
			catch (IOException e)
			{
				throw new SymbolException("Could not save the symbol table. ", e);
			}
		}

		public virtual int size()
		{
			return codeTable.Count;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(java.io.BufferedWriter out) throws org.maltparser.core.exception.MaltChainedException
		public virtual void save(StreamWriter @out)
		{
			try
			{
				@out.Write(name);
				@out.BaseStream.WriteByte('\n');
				foreach (int? code in codeTable.Keys)
				{
					@out.BaseStream.WriteByte(code + "");
					@out.BaseStream.WriteByte('\t');
					@out.BaseStream.WriteByte(trie.getValue(codeTable[code], this));
					@out.BaseStream.WriteByte('\n');
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
		public virtual void load(StreamReader @in)
		{
			int max = 0;
			int index = 0;
			string fileLine;
			try
			{
				while (!string.ReferenceEquals((fileLine = @in.ReadLine()), null))
				{
					if (fileLine.Length == 0 || (index = fileLine.IndexOf('\t')) == -1)
					{
						ValueCounter = max + 1;
						break;
					}
					int code = int.Parse(fileLine.Substring(0,index));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String str = fileLine.substring(index+1);
					string str = fileLine.Substring(index + 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TrieNode node = trie.addValue(str, this, code);
					TrieNode node = trie.addValue(str, this, code);
					codeTable[node.getEntry(this)] = node;
					if (max < code)
					{
						max = code;
					}
				}
			}
			catch (System.FormatException e)
			{
				throw new SymbolException("The symbol table file (.sym) contains a non-integer value in the first column. ", e);
			}
			catch (IOException e)
			{
				throw new SymbolException("Could not load the symbol table. ", e);
			}
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
				return valueCounter;
			}
			set
			{
				this.valueCounter = value;
			}
		}


		protected internal virtual void updateValueCounter(int code)
		{
			if (code > valueCounter)
			{
				valueCounter = code;
			}
		}

		protected internal virtual int increaseValueCounter()
		{
			return valueCounter++;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getNullValueCode(org.maltparser.core.symbol.nullvalue.NullValues.NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getNullValueCode(NullValues.NullValueId nullValueIdentifier)
		{
			if (nullValues == null)
			{
				throw new SymbolException("The symbol table does not have any null-values. ");
			}
			return nullValues.nullvalueToCode(nullValueIdentifier);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNullValueSymbol(org.maltparser.core.symbol.nullvalue.NullValues.NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getNullValueSymbol(NullValues.NullValueId nullValueIdentifier)
		{
			if (nullValues == null)
			{
				throw new SymbolException("The symbol table does not have any null-values. ");
			}
			return nullValues.nullvalueToSymbol(nullValueIdentifier);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isNullValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool isNullValue(string symbol)
		{
			if (nullValues != null)
			{
				return nullValues.isNullValue(symbol);
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isNullValue(int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool isNullValue(int code)
		{
			if (nullValues != null)
			{
				return nullValues.isNullValue(code);
			}
			return false;
		}

	//	public void copy(SymbolTable fromTable) throws MaltChainedException {
	//		final SortedMap<Integer, TrieNode> fromCodeTable =  ((TrieSymbolTable)fromTable).getCodeTable();
	//		int max = getValueCounter()-1;
	//		for (Integer code : fromCodeTable.keySet()) {
	//			final String str = trie.getValue(fromCodeTable.get(code), this);
	//			final TrieNode node = trie.addValue(str, this, code);
	//			codeTable.put(node.getEntry(this), node); //.getCode(), node);
	//			if (max < code) {
	//				max = code;
	//			}
	//		}
	//		setValueCounter(max+1);
	//	}

		public virtual SortedDictionary<int, TrieNode> CodeTable
		{
			get
			{
				return codeTable;
			}
		}

		public virtual ISet<int> Codes
		{
			get
			{
				return codeTable.Keys;
			}
		}

		protected internal virtual Trie Trie
		{
			get
			{
				return trie;
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
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TrieSymbolTable other = (TrieSymbolTable)obj;
			TrieSymbolTable other = (TrieSymbolTable)obj;
			return ((string.ReferenceEquals(name, null)) ? string.ReferenceEquals(other.name, null) : name.Equals(other.name));
		}

		public override int GetHashCode()
		{
			if (cachedHash == 0)
			{
				cachedHash = 217 + (null == name ? 0 : name.GetHashCode());
			}
			return cachedHash;
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