using System;
using System.Text;

namespace org.maltparser.core.feature.map
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.symbol;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public sealed class SuffixFeature : FeatureMapFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(org.maltparser.core.syntaxgraph.feature.InputColumnFeature), typeof(Integer)};
		private FeatureFunction parentFeature;
		private readonly MultipleFeatureValue multipleFeatureValue;
		private readonly SymbolTableHandler tableHandler;
		private SymbolTable table;
		private readonly DataFormatInstance dataFormatInstance;
		private ColumnDescription column;
		private int suffixLength;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SuffixFeature(org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public SuffixFeature(DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler)
		{
			this.dataFormatInstance = dataFormatInstance;
			this.tableHandler = tableHandler;
			this.multipleFeatureValue = new MultipleFeatureValue(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public void initialize(object[] arguments)
		{
			if (arguments.Length != 2)
			{
				throw new FeatureException("Could not initialize SuffixFeature: number of arguments are not correct. ");
			}
			if (!(arguments[0] is FeatureFunction))
			{
				throw new FeatureException("Could not initialize SuffixFeature: the first argument is not a feature. ");
			}
			if (!(arguments[1] is int?))
			{
				throw new FeatureException("Could not initialize SuffixFeature: the second argument is not a string. ");
			}
			ParentFeature = (FeatureFunction)arguments[0];
			SuffixLength = ((int?)arguments[1]).Value;
			ColumnDescription parentColumn = dataFormatInstance.getColumnDescriptionByName(parentFeature.SymbolTable.Name);
			if (parentColumn.Type != ColumnDescription.STRING)
			{
				throw new FeatureException("Could not initialize SuffixFeature: the first argument must be a string. ");
			}
			Column = dataFormatInstance.addInternalColumnDescription(tableHandler,"SUFFIX_" + suffixLength + "_" + parentFeature.SymbolTable.Name, parentColumn);
			SymbolTable = tableHandler.getSymbolTable(column.Name);
	//		setSymbolTable(tableHandler.addSymbolTable("SUFFIX_"+suffixLength+"_"+parentFeature.getSymbolTable().getName(), parentFeature.getSymbolTable()));
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public void update()
		{
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
					string suffixStr;
					if (symbol.Length - suffixLength > 0)
					{
						suffixStr = symbol.Substring(symbol.Length - suffixLength);
					}
					else
					{
						suffixStr = symbol;
					}
					int code = table.addSymbol(suffixStr);
					multipleFeatureValue.addFeatureValue(code, suffixStr);
					multipleFeatureValue.NullValue = false;
				}
			}
			else if (value is MultipleFeatureValue)
			{
				multipleFeatureValue.reset();
				if (((MultipleFeatureValue)value).NullValue)
				{
					multipleFeatureValue.addFeatureValue(parentFeature.SymbolTable.getSymbolStringToCode(((MultipleFeatureValue)value).FirstSymbol), ((MultipleFeatureValue)value).FirstSymbol);
					multipleFeatureValue.NullValue = true;
				}
				else
				{
					foreach (string symbol in ((MultipleFeatureValue)value).Symbols)
					{
						string suffixStr;
						if (symbol.Length - suffixLength > 0)
						{
							suffixStr = symbol.Substring(symbol.Length - suffixLength);
						}
						else
						{
							suffixStr = symbol;
						}
						int code = table.addSymbol(suffixStr);
						multipleFeatureValue.addFeatureValue(code, suffixStr);
						multipleFeatureValue.NullValue = true;
					}
				}
			}
		}

		public FeatureFunction ParentFeature
		{
			get
			{
				return parentFeature;
			}
			set
			{
				this.parentFeature = value;
			}
		}


		public int SuffixLength
		{
			get
			{
				return suffixLength;
			}
			set
			{
				this.suffixLength = value;
			}
		}


		public SymbolTableHandler TableHandler
		{
			get
			{
				return tableHandler;
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
				this.column = value;
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
			return obj.ToString().Equals(this.ToString());
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("Suffix(");
			sb.Append(parentFeature.ToString());
			sb.Append(", ");
			sb.Append(suffixLength);
			sb.Append(')');
			return sb.ToString();
		}
	}

}