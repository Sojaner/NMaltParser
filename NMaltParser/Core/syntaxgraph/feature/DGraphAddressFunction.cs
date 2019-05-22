using System;
using NMaltParser.Core.Feature.Function;
using NMaltParser.Core.Feature.Value;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph.Feature
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public sealed class DGraphAddressFunction : AddressFunction
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(AddressFunction)};
		public enum DGraphSubFunction
		{
			HEAD,
			LDEP,
			RDEP,
			RDEP2,
			LSIB,
			RSIB,
			PRED,
			SUCC,
			ANC,
			PANC,
			LDESC,
			PLDESC,
			RDESC,
			PRDESC
		}
		private AddressFunction addressFunction;
		private readonly string subFunctionName;
		private readonly DGraphSubFunction subFunction;

		public DGraphAddressFunction(string _subFunctionName) : base()
		{
			subFunctionName = _subFunctionName;
			subFunction = Enum.Parse(typeof(DGraphSubFunction), subFunctionName.ToUpper());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void initialize(object[] arguments)
		{
			if (arguments.Length != 1)
			{
				throw new SyntaxGraphException("Could not initialize DGraphAddressFunction: number of arguments are not correct. ");
			}
			if (!(arguments[0] is AddressFunction))
			{
				throw new SyntaxGraphException("Could not initialize DGraphAddressFunction: the second argument is not an addres function. ");
			}
			addressFunction = (AddressFunction)arguments[0];
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.value.AddressValue a = addressFunction.getAddressValue();
			AddressValue a = addressFunction.AddressValue;
			if (a.Address == null)
			{
				address.Address = null;
			}
			else
			{
	//			try { 
	//				a.getAddressClass().asSubclass(org.maltparser.core.syntaxgraph.node.DependencyNode.class);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode node = (org.maltparser.core.syntaxgraph.node.DependencyNode)a.getAddress();
					DependencyNode node = (DependencyNode)a.Address;
					if (subFunction == DGraphSubFunction.HEAD && !node.Root)
					{
						address.Address = node.Head;
					}
					else if (subFunction == DGraphSubFunction.LDEP)
					{
						address.Address = node.LeftmostDependent;
					}
					else if (subFunction == DGraphSubFunction.RDEP)
					{
						address.Address = node.RightmostDependent;
					}
					else if (subFunction == DGraphSubFunction.RDEP2)
					{
						// To emulate the behavior of MaltParser 0.4 (bug)
						if (!node.Root)
						{
							address.Address = node.RightmostDependent;
						}
						else
						{
							address.Address = null;
						}
					}
					else if (subFunction == DGraphSubFunction.LSIB)
					{
						address.Address = node.SameSideLeftSibling;
					}
					else if (subFunction == DGraphSubFunction.RSIB)
					{
						address.Address = node.SameSideRightSibling;
					}
					else if (subFunction == DGraphSubFunction.PRED && !node.Root)
					{
						address.Address = node.Predecessor;
					}
					else if (subFunction == DGraphSubFunction.SUCC && !node.Root)
					{
						address.Address = node.Successor;
					}
					else if (subFunction == DGraphSubFunction.ANC)
					{
						address.Address = node.Ancestor;
					}
					else if (subFunction == DGraphSubFunction.PANC)
					{
						address.Address = node.ProperAncestor;
					}
					else if (subFunction == DGraphSubFunction.LDESC)
					{
						address.Address = node.LeftmostDescendant;
					}
					else if (subFunction == DGraphSubFunction.PLDESC)
					{
						address.Address = node.LeftmostProperDescendant;
					}
					else if (subFunction == DGraphSubFunction.RDESC)
					{
						address.Address = node.RightmostDescendant;
					}
					else if (subFunction == DGraphSubFunction.PRDESC)
					{
						address.Address = node.RightmostProperDescendant;
					}
					else
					{
						address.Address = null;
					}
	//			} catch (ClassCastException e) {
	//				address.setAddress(null);
	//			}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public override void update(object[] arguments)
		{
			update();
		}

		public AddressFunction AddressFunction
		{
			get
			{
				return addressFunction;
			}
		}

		public string SubFunctionName
		{
			get
			{
				return subFunctionName;
			}
		}

		public DGraphSubFunction SubFunction
		{
			get
			{
				return subFunction;
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
			if (!addressFunction.Equals(((DGraphAddressFunction)obj).AddressFunction))
			{
				return false;
			}
			else if (!subFunction.Equals(((DGraphAddressFunction)obj).SubFunction))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return subFunctionName + "(" + addressFunction.ToString() + ")";
		}
	}

}