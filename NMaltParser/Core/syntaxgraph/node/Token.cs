using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Helper;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Edge;

namespace NMaltParser.Core.SyntaxGraph.Node
{
    public class Token : GraphNode, TokenNode, DependencyNode, PhraseStructureNode
	{
		/// <summary>
		/// the previous terminal node in the linear precedence
		/// </summary>
		protected internal TokenNode predecessor = null;
		/// <summary>
		/// the next terminal node in the linear precedence
		/// </summary>
		protected internal TokenNode successor = null;

		/// <summary>
		/// a reference to a node where the node is part of a component. If the node is unconnected it will reference to it self. 
		/// </summary>
		protected internal DependencyNode component;
		protected internal int rank;

		protected internal int index;

		protected internal PhraseStructureNode parent;
		protected internal readonly SortedSet<DependencyNode> heads;
		protected internal readonly SortedSet<DependencyNode> leftDependents;
		protected internal readonly SortedSet<DependencyNode> rightDependents;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Token() throws org.maltparser.core.exception.MaltChainedException
		public Token()
		{
			parent = null;
			heads = new SortedSet<DependencyNode>();
			leftDependents = new SortedSet<DependencyNode>();
			rightDependents = new SortedSet<DependencyNode>();
			clear();
		}

		/// <summary>
		/// Sets the predecessor terminal node in the linear order of the terminal nodes.
		/// </summary>
		/// <param name="predecessor"> the predecessor terminal node </param>
		public virtual void setPredecessor(TokenNode predecessor)
		{
			this.predecessor = predecessor;
		}

		/// <summary>
		/// Sets the predecessor terminal node in the linear order of the terminal nodes.
		/// </summary>
		/// <param name="successor"> the successor terminal node </param>
		public virtual void setSuccessor(TokenNode successor)
		{
			this.successor = successor;
		}

		/// <summary>
		/// Returns the predecessor terminal node in the linear order of the terminal nodes.
		/// </summary>
		/// <returns> the predecessor terminal node in the linear order of the terminal nodes. </returns>
		public virtual TokenNode TokenNodePredecessor
		{
			get
			{
				return predecessor;
			}
		}

		/// <summary>
		/// Returns the successor terminal node in the linear order of the terminal nodes.
		/// </summary>
		/// <returns> the successor terminal node in the linear order of the terminal nodes. </returns>
		public virtual TokenNode TokenNodeSuccessor
		{
			get
			{
				return successor;
			}
		}

		public virtual DependencyNode getPredecessor()
		{
			return predecessor;
		}

		public virtual DependencyNode getSuccessor()
		{
			return successor;
		}

		public virtual int Rank
		{
			get
			{
				return rank;
			}
			set
			{
				rank = value;
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
				component = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException
		public override void addIncomingEdge(Edge.Edge @in)
		{
			base.addIncomingEdge(@in);
			if (@in.Source != null)
			{
				if (@in.Type == Edge_Fields.DEPENDENCY_EDGE && @in.Source is DependencyNode)
				{
					heads.Add((DependencyNode)@in.Source);
				}
				else if (@in.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE && @in.Source is PhraseStructureNode)
				{
					parent = (PhraseStructureNode)@in.Source;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException
		public override void removeIncomingEdge(Edge.Edge @in)
		{
			base.removeIncomingEdge(@in);
			if (@in.Source != null)
			{
				if (@in.Type == Edge_Fields.DEPENDENCY_EDGE && @in.Source is DependencyNode)
				{
					heads.remove((DependencyNode)@in.Source);
				}
				else if (@in.Type == Edge_Fields.PHRASE_STRUCTURE_EDGE && @in.Source is PhraseStructureNode)
				{
					if (@in.Source == parent)
					{
						parent = null;
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public override void addOutgoingEdge(Edge.Edge @out)
		{
			base.addOutgoingEdge(@out);
			if (@out.Type == Edge_Fields.DEPENDENCY_EDGE && @out.Target is DependencyNode)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DependencyNode dependent = (DependencyNode)out.getTarget();
				DependencyNode dependent = (DependencyNode)@out.Target;
				if (compareTo(dependent) > 0)
				{
					leftDependents.Add((DependencyNode)dependent);
				}
				else if (compareTo(dependent) < 0)
				{
					rightDependents.Add((DependencyNode)dependent);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public override void removeOutgoingEdge(Edge.Edge @out)
		{
			base.removeOutgoingEdge(@out);
			if (@out.Type == Edge_Fields.DEPENDENCY_EDGE && @out.Target is DependencyNode)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DependencyNode dependent = (DependencyNode)out.getTarget();
				DependencyNode dependent = (DependencyNode)@out.Target;
				if (compareTo(dependent) > 0)
				{
					leftDependents.remove((DependencyNode)dependent);
				}
				else if (compareTo(dependent) < 0)
				{
					rightDependents.remove((DependencyNode)dependent);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setIndex(int index) throws org.maltparser.core.exception.MaltChainedException
		public override int Index
		{
			set
			{
				if (value > 0)
				{
					index = value;
				}
				else
				{
					throw new SyntaxGraphException("A terminal node must have a positive integer value and not index " + value + ". ");
				}
			}
			get
			{
				return index;
			}
		}


		public override int CompareToIndex
		{
			get
			{
				return index;
			}
		}

		public override bool Root
		{
			get
			{
				return false;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getAncestor() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode Ancestor
		{
			get
			{
				if (!hasHead())
				{
					return this;
				}
    
				DependencyNode tmp = this;
				while (tmp.hasHead())
				{
					tmp = tmp.Head;
				}
				return tmp;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getProperAncestor() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode ProperAncestor
		{
			get
			{
				if (!hasHead())
				{
					return null;
				}
    
				DependencyNode tmp = this;
				while (tmp.hasHead())
				{
					tmp = tmp.Head;
				}
				return tmp;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getLeftmostProperDescendant() throws org.maltparser.core.exception.MaltChainedException
		public override ComparableNode LeftmostProperDescendant
		{
			get
			{
				ComparableNode candidate = null;
				ComparableNode tmp = null;
				foreach (DependencyNode ldep in leftDependents)
				{
					if (candidate == null)
					{
						candidate = ldep;
					}
					else if (ldep.Index < candidate.Index)
					{
						candidate = ldep;
					}
					tmp = ((Token)ldep).LeftmostProperDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null)
					{
						candidate = tmp;
					}
					else if (tmp.Index < candidate.Index)
					{
						candidate = tmp;
					}
					if (candidate.Index == 1)
					{
						return candidate;
					}
				}
				foreach (DependencyNode rdep in rightDependents)
				{
					if (candidate == null)
					{
						candidate = rdep;
					}
					else if (rdep.Index < candidate.Index)
					{
						candidate = rdep;
					}
					tmp = ((Token)rdep).LeftmostProperDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null)
					{
						candidate = tmp;
					}
					else if (tmp.Index < candidate.Index)
					{
						candidate = tmp;
					}
					if (candidate.Index == 1)
					{
						return candidate;
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
				ComparableNode candidate = null;
				ComparableNode tmp = null;
				foreach (DependencyNode ldep in leftDependents)
				{
					if (candidate == null)
					{
						candidate = ldep;
					}
					else if (ldep.Index > candidate.Index)
					{
						candidate = ldep;
					}
					tmp = ((Token)ldep).RightmostProperDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null)
					{
						candidate = tmp;
					}
					else if (tmp.Index > candidate.Index)
					{
						candidate = tmp;
					}
				}
				foreach (DependencyNode rdep in rightDependents)
				{
					if (candidate == null)
					{
						candidate = rdep;
					}
					else if (rdep.Index > candidate.Index)
					{
						candidate = rdep;
					}
					tmp = ((Token)rdep).RightmostProperDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null)
					{
						candidate = tmp;
					}
					else if (tmp.Index > candidate.Index)
					{
						candidate = tmp;
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
				ComparableNode candidate = this;
				ComparableNode tmp = null;
				foreach (DependencyNode ldep in leftDependents)
				{
					if (candidate == null)
					{
						candidate = ldep;
					}
					else if (ldep.Index < candidate.Index)
					{
						candidate = ldep;
					}
					tmp = ((Token)ldep).LeftmostDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null)
					{
						candidate = tmp;
					}
					else if (tmp.Index < candidate.Index)
					{
						candidate = tmp;
					}
					if (candidate.Index == 1)
					{
						return candidate;
					}
				}
				foreach (DependencyNode rdep in rightDependents)
				{
					if (candidate == null)
					{
						candidate = rdep;
					}
					else if (rdep.Index < candidate.Index)
					{
						candidate = rdep;
					}
					tmp = ((Token)rdep).LeftmostDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null)
					{
						candidate = tmp;
					}
					else if (tmp.Index < candidate.Index)
					{
						candidate = tmp;
					}
					if (candidate.Index == 1)
					{
						return candidate;
					}
				}
				return candidate;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getRightmostDescendant() throws org.maltparser.core.exception.MaltChainedException
		public override ComparableNode RightmostDescendant
		{
			get
			{
				ComparableNode candidate = this;
				ComparableNode tmp = null;
				foreach (DependencyNode ldep in leftDependents)
				{
					if (candidate == null)
					{
						candidate = ldep;
					}
					else if (ldep.Index > candidate.Index)
					{
						candidate = ldep;
					}
					tmp = ((Token)ldep).RightmostDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null)
					{
						candidate = tmp;
					}
					else if (tmp.Index > candidate.Index)
					{
						candidate = tmp;
					}
				}
				foreach (DependencyNode rdep in rightDependents)
				{
					if (candidate == null)
					{
						candidate = rdep;
					}
					else if (rdep.Index > candidate.Index)
					{
						candidate = rdep;
					}
					tmp = ((Token)rdep).RightmostDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null)
					{
						candidate = tmp;
					}
					else if (tmp.Index > candidate.Index)
					{
						candidate = tmp;
					}
				}
				return candidate;
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

		public virtual bool hasAtMostOneHead()
		{
			return heads.Count <= 1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasAncestorInside(int left, int right) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool hasAncestorInside(int left, int right)
		{
			DependencyNode tmp = this;
			if (tmp.Head != null)
			{
				tmp = tmp.Head;
				if (tmp.Index >= left && tmp.Index <= right)
				{
					return true;
				}
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<org.maltparser.core.syntaxgraph.edge.Edge> getHeadEdges() throws org.maltparser.core.exception.MaltChainedException
		public virtual ISet<Edge.Edge> HeadEdges
		{
			get
			{
				return incomingEdges;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<DependencyNode> getHeads() throws org.maltparser.core.exception.MaltChainedException
		public virtual ISet<DependencyNode> Heads
		{
			get
			{
				return heads;
			}
		}

		public virtual bool hasHead()
		{
			return heads.Count != 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getHead() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode Head
		{
			get
			{
				if (heads.Count == 0)
				{
					return null;
				}
				if (heads.Count == 1)
				{
					foreach (DependencyNode head in heads)
					{
						return head;
					}
				}
    
				if (heads.Count > 1)
				{
					throw new SyntaxGraphException("The dependency node is multi-headed and it is ambigious to return a single-head dependency node. ");
				}
				// heads.first();
    
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge getHeadEdge() throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge HeadEdge
		{
			get
			{
				if (heads.Count == 0)
				{
					return null;
				}
				if (incomingEdges.Count == 1 && incomingEdges.Min is DependencyNode)
				{
					return incomingEdges.Min;
				}
				if (heads.Count == 1)
				{
					foreach (Edge.Edge e in incomingEdges)
					{
						if (e.Source == heads.Min)
						{
							return e;
						}
					}
				}
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table, String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addHeadEdgeLabel(SymbolTable table, string symbol)
		{
			if (hasHead())
			{
				HeadEdge.addLabel(table, symbol);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table, int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addHeadEdgeLabel(SymbolTable table, int code)
		{
			if (hasHead())
			{
				HeadEdge.addLabel(table, code);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHeadEdgeLabel(org.maltparser.core.syntaxgraph.LabelSet labelSet) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addHeadEdgeLabel(LabelSet labelSet)
		{
			if (hasHead())
			{
				HeadEdge.addLabel(labelSet);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool hasHeadEdgeLabel(SymbolTable table)
		{
			if (!hasHead())
			{
				return false;
			}
			return HeadEdge.hasLabel(table);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getHeadEdgeLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getHeadEdgeLabelSymbol(SymbolTable table)
		{
			return HeadEdge.getLabelSymbol(table);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getHeadEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getHeadEdgeLabelCode(SymbolTable table)
		{
			if (!hasHead())
			{
				return 0;
			}
			return HeadEdge.getLabelCode(table);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isHeadEdgeLabeled() throws org.maltparser.core.exception.MaltChainedException
		public virtual bool HeadEdgeLabeled
		{
			get
			{
				if (!hasHead())
				{
					return false;
				}
				return HeadEdge.Labeled;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nHeadEdgeLabels() throws org.maltparser.core.exception.MaltChainedException
		public virtual int nHeadEdgeLabels()
		{
			if (!hasHead())
			{
				return 0;
			}
			return HeadEdge.nLabels();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<org.maltparser.core.symbol.SymbolTable> getHeadEdgeLabelTypes() throws org.maltparser.core.exception.MaltChainedException
		public virtual ISet<SymbolTable> HeadEdgeLabelTypes
		{
			get
			{
				return HeadEdge.LabelTypes;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.LabelSet getHeadEdgeLabelSet() throws org.maltparser.core.exception.MaltChainedException
		public virtual LabelSet HeadEdgeLabelSet
		{
			get
			{
				return HeadEdge.LabelSet;
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
				if (Head == null)
				{
					return null;
				}
    
				DependencyNode candidate = null;
				foreach (DependencyNode node in Head.LeftDependents)
				{
					if (node == this)
					{
						return candidate;
					}
					candidate = node;
				}
				foreach (DependencyNode node in Head.RightDependents)
				{
					if (node == this)
					{
						return candidate;
					}
					candidate = node;
				}
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
				if (Head == null)
				{
					return null;
				}
				else if (Index < Head.Index)
				{
					try
					{
						return Head.LeftDependents.headSet(this).last();
					}
					catch (NoSuchElementException)
					{
						return null;
					}
				}
				else if (Index > Head.Index)
				{
					try
					{
						return Head.RightDependents.headSet(this).last();
					}
					catch (NoSuchElementException)
					{
						return null;
					}
				}
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

		/// <summary>
		/// Returns a sorted set of right dependents.
		/// </summary>
		/// <returns> a sorted set of right dependents. </returns>
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
				if (Head == null)
				{
					return null;
				}
    
				foreach (DependencyNode node in Head.LeftDependents)
				{
					if (node.Index > Index)
					{
						return node;
					}
				}
				foreach (DependencyNode node in Head.RightDependents)
				{
					if (node.Index > Index)
					{
						return node;
					}
				}
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
				if (Head == null)
				{
					return null;
				}
				else if (Index < Head.Index)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.SortedSet<DependencyNode> tailSet = getHead().getLeftDependents().tailSet(this);
					SortedSet<DependencyNode> tailSet = Head.LeftDependents.tailSet(this);
					if (tailSet.Count <= 1)
					{
						return null;
					}
					return tailSet.toArray(new DependencyNode[tailSet.Count])[1];
				}
				else if (Index > Head.Index)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.SortedSet<DependencyNode> tailSet = getHead().getRightDependents().tailSet(this);
					SortedSet<DependencyNode> tailSet = Head.RightDependents.tailSet(this);
					if (tailSet.Count <= 1)
					{
						return null;
					}
					return tailSet.toArray(new DependencyNode[tailSet.Count])[1];
				}
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


		/// <summary>
		/// Returns <code>true</code> if the node has one or more right dependents, otherwise <code>false</code>.
		/// </summary>
		/// <returns> <code>true</code> if the node has one or more right dependents, otherwise <code>false</code>. </returns>
		public virtual bool hasRightDependent()
		{
			return rightDependents.Count > 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isProjective() throws org.maltparser.core.exception.MaltChainedException
		public virtual bool Projective
		{
			get
			{
				if (hasHead() && !Head.Root)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final DependencyNode head = getHead();
					DependencyNode head = Head;
					if (Head.Index < Index)
					{
						DependencyNode terminals = head;
						DependencyNode tmp = null;
						while (true)
						{
							if (terminals == null || terminals.Successor == null)
							{
								return false;
							}
							if (terminals.Successor == this)
							{
								break;
							}
							tmp = terminals = terminals.Successor;
							while (tmp != this && tmp != head)
							{
								if (!tmp.hasHead())
								{
									return false;
								}
								tmp = tmp.Head;
							}
						}
					}
					else
					{
						DependencyNode terminals = this;
						DependencyNode tmp = null;
						while (true)
						{
							if (terminals == null || terminals.Successor == null)
							{
								return false;
							}
							if (terminals.Successor == head)
							{
								break;
							}
							tmp = terminals = terminals.Successor;
							while (tmp != this && tmp != head)
							{
								if (!tmp.hasHead())
								{
									return false;
								}
								tmp = tmp.Head;
							}
						}
					}
				}
				return true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDependencyNodeDepth() throws org.maltparser.core.exception.MaltChainedException
		public virtual int DependencyNodeDepth
		{
			get
			{
				DependencyNode tmp = this;
				int depth = 0;
				while (tmp.hasHead())
				{
					depth++;
					tmp = tmp.Head;
				}
				return depth;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			base.clear();
			predecessor = null;
			successor = null;
			component = this;
			rank = 0;
			parent = null;
			heads.Clear();
			leftDependents.Clear();
			rightDependents.Clear();
		}

		public override int compareTo(ComparableNode that)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;
			if (this == that)
			{
				return EQUAL;
			}

			if (that is TokenNode)
			{
				if (index < that.CompareToIndex)
				{
					return BEFORE;
				}
				if (index > that.CompareToIndex)
				{
					return AFTER;
				}
				return base.CompareTo(that);
			}
			if (that is NonTerminalNode)
			{
				try
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thisLCorner = this.index;
					int thisLCorner = index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thatLCorner = that.getLeftmostProperDescendantIndex();
					int thatLCorner = that.LeftmostProperDescendantIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thisRCorner = this.index;
					int thisRCorner = index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int thatRCorner = that.getRightmostProperDescendantIndex();
					int thatRCorner = that.RightmostProperDescendantIndex;

	//			    if (thisLCorner == -1 || thatLCorner == -1) {
	//				    if (thisRCorner < thatRCorner) return BEFORE;
	//				    if (thisRCorner > thatRCorner) return AFTER;
	//			    }
	//			    if (thisRCorner == -1 || thatRCorner == -1) {
	//				    if (thisLCorner < thatLCorner) return BEFORE;
	//				    if (thisLCorner > thatLCorner) return AFTER;
	//			    }

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



	//			    final int thisLCorner = this.index;
	//			    final int thatLCorner = that.getLeftmostDescendantIndex();
	//			    final int thisRCorner = this.index;
	//			    final int thatRCorner = that.getRightmostDescendantIndex();
	//
	//			    if (thisLCorner == -1 || thatLCorner == -1) {
	//				    if (thisRCorner < thatRCorner) return BEFORE;
	//				    if (thisRCorner > thatRCorner) return AFTER;
	//			    }
	//			    if (thisRCorner == -1 || thatRCorner == -1) {
	//				    if (thisLCorner < thatLCorner) return BEFORE;
	//				    if (thisLCorner > thatLCorner) return AFTER;
	//			    }
	//			    if (thisLCorner < thatLCorner && thisRCorner < thatRCorner) return BEFORE;
	//			    if (thisLCorner > thatLCorner && thisRCorner > thatRCorner) return AFTER;
	//			    if (thisLCorner > thatLCorner && thisRCorner < thatRCorner) return BEFORE;
	//			    if (thisLCorner < thatLCorner && thisRCorner > thatRCorner) return AFTER;



	//		    	int corner = that.getLeftmostDescendantIndex();
	//		    	if (corner != -1) {
	//		    		if (this.index < corner) return BEFORE;
	//		    		if (this.index > corner) return AFTER;
	//		    	}
	//		    	corner = that.getRightmostDescendantIndex();
	//		    	if (corner != -1) {
	//			    	if (this.index < corner) return BEFORE;
	//			    	if (this.index > corner) return AFTER;
	//		    	}
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
			}
			if (index < that.CompareToIndex)
			{
				return BEFORE;
			}
			if (index > that.CompareToIndex)
			{
				return AFTER;
			}
			return base.CompareTo(that);
		}

		public override bool Equals(object obj)
		{
			Token v = (Token)obj;
			if (!(predecessor == v.predecessor && successor == v.successor))
			{
				return false;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			int hash = 7;
			hash = 31 * hash + (null == predecessor ? 0 : predecessor.GetHashCode());
			hash = 31 * hash + (null == successor ? 0 : successor.GetHashCode());
			return 31 * hash + base.GetHashCode();
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