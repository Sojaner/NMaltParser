using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.symbol.nullvalue
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using HashMap = org.maltparser.core.helper.HashMap;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public abstract class NullValues
	{
		protected internal enum NullValueDegree
		{
			NONE,
			ONE,
			ROOTNODE,
			NOVALUE
		}
		public enum NullValueId
		{
			NO_NODE,
			ROOT_NODE,
			NO_VALUE
		}
		protected internal HashMap<NullValueId, string> nullValue2SymbolMap;
		protected internal HashMap<NullValueId, int> nullValue2CodeMap;
		protected internal HashMap<string, int> symbol2CodeMap;
		protected internal SortedDictionary<int, string> code2SymbolMap;
		protected internal SymbolTable table;
		protected internal NullValueDegree nullValueEncoding;
		protected internal string nullValueStrategy;
		protected internal int nextCode;

		public NullValues(SymbolTable table)
		{
			SymbolTable = table;
			nullValue2SymbolMap = new HashMap<NullValueId, string>();
			nullValue2CodeMap = new HashMap<NullValueId, int>();
			symbol2CodeMap = new HashMap<string, int>();
			code2SymbolMap = new SortedDictionary<int, string>();
		}

		private SymbolTable SymbolTable
		{
			set
			{
				this.table = value;
			}
			get
			{
				return table;
			}
		}


		public virtual string NullValueStrategy
		{
			get
			{
				return nullValueStrategy;
			}
			set
			{
				this.nullValueStrategy = value;
			}
		}


		public virtual NullValueDegree getNullValueEncoding()
		{
			return nullValueEncoding;
		}

		public virtual int NextCode
		{
			get
			{
				return nextCode;
			}
			set
			{
				this.nextCode = value;
			}
		}


		public virtual bool isNullValue(int code)
		{
			if (code < 0 || code >= nextCode)
			{
				return false;
			}
			return true;
		}

		public virtual bool isNullValue(string symbol)
		{
			if (string.ReferenceEquals(symbol, null) || symbol.Length == 0 || symbol[0] != '#')
			{
				return false;
			}
			if (symbol.Equals("#null#"))
			{
				return true;
			}

			if ((nullValueEncoding == NullValueDegree.ROOTNODE || nullValueEncoding == NullValueDegree.NOVALUE) && symbol.Equals("#rootnode#"))
			{
				return true;
			}

			if (nullValueEncoding == NullValueDegree.NOVALUE && symbol.Equals("#novalue#"))
			{
				return true;
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nullvalueToCode(NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException
		public virtual int nullvalueToCode(NullValueId nullValueIdentifier)
		{
			if (!nullValue2CodeMap.ContainsKey(nullValueIdentifier))
			{
				throw new SymbolException("Illegal null-value identifier. ");
			}
			return nullValue2CodeMap[nullValueIdentifier];
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nullvalueToSymbol(NullValueId nullValueIdentifier) throws org.maltparser.core.exception.MaltChainedException
		public virtual string nullvalueToSymbol(NullValueId nullValueIdentifier)
		{
			if (!nullValue2SymbolMap.ContainsKey(nullValueIdentifier))
			{
				throw new SymbolException("Illegal null-value identifier. ");
			}
			return nullValue2SymbolMap[nullValueIdentifier];
		}

		public virtual int symbolToCode(string symbol)
		{
			if (!symbol2CodeMap.ContainsKey(symbol))
			{
				return -1;
			}
			return symbol2CodeMap[symbol];
		}

		public virtual string codeToSymbol(int code)
		{
			if (!code2SymbolMap.ContainsKey(code))
			{
				return null;
			}
			return code2SymbolMap[code];
		}

		protected internal abstract void setNullValueEncoding(string nullValueStrategy);
		protected internal abstract void makeNullValues();

		/* (non-Javadoc)
		 * @see java.lang.Object#equals(java.lang.Object)
		 */
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

			NullValues nl = (NullValues)obj;
			if (!nullValueStrategy.Equals(nl.NullValueStrategy, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (nextCode != nl.NextCode)
			{
				return false;
			}
			if (!nullValue2SymbolMap.Equals(nl.nullValue2SymbolMap))
			{
				return false;
			}
			if (!nullValue2CodeMap.Equals(nl.nullValue2CodeMap))
			{
				return false;
			}
			if (!code2SymbolMap.Equals(nl.code2SymbolMap))
			{
				return false;
			}
			if (!symbol2CodeMap.Equals(nl.symbol2CodeMap))
			{
				return false;
			}
			return true;
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("Null-values:\n");
			sb.Append("  Strategy: " + nullValueStrategy);
			sb.Append("  NO_NODE -> " + nullValue2CodeMap[NullValueId.NO_NODE] + " " + nullValue2SymbolMap[NullValueId.NO_NODE] + "\n");
			sb.Append("  ROOT_NODE -> " + nullValue2CodeMap[NullValueId.ROOT_NODE] + " " + nullValue2SymbolMap[NullValueId.ROOT_NODE] + "\n");
			sb.Append("  NO_VALUE -> " + nullValue2CodeMap[NullValueId.NO_VALUE] + " " + nullValue2SymbolMap[NullValueId.NO_VALUE] + "\n");
			return sb.ToString();
		}
	}

}