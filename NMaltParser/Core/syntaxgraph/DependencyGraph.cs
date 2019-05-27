using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Pool;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class DependencyGraph : Sentence, IDependencyStructure
	{
		private readonly ObjectPoolList<Edge.Edge> edgePool;
		private readonly SortedSet<Edge.Edge> graphEdges;
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
			graphEdges = new SortedSet<Edge.Edge>();
			edgePool = new ObjectPoolListAnonymousInnerClass(this);
			Clear();
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<Edge.Edge>
		{
			private readonly DependencyGraph outerInstance;

			public ObjectPoolListAnonymousInnerClass(DependencyGraph outerInstance)
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
		protected internal virtual Edge.Edge addDependencyEdge(DependencyNode head, DependencyNode dependent)
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
//ORIGINAL LINE: protected org.maltparser.core.syntaxgraph.edge.Edge moveDependencyEdge(org.maltparser.core.syntaxgraph.node.DependencyNode newHead, org.maltparser.core.syntaxgraph.node.DependencyNode dependent) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual Edge.Edge moveDependencyEdge(DependencyNode newHead, DependencyNode dependent)
		{
			if (dependent == null || !dependent.hasHead())
			{
				return null;
			}
			Edge.Edge headEdge = dependent.HeadEdge;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LabelSet labels = checkOutNewLabelSet();
			LabelSet labels = CheckOutNewLabelSet();
			foreach (SymbolTable table in headEdge.LabelTypes)
			{
				labels.put(table, headEdge.getLabelCode(table));
			}
			headEdge.clear();
			headEdge.BelongsToGraph = this;
			headEdge.setEdge((Node.Node)newHead, (Node.Node)dependent, Edge_Fields.DEPENDENCY_EDGE);
			headEdge.addLabel(labels);
			labels.clear();
			CheckInLabelSet(labels);
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
			if (head == null || dependent == null)
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
		public virtual Edge.Edge addSecondaryEdge(ComparableNode source, ComparableNode target)
		{
			if (source == null || target == null)
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
			if (source == null || target == null)
			{
				throw new SyntaxGraphException("Head or dependent node is missing.");
			}
			else if (!target.Root)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<org.maltparser.core.syntaxgraph.edge.Edge> ie = ((org.maltparser.core.syntaxgraph.node.Node)target).getIncomingEdgeIterator();
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

	//	public boolean hasLabeledDependency(int index, SymbolTable table) {
	//		return (!getDependencyNode(index).isRoot() && getDependencyNode(index).getLabelCode(table) != null);
	//	}

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
//ORIGINAL LINE: public void linkAllTreesToRoot() throws org.maltparser.core.exception.MaltChainedException
		public virtual void LinkAllTreesToRoot()
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