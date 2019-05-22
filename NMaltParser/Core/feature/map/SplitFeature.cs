using System;
using System.Text;

namespace org.maltparser.core.feature.map
{
    using  function;
    using  value;
    using  io.dataformat;
    using  symbol;

    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public sealed class SplitFeature : FeatureMapFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(FeatureFunction), typeof(string)};
		private FeatureFunction parentFeature;
		private readonly MultipleFeatureValue multipleFeatureValue;
		private readonly DataFormatInstance dataFormatInstance;
		private readonly SymbolTableHandler tableHandler;
		private ColumnDescription column;
		private SymbolTable table;
		private string separators;
		private Pattern separatorsPattern;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SplitFeature(org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public SplitFeature(DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler)
		{
			this.dataFormatInstance = dataFormatInstance;
			this.tableHandler = tableHandler;
			multipleFeatureValue = new MultipleFeatureValue(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public void initialize(object[] arguments)
		{
			if (arguments.Length != 2)
			{
				throw new FeatureException("Could not initialize SplitFeature: number of arguments are not correct. ");
			}
			if (!(arguments[0] is FeatureFunction))
			{
				throw new FeatureException("Could not initialize SplitFeature: the first argument is not a feature. ");
			}
			if (!(arguments[1] is string))
			{
				throw new FeatureException("Could not initialize SplitFeature: the second argument is not a string. ");
			}
			ParentFeature = (FeatureFunction)arguments[0];
			Separators = (string)arguments[1];
			ColumnDescription parentColumn = dataFormatInstance.getColumnDescriptionByName(parentFeature.SymbolTable.Name);
			if (parentColumn.Type != ColumnDescription.STRING)
			{
				throw new FeatureException("Could not initialize SplitFeature: the first argument must be a string. ");
			}
			Column = dataFormatInstance.addInternalColumnDescription(tableHandler, "SPLIT_" + parentFeature.SymbolTable.Name, parentColumn);
			SymbolTable = tableHandler.getSymbolTable(column.Name);
	//		setSymbolTable(tableHandler.addSymbolTable("SPLIT_"+parentFeature.getSymbolTable().getName(), parentFeature.getSymbolTable()));
		}

		public Type[] ParameterTypes
		{
			get
			{
				return paramTypes;
			}
		}

		public FeatureValue FeatureValue
		{
			get
			{
				return multipleFeatureValue;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbol(int code) throws org.maltparser.core.exception.MaltChainedException
		public string getSymbol(int code)
		{
			return table.getSymbolCodeToString(code);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public int getCode(string symbol)
		{
			return table.getSymbolStringToCode(symbol);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public void update()
		{
			multipleFeatureValue.reset();
			parentFeature.update();
			FunctionValue value = parentFeature.FeatureValue;
			if (value is SingleFeatureValue)
			{
				string symbol = ((SingleFeatureValue)value).Symbol;
				if (((FeatureValue)value).NullValue)
				{
					multipleFeatureValue.addFeatureValue(parentFeature.SymbolTable.getSymbolStringToCode(symbol), symbol);
					multipleFeatureValue.NullValue = true;
				}
				else
				{
					string[] items;
					try
					{
						items = separatorsPattern.split(symbol);
					}
					catch (PatternSyntaxException e)
					{
						throw new FeatureException("The split feature '" + ToString() + "' could not split the value using the following separators '" + separators + "'",e);
					}
					for (int i = 0; i < items.Length; i++)
					{
						if (items[i].Length > 0)
						{
							multipleFeatureValue.addFeatureValue(table.addSymbol(items[i]), items[i]);
						}
					}
					multipleFeatureValue.NullValue = false;
				}
			}
			else if (value is MultipleFeatureValue)
			{
				if (((MultipleFeatureValue)value).NullValue)
				{
					multipleFeatureValue.addFeatureValue(parentFeature.SymbolTable.getSymbolStringToCode(((MultipleFeatureValue)value).FirstSymbol), ((MultipleFeatureValue)value).FirstSymbol);
					multipleFeatureValue.NullValue = true;
				}
				else
				{
					foreach (string symbol in ((MultipleFeatureValue)value).Symbols)
					{
						string[] items;
						try
						{
							items = separatorsPattern.split(symbol);
						}
						catch (PatternSyntaxException e)
						{
							throw new FeatureException("The split feature '" + ToString() + "' could not split the value using the following separators '" + separators + "'", e);
						}
						for (int i = 0; i < items.Length; i++)
						{
							multipleFeatureValue.addFeatureValue(table.addSymbol(items[i]), items[i]);
						}
						multipleFeatureValue.NullValue = false;
					}
				}
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

		public FeatureFunction ParentFeature
		{
			get
			{
				return parentFeature;
			}
			set
			{
				parentFeature = value;
			}
		}


		public string Separators
		{
			get
			{
				return separators;
			}
			set
			{
				separators = value;
				separatorsPattern = Pattern.compile(value);
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

		public DataFormatInstance DataFormatInstance
		{
			get
			{
				return dataFormatInstance;
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
				column = value;
			}
		}


		public int Type
		{
			get
			{
				return column.Type;
			}
		}

		public string MapIdentifier
		{
			get
			{
				return SymbolTable.Name;
			}
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("Split(");
			sb.Append(parentFeature.ToString());
			sb.Append(", ");
			sb.Append(separators);
			sb.Append(')');
			return sb.ToString();
		}
	}


}