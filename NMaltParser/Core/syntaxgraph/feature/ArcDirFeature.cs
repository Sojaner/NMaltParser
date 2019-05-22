using System;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Feature.Function;
using NMaltParser.Core.Feature.Value;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph.Feature
{
    public sealed class ArcDirFeature : FeatureFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(AddressFunction)};
		private AddressFunction addressFunction;
		private readonly SymbolTableHandler tableHandler;
		private SymbolTable table;
		private readonly SingleFeatureValue featureValue;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArcDirFeature(org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public ArcDirFeature(SymbolTableHandler tableHandler)
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
				throw new SyntaxGraphException("Could not initialize ArcDirFeature: number of arguments are not correct. ");
			}
			// Checks that the two arguments are address functions
			if (!(arguments[0] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize ArcDirFeature: the first argument is not an address function. ");
			}

			AddressFunction = (AddressFunction)arguments[0];

			// Creates a symbol table called "ARCDIR" using one null value
			SymbolTable = tableHandler.addSymbolTable("ARCDIR", ColumnDescription.INPUT, ColumnDescription.STRING, "one");

			table.addSymbol("LEFT"); // The address exists
			table.addSymbol("RIGHT"); // The address don't exists
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
			return table.getSymbolCodeToString(code);
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
			return table.getSymbolStringToCode(symbol);
		}

		/// <summary>
		/// Cause the feature function to update the feature value.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public void update()
		{
			if (addressFunction.AddressValue.Address != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node = (org.maltparser.core.syntaxgraph.node.DependencyNode)addressFunction.getAddressValue().getAddress();
				DependencyNode node = (DependencyNode)addressFunction.AddressValue.Address;
				if (!node.Root)
				{
					if (node.Head.Index < node.Index)
					{
						featureValue.IndexCode = table.getSymbolStringToCode("LEFT");
						featureValue.Value = 1;
						featureValue.Symbol = "LEFT";
						featureValue.NullValue = false;
					}
					else
					{
						featureValue.IndexCode = table.getSymbolStringToCode("RIGHT");
						featureValue.Value = 1;
						featureValue.Symbol = "RIGHT";
						featureValue.NullValue = false;
					}
				}
				else
				{
					featureValue.IndexCode = table.getNullValueCode(NullValueId.ROOT_NODE);
					featureValue.Value = 1;
					featureValue.Symbol = table.getNullValueSymbol(NullValueId.ROOT_NODE);
					featureValue.NullValue = true;
				}
			}
			else
			{
				featureValue.IndexCode = table.getNullValueCode(NullValueId.NO_NODE);
				featureValue.Value = 1;
				featureValue.Symbol = table.getNullValueSymbol(NullValueId.NO_NODE);
				featureValue.NullValue = true;
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
			sb.Append("ArcDir(");
			sb.Append(addressFunction.ToString());
			sb.Append(')');
			return sb.ToString();
		}
	}

}