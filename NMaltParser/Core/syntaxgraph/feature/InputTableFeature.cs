using System;
using System.Text;

namespace org.maltparser.core.syntaxgraph.feature
{
    using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.value;
	using  io.dataformat;
    using  symbol;
    using  node;

	public sealed class InputTableFeature : TableFeature
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(string), typeof(AddressFunction)};
		private AddressFunction addressFunction;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputTableFeature(org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public InputTableFeature(DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler) : base(tableHandler)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void initialize(object[] arguments)
		{
			if (arguments.Length != 2)
			{
				throw new SyntaxGraphException("Could not initialize InputTableFeature: number of arguments are not correct. ");
			}
			if (!(arguments[0] is string))
			{
				throw new SyntaxGraphException("Could not initialize InputTableFeature: the first argument is not a string. ");
			}
			if (!(arguments[1] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize InputTableFeature: the second argument is not an address function. ");
			}
			TableName = (string)arguments[0];
			SymbolTable = tableHandler.getSymbolTable(TableName);
			AddressFunction = (AddressFunction)arguments[1];
			Type = ColumnDescription.STRING; // TODO Probably it could possible to vary the type
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
				if (SymbolTable != null)
				{
					featureValue.IndexCode = SymbolTable.getNullValueCode(NullValueId.NO_NODE);
					featureValue.Symbol = SymbolTable.getNullValueSymbol(NullValueId.NO_NODE);
				}
				else
				{
					featureValue.IndexCode = 0;
					featureValue.Symbol = "#null";
				}
				featureValue.NullValue = true;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node = (org.maltparser.core.syntaxgraph.node.DependencyNode)a.getAddress();
				DependencyNode node = (DependencyNode)a.Address;
				if (!node.Root)
				{
					if (SymbolTable != null && node.hasLabel(SymbolTable))
					{
						featureValue.IndexCode = node.getLabelCode(SymbolTable);
						featureValue.Symbol = SymbolTable.getSymbolCodeToString(node.getLabelCode(SymbolTable));
						featureValue.NullValue = false;
					}
					else
					{
						if (SymbolTable != null)
						{
							featureValue.IndexCode = SymbolTable.getNullValueCode(NullValueId.NO_NODE);
							featureValue.Symbol = SymbolTable.getNullValueSymbol(NullValueId.NO_NODE);

						}
						featureValue.NullValue = true;
					}
				}
				else
				{
					if (SymbolTable != null)
					{
						featureValue.IndexCode = SymbolTable.getNullValueCode(NullValueId.ROOT_NODE);
						featureValue.Symbol = SymbolTable.getNullValueSymbol(NullValueId.ROOT_NODE);
					}

					featureValue.NullValue = true;
				}
			}
			featureValue.Value = 1;
		}

		public AddressFunction AddressFunction
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
			sb.Append("InputTable(");
			sb.Append(base.ToString());
			sb.Append(", ");
			sb.Append(addressFunction.ToString());
			sb.Append(")");
			return sb.ToString();
		}

	}

}