using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.syntaxgraph
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using ObjectPoolList = org.maltparser.core.pool.ObjectPoolList;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;
	using GraphEdge = org.maltparser.core.syntaxgraph.edge.GraphEdge;
	using ComparableNode = org.maltparser.core.syntaxgraph.node.ComparableNode;
	using DependencyNode = org.maltparser.core.syntaxgraph.node.DependencyNode;
	using Node = org.maltparser.core.syntaxgraph.node.Node;
	using Root = org.maltparser.core.syntaxgraph.node.Root;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class DependencyGraph : Sentence, DependencyStructure
	{
		private readonly ObjectPoolList<Edge> edgePool;
		private readonly SortedSet<Edge> graphEdges;
		private readonly Root root;
		private bool singleHeadedConstraint;
		private RootLabels rootLabels;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyGraph(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public DependencyGraph(SymbolTableHandler symbolTables) : base(symbolTables)
		{
			SingleHeadedConstraint = true;
			root = new Root();
			root.BelongsToGraph = this;
			graphEdges = new SortedSet<Edge>();
			edgePool = new ObjectPoolListAnonymousInnerClass(this);
			clear();
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<Edge>
		{
			private readonly DependencyGraph outerInstance;

			public ObjectPoolListAnonymousInnerClass(DependencyGraph outerInstance)
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
			else
			{ // if (headIndex > 0) {
				head = getOrAddTerminalNode(headIndex);
			}

			if (dependentIndex > 0)
			{
				dependent = getOrAddTerminalNode(dependentIndex);
			}
			return addDependencyEdge(head, dependent);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.core.syntaxgraph.edge.Edge addDependencyEdge(org.maltparser.core.syntaxgraph.node.DependencyNode head, org.maltparser.core.syntaxgraph.node.DependencyNode dependent) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual Edge addDependencyEdge(DependencyNode head, DependencyNode dependent)
		{
			if (head == null || dependent == null)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!dependent.Root)
			{
				if (singleHeadedConstraint && dependent.hasHead())
				{
					return moveDependencyEdge(head, dependent);
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode hc = ((org.maltparser.core.syntaxgraph.node.DependencyNode)head).findComponent();
				DependencyNode hc = ((DependencyNode)head).findComponent();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode dc = ((org.maltparser.core.syntaxgraph.node.DependencyNode)dependent).findComponent();
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
//ORIGINAL LINE: protected org.maltparser.core.syntaxgraph.edge.Edge moveDependencyEdge(org.maltparser.core.syntaxgraph.node.DependencyNode newHead, org.maltparser.core.syntaxgraph.node.DependencyNode dependent) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual Edge moveDependencyEdge(DependencyNode newHead, DependencyNode dependent)
		{
			if (dependent == null || !dependent.hasHead())
			{
				return null;
			}
			Edge headEdge = dependent.HeadEdge;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LabelSet labels = checkOutNewLabelSet();
			LabelSet labels = checkOutNewLabelSet();
			foreach (SymbolTable table in headEdge.LabelTypes)
			{
				labels.put(table, headEdge.getLabelCode(table));
			}
			headEdge.clear();
			headEdge.BelongsToGraph = this;
			headEdge.setEdge((Node)newHead, (Node)dependent, org.maltparser.core.syntaxgraph.edge.Edge_Fields.DEPENDENCY_EDGE);
			headEdge.addLabel(labels);
			labels.clear();
			checkInLabelSet(labels);
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
			if (head == null || dependent == null)
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
						graphEdges.remove(e);
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
						ie.remove();
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
			if (source == null || target == null)
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
			if (source == null || target == null)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!target.Root)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<org.maltparser.core.syntaxgraph.edge.Edge> ie = ((org.maltparser.core.syntaxgraph.node.Node)target).getIncomingEdgeIterator();
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

	//	public boolean hasLabeledDependency(int index, SymbolTable table) {
	//		return (!getDependencyNode(index).isRoot() && getDependencyNode(index).getLabelCode(table) != null);
	//	}

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
//ORIGINAL LINE: public void linkAllTreesToRoot() throws org.maltparser.core.exception.MaltChainedException
		public virtual void linkAllTreesToRoot()
		{
			foreach (int i in terminalNodes.Keys)
			{
				if (!terminalNodes[i].hasHead())
				{
					addDependencyEdge(root,terminalNodes[i]);
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

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (int index in terminalNodes.Keys)
			{
				sb.Append(terminalNodes[index].ToString().Trim());
				sb.Append('\n');
			}
			sb.Append('\n');
			return sb.ToString();
		}
	}

}