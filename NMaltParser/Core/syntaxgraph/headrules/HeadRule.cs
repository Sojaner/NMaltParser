using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.syntaxgraph.headrules
{

	using Logger = org.apache.log4j.Logger;
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using NonTerminalNode = org.maltparser.core.syntaxgraph.node.NonTerminalNode;
	using PhraseStructureNode = org.maltparser.core.syntaxgraph.node.PhraseStructureNode;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class HeadRule : List<PrioList>
	{
		public const long serialVersionUID = 8045568022124826323L;
		protected internal HeadRules headRules;
		protected internal SymbolTable table;
		protected internal int symbolCode;
		protected internal Direction defaultDirection;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HeadRule(HeadRules headRules, String ruleSpec) throws org.maltparser.core.exception.MaltChainedException
		public HeadRule(HeadRules headRules, string ruleSpec)
		{
			HeadRules = headRules;
			init(ruleSpec);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void init(String ruleSpec) throws org.maltparser.core.exception.MaltChainedException
		public virtual void init(string ruleSpec)
		{
			string spec = ruleSpec.Trim();
			string[] items = spec.Split("\t", true);
			if (items.Length != 3)
			{
				throw new HeadRuleException("The specification of the head rule is not correct '" + ruleSpec + "'. ");
			}

			int index = items[0].IndexOf(':');
			if (index != -1)
			{
				SymbolTable t = headRules.SymbolTableHandler.getSymbolTable(items[0].Substring(0, index));
				if (t == null)
				{
					throw new HeadRuleException("The specification of the head rule is not correct '" + ruleSpec + "'. ");
				}
				Table = t;
				SymbolCode = table.addSymbol(items[0].Substring(index + 1));
			}
			else
			{
				throw new HeadRuleException("The specification of the head rule is not correct '" + ruleSpec + "'. ");
			}
			if (items[1][0] == 'r')
			{
				defaultDirection = Direction.RIGHT;
			}
			else if (items[1][0] == 'l')
			{
				defaultDirection = Direction.LEFT;
			}
			else
			{
				throw new HeadRuleException("Could not determine the default direction of the head rule '" + ruleSpec + "'. ");
			}
			if (items[2].Length > 1)
			{
				if (items[2].IndexOf(';') == -1)
				{
					this.Add(new PrioList(this, items[2]));
				}
				else
				{
					string[] lists = items[2].Split(";", true);
					for (int i = 0; i < lists.Length; i++)
					{
						this.Add(new PrioList(this, lists[i]));
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode getHeadChild(org.maltparser.core.syntaxgraph.node.NonTerminalNode nt) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode getHeadChild(NonTerminalNode nt)
		{
			PhraseStructureNode headChild = null;
			for (int i = 0; i < this.Count; i++)
			{
				headChild = this[i].getHeadChild(nt);
				if (headChild != null)
				{
					break;
				}
			}
			return headChild;
		}

		public virtual SymbolTable Table
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


		public virtual int SymbolCode
		{
			get
			{
				return symbolCode;
			}
			set
			{
				this.symbolCode = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSymbolString() throws org.maltparser.core.exception.MaltChainedException
		public virtual string SymbolString
		{
			get
			{
				return table.getSymbolCodeToString(symbolCode);
			}
		}

		public virtual Direction DefaultDirection
		{
			get
			{
				return defaultDirection;
			}
			set
			{
				this.defaultDirection = value;
			}
		}


		public virtual Logger Logger
		{
			get
			{
				return headRules.Logger;
			}
		}

		public virtual DataFormatInstance DataFormatInstance
		{
			get
			{
				return headRules.DataFormatInstance;
			}
		}

		public virtual SymbolTableHandler SymbolTableHandler
		{
			get
			{
				return headRules.SymbolTableHandler;
			}
		}

		public virtual HeadRules HeadRules
		{
			set
			{
				this.headRules = value;
			}
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(table.Name);
			sb.Append(':');
			try
			{
				sb.Append(SymbolString);
			}
			catch (MaltChainedException e)
			{
				if (Logger.DebugEnabled)
				{
					Logger.debug("",e);
				}
				else
				{
					Logger.error(e.MessageChain);
				}
			}
			sb.Append('\t');
			if (defaultDirection == Direction.LEFT)
			{
				sb.Append('l');
			}
			else if (defaultDirection == Direction.RIGHT)
			{
				sb.Append('r');
			}
			sb.Append('\t');
			if (this.Count == 0)
			{
				sb.Append('*');
			}
			else
			{
				for (int i = 0; i < this.Count; i++)
				{
					sb.Append(this[i]);
					if (i < this.Count - 1)
					{
						sb.Append(';');
					}
				}
			}
			return sb.ToString();
		}
	}

}