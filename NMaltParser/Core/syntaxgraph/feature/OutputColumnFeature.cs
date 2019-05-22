using System;
using System.Text;
using NMaltParser.Core.Feature.Function;
using NMaltParser.Core.Feature.Value;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph.Feature
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class OutputColumnFeature : ColumnFeature
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(string), typeof(AddressFunction)};
		private readonly DataFormatInstance dataFormatInstance;
		private readonly SymbolTableHandler tableHandler;
		private AddressFunction addressFunction;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OutputColumnFeature(org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public OutputColumnFeature(DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler) : base()
		{
			this.dataFormatInstance = dataFormatInstance;
			this.tableHandler = tableHandler;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void initialize(object[] arguments)
		{
			if (arguments.Length != 2)
			{
				throw new SyntaxGraphException("Could not initialize OutputColumnFeature: number of arguments are not correct. ");
			}
			if (!(arguments[0] is string))
			{
				throw new SyntaxGraphException("Could not initialize OutputColumnFeature: the first argument is not a string. ");
			}
			if (!(arguments[1] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize OutputColumnFeature: the second argument is not an address function. ");
			}
			ColumnDescription column = dataFormatInstance.getColumnDescriptionByName((string)arguments[0]);
			if (column == null)
			{
				throw new SyntaxGraphException("Could not initialize OutputColumnFeature: the output column type '" + (string)arguments[0] + "' could not be found in the data format specification. ' ");
			}
			Column = column;
			SymbolTable = tableHandler.getSymbolTable(column.Name);
			AddressFunction = (AddressFunction)arguments[1];
		}

		public override Type[] ParameterTypes
		{
			get
			{
				return paramTypes;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public override void update()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.value.AddressValue a = addressFunction.getAddressValue();
			AddressValue a = addressFunction.AddressValue;

			if (a.Address == null)
			{
				featureValue.update(symbolTable.getNullValueCode(NullValueId.NO_NODE), symbolTable.getNullValueSymbol(NullValueId.NO_NODE), true, 1);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node = (org.maltparser.core.syntaxgraph.node.DependencyNode)a.getAddress();
				DependencyNode node = (DependencyNode)a.Address;
				if (!node.Root)
				{
					if (node.hasHead())
					{
						int indexCode = node.HeadEdge.getLabelCode(symbolTable);
						string symbol = symbolTable.getSymbolCodeToString(indexCode);
						if (column.Type == ColumnDescription.STRING)
						{
							featureValue.update(indexCode, symbol, false, 1);
						}
						else
						{
							castFeatureValue(symbol);
						}
					}
					else
					{
						featureValue.update(symbolTable.getNullValueCode(NullValueId.NO_VALUE), symbolTable.getNullValueSymbol(NullValueId.NO_VALUE), true, 1);
					}
				}
				else
				{
					featureValue.update(symbolTable.getNullValueCode(NullValueId.ROOT_NODE), symbolTable.getNullValueSymbol(NullValueId.ROOT_NODE), true, 1);
				}
			}
		}

		public virtual AddressFunction AddressFunction
		{
			get
			{
				return addressFunction;
			}
			set
			{
				addressFunction = value;
			}
		}


		public virtual DataFormatInstance DataFormatInstance
		{
			get
			{
				return dataFormatInstance;
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
			return obj.ToString().Equals(ToString());
		}

		public override int GetHashCode()
		{
			return 217 + (null == ToString() ? 0 : ToString().GetHashCode());
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("OutputColumn(");
			sb.Append(base.ToString());
			sb.Append(", ");
			sb.Append(addressFunction.ToString());
			sb.Append(")");
			return sb.ToString();
		}
	}

}