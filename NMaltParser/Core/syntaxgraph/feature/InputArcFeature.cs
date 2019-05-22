using System;

namespace org.maltparser.core.syntaxgraph.feature
{
    using  org.maltparser.core.feature;
	using  org.maltparser.core.feature.function;
    using  org.maltparser.core.feature.value;
    using  io.dataformat;
    using  symbol;
    using  node;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class InputArcFeature : FeatureFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(string), typeof(AddressFunction), typeof(AddressFunction)};
		private AddressFunction addressFunction1;
		private AddressFunction addressFunction2;
		private ColumnDescription column;
		private readonly DataFormatInstance dataFormatInstance;
		private readonly SymbolTableHandler tableHandler;
		private SymbolTable table;
		private readonly SingleFeatureValue featureValue;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputArcFeature(org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public InputArcFeature(DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler)
		{
			this.dataFormatInstance = dataFormatInstance;
			this.tableHandler = tableHandler;
			featureValue = new SingleFeatureValue(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public void initialize(object[] arguments)
		{
			if (arguments.Length != 3)
			{
				throw new FeatureException("Could not initialize InputArcFeature: number of arguments are not correct. ");
			}
			// Checks that the two arguments are address functions

			if (!(arguments[0] is string))
			{
				throw new FeatureException("Could not initialize InputArcFeature: the first argument is not a string. ");
			}
			if (!(arguments[1] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize InputArcFeature: the second argument is not an address function. ");
			}
			if (!(arguments[2] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize InputArcFeature: the third argument is not an address function. ");
			}
			AddressFunction1 = (AddressFunction)arguments[1];
			AddressFunction2 = (AddressFunction)arguments[2];

			Column = dataFormatInstance.getColumnDescriptionByName((string)arguments[0]);
			SymbolTable = tableHandler.addSymbolTable("ARC_" + column.Name,ColumnDescription.INPUT, ColumnDescription.STRING, "one");
			table.addSymbol("LEFT");
			table.addSymbol("RIGHT");
		}

		public Type[] ParameterTypes
		{
			get
			{
				return paramTypes;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public int getCode(string symbol)
		{
			return table.getSymbolStringToCode(symbol);
		}


		public FeatureValue FeatureValue
		{
			get
			{
				return featureValue;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbol(int code) throws org.maltparser.core.exception.MaltChainedException
		public string getSymbol(int code)
		{
			return table.getSymbolCodeToString(code);
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void updateCardinality() throws org.maltparser.core.exception.MaltChainedException
		public void updateCardinality()
		{
	//		featureValue.setCardinality(table.getValueCounter());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public void update()
		{
			// Retrieve the address value 
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.value.AddressValue arg1 = addressFunction1.getAddressValue();
			AddressValue arg1 = addressFunction1.AddressValue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.value.AddressValue arg2 = addressFunction2.getAddressValue();
			AddressValue arg2 = addressFunction2.AddressValue;
			if (arg1.Address != null && arg1.AddressClass == typeof(DependencyNode) && arg2.Address != null && arg2.AddressClass == typeof(DependencyNode))
			{
				DependencyNode node1 = (DependencyNode)arg1.Address;
				DependencyNode node2 = (DependencyNode)arg2.Address;
				try
				{
				SymbolTable symbolTable = tableHandler.getSymbolTable(column.Name);
				int head1 = int.Parse(node1.getLabelSymbol(symbolTable));
				int head2 = int.Parse(node2.getLabelSymbol(symbolTable));
				if (!node1.Root && head1 == node2.Index)
				{
					featureValue.IndexCode = table.getSymbolStringToCode("LEFT");
					featureValue.Symbol = "LEFT";
					featureValue.NullValue = false;
				}
				else if (!node2.Root && head2 == node1.Index)
				{
					featureValue.IndexCode = table.getSymbolStringToCode("RIGHT");
					featureValue.Symbol = "RIGHT";
					featureValue.NullValue = false;
				}
				else
				{
					featureValue.IndexCode = table.getNullValueCode(NullValueId.NO_NODE);
					featureValue.Symbol = table.getNullValueSymbol(NullValueId.NO_NODE);
					featureValue.NullValue = true;
				}
				}
				catch (FormatException e)
				{
					throw new FeatureException("The index of the feature must be an integer value. ", e);
				}
			}
			else
			{
				featureValue.IndexCode = table.getNullValueCode(NullValueId.NO_NODE);
				featureValue.Symbol = table.getNullValueSymbol(NullValueId.NO_NODE);
				featureValue.NullValue = true;
			}
			featureValue.Value = 1;
		}

		public ColumnDescription Column
		{
			get
			{
				return column;
			}
			set
			{
				if (value.Type != ColumnDescription.INTEGER)
				{
					throw new FeatureException("InputArc feature column must be of type integer. ");
				}
				column = value;
			}
		}


		/// <summary>
		/// Returns the address function 1 (argument 1) 
		/// </summary>
		/// <returns> the address function 1 (argument 1)  </returns>
		public AddressFunction AddressFunction1
		{
			get
			{
				return addressFunction1;
			}
			set
			{
				addressFunction1 = value;
			}
		}



		/// <summary>
		/// Returns the address function 2 (argument 2) 
		/// </summary>
		/// <returns> the address function 1 (argument 2)  </returns>
		public AddressFunction AddressFunction2
		{
			get
			{
				return addressFunction2;
			}
			set
			{
				addressFunction2 = value;
			}
		}


		public DataFormatInstance DataFormatInstance
		{
			get
			{
				return dataFormatInstance;
			}
		}

		public SymbolTable SymbolTable
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


		public SymbolTableHandler TableHandler
		{
			get
			{
				return tableHandler;
			}
		}

		public int Type
		{
			get
			{
				return ColumnDescription.STRING;
			}
		}

		public string MapIdentifier
		{
			get
			{
				return SymbolTable.Name;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is InputArcFeature))
			{
				return false;
			}
			if (!obj.ToString().Equals(ToString()))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return "InputArc(" + column.Name + ")";
		}
	}

}