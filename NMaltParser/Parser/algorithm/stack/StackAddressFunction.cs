using System;
using System.Text;

namespace org.maltparser.parser.algorithm.stack
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.value;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class StackAddressFunction : AddressFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(Integer)};
		public enum StackSubFunction
		{
			STACK,
			INPUT,
			LOOKAHEAD
		}
		private readonly string subFunctionName;
		private readonly StackSubFunction subFunction;
		private readonly AlgoritmInterface parsingAlgorithm;
		private int index;

		public StackAddressFunction(string _subFunctionName, AlgoritmInterface _parsingAlgorithm) : base()
		{
			this.subFunctionName = _subFunctionName;
			this.subFunction = Enum.Parse(typeof(StackSubFunction), subFunctionName.ToUpper());
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
			update((StackConfig)parsingAlgorithm.CurrentParserConfiguration);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void update(object[] arguments)
		{
			if (subFunction == StackSubFunction.STACK)
			{
				address.Address = ((StackConfig)arguments[0]).getStackNode(index);
			}
			else if (subFunction == StackSubFunction.LOOKAHEAD)
			{
				address.Address = ((StackConfig)arguments[0]).getLookaheadNode(index);
			}
			else if (subFunction == StackSubFunction.INPUT)
			{
				address.Address = ((StackConfig)arguments[0]).getInputNode(index);
			}
			else
			{
				address.Address = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void update(StackConfig config) throws org.maltparser.core.exception.MaltChainedException
		private void update(StackConfig config)
		{
			if (subFunction == StackSubFunction.STACK)
			{
				address.Address = config.getStackNode(index);
			}
			else if (subFunction == StackSubFunction.LOOKAHEAD)
			{
				address.Address = config.getLookaheadNode(index);
			}
			else if (subFunction == StackSubFunction.INPUT)
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

		public StackSubFunction SubFunction
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

			StackAddressFunction other = (StackAddressFunction) obj;
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