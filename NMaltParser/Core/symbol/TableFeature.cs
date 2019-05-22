using System;

namespace org.maltparser.core.symbol
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureFunction = org.maltparser.core.feature.function.FeatureFunction;
	using Modifiable = org.maltparser.core.feature.function.Modifiable;
	using FeatureValue = org.maltparser.core.feature.value.FeatureValue;
	using SingleFeatureValue = org.maltparser.core.feature.value.SingleFeatureValue;
	using NullValueId = org.maltparser.core.symbol.nullvalue.NullValues.NullValueId;

	public abstract class TableFeature : FeatureFunction, Modifiable
	{
		protected internal readonly SingleFeatureValue featureValue;
		protected internal SymbolTable table;
		protected internal string tableName;
		protected internal SymbolTableHandler tableHandler;
		protected internal int type;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TableFeature(SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public TableFeature(SymbolTableHandler tableHandler)
		{
			this.tableHandler = tableHandler;
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
			return table.getSymbolCodeToString(value);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCode(String value) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getCode(string value)
		{
			return table.getSymbolStringToCode(value);
		}

		public virtual SymbolTable SymbolTable
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


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFeatureValue(int indexCode) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setFeatureValue(int indexCode)
		{
			if (string.ReferenceEquals(table.getSymbolCodeToString(indexCode), null))
			{
				featureValue.IndexCode = indexCode;
				featureValue.Value = 1;
				featureValue.Symbol = table.getNullValueSymbol(NullValueId.NO_NODE);
				featureValue.NullValue = true;
			}
			else
			{
				featureValue.IndexCode = indexCode;
				featureValue.Value = 1;
				featureValue.Symbol = table.getSymbolCodeToString(indexCode);
				featureValue.NullValue = table.isNullValue(indexCode);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFeatureValue(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setFeatureValue(string symbol)
		{
			if (table.getSymbolStringToCode(symbol) < 0)
			{
				featureValue.IndexCode = table.getNullValueCode(NullValueId.NO_NODE);
				featureValue.Value = 1;
				featureValue.Symbol = symbol;
				featureValue.NullValue = true;
			}
			else
			{
				featureValue.IndexCode = table.getSymbolStringToCode(symbol);
				featureValue.Value = 1;
				featureValue.Symbol = symbol;
				featureValue.NullValue = table.isNullValue(symbol);
			}
		}

		public virtual FeatureValue getFeatureValue()
		{
			return featureValue;
		}

		public virtual SymbolTableHandler TableHandler
		{
			get
			{
				return tableHandler;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is TableFeature))
			{
				return false;
			}
			if (!obj.ToString().Equals(this.ToString()))
			{
				return false;
			}
			return true;
		}

		public virtual string TableName
		{
			set
			{
				this.tableName = value;
			}
			get
			{
				return tableName;
			}
		}


		public virtual int Type
		{
			get
			{
				return type;
			}
			set
			{
				this.type = value;
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
			return tableName;
		}
	}

}