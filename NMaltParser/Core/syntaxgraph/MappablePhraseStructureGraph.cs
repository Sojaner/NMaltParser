﻿using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Helper;
using NMaltParser.Core.Pool;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Ds2PS;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class MappablePhraseStructureGraph : Sentence, IDependencyStructure, PhraseStructure
	{
		private readonly ObjectPoolList<Edge.Edge> edgePool;
		private readonly SortedSet<Edge.Edge> graphEdges;
		private Root root;
		private bool singleHeadedConstraint;
		private readonly SortedDictionary<int, NonTerminal> nonTerminalNodes;
		private readonly ObjectPoolList<NonTerminal> nonTerminalPool;
		private LosslessMapping mapping;
		private RootLabels rootLabels;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MappablePhraseStructureGraph(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public MappablePhraseStructureGraph(SymbolTableHandler symbolTables) : base(symbolTables)
		{
			SingleHeadedConstraint = true;
			root = new Root();
			root.BelongsToGraph = this;
			graphEdges = new SortedSet<Edge.Edge>();
			edgePool = new ObjectPoolListAnonymousInnerClass(this);

			nonTerminalNodes = new SortedDictionary<int, NonTerminal>();
			nonTerminalPool = new ObjectPoolListAnonymousInnerClass2(this);
			Clear();
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<Edge.Edge>
		{
			private readonly MappablePhraseStructureGraph outerInstance;

			public ObjectPoolListAnonymousInnerClass(MappablePhraseStructureGraph outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			protected internal override Edge.Edge create()
			{
				return new GraphEdge();
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetObject(org.maltparser.core.syntaxgraph.edge.Edge o) throws org.maltparser.core.exception.MaltChainedException
			public void resetObject(Edge.Edge o)
			{
				o.clear();
			}
		}

		private class ObjectPoolListAnonymousInnerClass2 : ObjectPoolList<NonTerminal>
		{
			private readonly MappablePhraseStructureGraph outerInstance;

			public ObjectPoolListAnonymousInnerClass2(MappablePhraseStructureGraph outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.core.syntaxgraph.node.NonTerminal create() throws org.maltparser.core.exception.MaltChainedException
			protected internal override NonTerminal create()
			{
				return new NonTerminal();
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetObject(org.maltparser.core.syntaxgraph.node.NonTerminal o) throws org.maltparser.core.exception.MaltChainedException
			public void resetObject(NonTerminal o)
			{
				o.clear();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode addDependencyNode() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode AddDependencyNode()
		{
			return AddTokenNode();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode addDependencyNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode AddDependencyNode(int index)
		{
			if (index == 0)
			{
				return root;
			}
			return AddTokenNode(index);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getDependencyNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode GetDependencyNode(int index)
		{
			if (index == 0)
			{
				return root;
			}
			return GetTokenNode(index);
		}

		public virtual int NDependencyNode()
		{
			return NTokenNode() + 1;
		}

		public virtual int HighestDependencyNodeIndex
		{
			get
			{
				if (HasTokens())
				{
					return HighestTokenIndex;
				}
				return 0;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge addDependencyEdge(int headIndex, int dependentIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge AddDependencyEdge(int headIndex, int dependentIndex)
		{
			DependencyNode head = null;
			DependencyNode dependent = null;
			if (headIndex == 0)
			{
				head = root;
			}
			else if (headIndex > 0)
			{
				head = getOrAddTerminalNode(headIndex);
			}

			if (dependentIndex > 0)
			{
				dependent = getOrAddTerminalNode(dependentIndex);
			}
			return addDependencyEdge(head, dependent);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge addDependencyEdge(org.maltparser.core.syntaxgraph.node.DependencyNode head, org.maltparser.core.syntaxgraph.node.DependencyNode dependent) throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge addDependencyEdge(DependencyNode head, DependencyNode dependent)
		{
			if (head == null || dependent == null || head.BelongsToGraph != this || dependent.BelongsToGraph != this)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!dependent.Root)
			{
				if (singleHeadedConstraint && dependent.hasHead())
				{
					throw new SyntaxGraphException("The dependent already have a head. ");
				}
				DependencyNode hc = ((DependencyNode)head).findComponent();
				DependencyNode dc = ((DependencyNode)dependent).findComponent();
				if (hc != dc)
				{
					link(hc, dc);
					numberOfComponents--;
				}
				Edge.Edge e = edgePool.checkOut();
				e.BelongsToGraph = this;
				e.setEdge((Node.Node)head, (Node.Node)dependent, Edge_Fields.DEPENDENCY_EDGE);
				graphEdges.Add(e);
				return e;
			}
			else
			{
				throw new SyntaxGraphException("Head node is not a root node or a terminal node.");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge moveDependencyEdge(int newHeadIndex, int dependentIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge MoveDependencyEdge(int newHeadIndex, int dependentIndex)
		{
			DependencyNode newHead = null;
			DependencyNode dependent = null;
			if (newHeadIndex == 0)
			{
				newHead = root;
			}
			else if (newHeadIndex > 0)
			{
				newHead = terminalNodes[newHeadIndex];
			}

			if (dependentIndex > 0)
			{
				dependent = terminalNodes[dependentIndex];
			}
			return moveDependencyEdge(newHead, dependent);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge moveDependencyEdge(org.maltparser.core.syntaxgraph.node.DependencyNode newHead, org.maltparser.core.syntaxgraph.node.DependencyNode dependent) throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge moveDependencyEdge(DependencyNode newHead, DependencyNode dependent)
		{
			if (dependent == null || !dependent.hasHead() || newHead.BelongsToGraph != this || dependent.BelongsToGraph != this)
			{
				return null;
			}
			Edge.Edge headEdge = dependent.HeadEdge;

			LabelSet labels = null;
			if (headEdge.Labeled)
			{
				labels = CheckOutNewLabelSet();
				foreach (SymbolTable table in headEdge.LabelTypes)
				{
					labels.put(table, headEdge.getLabelCode(table));
				}
			}
			headEdge.clear();
			headEdge.BelongsToGraph = this;
			headEdge.setEdge((Node.Node)newHead, (Node.Node)dependent, Edge_Fields.DEPENDENCY_EDGE);
			if (labels != null)
			{
				headEdge.addLabel(labels);
				labels.clear();
				CheckInLabelSet(labels);
			}
			return headEdge;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeDependencyEdge(int headIndex, int dependentIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual void RemoveDependencyEdge(int headIndex, int dependentIndex)
		{
			Node.Node head = null;
			Node.Node dependent = null;
			if (headIndex == 0)
			{
				head = root;
			}
			else if (headIndex > 0)
			{
				head = terminalNodes[headIndex];
			}

			if (dependentIndex > 0)
			{
				dependent = terminalNodes[dependentIndex];
			}
			removeDependencyEdge(head, dependent);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void removeDependencyEdge(org.maltparser.core.syntaxgraph.node.Node head, org.maltparser.core.syntaxgraph.node.Node dependent) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void removeDependencyEdge(Node.Node head, Node.Node dependent)
		{
			if (head == null || dependent == null || head.BelongsToGraph != this || dependent.BelongsToGraph != this)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!dependent.Root)
			{
				IEnumerator<Edge.Edge> ie = dependent.IncomingEdgeIterator;
				while (ie.MoveNext())
				{
					Edge.Edge e = ie.Current;
					if (e.Source == head)
					{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
						ie.remove();
						graphEdges.remove(e);
						edgePool.checkIn(e);
					}
				}
			}
			else
			{
				throw new SyntaxGraphException("Head node is not a root node or a terminal node.");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge addSecondaryEdge(org.maltparser.core.syntaxgraph.node.ComparableNode source, org.maltparser.core.syntaxgraph.node.ComparableNode target) throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge addSecondaryEdge(ComparableNode source, ComparableNode target)
		{
			if (source == null || target == null || source.BelongsToGraph != this || target.BelongsToGraph != this)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!target.Root)
			{
				Edge.Edge e = edgePool.checkOut();
				e.BelongsToGraph = this;
				e.setEdge((Node.Node)source, (Node.Node)target, Edge_Fields.SECONDARY_EDGE);
				graphEdges.Add(e);
				return e;
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeSecondaryEdge(org.maltparser.core.syntaxgraph.node.ComparableNode source, org.maltparser.core.syntaxgraph.node.ComparableNode target) throws org.maltparser.core.exception.MaltChainedException
		public virtual void removeSecondaryEdge(ComparableNode source, ComparableNode target)
		{
			if (source == null || target == null || source.BelongsToGraph != this || target.BelongsToGraph != this)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!target.Root)
			{
				IEnumerator<Edge.Edge> ie = ((Node.Node)target).IncomingEdgeIterator;
				while (ie.MoveNext())
				{
					Edge.Edge e = ie.Current;
					if (e.Source == source)
					{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
						ie.remove();
						graphEdges.remove(e);
						edgePool.checkIn(e);
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasLabeledDependency(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool HasLabeledDependency(int index)
		{
			return (GetDependencyNode(index).hasHead() && GetDependencyNode(index).HeadEdge.Labeled);
		}

		public virtual bool Connected
		{
			get
			{
				return (numberOfComponents == 1);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isProjective() throws org.maltparser.core.exception.MaltChainedException
		public virtual bool Projective
		{
			get
			{
				foreach (int i in terminalNodes.Keys)
				{
					if (!terminalNodes[i].Projective)
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual bool Tree
		{
			get
			{
				return Connected && SingleHeaded;
			}
		}

		public virtual bool SingleHeaded
		{
			get
			{
				foreach (int i in terminalNodes.Keys)
				{
					if (!terminalNodes[i].hasAtMostOneHead())
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual bool SingleHeadedConstraint
		{
			get
			{
				return singleHeadedConstraint;
			}
			set
			{
				singleHeadedConstraint = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nNonProjectiveEdges() throws org.maltparser.core.exception.MaltChainedException
		public virtual int NNonProjectiveEdges()
		{
			int c = 0;
			foreach (int i in terminalNodes.Keys)
			{
				if (!terminalNodes[i].Projective)
				{
					c++;
				}
			}
			return c;
		}

		public virtual int NEdges()
		{
			return graphEdges.Count;
		}

		public virtual SortedSet<Edge.Edge> Edges
		{
			get
			{
				return graphEdges;
			}
		}

		public virtual SortedSet<int> DependencyIndices
		{
			get
			{
				SortedSet<int> indices = new SortedSet<int>(terminalNodes.Keys);
				indices.Add(0);
				return indices;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.core.syntaxgraph.node.DependencyNode link(org.maltparser.core.syntaxgraph.node.DependencyNode x, org.maltparser.core.syntaxgraph.node.DependencyNode y) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual DependencyNode link(DependencyNode x, DependencyNode y)
		{
			if (x.Rank > y.Rank)
			{
				y.Component = x;
			}
			else
			{
				x.Component = y;
				if (x.Rank == y.Rank)
				{
					y.Rank = y.Rank + 1;
				}
				return y;
			}
			return x;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void linkAllTerminalsToRoot() throws org.maltparser.core.exception.MaltChainedException
		public virtual void linkAllTerminalsToRoot()
		{
			Clear();

			foreach (int i in terminalNodes.Keys)
			{
				DependencyNode node = terminalNodes[i];
				addDependencyEdge(root,node);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void linkAllTreesToRoot() throws org.maltparser.core.exception.MaltChainedException
		public virtual void LinkAllTreesToRoot()
		{
			foreach (int i in terminalNodes.Keys)
			{
				if (!terminalNodes[i].hasHead())
				{
					Edge.Edge e = addDependencyEdge(root,terminalNodes[i]);
					mapping.updatePhraseStructureGraph(this, e, false);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LabelSet getDefaultRootEdgeLabels() throws org.maltparser.core.exception.MaltChainedException
		public virtual LabelSet DefaultRootEdgeLabels
		{
			get
			{
				if (rootLabels == null)
				{
					return null;
				}
				return rootLabels.DefaultRootLabels;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getDefaultRootEdgeLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual string GetDefaultRootEdgeLabelSymbol(SymbolTable table)
		{
			if (rootLabels == null)
			{
				return null;
			}
			return rootLabels.getDefaultRootLabelSymbol(table);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDefaultRootEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual int GetDefaultRootEdgeLabelCode(SymbolTable table)
		{
			if (rootLabels == null)
			{
				return -1;
			}
			return rootLabels.getDefaultRootLabelCode(table).Value;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDefaultRootEdgeLabel(org.maltparser.core.symbol.SymbolTable table, String defaultRootSymbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void SetDefaultRootEdgeLabel(SymbolTable table, string defaultRootSymbol)
		{
			if (rootLabels == null)
			{
				rootLabels = new RootLabels();
			}
			rootLabels.setDefaultRootLabel(table, defaultRootSymbol);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDefaultRootEdgeLabels(String rootLabelOption, java.util.SortedMap<String, org.maltparser.core.symbol.SymbolTable> edgeSymbolTables) throws org.maltparser.core.exception.MaltChainedException
		public virtual void SetDefaultRootEdgeLabels(string rootLabelOption, SortedDictionary<string, SymbolTable> edgeSymbolTables)
		{
			if (rootLabels == null)
			{
				rootLabels = new RootLabels();
			}
			rootLabels.setRootLabels(rootLabelOption, edgeSymbolTables);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void Clear()
		{
			edgePool.checkInAll();
			graphEdges.Clear();
			root.clear();
			root.BelongsToGraph = this;
			nonTerminalPool.checkInAll();
			nonTerminalNodes.Clear();
			if (mapping != null)
			{
				mapping.clear();
			}
			base.Clear();

			numberOfComponents++;
		}

		public virtual DependencyNode DependencyRoot
		{
			get
			{
				return root;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addTerminalNode() throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode addTerminalNode()
		{
			return AddTokenNode();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode addTerminalNode(int index)
		{
			return AddTokenNode(index);
		}

		public virtual PhraseStructureNode getTerminalNode(int index)
		{
			return GetTokenNode(index);
		}

		public virtual int nTerminalNode()
		{
			return NTokenNode();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addNonTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode addNonTerminalNode(int index)
		{
			NonTerminal node = nonTerminalPool.checkOut();
			node.Index = index;
			node.BelongsToGraph = this;
			nonTerminalNodes[index] = node;
			return node;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addNonTerminalNode() throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode addNonTerminalNode()
		{
			int index = HighestNonTerminalIndex;
			if (index > 0)
			{
				return addNonTerminalNode(index + 1);
			}
			return addNonTerminalNode(1);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode getNonTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode getNonTerminalNode(int index)
		{
			return nonTerminalNodes[index];
		}

		public virtual int HighestNonTerminalIndex
		{
			get
			{
				try
				{
					return nonTerminalNodes.lastKey();
				}
				catch (NoSuchElementException)
				{
					return 0;
				}
			}
		}

		public virtual ISet<int> NonTerminalIndices
		{
			get
			{
				return new SortedSet<int>(nonTerminalNodes.Keys);
			}
		}

		public virtual bool hasNonTerminals()
		{
			return nonTerminalNodes.Count > 0;
		}

		public virtual int nNonTerminals()
		{
			return nonTerminalNodes.Count;
		}

		public virtual PhraseStructureNode PhraseStructureRoot
		{
			get
			{
				return root;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge addPhraseStructureEdge(org.maltparser.core.syntaxgraph.node.PhraseStructureNode parent, org.maltparser.core.syntaxgraph.node.PhraseStructureNode child) throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge.Edge addPhraseStructureEdge(PhraseStructureNode parent, PhraseStructureNode child)
		{
			if (parent == null || child == null)
			{
				throw new MaltChainedException("Parent or child node is missing in sentence " + SentenceID);
			}
			else if (parent.BelongsToGraph != this || child.BelongsToGraph != this)
			{
				throw new MaltChainedException("Parent or child node is not a member of the graph in sentence " + SentenceID);
			}
			else if (parent == child)
			{
				throw new MaltChainedException("It is not allowed to add a phrase structure edge connecting the same node in sentence " + SentenceID);
			}
			else if (parent is NonTerminalNode && !child.Root)
			{
				Edge.Edge e = edgePool.checkOut();
				e.BelongsToGraph = this;
				e.setEdge((Node.Node)parent, (Node.Node)child, Edge_Fields.PHRASE_STRUCTURE_EDGE);
				graphEdges.Add(e);
				return e;
			}
			else
			{
				throw new MaltChainedException("Parent or child node is not of correct node type.");
			}
		}

		public override void update(Observable o, object arg)
		{
			if (o is Edge.Edge && mapping != null)
			{
				try
				{
					mapping.update(this, (Edge.Edge)o, arg);
				}
				catch (MaltChainedException ex)
				{
					if (SystemLogger.logger().DebugEnabled)
					{
						SystemLogger.logger().debug("",ex);
					}
					else
					{
						SystemLogger.logger().error(ex.MessageChain);
					}
					Environment.Exit(1);
				}
			}
		}

		public virtual LosslessMapping Mapping
		{
			get
			{
				return mapping;
			}
			set
			{
				mapping = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(Element element, String labelFunction, String label) throws org.maltparser.core.exception.MaltChainedException
		public override void AddLabel(Element element, string labelFunction, string label)
		{
			base.AddLabel(element, labelFunction, label);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removePhraseStructureEdge(org.maltparser.core.syntaxgraph.node.PhraseStructureNode parent, org.maltparser.core.syntaxgraph.node.PhraseStructureNode child) throws org.maltparser.core.exception.MaltChainedException
		public virtual void removePhraseStructureEdge(PhraseStructureNode parent, PhraseStructureNode child)
		{
			if (parent == null || child == null)
			{
				throw new MaltChainedException("Parent or child node is missing.");
			}
			else if (parent is NonTerminalNode && !child.Root)
			{
				foreach (Edge.Edge e in graphEdges)
				{
					if (e.Source == parent && e.Target == child)
					{
						e.clear();
						graphEdges.remove(e);
						if (e is GraphEdge)
						{
							edgePool.checkIn(e);
						}
					}
				}
			}
			else
			{
				throw new SyntaxGraphException("Head node is not a root node or a terminal node.");
			}
		}

		public virtual bool Continuous
		{
			get
			{
				foreach (int index in nonTerminalNodes.Keys)
				{
					NonTerminalNode node = nonTerminalNodes[index];
    
					if (!node.Continuous)
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual bool ContinuousExcludeTerminalsAttachToRoot
		{
			get
			{
				foreach (int index in nonTerminalNodes.Keys)
				{
					NonTerminalNode node = nonTerminalNodes[index];
					if (!node.ContinuousExcludeTerminalsAttachToRoot)
					{
						return false;
					}
				}
				return true;
			}
		}

	//	public void makeContinuous() throws MaltChainedException {
	//		if (root != null) {
	//			root.reArrangeChildrenAccordingToLeftAndRightProperDesendant();
	//		}
	//	}

		public virtual string toStringTerminalNode(TokenNode node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode depnode = node;
			DependencyNode depnode = node;

			sb.Append(node.ToString().Trim());
			if (depnode.hasHead())
			{
				sb.Append('\t');
				try
				{
					sb.Append(depnode.Head.Index);
					sb.Append('\t');
					sb.Append(depnode.HeadEdge.ToString());
				}
				catch (MaltChainedException e)
				{
					Console.Error.WriteLine(e);
				}
			}
			sb.Append('\n');

			return sb.ToString();
		}

		public virtual string toStringNonTerminalNode(NonTerminalNode node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();

			sb.Append(node.ToString().Trim());
			sb.Append('\n');
			IEnumerator<Edge.Edge> ie = ((Node.Node)node).OutgoingEdgeIterator;
			while (ie.MoveNext())
			{
				Edge.Edge e = ie.Current;
				if (e.Target is TokenNode)
				{
					sb.Append("   T");
					sb.Append(e.Target.Index);
				}
				if (e.Target is NonTerminalNode)
				{
					sb.Append("   N");
					sb.Append(e.Target.Index);
				}
				sb.Append('\t');
				sb.Append(e.ToString());
				sb.Append('\n');
			}
			return sb.ToString();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (int index in terminalNodes.Keys)
			{
				sb.Append(toStringTerminalNode(terminalNodes[index]));
			}
			sb.Append('\n');
			sb.Append(toStringNonTerminalNode((NonTerminalNode)PhraseStructureRoot));
			foreach (int index in nonTerminalNodes.Keys)
			{
				sb.Append(toStringNonTerminalNode(nonTerminalNodes[index]));
			}
			return sb.ToString();
		}
	}

}