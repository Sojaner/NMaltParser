using System;
using System.Text;

namespace org.maltparser.core.feature.map
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureFunction = org.maltparser.core.feature.function.FeatureFunction;
	using FeatureMapFunction = org.maltparser.core.feature.function.FeatureMapFunction;
	using FeatureValue = org.maltparser.core.feature.value.FeatureValue;
	using MultipleFeatureValue = org.maltparser.core.feature.value.MultipleFeatureValue;
	using SingleFeatureValue = org.maltparser.core.feature.value.SingleFeatureValue;
	using ColumnDescription = org.maltparser.core.io.dataformat.ColumnDescription;
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public sealed class PrefixFeature : FeatureMapFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(org.maltparser.core.syntaxgraph.feature.InputColumnFeature), typeof(Integer)};
		private FeatureFunction parentFeature;
		private MultipleFeatureValue multipleFeatureValue;
		private readonly SymbolTableHandler tableHandler;
		private SymbolTable table;
		private readonly DataFormatInstance dataFormatInstance;
		private ColumnDescription column;
		private int prefixLength;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrefixFeature(org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public PrefixFeature(DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler)
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
				throw new FeatureException("Could not initialize PrefixFeature: number of arguments are not correct. ");
			}
			if (!(arguments[0] is FeatureFunction))
			{
				throw new FeatureException("Could not initialize PrefixFeature: the first argument is not a feature. ");
			}
			if (!(arguments[1] is int?))
			{
				throw new FeatureException("Could not initialize PrefixFeature: the second argument is not a string. ");
			}
			ParentFeature = (FeatureFunction)arguments[0];
			PrefixLength = ((int?)arguments[1]).Value;
			ColumnDescription parentColumn = dataFormatInstance.getColumnDescriptionByName(parentFeature.SymbolTable.Name);
			if (parentColumn.Type != ColumnDescription.STRING)
			{
				throw new FeatureException("Could not initialize PrefixFeature: the first argument must be a string. ");
			}
			Column = dataFormatInstance.addInternalColumnDescription(tableHandler, "PREFIX_" + prefixLength + "_" + parentFeature.SymbolTable.Name, parentColumn);
			SymbolTable = tableHandler.getSymbolTable(column.Name);
	//		setSymbolTable(tableHandler.addSymbolTable("PREFIX_"+prefixLength+"_"+parentFeature.getSymbolTable().getName(), parentFeature.getSymbolTable()));
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
			FeatureValue value = parentFeature.FeatureValue;
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
					string prefixStr;
					if (symbol.Length - prefixLength > 0)
					{
						prefixStr = symbol.Substring(0, prefixLength);
					}
					else
					{
						prefixStr = symbol;
					}
					int code = table.addSymbol(prefixStr);
					multipleFeatureValue.addFeatureValue(code, prefixStr);
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
						string prefixStr;
						if (symbol.Length - prefixLength > 0)
						{
							prefixStr = symbol.Substring(0, prefixLength);
						}
						else
						{
							prefixStr = symbol;
						}
						int code = table.addSymbol(prefixStr);
						multipleFeatureValue.addFeatureValue(code, prefixStr);
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


		public int PrefixLength
		{
			get
			{
				return prefixLength;
			}
			set
			{
				this.prefixLength = value;
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
			sb.Append("Prefix(");
			sb.Append(parentFeature.ToString());
			sb.Append(", ");
			sb.Append(prefixLength);
			sb.Append(')');
			return sb.ToString();
		}
	}


}