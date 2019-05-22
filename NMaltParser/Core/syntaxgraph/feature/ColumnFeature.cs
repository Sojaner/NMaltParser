using System;

namespace org.maltparser.core.syntaxgraph.feature
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.symbol.nullvalue.NullValues;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public abstract class ColumnFeature : FeatureFunction, Modifiable
	{
		protected internal ColumnDescription column;
		protected internal SymbolTable symbolTable;
		protected internal readonly SingleFeatureValue featureValue;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ColumnFeature() throws org.maltparser.core.exception.MaltChainedException
		public ColumnFeature()
		{
			this.featureValue = new SingleFeatureValue(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void update() throws org.maltparser.core.exception.MaltChainedException;
		public abstract void update();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException;
		public abstract void initialize(object[] arguments);
		public abstract Type[] ParameterTypes {get;}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbol(int value) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getSymbol(int value)
		{
			return symbolTable.getSymbolCodeToString(value);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCode(String value) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getCode(string value)
		{
			return symbolTable.getSymbolStringToCode(value);
		}

		public virtual ColumnDescription Column
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


		public virtual SymbolTable SymbolTable
		{
			get
			{
				return symbolTable;
			}
			set
			{
				this.symbolTable = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFeatureValue(int indexCode) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setFeatureValue(int indexCode)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String symbol = symbolTable.getSymbolCodeToString(indexCode);
			string symbol = symbolTable.getSymbolCodeToString(indexCode);

			if (string.ReferenceEquals(symbol, null))
			{
				featureValue.update(indexCode, symbolTable.getNullValueSymbol(NullValueId.NO_NODE), true, 1);
			}
			else
			{
				bool nullValue = symbolTable.isNullValue(indexCode);
				if (column.Type == ColumnDescription.STRING || nullValue)
				{
					featureValue.update(indexCode, symbol, nullValue, 1);
				}
				else
				{
					castFeatureValue(symbol);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFeatureValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setFeatureValue(string symbol)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int indexCode = symbolTable.getSymbolStringToCode(symbol);
			int indexCode = symbolTable.getSymbolStringToCode(symbol);
			if (indexCode < 0)
			{
				featureValue.update(symbolTable.getNullValueCode(NullValueId.NO_NODE), symbol, true, 1);
			}
			else
			{
				bool nullValue = symbolTable.isNullValue(symbol);
				if (column.Type == ColumnDescription.STRING || nullValue)
				{
					featureValue.update(indexCode, symbol, nullValue, 1);
				}
				else
				{
					castFeatureValue(symbol);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void castFeatureValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void castFeatureValue(string symbol)
		{
			if (column.Type == ColumnDescription.INTEGER)
			{
				try
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dotIndex = symbol.indexOf('.');
					int dotIndex = symbol.IndexOf('.');
					if (dotIndex == -1)
					{
						featureValue.Value = int.Parse(symbol);
						featureValue.Symbol = symbol;
					}
					else
					{
						featureValue.Value = int.Parse(symbol.Substring(0,dotIndex));
						featureValue.Symbol = symbol.Substring(0,dotIndex);
					}
					featureValue.NullValue = false;
					featureValue.IndexCode = 1;
				}
				catch (System.FormatException e)
				{
					throw new FeatureException("Could not cast the feature value '" + symbol + "' to integer value.", e);
				}
			}
			else if (column.Type == ColumnDescription.BOOLEAN)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dotIndex = symbol.indexOf('.');
				int dotIndex = symbol.IndexOf('.');
				if (symbol.Equals("1") || symbol.Equals("true") || symbol.Equals("#true#") || (dotIndex != -1 && symbol.Substring(0,dotIndex).Equals("1")))
				{
					featureValue.Value = 1;
					featureValue.Symbol = "true";
				}
				else if (symbol.Equals("false") || symbol.Equals("0") || (dotIndex != -1 && symbol.Substring(0,dotIndex).Equals("0")))
				{
					featureValue.Value = 0;
					featureValue.Symbol = "false";
				}
				else
				{
					throw new FeatureException("Could not cast the feature value '" + symbol + "' to boolean value.");
				}
				featureValue.NullValue = false;
				featureValue.IndexCode = 1;
			}
			else if (column.Type == ColumnDescription.REAL)
			{
				try
				{
					featureValue.Value = double.Parse(symbol);
	//				featureValue.setValue(symbolTable.getSymbolStringToValue(symbol));
					featureValue.Symbol = symbol;
				}
				catch (System.FormatException e)
				{
					throw new FeatureException("Could not cast the feature value '" + symbol + "' to real value.", e);
				}
				featureValue.NullValue = false;
				featureValue.IndexCode = 1;
			}
		}

		public virtual FeatureValue getFeatureValue()
		{
			return featureValue;
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

		public virtual string ColumnName
		{
			get
			{
				return column.Name;
			}
		}

		public virtual int Type
		{
			get
			{
				return column.Type;
			}
		}

		public virtual string MapIdentifier
		{
			get
			{
				return SymbolTable.Name;
			}
		}

		public override string ToString()
		{
			return column.Name;
		}
	}

}