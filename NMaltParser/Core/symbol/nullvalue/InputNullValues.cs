using System;
using System.Text;

namespace org.maltparser.core.symbol.nullvalue
{
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.0
	/// </summary>
	public class InputNullValues : NullValues
	{

		public InputNullValues(string nullValueStrategy, SymbolTable table) : base(table)
		{
			NullValueEncoding = nullValueStrategy;
			makeNullValues();
		}

		protected internal override void makeNullValues()
		{
			if (nullValueEncoding == NullValueDegree.NONE || nullValueEncoding == NullValueDegree.ONE)
			{
				nullValue2SymbolMap[NullValueId.NO_NODE] = "#null#";
				nullValue2SymbolMap[NullValueId.ROOT_NODE] = "#null#";
				nullValue2CodeMap[NullValueId.NO_NODE] = 0;
				nullValue2CodeMap[NullValueId.ROOT_NODE] = 0;
				symbol2CodeMap["#null#"] = 0;
				code2SymbolMap[0] = "#null#";
				NextCode = 1;
			}
			else if (nullValueEncoding == NullValueDegree.ROOTNODE)
			{
				nullValue2SymbolMap[NullValueId.NO_NODE] = "#null#";
				nullValue2SymbolMap[NullValueId.ROOT_NODE] = "#rootnode#";
				nullValue2CodeMap[NullValueId.NO_NODE] = 0;
				nullValue2CodeMap[NullValueId.ROOT_NODE] = 1;
				symbol2CodeMap["#null#"] = 0;
				symbol2CodeMap["#rootnode#"] = 1;
				code2SymbolMap[0] = "#null#";
				code2SymbolMap[1] = "#rootnode#";
				NextCode = 2;
			}
		}

		protected internal override string NullValueEncoding
		{
			set
			{
				NullValueStrategy = value;
				if (value.Equals("none", StringComparison.OrdinalIgnoreCase))
				{
					nullValueEncoding = NullValueDegree.NONE;
				}
				else if (value.Equals("rootnode", StringComparison.OrdinalIgnoreCase))
				{
					nullValueEncoding = NullValueDegree.ROOTNODE;
				}
				else if (value.Equals("novalue", StringComparison.OrdinalIgnoreCase))
				{
					nullValueEncoding = NullValueDegree.ROOTNODE;
				}
				else
				{
					nullValueEncoding = NullValueDegree.ONE;
				}
			}
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();

			return sb.ToString();
		}
	}

}