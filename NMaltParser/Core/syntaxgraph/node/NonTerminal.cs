using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Helper;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.HeadRules;

namespace NMaltParser.Core.SyntaxGraph.Node
{
    public class NonTerminal : GraphNode, PhraseStructureNode, NonTerminalNode
	{
		public const int INDEX_OFFSET = 10000000;
		protected internal readonly SortedSet<PhraseStructureNode> children;
		protected internal PhraseStructureNode parent;
		protected internal int index;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NonTerminal() throws org.maltparser.core.exception.MaltChainedException
		public NonTerminal() : base()
		{
			index = -1;
			children = new SortedSet<PhraseStructureNode>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException
		public override void addIncomingEdge(Edge.Edge @in)
		{
			base.addIncomingEdge(@in);
			if (@in.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE && @in.Source is PhraseStructureNode)
			{
				parent = (PhraseStructureNode)@in.Source;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException
		public override void removeIncomingEdge(Edge.Edge @in)
		{
			base.removeIncomingEdge(@in);
			if (@in.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE && @in.Source is PhraseStructureNode)
			{
				if (@in.Source == parent)
				{
					parent = null;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public override void addOutgoingEdge(Edge.Edge @out)
		{
			base.addOutgoingEdge(@out);
			if (@out.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE && @out.Target is PhraseStructureNode)
			{
				children.Add((PhraseStructureNode)@out.Target);
	//			boolean notSorted = true;
	//			PhraseStructureNode prev = children.first();
	//			for (PhraseStructureNode node : children) {
	//				if (prev != node) {
	//					if (node.compareTo(prev) == -1) {
	//						notSorted = false;
	//						System.out.println("NS");
	//						break;
	//					}
	//				} 
	//				prev = node;
	//			}
	//			if (notSorted == false) {
	//				SortedSet<PhraseStructureNode> tmp = new TreeSet<PhraseStructureNode>(children);
	//				children.clear();
	//				for (PhraseStructureNode node : tmp) {
	//					children.add(node);
	//				}
	//			}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public override void removeOutgoingEdge(Edge.Edge @out)
		{
			base.removeOutgoingEdge(@out);
			if (@out.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE && @out.Target is PhraseStructureNode)
			{
				children.remove(@out.Target);
			}
		}

		public virtual PhraseStructureNode Parent
		{
			get
			{
				return parent;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge getParentEdge() throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge ParentEdge
		{
			get
			{
				foreach (Edge.Edge e in incomingEdges)
				{
					if (e.Source == parent && e.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE)
					{
						return e;
					}
				}
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getParentEdgeLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getParentEdgeLabelSymbol(SymbolTable table)
		{
			foreach (Edge.Edge e in incomingEdges)
			{
				if (e.Source == parent && e.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE)
				{
					return e.getLabelSymbol(table);
				}
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getParentEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getParentEdgeLabelCode(SymbolTable table)
		{
			foreach (Edge.Edge e in incomingEdges)
			{
				if (e.Source == parent && e.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE)
				{
					return e.getLabelCode(table);
				}
			}
			return -1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasParentEdgeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool hasParentEdgeLabel(SymbolTable table)
		{
			foreach (Edge.Edge e in incomingEdges)
			{
				if (e.Source == parent && e.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE)
				{
					return e.hasLabel(table);
				}
			}
			return false;
		}

		public virtual int Height
		{
			get
			{
				int max = -1;
				foreach (PhraseStructureNode node in children)
				{
					if (node is Token)
					{
						if (max < 0)
						{
							max = 0;
						}
					}
					else
					{
						int nodeheight = ((NonTerminalNode)node).Height;
						if (max < nodeheight)
						{
							max = nodeheight;
						}
					}
				}
				return max + 1;
			}
		}

		public virtual SortedSet<PhraseStructureNode> Children
		{
			get
			{
				return new SortedSet<PhraseStructureNode>(children);
			}
		}

		public virtual PhraseStructureNode getChild(int index)
		{
			if (index >= 0 && index < children.Count)
			{
				int i = 0;
				foreach (PhraseStructureNode node in children)
				{
					if (i == index)
					{
						return node;
					}
					i++;
				}
	//			return children.toArray(new PhraseStructureNode[children.size()])[index];
			}
			return null;
		}

		public virtual PhraseStructureNode LeftChild
		{
			get
			{
				foreach (PhraseStructureNode node in children)
				{
					return node;
				}
				return null;
			}
		}

		public virtual PhraseStructureNode RightChild
		{
			get
			{
				int n = children.Count;
				int i = 1;
				foreach (PhraseStructureNode node in children)
				{
					if (i == n)
					{
						return node;
					}
					i++;
				}
				return null;
			}
		}

		public virtual int nChildren()
		{
			return children.Count;
		}

		public virtual bool hasNonTerminalChildren()
		{
			foreach (PhraseStructureNode node in children)
			{
				if (node is NonTerminal)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool hasTerminalChildren()
		{
			foreach (PhraseStructureNode node in children)
			{
				if (node is Token)
				{
					return true;
				}
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TokenNode getLexicalHead(org.maltparser.core.syntaxgraph.headrules.HeadRules headRules) throws org.maltparser.core.exception.MaltChainedException
		public virtual TokenNode getLexicalHead(HeadRules.HeadRules headRules)
		{
			return identifyHead(headRules);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PhraseStructureNode getHeadChild(org.maltparser.core.syntaxgraph.headrules.HeadRules headRules) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode getHeadChild(HeadRules.HeadRules headRules)
		{
			return identifyHeadChild(headRules);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TokenNode getLexicalHead() throws org.maltparser.core.exception.MaltChainedException
		public virtual TokenNode LexicalHead
		{
			get
			{
				return identifyHead(null);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PhraseStructureNode getHeadChild() throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode HeadChild
		{
			get
			{
				return identifyHeadChild(null);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private PhraseStructureNode identifyHeadChild(org.maltparser.core.syntaxgraph.headrules.HeadRules headRules) throws org.maltparser.core.exception.MaltChainedException
		private PhraseStructureNode identifyHeadChild(HeadRules.HeadRules headRules)
		{
			PhraseStructureNode headChild = (headRules == null)?null:headRules.getHeadChild(this);
			if (headChild == null)
			{
				Direction direction = (headRules == null)?Direction.LEFT:headRules.getDefaultDirection(this);
				if (direction == Direction.LEFT)
				{
					if ((headChild = leftmostTerminalChild()) == null)
					{
						headChild = leftmostNonTerminalChild();
					}
				}
				else
				{
					if ((headChild = rightmostTerminalChild()) == null)
					{
						headChild = rightmostNonTerminalChild();
					}
				}
			}
			return headChild;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TokenNode identifyHead(org.maltparser.core.syntaxgraph.headrules.HeadRules headRules) throws org.maltparser.core.exception.MaltChainedException
		public virtual TokenNode identifyHead(HeadRules.HeadRules headRules)
		{
			PhraseStructureNode headChild = identifyHeadChild(headRules);
			TokenNode lexicalHead = null;
			if (headChild is NonTerminalNode)
			{
				lexicalHead = ((NonTerminalNode)headChild).identifyHead(headRules);
			}
			else if (headChild is TokenNode)
			{
				lexicalHead = (TokenNode)headChild;
			}
			foreach (PhraseStructureNode node in children)
			{
				if (node != headChild && node is NonTerminalNode)
				{
					((NonTerminalNode)node).identifyHead(headRules);
				}
			}
			return lexicalHead;
		}

		public override int Index
		{
			get
			{
				return index;
			}
			set
			{
				if (value > 0)
				{
					index = value; //INDEX_OFFSET+value;
				}
				else
				{
					throw new SyntaxGraphException("The index must be a positive index");
				}
    
			}
		}

		public override int CompareToIndex
		{
			get
			{
				return index + INDEX_OFFSET;
			}
		}

		private PhraseStructureNode leftmostTerminalChild()
		{
			foreach (PhraseStructureNode node in children)
			{
				if (node is TokenNode)
				{
					return node;
				}
			}
			return null;
		}

		private PhraseStructureNode leftmostNonTerminalChild()
		{
			foreach (PhraseStructureNode node in children)
			{
				if (node is NonTerminalNode)
				{
					return node;
				}
			}
			return null;
		}

		private PhraseStructureNode rightmostTerminalChild()
		{
			try
			{
				if (children.Max is TokenNode)
				{
					return children.Max;
				}
			}
			catch (NoSuchElementException)
			{
			}

			PhraseStructureNode candidate = null;
			foreach (PhraseStructureNode node in children)
			{
				if (node is TokenNode)
				{
					candidate = node;
				}
			}
			return candidate;
		}

		private PhraseStructureNode rightmostNonTerminalChild()
		{
			try
			{
				if (children.Max is NonTerminalNode)
				{
					return children.Max;
				}
			}
			catch (NoSuchElementException)
			{
			}

			PhraseStructureNode candidate = null;
			foreach (PhraseStructureNode node in children)
			{
				if (node is NonTerminalNode)
				{
					candidate = node;
				}
			}
			return candidate;
		}

	//	protected void getPhraseDominationSet(SortedSet<PhraseStructureNode> dominationSet) {
	//		for (PhraseStructureNode node : children) {
	//			if (node instanceof TerminalNode) {
	//				dominationSet.add(node);
	//			} else {
	//				((NonTerminal)node).getPhraseDominationSet(dominationSet);
	//			}
	//		}
	//	}
	//	
	//	private SortedSet<PhraseStructureNode> getPhraseDominationSet() {
	//		SortedSet<PhraseStructureNode> dominationSet = new TreeSet<PhraseStructureNode>();
	//		getPhraseDominationSet(dominationSet);
	//		return dominationSet;
	//	}



		public virtual bool Continuous
		{
			get
			{
				int lcorner = LeftmostProperDescendant.Index;
				int rcorner = RightmostProperDescendant.Index;
    
				if (lcorner == rcorner)
				{
					return true;
				}
    
				TokenNode terminal = ((ITokenStructure)BelongsToGraph).GetTokenNode(lcorner);
				while (terminal.Index != rcorner)
				{
					PhraseStructureNode tmp = terminal.Parent;
					while (true)
					{
						if (tmp == this)
						{
							break;
						}
						if (tmp == null)
						{
							return false;
						}
						tmp = tmp.Parent;
					}
					terminal = terminal.TokenNodeSuccessor;
				}
    
				return true;
			}
		}

		public virtual bool ContinuousExcludeTerminalsAttachToRoot
		{
			get
			{
				int lcorner = LeftmostProperDescendant.Index;
				int rcorner = RightmostProperDescendant.Index;
    
				if (lcorner == rcorner)
				{
					return true;
				}
    
				TokenNode terminal = ((ITokenStructure)BelongsToGraph).GetTokenNode(lcorner);
				while (terminal.Index != rcorner)
				{
					if (terminal.Parent != null && terminal.Parent.Root)
					{
						terminal = terminal.TokenNodeSuccessor;
						continue;
					}
					PhraseStructureNode tmp = terminal.Parent;
					while (true)
					{
						if (tmp == this)
						{
							break;
						}
						if (tmp == null)
						{
							return false;
						}
						tmp = tmp.Parent;
					}
					terminal = terminal.TokenNodeSuccessor;
				}
				return true;
			}
		}

		public override bool Root
		{
			get
			{
				return false;
			}
		}


		public override ComparableNode LeftmostProperDescendant
		{
			get
			{
				NonTerminalNode node = this;
				PhraseStructureNode candidate = null;
				while (node != null)
				{
					candidate = node.LeftChild;
					if (candidate == null || candidate is TokenNode)
					{
						return candidate;
					}
					node = (NonTerminalNode)candidate;
				}
				return null;
			}
		}

		public override ComparableNode RightmostProperDescendant
		{
			get
			{
				NonTerminalNode node = this;
				PhraseStructureNode candidate = null;
				while (node != null)
				{
					candidate = node.RightChild;
					if (candidate == null || candidate is TokenNode)
					{
						return candidate;
					}
					node = (NonTerminalNode)candidate;
				}
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getLeftmostDescendant() throws org.maltparser.core.exception.MaltChainedException
		public override ComparableNode LeftmostDescendant
		{
			get
			{
				return LeftmostProperDescendant;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getRightmostDescendant() throws org.maltparser.core.exception.MaltChainedException
		public override ComparableNode RightmostDescendant
		{
			get
			{
				return RightmostProperDescendant;
			}
		}

	//	public void reArrangeChildrenAccordingToLeftAndRightProperDesendant() throws MaltChainedException {
	//		int i = 0;
	//		int leftMostCorner = -1;
	//		int leftMostCornerChildIndex = -1;
	//		while (i < children.size()) {	
	//			if (leftMostCorner == -1) {
	//				leftMostCorner = getChild(i).getLeftmostProperDescendantIndex();
	//				leftMostCornerChildIndex = i;
	//			} else if (getChild(i) instanceof NonTerminal && getChild(i).getLeftmostProperDescendantIndex() < leftMostCorner) {
	//				NonTerminalNode child = (NonTerminal)getChild(i);
	//				PhraseStructureNode leftMostCornerChild = getChild(leftMostCornerChildIndex);
	//				if (leftMostCornerChild.getParent() != null) {
	//					LabelSet labelSet = leftMostCornerChild.getParentEdge().getLabelSet();
	//					((PhraseStructure)getBelongsToGraph()).removePhraseStructureEdge(this, leftMostCornerChild);
	//					Edge e = ((PhraseStructure)getBelongsToGraph()).addPhraseStructureEdge(child, leftMostCornerChild);
	//					e.addLabel(labelSet);
	//				}
	//				child.reArrangeChildrenAccordingToLeftAndRightProperDesendant();
	//				i = -1;
	//				leftMostCorner = -1;
	//				leftMostCornerChildIndex = -1;
	//			} else {
	//				leftMostCorner = getChild(i).getLeftmostProperDescendantIndex();
	//				leftMostCornerChildIndex = i;
	//			}
	//			i++;
	//		}
	//
	//		for (int j = 0, n=children.size(); j < n; j++) {
	//			if (getChild(j) instanceof NonTerminalNode) {
	//				((NonTerminalNode)getChild(j)).reArrangeChildrenAccordingToLeftAndRightProperDesendant();
	//			}
	//		}
	//	}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			base.clear();
			children.Clear();
			parent = null;
			index = -1;
		}

		public override int compareTo(ComparableNode o)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;
			if (this == o)
			{
				return EQUAL;
			}
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thisLCorner = this.getLeftmostProperDescendantIndex();
				int thisLCorner = LeftmostProperDescendantIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thatLCorner = (o instanceof TokenNode)?o.getCompareToIndex():o.getLeftmostProperDescendantIndex();
				int thatLCorner = (o is TokenNode)?o.CompareToIndex:o.LeftmostProperDescendantIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thisRCorner = this.getRightmostProperDescendantIndex();
				int thisRCorner = RightmostProperDescendantIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thatRCorner = (o instanceof TokenNode)?o.getCompareToIndex():o.getRightmostProperDescendantIndex();
				int thatRCorner = (o is TokenNode)?o.CompareToIndex:o.RightmostProperDescendantIndex;

	//		    if (thisLCorner == -1 || thatLCorner == -1) {
	//			    if (thisRCorner < thatRCorner) return BEFORE;
	//			    if (thisRCorner > thatRCorner) return AFTER;
	//		    }
	//		    if (thisRCorner == -1 || thatRCorner == -1) {
	//			    if (thisLCorner < thatLCorner) return BEFORE;
	//			    if (thisLCorner > thatLCorner) return AFTER;
	//		    }

				if (thisLCorner != -1 && thatLCorner != -1 && thisRCorner != -1 && thatRCorner != -1)
				{
					if (thisLCorner < thatLCorner && thisRCorner < thatRCorner)
					{
						return BEFORE;
					}
					if (thisLCorner > thatLCorner && thisRCorner > thatRCorner)
					{
						return AFTER;
					}
					if (thisLCorner > thatLCorner && thisRCorner < thatRCorner)
					{
						return BEFORE;
					}
					if (thisLCorner < thatLCorner && thisRCorner > thatRCorner)
					{
						return AFTER;
					}
				}
				else
				{
					if (thisLCorner != -1 && thatLCorner != -1)
					{
						if (thisLCorner < thatLCorner)
						{
							return BEFORE;
						}
						if (thisLCorner > thatLCorner)
						{
							return AFTER;
						}
					}
					if (thisRCorner != -1 && thatRCorner != -1)
					{
						if (thisRCorner < thatRCorner)
						{
							return BEFORE;
						}
						if (thisRCorner > thatRCorner)
						{
							return AFTER;
						}
					}
				}



	//		    final int thisLCorner = this.getLeftmostDescendantIndex();
	//		    final int thatLCorner = (o instanceof TerminalNode)?o.getCompareToIndex():o.getLeftmostDescendantIndex();
	//		    final int thisRCorner = this.getRightmostDescendantIndex();
	//		    final int thatRCorner = (o instanceof TerminalNode)?o.getCompareToIndex():o.getRightmostDescendantIndex();
	//
	//		    if (thisLCorner == -1 || thatLCorner == -1) {
	//			    if (thisRCorner < thatRCorner) return BEFORE;
	//			    if (thisRCorner > thatRCorner) return AFTER;
	//		    }
	//		    if (thisRCorner == -1 || thatRCorner == -1) {
	//			    if (thisLCorner < thatLCorner) return BEFORE;
	//			    if (thisLCorner > thatLCorner) return AFTER;
	//		    }
	//		    if (thisLCorner < thatLCorner && thisRCorner < thatRCorner) return BEFORE;
	//		    if (thisLCorner > thatLCorner && thisRCorner > thatRCorner) return AFTER;
	//		    if (thisLCorner > thatLCorner && thisRCorner < thatRCorner) return BEFORE;
	//		    if (thisLCorner < thatLCorner && thisRCorner > thatRCorner) return AFTER;

	//		    System.out.println(thisLCorner + " " + thatLCorner + " " +thisRCorner + " " + thatRCorner);

	//		    int thisCorner = this.getLeftmostDescendantIndex();
	//		    int thatCorner = (o instanceof TerminalNode)?o.getCompareToIndex():o.getLeftmostDescendantIndex();
	//		    if (thisCorner != -1 && thatCorner != -1) {
	//			    if (thisCorner < thatCorner) return BEFORE;
	//			    if (thisCorner > thatCorner) return AFTER;
	//		    }
	//		    thisCorner = this.getRightmostDescendantIndex();
	//		    thatCorner = (o instanceof TerminalNode)?o.getCompareToIndex():o.getRightmostDescendantIndex();
	//		    if (thisCorner != -1 && thatCorner != -1) {
	//			    if (thisCorner < thatCorner) return BEFORE;
	//			    if (thisCorner > thatCorner) return AFTER;
	//		    }
			}
			catch (MaltChainedException e)
			{
				if (SystemLogger.logger().DebugEnabled)
				{
					SystemLogger.logger().debug("",e);
				}
				else
				{
					SystemLogger.logger().error(e.MessageChain);
				}
				Environment.Exit(1);
			}
			if (CompareToIndex < o.CompareToIndex)
			{
				return BEFORE;
			}
			if (CompareToIndex > o.CompareToIndex)
			{
				return AFTER;
			}
			return base.CompareTo(o);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is NonTerminal))
			{
				return false;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return 31 * 7 + base.GetHashCode();
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(Index);
			sb.Append('\t');

			foreach (SymbolTable table in LabelTypes)
			{
				try
				{
					sb.Append(getLabelSymbol(table));
				}
				catch (MaltChainedException e)
				{
					Console.Error.WriteLine("Print error : " + e.MessageChain);
				}
				sb.Append('\t');
			}

			return sb.ToString();
		}

	}

}