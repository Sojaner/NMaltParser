using System;

namespace org.maltparser.core.syntaxgraph.feature
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.symbol.nullvalue.NullValues;
	using  org.maltparser.core.syntaxgraph.node;

	/// 
	/// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public sealed class OutputArcFeature : FeatureFunction
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
//ORIGINAL LINE: public OutputArcFeature(org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public OutputArcFeature(DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler)
		{
			this.dataFormatInstance = dataFormatInstance;
			this.tableHandler = tableHandler;
			this.featureValue = new SingleFeatureValue(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public void initialize(object[] arguments)
		{
			if (arguments.Length != 3)
			{
				throw new FeatureException("Could not initialize OutputArcFeature: number of arguments are not correct. ");
			}
			// Checks that the two arguments are address functions

			if (!(arguments[0] is string))
			{
				throw new FeatureException("Could not initialize OutputArcFeature: the first argument is not a string. ");
			}
			if (!(arguments[1] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize OutputArcFeature: the second argument is not an address function. ");
			}
			if (!(arguments[2] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize OutputArcFeature: the third argument is not an address function. ");
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
			try
			{

				if (arg1.Address == null || arg2.Address == null)
				{
					featureValue.IndexCode = table.getNullValueCode(NullValueId.NO_NODE);
					featureValue.Symbol = table.getNullValueSymbol(NullValueId.NO_NODE);
					featureValue.NullValue = true;
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node1 = (org.maltparser.core.syntaxgraph.node.DependencyNode)arg1.getAddress();
					DependencyNode node1 = (DependencyNode)arg1.Address;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node2 = (org.maltparser.core.syntaxgraph.node.DependencyNode)arg2.getAddress();
					DependencyNode node2 = (DependencyNode)arg2.Address;

					int head1 = -1;
					int head2 = -1;

					if (node1.hasHead())
					{
						head1 = node1.Head.Index; //lines below don't seem to work
						//head1 = Integer.parseInt(node1.getHeadEdgeLabelSymbol(column.getSymbolTable()));
						//if ( node1.hasHeadEdgeLabel(column.getSymbolTable()) )
						//	head1 = Integer.parseInt(node1.getHeadEdgeLabelSymbol(column.getSymbolTable()));
					}
					if (node2.hasHead())
					{
						head2 = node2.Head.Index; //lines below don't seem to work
						//head2 = Integer.parseInt(node2.getHeadEdgeLabelSymbol(column.getSymbolTable()));
						//if ( node2.hasHeadEdgeLabel(column.getSymbolTable()) )
						//	head2 = Integer.parseInt(node2.getHeadEdgeLabelSymbol(column.getSymbolTable()));
					}
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
			}
			catch (System.FormatException e)
			{
				throw new FeatureException("The index of the feature must be an integer value. ", e);
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
					throw new FeatureException("OutputArc feature column must be of type integer. ");
				}
				this.column = value;
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
				this.addressFunction1 = value;
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
				this.addressFunction2 = value;
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
				this.table = value;
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
			if (!obj.ToString().Equals(this.ToString()))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return "OutputArc(" + column.Name + ")";
		}
	}

}