using System;
using System.Text;

namespace org.maltparser.parser.algorithm.nivre
{
    using  core.feature.function;
	using  core.feature.value;

	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class NivreAddressFunction : AddressFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(Integer)};
		public enum NivreSubFunction
		{
			STACK,
			INPUT
		}
		private readonly string subFunctionName;
		private readonly NivreSubFunction subFunction;
		private readonly AlgoritmInterface parsingAlgorithm;
		private int index;

		public NivreAddressFunction(string _subFunctionName, AlgoritmInterface _parsingAlgorithm) : base()
		{
			subFunctionName = _subFunctionName;
			subFunction = Enum.Parse(typeof(NivreSubFunction), subFunctionName.ToUpper());
			parsingAlgorithm = _parsingAlgorithm;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void initialize(object[] arguments)
		{
			if (arguments.Length != 1)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ParsingException("Could not initialize " + GetType().FullName + ": number of arguments are not correct. ");
			}
			if (!(arguments[0] is int?))
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ParsingException("Could not initialize " + GetType().FullName + ": the first argument is not an integer. ");
			}

			Index = ((int?)arguments[0]).Value;
		}

		public override Type[] ParameterTypes
		{
			get
			{
				return paramTypes;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public override void update()
		{
			update((NivreConfig)parsingAlgorithm.CurrentParserConfiguration);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void update(object[] arguments)
		{
	//		if (arguments.length != 1 || !(arguments[0] instanceof NivreConfig)) {
	//			throw new ParsingException("Arguments to the Nivre address function is not correct. ");
	//		}
	//		update((NivreConfig)arguments[0]);
			if (subFunction == NivreSubFunction.STACK)
			{
				address.Address = ((NivreConfig)arguments[0]).getStackNode(index);
			}
			else if (subFunction == NivreSubFunction.INPUT)
			{
				address.Address = ((NivreConfig)arguments[0]).getInputNode(index);
			}
			else
			{
				address.Address = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void update(NivreConfig config) throws org.maltparser.core.exception.MaltChainedException
		private void update(NivreConfig config)
		{
			if (subFunction == NivreSubFunction.STACK)
			{
				address.Address = config.getStackNode(index);
			}
			else if (subFunction == NivreSubFunction.INPUT)
			{
				address.Address = config.getInputNode(index);
			}
			else
			{
				address.Address = null;
			}
		}

		public string SubFunctionName
		{
			get
			{
				return subFunctionName;
			}
		}

		public NivreSubFunction SubFunction
		{
			get
			{
				return subFunction;
			}
		}

		public override AddressValue AddressValue
		{
			get
			{
				return address;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
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

			NivreAddressFunction other = (NivreAddressFunction) obj;
			if (index != other.index)
			{
				return false;
			}
			if (parsingAlgorithm == null)
			{
				if (other.parsingAlgorithm != null)
				{
					return false;
				}
			}
			else if (!parsingAlgorithm.Equals(other.parsingAlgorithm))
			{
				return false;
			}
			if (subFunction == null)
			{
				if (other.subFunction != null)
				{
					return false;
				}
			}
			else if (!subFunction.Equals(other.subFunction))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(subFunctionName);
			sb.Append('[');
			sb.Append(index);
			sb.Append(']');
			return sb.ToString();
		}
	}

}