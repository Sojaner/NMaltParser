using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.syntaxgraph.node
{


	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using SystemLogger = org.maltparser.core.helper.SystemLogger;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;
	using Direction = org.maltparser.core.syntaxgraph.headrules.Direction;
	using HeadRules = org.maltparser.core.syntaxgraph.headrules.HeadRules;


	public class Root : GraphNode, DependencyNode, PhraseStructureNode, NonTerminalNode
	{
		protected internal readonly SortedSet<DependencyNode> leftDependents;
		protected internal readonly SortedSet<DependencyNode> rightDependents;
		protected internal readonly SortedSet<PhraseStructureNode> children;

		/// <summary>
		/// a reference to a node where the node is part of a component. If the node is unconnected it will reference to it self. 
		/// </summary>
		protected internal DependencyNode component;
		protected internal int rank;
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Root() throws org.maltparser.core.exception.MaltChainedException
		public Root() : base()
		{
			leftDependents = new SortedSet<DependencyNode>();
			rightDependents = new SortedSet<DependencyNode>();
			children = new SortedSet<PhraseStructureNode>();
			clear();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException
		public override void addIncomingEdge(Edge @in)
		{
			throw new SyntaxGraphException("It is not allowed for a root node to have an incoming edge");
		}

		public override void removeIncomingEdge(Edge @in)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public override void addOutgoingEdge(Edge @out)
		{
			base.addOutgoingEdge(@out);
			if (@out.Target != null)
			{
				if (@out.Type == org.maltparser.core.syntaxgraph.edge.Edge_Fields.DEPENDENCY_EDGE && @out.Target is DependencyNode)
				{
					Node dependent = @out.Target;
					if (compareTo(dependent) > 0)
					{
						leftDependents.Add((DependencyNode)dependent);
					}
					else if (compareTo(dependent) < 0)
					{
						rightDependents.Add((DependencyNode)dependent);
					}
				}
				else if (@out.Type == org.maltparser.core.syntaxgraph.edge.Edge_Fields.PHRASE_STRUCTURE_EDGE && @out.Target is PhraseStructureNode)
				{
					children.Add((PhraseStructureNode)@out.Target);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public override void removeOutgoingEdge(Edge @out)
		{
			base.removeOutgoingEdge(@out);
			if (@out.Target != null)
			{
				if (@out.Type == org.maltparser.core.syntaxgraph.edge.Edge_Fields.DEPENDENCY_EDGE && @out.Target is DependencyNode)
				{
					Node dependent = @out.Target;
					if (compareTo(dependent) > 0)
					{
						leftDependents.remove((DependencyNode)dependent);
					}
					else if (compareTo(dependent) < 0)
					{
						rightDependents.remove((DependencyNode)dependent);
					}
				}
				else if (@out.Type == org.maltparser.core.syntaxgraph.edge.Edge_Fields.PHRASE_STRUCTURE_EDGE && @out.Target is PhraseStructureNode)
				{
					children.remove((PhraseStructureNode)@out.Target);
				}
			}
		}
		public virtual DependencyNode Predecessor
		{
			get
			{
				return null;
			}
		}

		public virtual DependencyNode Successor
		{
			get
			{
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getAncestor() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode Ancestor
		{
			get
			{
				return this;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getProperAncestor() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode ProperAncestor
		{
			get
			{
				return null;
			}
		}

		public virtual int Rank
		{
			get
			{
				return rank;
			}
			set
			{
				this.rank = value;
			}
		}


		public virtual DependencyNode findComponent()
		{
			return findComponent(this);
		}

		private DependencyNode findComponent(DependencyNode x)
		{
			if (x != x.Component)
			{
				x.Component = findComponent(x.Component);
			}
			return x.Component;
		}

		public virtual DependencyNode Component
		{
			get
			{
				return component;
			}
			set
			{
				this.component = value;
			}
		}



		public virtual bool Continuous
		{
			get
			{
				return true;
			}
		}

		public virtual bool ContinuousExcludeTerminalsAttachToRoot
		{
			get
			{
				return true;
			}
		}

		public virtual PhraseStructureNode Parent
		{
			get
			{
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge getParentEdge() throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge ParentEdge
		{
			get
			{
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getParentEdgeLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getParentEdgeLabelSymbol(SymbolTable table)
		{
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getParentEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getParentEdgeLabelCode(SymbolTable table)
		{
			return -1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasParentEdgeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool hasParentEdgeLabel(SymbolTable table)
		{
			return false;
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
				return children.toArray(new PhraseStructureNode[children.Count])[index];
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TokenNode getLexicalHead(org.maltparser.core.syntaxgraph.headrules.HeadRules headRules) throws org.maltparser.core.exception.MaltChainedException
		public virtual TokenNode getLexicalHead(HeadRules headRules)
		{
			return identifyHead(headRules);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PhraseStructureNode getHeadChild(org.maltparser.core.syntaxgraph.headrules.HeadRules headRules) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode getHeadChild(HeadRules headRules)
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
		private PhraseStructureNode identifyHeadChild(HeadRules headRules)
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
		public virtual TokenNode identifyHead(HeadRules headRules)
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

		public virtual bool hasAtMostOneHead()
		{
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasAncestorInside(int left, int right) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool hasAncestorInside(int left, int right)
		{
			return false;
		}

		public virtual bool hasHead()
		{
			return false;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getHead() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode Head
		{
			get
			{
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge getHeadEdge() throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge HeadEdge
		{
			get
			{
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table, String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addHeadEdgeLabel(SymbolTable table, string symbol)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table, int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addHeadEdgeLabel(SymbolTable table, int code)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHeadEdgeLabel(org.maltparser.core.syntaxgraph.LabelSet labelSet) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addHeadEdgeLabel(LabelSet labelSet)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getHeadEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getHeadEdgeLabelCode(SymbolTable table)
		{
			return 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.LabelSet getHeadEdgeLabelSet() throws org.maltparser.core.exception.MaltChainedException
		public virtual LabelSet HeadEdgeLabelSet
		{
			get
			{
				return null;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getHeadEdgeLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getHeadEdgeLabelSymbol(SymbolTable table)
		{
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<org.maltparser.core.symbol.SymbolTable> getHeadEdgeLabelTypes() throws org.maltparser.core.exception.MaltChainedException
		public virtual ISet<SymbolTable> HeadEdgeLabelTypes
		{
			get
			{
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool hasHeadEdgeLabel(SymbolTable table)
		{
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isHeadEdgeLabeled() throws org.maltparser.core.exception.MaltChainedException
		public virtual bool HeadEdgeLabeled
		{
			get
			{
				return false;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nHeadEdgeLabels() throws org.maltparser.core.exception.MaltChainedException
		public virtual int nHeadEdgeLabels()
		{
			return 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<org.maltparser.core.syntaxgraph.edge.Edge> getHeadEdges() throws org.maltparser.core.exception.MaltChainedException
		public virtual ISet<Edge> HeadEdges
		{
			get
			{
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<DependencyNode> getHeads() throws org.maltparser.core.exception.MaltChainedException
		public virtual ISet<DependencyNode> Heads
		{
			get
			{
				return null;
			}
		}

		public virtual bool hasDependent()
		{
			return hasLeftDependent() || hasRightDependent();
		}

		/// <summary>
		/// Returns <code>true</code> if the node has one or more left dependents, otherwise <code>false</code>.
		/// </summary>
		/// <returns> <code>true</code> if the node has one or more left dependents, otherwise <code>false</code>. </returns>
		public virtual bool hasLeftDependent()
		{
			return leftDependents.Count > 0;
		}

		/// <summary>
		/// Returns the left dependent at the position <code>index</code>, where <code>index==0</code> equals the left most dependent.
		/// </summary>
		/// <param name="index"> the index </param>
		/// <returns> the left dependent at the position <code>index</code>, where <code>index==0</code> equals the left most dependent </returns>
		public virtual DependencyNode getLeftDependent(int index)
		{
			if (0 <= index && index < leftDependents.Count)
			{
				int i = 0;
	//			DependencyNode candidate = null;

				foreach (DependencyNode node in leftDependents)
				{
	//				candidate = node;
					if (i == index)
					{
						return node;
					}
					i++;
				}
			}
			return null;
		}

		/// <summary>
		/// Return the number of left dependents
		/// </summary>
		/// <returns> the number of left dependents </returns>
		public virtual int LeftDependentCount
		{
			get
			{
				return leftDependents.Count;
			}
		}

		public virtual SortedSet<DependencyNode> LeftDependents
		{
			get
			{
				return leftDependents;
			}
		}

		/// <summary>
		/// Returns the left sibling if it exists, otherwise <code>null</code>
		/// </summary>
		/// <returns> the left sibling if it exists, otherwise <code>null</code> </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getLeftSibling() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode LeftSibling
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the left sibling at the same side of head as the node it self. If not found <code>null</code> is returned
		/// </summary>
		/// <returns> the left sibling at the same side of head as the node it self. If not found <code>null</code> is returned </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getSameSideLeftSibling() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode SameSideLeftSibling
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the closest left dependent to the node it self, if not found <code>null</code> is returned.
		/// </summary>
		/// <returns> the closest left dependent to the node it self, if not found <code>null</code> is returned. </returns>
		public virtual DependencyNode ClosestLeftDependent
		{
			get
			{
				try
				{
					return leftDependents.Max;
				}
				catch (NoSuchElementException)
				{
					return null;
				}
			}
		}

		public virtual DependencyNode LeftmostDependent
		{
			get
			{
				foreach (DependencyNode dep in leftDependents)
				{
					return dep;
				}
				return null;
		//		try {
		//			return leftDependents.first();
		//		} catch (NoSuchElementException e) {
		//			return null;
		//		}
			}
		}

		public virtual DependencyNode getRightDependent(int index)
		{
			int size = rightDependents.Count;
			if (index < size)
			{
				return rightDependents.toArray(new DependencyNode[size])[size - 1 - index];
			}
			return null;
	//		if (0 <= index && index < rightDependents.size()) {
	//			int i = 0;
	//			DependencyNode candidate = null;
	//			
	//			for (DependencyNode node : rightDependents) {
	//				candidate = node;
	//				if (i == index) {
	//					return candidate;
	//				}
	//				i++;
	//			}
	//		}
	//		return null;
		}

		/// <summary>
		/// Return the number of right dependents
		/// </summary>
		/// <returns> the number of right dependents </returns>
		public virtual int RightDependentCount
		{
			get
			{
				return rightDependents.Count;
			}
		}

		public virtual SortedSet<DependencyNode> RightDependents
		{
			get
			{
				return rightDependents;
			}
		}

		/// <summary>
		/// Returns the right sibling if it exists, otherwise <code>null</code>
		/// </summary>
		/// <returns> the right sibling if it exists, otherwise <code>null</code> </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getRightSibling() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode RightSibling
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the right sibling at the same side of head as the node it self. If not found <code>null</code> is returned
		/// </summary>
		/// <returns> the right sibling at the same side of head as the node it self. If not found <code>null</code> is returned </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getSameSideRightSibling() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode SameSideRightSibling
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the closest right dependent to the node it self, if not found <code>null</code> is returned.
		/// </summary>
		/// <returns> the closest right dependent to the node it self, if not found <code>null</code> is returned. </returns>
		public virtual DependencyNode ClosestRightDependent
		{
			get
			{
				foreach (DependencyNode dep in rightDependents)
				{
					return dep;
				}
				return null;
		//		try {
		//			return rightDependents.first();
		//		} catch (NoSuchElementException e) {
		//			return null;
		//		}
			}
		}

		public virtual DependencyNode RightmostDependent
		{
			get
			{
				int n = rightDependents.Count;
				int i = 1;
				foreach (DependencyNode node in rightDependents)
				{
					if (i == n)
					{
						return node;
					}
					i++;
				}
				return null;
		//		try {
		//			return rightDependents.last();
		//		} catch (NoSuchElementException e) {
		//			return null;
		//		}
			}
		}

		/// <summary>
		/// Returns <code>true</code> if the node has one or more right dependents, otherwise <code>false</code>.
		/// </summary>
		/// <returns> <code>true</code> if the node has one or more right dependents, otherwise <code>false</code>. </returns>
		public virtual bool hasRightDependent()
		{
			return rightDependents.Count > 0;
		}

		protected internal virtual void getDependencyDominationSet(SortedSet<DependencyNode> dominationSet)
		{
			if (leftDependents.Count > 0 || rightDependents.Count > 0)
			{
				dominationSet.addAll(leftDependents);
				dominationSet.addAll(rightDependents);

				foreach (DependencyNode node in leftDependents)
				{
					((Token)node).getDependencyDominationSet(dominationSet);
				}
				foreach (DependencyNode node in rightDependents)
				{
					((Token)node).getDependencyDominationSet(dominationSet);
				}
			}
		}

	//	private SortedSet<DependencyNode> getDependencyDominationSet() {
	//		SortedSet<DependencyNode> dominationSet = new TreeSet<DependencyNode>();
	//		getDependencyDominationSet(dominationSet);
	//		return dominationSet;
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isProjective() throws org.maltparser.core.exception.MaltChainedException
		public virtual bool Projective
		{
			get
			{
				return true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDependencyNodeDepth() throws org.maltparser.core.exception.MaltChainedException
		public virtual int DependencyNodeDepth
		{
			get
			{
				return 0;
			}
		}
		public override int Index
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override int CompareToIndex
		{
			get
			{
				return 0;
			}
		}

		public override bool isRoot()
		{
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getLeftmostProperDescendant() throws org.maltparser.core.exception.MaltChainedException
		public override ComparableNode LeftmostProperDescendant
		{
			get
			{
				NonTerminalNode node = this;
				ComparableNode candidate = null;
				while (node != null)
				{
					candidate = node.LeftChild;
					if (candidate == null || candidate is TokenNode)
					{
						break;
					}
					node = (NonTerminalNode)candidate;
				}
    
				if (candidate == null && candidate is NonTerminalNode)
				{
					candidate = null;
					DependencyNode dep = null;
					foreach (int index in ((TokenStructure)BelongsToGraph).TokenIndices)
					{
						dep = ((TokenStructure)BelongsToGraph).getTokenNode(index);
						while (dep != null)
						{
							if (dep == this)
							{
								return dep;
							}
							dep = dep.Head;
						}
					}
				}
				return candidate;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getRightmostProperDescendant() throws org.maltparser.core.exception.MaltChainedException
		public override ComparableNode RightmostProperDescendant
		{
			get
			{
				NonTerminalNode node = this;
				ComparableNode candidate = null;
				while (node != null)
				{
					candidate = node.RightChild;
					if (candidate == null || candidate is TokenNode)
					{
						break;
					}
					node = (NonTerminalNode)candidate;
				}
				if (candidate == null && candidate is NonTerminalNode)
				{
					candidate = null;
					DependencyNode dep = null;
					for (int i = ((TokenStructure)BelongsToGraph).HighestTokenIndex; i > 0; i--)
					{
						dep = ((TokenStructure)BelongsToGraph).getTokenNode(i);
						while (dep != null)
						{
							if (dep == this)
							{
								return dep;
							}
							dep = dep.Head;
						}
					}
				}
				return candidate;
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

		public virtual IList<DependencyNode> ListOfDependents
		{
			get
			{
				IList<DependencyNode> dependentList = new List<DependencyNode>();
				foreach (DependencyNode node in leftDependents)
				{
					dependentList.Add(node);
				}
				foreach (DependencyNode node in rightDependents)
				{
					dependentList.Add(node);
				}
				return dependentList;
			}
		}

		public virtual IList<DependencyNode> ListOfLeftDependents
		{
			get
			{
				IList<DependencyNode> leftDependentList = new List<DependencyNode>();
    
				foreach (DependencyNode node in leftDependents)
				{
					leftDependentList.Add(node);
				}
				return leftDependentList;
			}
		}

		public virtual IList<DependencyNode> ListOfRightDependents
		{
			get
			{
				IList<DependencyNode> rightDependentList = new List<DependencyNode>();
				foreach (DependencyNode node in rightDependents)
				{
					rightDependentList.Add(node);
				}
				return rightDependentList;
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
			component = this;
			rank = 0;
			leftDependents.Clear();
			rightDependents.Clear();
			children.Clear();
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
				int thisLCorner = this.LeftmostProperDescendantIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thatLCorner = (o instanceof TokenNode)?o.getCompareToIndex():o.getLeftmostProperDescendantIndex();
				int thatLCorner = (o is TokenNode)?o.CompareToIndex:o.LeftmostProperDescendantIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thisRCorner = this.getRightmostProperDescendantIndex();
				int thisRCorner = this.RightmostProperDescendantIndex;
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
			if (0 < o.CompareToIndex)
			{
				return BEFORE;
			}
			if (0 > o.CompareToIndex)
			{
				return AFTER;
			}
			return base.CompareTo(o);
	//		
	//		if (o instanceof TerminalNode) {
	//			if (0 == o.getIndex()) {
	//				
	//			} else if (0 < o.getIndex()) {
	//				return -1;
	//			} else {
	//				return 1;
	//			}
	//		} else if (o instanceof Root) {
	//			return super.compareTo(o);
	//		} else if (o instanceof NonTerminalNode) {
	//			int lcorner = ((NonTerminalNode)o).getLeftCornerIndex();
	//			if (0 == lcorner) {
	//				int rcorner = ((NonTerminalNode)o).getRightCornerIndex();
	//				if (0 == rcorner) {
	//					if (0 == o.getIndex()) {
	//						return super.compareTo(o);
	//					} else if (0 < o.getIndex()) {
	//						return 1;
	//					} else {
	//						return -1;
	//					}
	//				} else if (0 < rcorner) {
	//					return -1;
	//				} else {
	//					return 1;
	//				}
	//			} else if (0 < lcorner) {
	//				return -1;
	//			} else {
	//				return 1;
	//			}
	//		}
	//		return super.compareTo(o);
		}

		public override bool Equals(object obj)
		{
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
			sb.Append(base.ToString());
			return sb.ToString();
		}
	}

}