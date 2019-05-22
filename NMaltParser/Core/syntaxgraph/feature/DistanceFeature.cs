using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Feature.Function;
using NMaltParser.Core.Feature.Value;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph.Feature
{
    public sealed class DistanceFeature : FeatureFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(AddressFunction), typeof(AddressFunction), typeof(string)};
		private static readonly Pattern splitPattern = Pattern.compile("\\|");
		private AddressFunction addressFunction1;
		private AddressFunction addressFunction2;
		private readonly SymbolTableHandler tableHandler;
		private SymbolTable table;
		private readonly SingleFeatureValue featureValue;
		private string normalizationString;
		private readonly IDictionary<int, string> normalization;

	//	public DistanceFeature(FeatureRegistry registry) throws MaltChainedException {
	//		this(registry.getSymbolTableHandler());
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DistanceFeature(org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public DistanceFeature(SymbolTableHandler tableHandler)
		{
			featureValue = new SingleFeatureValue(this);
			this.tableHandler = tableHandler;
			normalization = new LinkedHashMap<int, string>();
		}

		/// <summary>
		/// Initialize the distance feature function
		/// </summary>
		/// <param name="arguments"> an array of arguments with the type returned by getParameterTypes() </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public void initialize(object[] arguments)
		{
			if (arguments.Length != 3)
			{
				throw new SyntaxGraphException("Could not initialize DistanceFeature: number of arguments is not correct. ");
			}
			// Checks that the two arguments are address functions
			if (!(arguments[0] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize DistanceFeature: the first argument is not an address function. ");
			}
			if (!(arguments[1] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize DistanceFeature: the second argument is not an address function. ");
			}
			if (!(arguments[2] is string))
			{
				throw new SyntaxGraphException("Could not initialize DistanceFeature: the third argument is not a string. ");
			}
			AddressFunction1 = (AddressFunction)arguments[0];
			AddressFunction2 = (AddressFunction)arguments[1];

			normalizationString = (string)arguments[2];
			// Creates a symbol table called "DISTANCE" using one null value
			SymbolTable = tableHandler.addSymbolTable("DISTANCE_" + normalizationString, ColumnDescription.INPUT, ColumnDescription.STRING, "one");

			string[] items = splitPattern.split(normalizationString);

			if (items.Length <= 0 || !items[0].Equals("0"))
			{
				throw new SyntaxGraphException("Could not initialize DistanceFeature (" + this + "): the third argument (normalization) must contain a list of integer values separated with | and the first element must be 0.");
			}
			int tmp = -1;
			for (int i = 0; i < items.Length; i++)
			{
				int v;
				try
				{
					v = int.Parse(items[i]);
				}
				catch (FormatException e)
				{
					throw new SyntaxGraphException("Could not initialize DistanceFeature (" + this + "): the third argument (normalization) must contain a sorted list of integer values separated with |", e);
				}
				normalization[v] = ">=" + v;
				table.addSymbol(">=" + v);
				if (tmp != -1 && tmp >= v)
				{
					throw new SyntaxGraphException("Could not initialize DistanceFeature (" + this + "): the third argument (normalization) must contain a sorted list of integer values separated with |");
				}
				tmp = v;
			}
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
		/// Returns the string representation of the integer <code>code</code> according to the distance feature function. 
		/// </summary>
		/// <param name="code"> the integer representation of the symbol </param>
		/// <returns> the string representation of the integer <code>code</code> according to the distance feature function. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbol(int code) throws org.maltparser.core.exception.MaltChainedException
		public string getSymbol(int code)
		{
			return table.getSymbolCodeToString(code);
		}

		/// <summary>
		/// Returns the integer representation of the string <code>symbol</code> according to the distance feature function.
		/// </summary>
		/// <param name="symbol"> the string representation of the symbol </param>
		/// <returns> the integer representation of the string <code>symbol</code> according to the distance feature function. </returns>
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
			// Retrieve the address value 
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.value.AddressValue arg1 = addressFunction1.getAddressValue();
			AddressValue arg1 = addressFunction1.AddressValue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.value.AddressValue arg2 = addressFunction2.getAddressValue();
			AddressValue arg2 = addressFunction2.AddressValue;
	//		featureValue.setKnown(true);
			// if arg1 or arg2 is null, then set a NO_NODE null value as feature value
			if (arg1.Address == null || arg2.Address == null)
			{
				featureValue.IndexCode = table.getNullValueCode(NullValueId.NO_NODE);
				featureValue.Symbol = table.getNullValueSymbol(NullValueId.NO_NODE);
				featureValue.Value = 1;

				featureValue.NullValue = true;
			}
			else
			{
				// Unfortunately this method takes a lot of time  arg1.getAddressClass().asSubclass(org.maltparser.core.syntaxgraph.node.DependencyNode.class);
				// Cast the address arguments to dependency nodes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node1 = (org.maltparser.core.syntaxgraph.node.DependencyNode)arg1.getAddress();
				DependencyNode node1 = (DependencyNode)arg1.Address;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node2 = (org.maltparser.core.syntaxgraph.node.DependencyNode)arg2.getAddress();
				DependencyNode node2 = (DependencyNode)arg2.Address;

				if (!node1.Root && !node2.Root)
				{
					// Calculates the distance
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index1 = node1.getIndex();
					int index1 = node1.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index2 = node2.getIndex();
					int index2 = node2.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int distance = Math.abs(index1-index2);
					int distance = Math.Abs(index1 - index2);


					int lower = -1;
					bool f = false;
					foreach (int? upper in normalization.Keys)
					{
						if (distance >= lower && distance < upper.Value)
						{
							featureValue.IndexCode = table.getSymbolStringToCode(normalization[lower]);
							featureValue.Symbol = normalization[lower];
							featureValue.Value = 1;
							f = true;
							break;
						}
						lower = upper.Value;
					}
					if (f == false)
					{
						featureValue.IndexCode = table.getSymbolStringToCode(normalization[lower]);
						featureValue.Symbol = normalization[lower];
						featureValue.Value = 1;
					}

					// Tells the feature value that the feature is known and is not a null value

					featureValue.NullValue = false;

				}
				else
				{
					// if node1 or node2 is a root node, set a ROOT_NODE null value as feature value
					featureValue.IndexCode = table.getNullValueCode(NullValueId.ROOT_NODE);
					featureValue.Symbol = table.getNullValueSymbol(NullValueId.ROOT_NODE);
					featureValue.Value = 1;
					featureValue.NullValue = true;
				}
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
		/// Returns the symbol table used by the distance feature function
		/// </summary>
		/// <returns> the symbol table used by the distance feature function </returns>
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
			sb.Append("Distance(");
			sb.Append(addressFunction1.ToString());
			sb.Append(", ");
			sb.Append(addressFunction2.ToString());
			sb.Append(", ");
			sb.Append(normalizationString);
			sb.Append(')');
			return sb.ToString();
		}
	}


}