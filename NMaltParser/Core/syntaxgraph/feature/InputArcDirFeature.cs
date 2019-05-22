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
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class InputArcDirFeature : FeatureFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(string), typeof(AddressFunction)};
		private ColumnDescription column;
		private readonly DataFormatInstance dataFormatInstance;
		private readonly SymbolTableHandler tableHandler;
		private SymbolTable table;
		private readonly SingleFeatureValue featureValue;
		private AddressFunction addressFunction;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public InputArcDirFeature(org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public InputArcDirFeature(DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler)
		{
			this.dataFormatInstance = dataFormatInstance;
			this.tableHandler = tableHandler;
			this.featureValue = new SingleFeatureValue(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public void initialize(object[] arguments)
		{
			if (arguments.Length != 2)
			{
				throw new FeatureException("Could not initialize InputArcDirFeature: number of arguments are not correct. ");
			}
			if (!(arguments[0] is string))
			{
				throw new FeatureException("Could not initialize InputArcDirFeature: the first argument is not a string. ");
			}
			if (!(arguments[1] is AddressFunction))
			{
				throw new FeatureException("Could not initialize InputArcDirFeature: the second argument is not an address function. ");
			}
			Column = dataFormatInstance.getColumnDescriptionByName((string)arguments[0]);
			SymbolTable = tableHandler.addSymbolTable("ARCDIR_" + column.Name,ColumnDescription.INPUT, ColumnDescription.STRING, "one");
			table.addSymbol("LEFT");
			table.addSymbol("RIGHT");
			table.addSymbol("ROOT");
			AddressFunction = (AddressFunction)arguments[1];
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbol(int code) throws org.maltparser.core.exception.MaltChainedException
		public string getSymbol(int code)
		{
			return table.getSymbolCodeToString(code);
		}

		public FeatureValue FeatureValue
		{
			get
			{
				return featureValue;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public void update()
		{
			AddressValue a = addressFunction.AddressValue;
			if (a.Address != null && a.AddressClass == typeof(DependencyNode))
			{
				DependencyNode node = (DependencyNode)a.Address;
				try
				{
					int index = int.Parse(node.getLabelSymbol(tableHandler.getSymbolTable(column.Name)));
					if (node.Root)
					{
						featureValue.IndexCode = table.getNullValueCode(NullValueId.ROOT_NODE);
						featureValue.Symbol = table.getNullValueSymbol(NullValueId.ROOT_NODE);
						featureValue.NullValue = true;
					}
					else if (index == 0)
					{
						featureValue.IndexCode = table.getSymbolStringToCode("ROOT");
						featureValue.Symbol = "ROOT";
						featureValue.NullValue = false;
					}
					else if (index < node.Index)
					{
						featureValue.IndexCode = table.getSymbolStringToCode("LEFT");
						featureValue.Symbol = "LEFT";
						featureValue.NullValue = false;
					}
					else if (index > node.Index)
					{
						featureValue.IndexCode = table.getSymbolStringToCode("RIGHT");
						featureValue.Symbol = "RIGHT";
						featureValue.NullValue = false;
					}
				}
				catch (System.FormatException e)
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

		public AddressFunction AddressFunction
		{
			get
			{
				return addressFunction;
			}
			set
			{
				this.addressFunction = value;
			}
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
				this.column = value;
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
			if (!(obj is InputArcDirFeature))
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
			return "InputArcDir(" + column.Name + ", " + addressFunction.ToString() + ")";
		}
	}

}