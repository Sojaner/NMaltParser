using System;
using System.Text;

namespace org.maltparser.core.syntaxgraph.feature
{
	using  exception;
	using  org.maltparser.core.feature.function;
    using  org.maltparser.core.feature.value;
    using  io.dataformat;
	using  symbol;

    public sealed class ExistsFeature : FeatureFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(AddressFunction)};
		private AddressFunction addressFunction;
		private readonly SymbolTableHandler tableHandler;
		private SymbolTable table;
		private readonly SingleFeatureValue featureValue;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ExistsFeature(org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public ExistsFeature(SymbolTableHandler tableHandler)
		{
			this.tableHandler = tableHandler;
			featureValue = new SingleFeatureValue(this);
		}

		/// <summary>
		/// Initialize the exists feature function
		/// </summary>
		/// <param name="arguments"> an array of arguments with the type returned by getParameterTypes() </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public void initialize(object[] arguments)
		{
			if (arguments.Length != 1)
			{
				throw new SyntaxGraphException("Could not initialize ExistsFeature: number of arguments are not correct. ");
			}
			// Checks that the two arguments are address functions
			if (!(arguments[0] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize ExistsFeature: the first argument is not an address function. ");
			}

			AddressFunction = (AddressFunction)arguments[0];

			// Creates a symbol table called "EXISTS" using one null value
	//		setSymbolTable(tableHandler.addSymbolTable("EXISTS", ColumnDescription.INPUT, "one"));
	//		
	//		table.addSymbol("TRUE"); // The address exists
	//		table.addSymbol("FALSE"); // The address don't exists
		}

		/// <summary>
		/// Returns an array of class types used by the feature extraction system to invoke initialize with
		/// correct arguments.
		/// </summary>
		/// <returns> an array of class types </returns>
		public Type[] ParameterTypes
		{
			get
			{
				return paramTypes;
			}
		}
		/// <summary>
		/// Returns the string representation of the integer <code>code</code> according to the exists feature function. 
		/// </summary>
		/// <param name="code"> the integer representation of the symbol </param>
		/// <returns> the string representation of the integer <code>code</code> according to the exists feature function. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbol(int code) throws org.maltparser.core.exception.MaltChainedException
		public string getSymbol(int code)
		{
			return (code == 1)?"true":"false";
		}

		/// <summary>
		/// Returns the integer representation of the string <code>symbol</code> according to the exists feature function.
		/// </summary>
		/// <param name="symbol"> the string representation of the symbol </param>
		/// <returns> the integer representation of the string <code>symbol</code> according to the exists feature function. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getCode(String symbol) throws org.maltparser.core.exception.MaltChainedException
		public int getCode(string symbol)
		{
			return (symbol.Equals("true"))?1:0;
		}

		/// <summary>
		/// Cause the feature function to update the feature value.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public void update()
		{
			featureValue.IndexCode = 1;
			featureValue.NullValue = false;
			if (addressFunction.AddressValue.Address != null)
			{
				featureValue.Symbol = "true";
				featureValue.Value = 1;
			}
			else
			{
				featureValue.Symbol = "false";
				featureValue.Value = 0;
			}
		}

		/// <summary>
		/// Returns the feature value
		/// </summary>
		/// <returns> the feature value </returns>
		public FeatureValue FeatureValue
		{
			get
			{
				return featureValue;
			}
		}

		/// <summary>
		/// Returns the symbol table used by the exists feature function
		/// </summary>
		/// <returns> the symbol table used by the exists feature function </returns>
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

		/// <summary>
		/// Returns the address function 
		/// </summary>
		/// <returns> the address function  </returns>
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



		/// <summary>
		/// Returns symbol table handler
		/// </summary>
		/// <returns> a symbol table handler </returns>
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
				return ColumnDescription.BOOLEAN;
			}
		}

		public string MapIdentifier
		{
			get
			{
				return "EXISTS";
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
			sb.Append("Exists(");
			sb.Append(addressFunction.ToString());
			sb.Append(')');
			return sb.ToString();
		}
	}

}