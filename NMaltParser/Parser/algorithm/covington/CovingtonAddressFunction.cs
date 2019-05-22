using System;
using System.Text;

namespace org.maltparser.parser.algorithm.covington
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using AddressFunction = org.maltparser.core.feature.function.AddressFunction;
	using AddressValue = org.maltparser.core.feature.value.AddressValue;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class CovingtonAddressFunction : AddressFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(Integer)};
		public enum CovingtonSubFunction
		{
			LEFT,
			RIGHT,
			LEFTCONTEXT,
			RIGHTCONTEXT
		}
		private readonly string subFunctionName;
		private readonly CovingtonSubFunction subFunction;
		private readonly AlgoritmInterface parsingAlgorithm;
		private int index;

		public CovingtonAddressFunction(string _subFunctionName, AlgoritmInterface _parsingAlgorithm) : base()
		{
			this.subFunctionName = _subFunctionName;
			this.subFunction = Enum.Parse(typeof(CovingtonSubFunction), subFunctionName.ToUpper());
			this.parsingAlgorithm = _parsingAlgorithm;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void initialize(object[] arguments)
		{
			if (arguments.Length != 1)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ParsingException("Could not initialize " + this.GetType().FullName + ": number of arguments are not correct. ");
			}
			if (!(arguments[0] is int?))
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ParsingException("Could not initialize " + this.GetType().FullName + ": the first argument is not an integer. ");
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
			update((CovingtonConfig)parsingAlgorithm.CurrentParserConfiguration);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void update(object[] arguments)
		{
	//		if (arguments.length != 1 || !(arguments[0] instanceof CovingtonConfig)) {
	//			throw new ParsingException("Number of arguments to the Covington address function is not correct. ");
	//		}
			update((CovingtonConfig)arguments[0]);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void update(CovingtonConfig config) throws org.maltparser.core.exception.MaltChainedException
		private void update(CovingtonConfig config)
		{
			if (subFunction == CovingtonSubFunction.LEFT)
			{
				address.Address = config.getLeftNode(index);
			}
			else if (subFunction == CovingtonSubFunction.RIGHT)
			{
				address.Address = config.getRightNode(index);
			}
			else if (subFunction == CovingtonSubFunction.LEFTCONTEXT)
			{
				address.Address = config.getLeftContextNode(index);
			}
			else if (subFunction == CovingtonSubFunction.RIGHTCONTEXT)
			{
				address.Address = config.getRightContextNode(index);
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

		public CovingtonSubFunction SubFunction
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
				this.index = value;
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

			CovingtonAddressFunction other = (CovingtonAddressFunction) obj;
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