using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.syntaxgraph.headrules
{
    using  io.dataformat;
	using  symbol;
	using  node;

    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class PrioList : List<PrioSet>
	{
		public const long serialVersionUID = 8045568022124816323L;
		protected internal HeadRule headRule;
		protected internal Direction direction;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrioList(HeadRule headRule, String listSpec) throws org.maltparser.core.exception.MaltChainedException
		public PrioList(HeadRule headRule, string listSpec)
		{
			HeadRule = headRule;
			init(listSpec);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void init(String listSpec) throws org.maltparser.core.exception.MaltChainedException
		public virtual void init(string listSpec)
		{
			string spec = listSpec.Trim();
			if (spec.Length < 8)
			{
				throw new HeadRuleException("The specification of the priority list is not correct '" + listSpec + "'. ");
			}
			if (spec[0] == 'r')
			{
				direction = Direction.RIGHT;
			}
			else if (spec[0] == 'l')
			{
				direction = Direction.LEFT;
			}
			else
			{
				throw new HeadRuleException("Could not determine the direction of the priority list '" + listSpec + "'. ");
			}
			if (spec[1] == '[' && spec[spec.Length - 1] == ']')
			{
				string[] items = spec.Substring(2, (spec.Length - 1) - 2).Split(" ", true);
				for (int i = 0; i < items.Length; i++)
				{
					Add(new PrioSet(this, items[i]));
				}
			}
			else
			{
				throw new HeadRuleException("The specification of the priority list is not correct '" + listSpec + "'. ");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode getHeadChild(org.maltparser.core.syntaxgraph.node.NonTerminalNode nt) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode getHeadChild(NonTerminalNode nt)
		{
			PhraseStructureNode headChild = null;
			for (int i = 0, n = Count; i < n; i++)
			{
				headChild = this[i].getHeadChild(nt, direction);
				if (headChild != null)
				{
					break;
				}
			}
			return headChild;
		}

		public virtual Logger Logger
		{
			get
			{
				return headRule.Logger;
			}
		}

		public virtual DataFormatInstance DataFormatInstance
		{
			get
			{
				return headRule.DataFormatInstance;
			}
		}

		public virtual SymbolTableHandler SymbolTableHandler
		{
			get
			{
				return headRule.SymbolTableHandler;
			}
		}

		public virtual HeadRule HeadRule
		{
			get
			{
				return headRule;
			}
			set
			{
				headRule = value;
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
//JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
//ORIGINAL LINE: return super.equals(obj);
			return base.SequenceEqual(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			if (direction == Direction.LEFT)
			{
				sb.Append("l[");
			}
			else if (direction == Direction.RIGHT)
			{
				sb.Append("r[");
			}
			foreach (PrioSet set in this)
			{
				sb.Append(set);
				sb.Append(' ');
			}
			if (sb.Length != 0)
			{
				sb.Length = sb.Length - 1;
			}
			sb.Append("]");
			return sb.ToString();
		}


	}

}