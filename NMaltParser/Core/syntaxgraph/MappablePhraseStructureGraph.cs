using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.syntaxgraph
{


	using  org.maltparser.core.exception;
	using  org.maltparser.core.helper;
	using  org.maltparser.core.pool;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.syntaxgraph.ds2ps;
	using  org.maltparser.core.syntaxgraph.edge;
	using  org.maltparser.core.syntaxgraph.edge;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.core.syntaxgraph.node;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class MappablePhraseStructureGraph : Sentence, DependencyStructure, PhraseStructure
	{
		private readonly ObjectPoolList<Edge> edgePool;
		private readonly SortedSet<Edge> graphEdges;
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
			graphEdges = new SortedSet<Edge>();
			edgePool = new ObjectPoolListAnonymousInnerClass(this);

			nonTerminalNodes = new SortedDictionary<int, NonTerminal>();
			nonTerminalPool = new ObjectPoolListAnonymousInnerClass2(this);
			clear();
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<Edge>
		{
			private readonly MappablePhraseStructureGraph outerInstance;

			public ObjectPoolListAnonymousInnerClass(MappablePhraseStructureGraph outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			protected internal override Edge create()
			{
				return new GraphEdge();
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetObject(org.maltparser.core.syntaxgraph.edge.Edge o) throws org.maltparser.core.exception.MaltChainedException
			public void resetObject(Edge o)
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
		public virtual DependencyNode addDependencyNode()
		{
			return addTokenNode();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode addDependencyNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode addDependencyNode(int index)
		{
			if (index == 0)
			{
				return root;
			}
			return addTokenNode(index);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getDependencyNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyNode getDependencyNode(int index)
		{
			if (index == 0)
			{
				return root;
			}
			return getTokenNode(index);
		}

		public virtual int nDependencyNode()
		{
			return nTokenNode() + 1;
		}

		public virtual int HighestDependencyNodeIndex
		{
			get
			{
				if (hasTokens())
				{
					return HighestTokenIndex;
				}
				return 0;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge addDependencyEdge(int headIndex, int dependentIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual Edge addDependencyEdge(int headIndex, int dependentIndex)
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
		public virtual Edge addDependencyEdge(DependencyNode head, DependencyNode dependent)
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
				Edge e = edgePool.checkOut();
				e.BelongsToGraph = this;
				e.setEdge((Node)head, (Node)dependent, org.maltparser.core.syntaxgraph.edge.Edge_Fields.DEPENDENCY_EDGE);
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
		public virtual Edge moveDependencyEdge(int newHeadIndex, int dependentIndex)
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
		public virtual Edge moveDependencyEdge(DependencyNode newHead, DependencyNode dependent)
		{
			if (dependent == null || !dependent.hasHead() || newHead.BelongsToGraph != this || dependent.BelongsToGraph != this)
			{
				return null;
			}
			Edge headEdge = dependent.HeadEdge;

			LabelSet labels = null;
			if (headEdge.Labeled)
			{
				labels = checkOutNewLabelSet();
				foreach (SymbolTable table in headEdge.LabelTypes)
				{
					labels.put(table, headEdge.getLabelCode(table));
				}
			}
			headEdge.clear();
			headEdge.BelongsToGraph = this;
			headEdge.setEdge((Node)newHead, (Node)dependent, org.maltparser.core.syntaxgraph.edge.Edge_Fields.DEPENDENCY_EDGE);
			if (labels != null)
			{
				headEdge.addLabel(labels);
				labels.clear();
				checkInLabelSet(labels);
			}
			return headEdge;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeDependencyEdge(int headIndex, int dependentIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual void removeDependencyEdge(int headIndex, int dependentIndex)
		{
			Node head = null;
			Node dependent = null;
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
		protected internal virtual void removeDependencyEdge(Node head, Node dependent)
		{
			if (head == null || dependent == null || head.BelongsToGraph != this || dependent.BelongsToGraph != this)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!dependent.Root)
			{
				IEnumerator<Edge> ie = dependent.IncomingEdgeIterator;
				while (ie.MoveNext())
				{
					Edge e = ie.Current;
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
		public virtual Edge addSecondaryEdge(ComparableNode source, ComparableNode target)
		{
			if (source == null || target == null || source.BelongsToGraph != this || target.BelongsToGraph != this)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!target.Root)
			{
				Edge e = edgePool.checkOut();
				e.BelongsToGraph = this;
				e.setEdge((Node)source, (Node)target, org.maltparser.core.syntaxgraph.edge.Edge_Fields.SECONDARY_EDGE);
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
				IEnumerator<Edge> ie = ((Node)target).IncomingEdgeIterator;
				while (ie.MoveNext())
				{
					Edge e = ie.Current;
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
		public virtual bool hasLabeledDependency(int index)
		{
			return (getDependencyNode(index).hasHead() && getDependencyNode(index).HeadEdge.Labeled);
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
				this.singleHeadedConstraint = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nNonProjectiveEdges() throws org.maltparser.core.exception.MaltChainedException
		public virtual int nNonProjectiveEdges()
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

		public virtual int nEdges()
		{
			return graphEdges.Count;
		}

		public virtual SortedSet<Edge> Edges
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
			clear();

			foreach (int i in terminalNodes.Keys)
			{
				DependencyNode node = terminalNodes[i];
				addDependencyEdge(root,node);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void linkAllTreesToRoot() throws org.maltparser.core.exception.MaltChainedException
		public virtual void linkAllTreesToRoot()
		{
			foreach (int i in terminalNodes.Keys)
			{
				if (!terminalNodes[i].hasHead())
				{
					Edge e = addDependencyEdge(root,terminalNodes[i]);
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
		public virtual string getDefaultRootEdgeLabelSymbol(SymbolTable table)
		{
			if (rootLabels == null)
			{
				return null;
			}
			return rootLabels.getDefaultRootLabelSymbol(table);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDefaultRootEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getDefaultRootEdgeLabelCode(SymbolTable table)
		{
			if (rootLabels == null)
			{
				return -1;
			}
			return rootLabels.getDefaultRootLabelCode(table).Value;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDefaultRootEdgeLabel(org.maltparser.core.symbol.SymbolTable table, String defaultRootSymbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setDefaultRootEdgeLabel(SymbolTable table, string defaultRootSymbol)
		{
			if (rootLabels == null)
			{
				rootLabels = new RootLabels();
			}
			rootLabels.setDefaultRootLabel(table, defaultRootSymbol);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDefaultRootEdgeLabels(String rootLabelOption, java.util.SortedMap<String, org.maltparser.core.symbol.SymbolTable> edgeSymbolTables) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setDefaultRootEdgeLabels(string rootLabelOption, SortedDictionary<string, SymbolTable> edgeSymbolTables)
		{
			if (rootLabels == null)
			{
				rootLabels = new RootLabels();
			}
			rootLabels.setRootLabels(rootLabelOption, edgeSymbolTables);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
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
			base.clear();

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
			return addTokenNode();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual PhraseStructureNode addTerminalNode(int index)
		{
			return addTokenNode(index);
		}

		public virtual PhraseStructureNode getTerminalNode(int index)
		{
			return getTokenNode(index);
		}

		public virtual int nTerminalNode()
		{
			return nTokenNode();
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
		public virtual Edge addPhraseStructureEdge(PhraseStructureNode parent, PhraseStructureNode child)
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
				Edge e = edgePool.checkOut();
				e.BelongsToGraph = this;
				e.setEdge((Node)parent, (Node)child, org.maltparser.core.syntaxgraph.edge.Edge_Fields.PHRASE_STRUCTURE_EDGE);
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
			if (o is Edge && mapping != null)
			{
				try
				{
					mapping.update(this, (Edge)o, arg);
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
				this.mapping = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(Element element, String labelFunction, String label) throws org.maltparser.core.exception.MaltChainedException
		public override void addLabel(Element element, string labelFunction, string label)
		{
			base.addLabel(element, labelFunction, label);
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
				foreach (Edge e in graphEdges)
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
			IEnumerator<Edge> ie = ((Node)node).OutgoingEdgeIterator;
			while (ie.MoveNext())
			{
				Edge e = ie.Current;
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