using System;

namespace org.maltparser.parser.algorithm.twoplanar
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using AddressFunction = org.maltparser.core.feature.function.AddressFunction;
	using AddressValue = org.maltparser.core.feature.value.AddressValue;

	/// 
	/// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public sealed class TwoPlanarAddressFunction : AddressFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(Integer)};
		public enum TwoPlanarSubFunction
		{
			ACTIVESTACK,
			INACTIVESTACK,
			INPUT
		}
		private readonly string subFunctionName;
		private readonly TwoPlanarSubFunction subFunction;
		private readonly AlgoritmInterface parsingAlgorithm;
		private int index;

		public TwoPlanarAddressFunction(string _subFunctionName, AlgoritmInterface _parsingAlgorithm) : base()
		{
			this.subFunctionName = _subFunctionName;
			this.subFunction = Enum.Parse(typeof(TwoPlanarSubFunction), subFunctionName.ToUpper());
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
			update((TwoPlanarConfig)parsingAlgorithm.CurrentParserConfiguration);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void update(object[] arguments)
		{
			if (arguments.Length != 1 || !(arguments[0] is TwoPlanarConfig))
			{
				throw new ParsingException("Arguments to the two-planar address function are not correct. ");
			}
			update((TwoPlanarConfig)arguments[0]);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void update(TwoPlanarConfig config) throws org.maltparser.core.exception.MaltChainedException
		private void update(TwoPlanarConfig config)
		{
			if (subFunction == TwoPlanarSubFunction.ACTIVESTACK)
			{
				address.Address = config.getActiveStackNode(index);
			}
			else if (subFunction == TwoPlanarSubFunction.INACTIVESTACK)
			{
				address.Address = config.getInactiveStackNode(index);
			}
			else if (subFunction == TwoPlanarSubFunction.INPUT)
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

		public TwoPlanarSubFunction SubFunction
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

			TwoPlanarAddressFunction other = (TwoPlanarAddressFunction) obj;
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
			return subFunctionName + "[" + index + "]";
		}
	}

}