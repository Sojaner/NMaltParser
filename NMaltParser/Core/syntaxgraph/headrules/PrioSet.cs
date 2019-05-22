using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Utilities;

namespace NMaltParser.Core.SyntaxGraph.HeadRules
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class PrioSet : List<PrioSetMember>
	{
		public const long serialVersionUID = 8045568022124816313L;
		protected internal PrioList prioList;
		protected internal PrioSetMember cache;

		public PrioSet(PrioList prioList)
		{
			PrioList = prioList;
			cache = new PrioSetMember(this, null, null, -1, RelationToPrevMember.START);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrioSet(PrioList prioList, String setSpec) throws org.maltparser.core.exception.MaltChainedException
		public PrioSet(PrioList prioList, string setSpec)
		{
			PrioList = prioList;
			cache = new PrioSetMember(this, null, null, -1, RelationToPrevMember.START);
			init(setSpec);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void init(String setSpec) throws org.maltparser.core.exception.MaltChainedException
		public virtual void init(string setSpec)
		{
			string spec = setSpec.Trim();
			string[] disItems = spec.Split("\\|", true);
			for (int i = 0; i < disItems.Length; i++)
			{
				string[] conItems = spec.Split("\\&", true);
				for (int j = 0; j < conItems.Length; j++)
				{
					int index = conItems[j].IndexOf(':');
					if (index != -1)
					{
						SymbolTable table = prioList.SymbolTableHandler.getSymbolTable(conItems[j].Substring(0, index));
						ColumnDescription column = prioList.DataFormatInstance.getColumnDescriptionByName(conItems[j].Substring(0, index));
						if (i == 0 && j == 0)
						{
							addPrioSetMember(table, column, conItems[j].Substring(index + 1), RelationToPrevMember.START);
						}
						else if (j == 0)
						{
							addPrioSetMember(table, column, conItems[j].Substring(index + 1), RelationToPrevMember.DISJUNCTION);
						}
						else
						{
							addPrioSetMember(table, column, conItems[j].Substring(index + 1), RelationToPrevMember.CONJUNCTION);
						}
					}
					else
					{
						throw new HeadRuleException("The specification of the priority list is not correct '" + setSpec + "'. ");
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrioSetMember addPrioSetMember(org.maltparser.core.symbol.SymbolTable table, org.maltparser.core.io.dataformat.ColumnDescription column, String symbolString, org.maltparser.core.syntaxgraph.headrules.PrioSetMember.RelationToPrevMember relationToPrevMember) throws org.maltparser.core.exception.MaltChainedException
		public virtual PrioSetMember addPrioSetMember(SymbolTable table, ColumnDescription column, string symbolString, RelationToPrevMember relationToPrevMember)
		{
			if (table == null)
			{
				throw new HeadRuleException("Could add a member to priority set because the symbol table could be found. ");
			}
			return addPrioSetMember(table, column, table.addSymbol(symbolString), relationToPrevMember);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PrioSetMember addPrioSetMember(org.maltparser.core.symbol.SymbolTable table, org.maltparser.core.io.dataformat.ColumnDescription column, int symbolCode, org.maltparser.core.syntaxgraph.headrules.PrioSetMember.RelationToPrevMember relationToPrevMember) throws org.maltparser.core.exception.MaltChainedException
		public virtual PrioSetMember addPrioSetMember(SymbolTable table, ColumnDescription column, int symbolCode, RelationToPrevMember relationToPrevMember)
		{
			cache.Table = table;
			cache.SymbolCode = symbolCode;
			if (!Contains(cache))
			{
				PrioSetMember newItem = new PrioSetMember(this, table, column, symbolCode, relationToPrevMember);
				Add(newItem);
				return newItem;
			}
			return cache;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode getHeadChild(org.maltparser.core.syntaxgraph.node.NonTerminalNode nt, Direction direction) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode getHeadChild(NonTerminalNode nt, Direction direction)
		{
			bool match = false;
			if (direction == Direction.LEFT)
			{
				foreach (PhraseStructureNode child in nt.Children)
				{
					for (int j = 0; j < Count; j++)
					{
						match = matchHeadChild(child, this[j]);
						if (match == true)
						{
							if (j + 1 >= Count)
							{
								return child;
							}
							else if (this[j].RelationToPrevMember != RelationToPrevMember.CONJUNCTION)
							{
								return child;
							}
						}
					}
				}
			}
			else if (direction == Direction.RIGHT)
			{
				for (int i = nt.nChildren() - 1; i >= 0; i--)
				{
					PhraseStructureNode child = nt.getChild(i);
					for (int j = 0; j < Count; j++)
					{
						match = matchHeadChild(child, this[j]);
						if (match == true)
						{
							if (j + 1 >= Count)
							{
								return child;
							}
							else if (this[j].RelationToPrevMember != RelationToPrevMember.CONJUNCTION)
							{
								return child;
							}
						}
					}
				}
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean matchHeadChild(org.maltparser.core.syntaxgraph.node.PhraseStructureNode child, PrioSetMember member) throws org.maltparser.core.exception.MaltChainedException
		private bool matchHeadChild(PhraseStructureNode child, PrioSetMember member)
		{
			if (child is NonTerminalNode && member.Table.Name.Equals("CAT") && member.SymbolCode == child.getLabelCode(member.Table))
			{
				return true;
			}
			else if (member.Table.Name.Equals("LABEL") && member.SymbolCode == child.getParentEdgeLabelCode(member.Table))
			{
				return true;
			}
			else if (child is TokenNode && member.Column.Category == ColumnDescription.INPUT && member.SymbolCode == child.getLabelCode(member.Table))
			{
				return true;
			}
			return false;
		}

		public virtual Logger Logger
		{
			get
			{
				return prioList.Logger;
			}
		}

		public virtual PrioList PrioList
		{
			get
			{
				return prioList;
			}
			set
			{
				prioList = value;
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
			for (int i = 0; i < Count; i++)
			{
				if (i != 0)
				{
					if (this[i].RelationToPrevMember == RelationToPrevMember.CONJUNCTION)
					{
						sb.Append('&');
					}
					else if (this[i].RelationToPrevMember == RelationToPrevMember.DISJUNCTION)
					{
						sb.Append('|');
					}
				}
				sb.Append(this[i]);
			}
			return sb.ToString();
		}
	}

}