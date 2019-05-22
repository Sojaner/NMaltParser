using System.Collections.Generic;
using System.Text;
using NMaltParser.Concurrent.Graph.DataFormat;
using NMaltParser.Core.Helper;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Utilities;

namespace NMaltParser.Core.LW.Graph
{
    /// <summary>
	/// A lightweight version of org.maltparser.core.syntaxgraph.DependencyGraph.
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class LWDependencyGraph : DependencyStructure
	{
		private const string TAB_SIGN = "\t";

		private readonly DataFormat dataFormat;
		private readonly SymbolTableHandler symbolTables;
		private readonly RootLabels rootLabels;
		private readonly IList<LWNode> nodes;
		private readonly HashMap<int, List<string>> comments;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LWDependencyGraph(org.maltparser.concurrent.graph.dataformat.DataFormat _dataFormat, org.maltparser.core.symbol.SymbolTableHandler _symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public LWDependencyGraph(DataFormat _dataFormat, SymbolTableHandler _symbolTables)
		{
			dataFormat = _dataFormat;
			symbolTables = _symbolTables;
			rootLabels = new RootLabels();
			nodes = new List<LWNode>();
			nodes.Add(new LWNode(this, 0)); // ROOT
			comments = new HashMap<int, List<string>>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LWDependencyGraph(org.maltparser.concurrent.graph.dataformat.DataFormat _dataFormat, org.maltparser.core.symbol.SymbolTableHandler _symbolTables, String[] inputTokens, String defaultRootLabel) throws org.maltparser.core.exception.MaltChainedException
		public LWDependencyGraph(DataFormat _dataFormat, SymbolTableHandler _symbolTables, string[] inputTokens, string defaultRootLabel) : this(_dataFormat, _symbolTables, inputTokens, defaultRootLabel, true)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LWDependencyGraph(org.maltparser.concurrent.graph.dataformat.DataFormat _dataFormat, org.maltparser.core.symbol.SymbolTableHandler _symbolTables, String[] inputTokens, String defaultRootLabel, boolean addEdges) throws org.maltparser.core.exception.MaltChainedException
		public LWDependencyGraph(DataFormat _dataFormat, SymbolTableHandler _symbolTables, string[] inputTokens, string defaultRootLabel, bool addEdges)
		{
			dataFormat = _dataFormat;
			symbolTables = _symbolTables;
			rootLabels = new RootLabels();
			nodes = new List<LWNode>(inputTokens.Length + 1);
			comments = new HashMap<int, List<string>>();
			resetTokens(inputTokens, defaultRootLabel, addEdges);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetTokens(String[] inputTokens, String defaultRootLabel, boolean addEdges) throws org.maltparser.core.exception.MaltChainedException
		public void resetTokens(string[] inputTokens, string defaultRootLabel, bool addEdges)
		{
			nodes.Clear();
			comments.Clear();
			symbolTables.cleanUp();
			// Add nodes
			nodes.Add(new LWNode(this, 0)); // ROOT
			for (int i = 0; i < inputTokens.Length; i++)
			{
				nodes.Add(new LWNode(this, i + 1));
			}

			for (int i = 0; i < inputTokens.Length; i++)
			{
				nodes[i + 1].addColumnLabels(inputTokens[i].Split(TAB_SIGN, true), addEdges);
			}
			// Check graph
			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i].HeadIndex >= nodes.Count)
				{
					throw new LWGraphException("Not allowed to add a head node that doesn't exists");
				}
			}

			for (int i = 0; i < dataFormat.numberOfColumns(); i++)
			{
				ColumnDescription column = dataFormat.getColumnDescription(i);
				if (!column.Internal && column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
				{
					rootLabels.setDefaultRootLabel(symbolTables.getSymbolTable(column.Name), defaultRootLabel);
				}
			}
		}

		public DataFormat DataFormat
		{
			get
			{
				return dataFormat;
			}
		}

		public LWNode getNode(int nodeIndex)
		{
			if (nodeIndex < 0 || nodeIndex >= nodes.Count)
			{
				return null;
			}
			return nodes[nodeIndex];
		}

		public int nNodes()
		{
			return nodes.Count;
		}

		protected internal bool hasDependent(int nodeIndex)
		{
			for (int i = 1; i < nodes.Count; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					return true;
				}
			}
			return false;
		}

		protected internal bool hasLeftDependent(int nodeIndex)
		{
			for (int i = 1; i < nodeIndex; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					return true;
				}
			}
			return false;
		}

		protected internal bool hasRightDependent(int nodeIndex)
		{
			for (int i = nodeIndex + 1; i < nodes.Count; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					return true;
				}
			}
			return false;
		}

		protected internal IList<DependencyNode> getListOfLeftDependents(int nodeIndex)
		{
			IList<DependencyNode> leftDependents = Collections.synchronizedList(new List<DependencyNode>());
			for (int i = 1; i < nodeIndex; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					leftDependents.Add(nodes[i]);
				}
			}
			return leftDependents;
		}

		protected internal SortedSet<DependencyNode> getSortedSetOfLeftDependents(int nodeIndex)
		{
			SortedSet<DependencyNode> leftDependents = Collections.synchronizedSortedSet(new SortedSet<DependencyNode>());
			for (int i = 1; i < nodeIndex; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					leftDependents.Add(nodes[i]);
				}
			}
			return leftDependents;
		}

		protected internal IList<DependencyNode> getListOfRightDependents(int nodeIndex)
		{
			IList<DependencyNode> rightDependents = Collections.synchronizedList(new List<DependencyNode>());
			for (int i = nodeIndex + 1; i < nodes.Count; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					rightDependents.Add(nodes[i]);
				}
			}
			return rightDependents;
		}

		protected internal SortedSet<DependencyNode> getSortedSetOfRightDependents(int nodeIndex)
		{
			SortedSet<DependencyNode> rightDependents = Collections.synchronizedSortedSet(new SortedSet<DependencyNode>());
			for (int i = nodeIndex + 1; i < nodes.Count; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					rightDependents.Add(nodes[i]);
				}
			}
			return rightDependents;
		}

		protected internal IList<DependencyNode> getListOfDependents(int nodeIndex)
		{
			IList<DependencyNode> dependents = Collections.synchronizedList(new List<DependencyNode>());
			for (int i = 1; i < nodes.Count; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					dependents.Add(nodes[i]);
				}
			}
			return dependents;
		}

		protected internal SortedSet<DependencyNode> getSortedSetOfDependents(int nodeIndex)
		{
			SortedSet<DependencyNode> dependents = Collections.synchronizedSortedSet(new SortedSet<DependencyNode>());
			for (int i = 1; i < nodes.Count; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					dependents.Add(nodes[i]);
				}
			}
			return dependents;
		}

		protected internal int getRank(int nodeIndex)
		{
			int[] components = new int[nodes.Count];
			int[] ranks = new int[nodes.Count];
			for (int i = 0; i < components.Length; i++)
			{
				components[i] = i;
				ranks[i] = 0;
			}
			for (int i = 1; i < nodes.Count; i++)
			{
				if (nodes[i].hasHead())
				{
					int hcIndex = findComponent(nodes[i].Head.Index, components);
					int dcIndex = findComponent(nodes[i].Index, components);
					if (hcIndex != dcIndex)
					{
						link(hcIndex, dcIndex, components, ranks);
					}
				}
			}
			return ranks[nodeIndex];
		}

		protected internal DependencyNode findComponent(int nodeIndex)
		{
			int[] components = new int[nodes.Count];
			int[] ranks = new int[nodes.Count];
			for (int i = 0; i < components.Length; i++)
			{
				components[i] = i;
				ranks[i] = 0;
			}
			for (int i = 1; i < nodes.Count; i++)
			{
				if (nodes[i].hasHead())
				{
					int hcIndex = findComponent(nodes[i].Head.Index, components);
					int dcIndex = findComponent(nodes[i].Index, components);
					if (hcIndex != dcIndex)
					{
						link(hcIndex, dcIndex, components, ranks);
					}
				}
			}
			return nodes[findComponent(nodeIndex, components)];
		}

		private int[] findComponents()
		{
			int[] components = new int[nodes.Count];
			int[] ranks = new int[nodes.Count];
			for (int i = 0; i < components.Length; i++)
			{
				components[i] = i;
				ranks[i] = 0;
			}
			for (int i = 1; i < nodes.Count; i++)
			{
				if (nodes[i].hasHead())
				{
					int hcIndex = findComponent(nodes[i].Head.Index, components);
					int dcIndex = findComponent(nodes[i].Index, components);
					if (hcIndex != dcIndex)
					{
						link(hcIndex, dcIndex, components, ranks);
					}
				}
			}
			return components;
		}

		private int findComponent(int xIndex, int[] components)
		{
			if (xIndex != components[xIndex])
			{
				components[xIndex] = findComponent(components[xIndex], components);
			}
			return components[xIndex];
		}

		private int link(int xIndex, int yIndex, int[] components, int[] ranks)
		{
			if (ranks[xIndex] > ranks[yIndex])
			{
				components[yIndex] = xIndex;
			}
			else
			{
				components[xIndex] = yIndex;

				if (ranks[xIndex] == ranks[yIndex])
				{
					ranks[yIndex]++;
				}
				return yIndex;
			}
			return xIndex;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.node.TokenNode addTokenNode() throws org.maltparser.core.exception.MaltChainedException
		public TokenNode addTokenNode()
		{
			throw new LWGraphException("Not implemented in the light-weight dependency graph package");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.node.TokenNode addTokenNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public TokenNode addTokenNode(int index)
		{
			throw new LWGraphException("Not implemented in the light-weight dependency graph package");
		}

		public TokenNode getTokenNode(int index)
		{
	//		throw new LWGraphException("Not implemented in the light-weight dependency graph package");
			return null;
		}


		public void addComment(string comment, int at_index)
		{
			List<string> commentList = comments[at_index];
			if (commentList == null)
			{
				commentList = comments[at_index] = new List<string>();
			}
			commentList.Add(comment);
		}

		public List<string> getComment(int at_index)
		{
			return comments[at_index];
		}

		public bool hasComments()
		{
			return comments.Count > 0;
		}

		public int nTokenNode()
		{
			return nodes.Count - 1;
		}

		public SortedSet<int> TokenIndices
		{
			get
			{
				SortedSet<int> indices = Collections.synchronizedSortedSet(new SortedSet<int>());
				for (int i = 1; i < nodes.Count; i++)
				{
					indices.Add(i);
				}
				return indices;
			}
		}

		public int HighestTokenIndex
		{
			get
			{
				return nodes.Count - 1;
			}
		}

		public bool hasTokens()
		{
			return nodes.Count > 1;
		}

		public int SentenceID
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			nodes.Clear();
		}

		public override SymbolTableHandler SymbolTables
		{
			get
			{
				return symbolTables;
			}
			set
			{
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addLabel(org.maltparser.core.syntaxgraph.Element element, String labelFunction, String label) throws org.maltparser.core.exception.MaltChainedException
		public override void addLabel(Element element, string labelFunction, string label)
		{
			element.addLabel(symbolTables.addSymbolTable(labelFunction), label);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.LabelSet checkOutNewLabelSet() throws org.maltparser.core.exception.MaltChainedException
		public override LabelSet checkOutNewLabelSet()
		{
			throw new LWGraphException("Not implemented in light-weight dependency graph");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void checkInLabelSet(org.maltparser.core.syntaxgraph.LabelSet labelSet) throws org.maltparser.core.exception.MaltChainedException
		public override void checkInLabelSet(LabelSet labelSet)
		{
			throw new LWGraphException("Not implemented in light-weight dependency graph");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.edge.Edge addSecondaryEdge(org.maltparser.core.syntaxgraph.node.ComparableNode source, org.maltparser.core.syntaxgraph.node.ComparableNode target) throws org.maltparser.core.exception.MaltChainedException
		public Edge addSecondaryEdge(ComparableNode source, ComparableNode target)
		{
			throw new LWGraphException("Not implemented in light-weight dependency graph");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void removeSecondaryEdge(org.maltparser.core.syntaxgraph.node.ComparableNode source, org.maltparser.core.syntaxgraph.node.ComparableNode target) throws org.maltparser.core.exception.MaltChainedException
		public void removeSecondaryEdge(ComparableNode source, ComparableNode target)
		{
			throw new LWGraphException("Not implemented in light-weight dependency graph");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.node.DependencyNode addDependencyNode() throws org.maltparser.core.exception.MaltChainedException
		public DependencyNode addDependencyNode()
		{
			LWNode node = new LWNode(this, nodes.Count);
			nodes.Add(node);
			return node;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.node.DependencyNode addDependencyNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public DependencyNode addDependencyNode(int index)
		{
			if (index == 0)
			{
				return nodes[0];
			}
			else if (index == nodes.Count)
			{
				return addDependencyNode();
			}
			throw new LWGraphException("Not implemented in light-weight dependency graph");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.node.DependencyNode getDependencyNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public DependencyNode getDependencyNode(int index)
		{
			if (index < 0 || index >= nodes.Count)
			{
				return null;
			}
			return nodes[index];
		}

		public int nDependencyNode()
		{
			return nodes.Count;
		}

		public int HighestDependencyNodeIndex
		{
			get
			{
				return nodes.Count - 1;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.edge.Edge addDependencyEdge(int headIndex, int dependentIndex) throws org.maltparser.core.exception.MaltChainedException
		public Edge addDependencyEdge(int headIndex, int dependentIndex)
		{
			if (headIndex < 0 && headIndex >= nodes.Count)
			{
				throw new LWGraphException("The head doesn't exists");
			}
			if (dependentIndex < 0 && dependentIndex >= nodes.Count)
			{
				throw new LWGraphException("The dependent doesn't exists");
			}
			LWNode head = nodes[headIndex];
			LWNode dependent = nodes[dependentIndex];
			Edge headEdge = new LWEdge(head, dependent);
			dependent.addIncomingEdge(headEdge);
			return headEdge;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.edge.Edge moveDependencyEdge(int newHeadIndex, int dependentIndex) throws org.maltparser.core.exception.MaltChainedException
		public Edge moveDependencyEdge(int newHeadIndex, int dependentIndex)
		{
			if (newHeadIndex < 0 && newHeadIndex >= nodes.Count)
			{
				throw new LWGraphException("The head doesn't exists");
			}
			if (dependentIndex < 0 && dependentIndex >= nodes.Count)
			{
				throw new LWGraphException("The dependent doesn't exists");
			}

			LWNode head = nodes[newHeadIndex];
			LWNode dependent = nodes[dependentIndex];
			Edge oldheadEdge = dependent.HeadEdge;
			Edge headEdge = new LWEdge(head, dependent);
			headEdge.addLabel(oldheadEdge.LabelSet);
			dependent.addIncomingEdge(headEdge);
			return headEdge;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void removeDependencyEdge(int headIndex, int dependentIndex) throws org.maltparser.core.exception.MaltChainedException
		public void removeDependencyEdge(int headIndex, int dependentIndex)
		{
			if (headIndex < 0 && headIndex >= nodes.Count)
			{
				throw new LWGraphException("The head doesn't exists");
			}
			if (dependentIndex < 0 && dependentIndex >= nodes.Count)
			{
				throw new LWGraphException("The dependent doesn't exists");
			}
			LWNode head = nodes[headIndex];
			LWNode dependent = nodes[dependentIndex];
			Edge headEdge = new LWEdge(head, dependent);
			dependent.removeIncomingEdge(headEdge);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void linkAllTreesToRoot() throws org.maltparser.core.exception.MaltChainedException
		public void linkAllTreesToRoot()
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (!nodes[i].hasHead())
				{
					LWNode head = nodes[0];
					LWNode dependent = nodes[i];
					Edge headEdge = new LWEdge(head, dependent);
					headEdge.addLabel(DefaultRootEdgeLabels);
					dependent.addIncomingEdge(headEdge);
				}
			}
		}

		public int nEdges()
		{
			int n = 0;
			for (int i = 1; i < nodes.Count; i++)
			{
				if (nodes[i].hasHead())
				{
					n++;
				}
			}
			return n;
		}

		public SortedSet<Edge> Edges
		{
			get
			{
				SortedSet<Edge> edges = Collections.synchronizedSortedSet(new SortedSet<Edge>());
				for (int i = 1; i < nodes.Count; i++)
				{
					if (nodes[i].hasHead())
					{
						edges.Add(nodes[i].HeadEdge);
					}
				}
				return edges;
			}
		}

		public SortedSet<int> DependencyIndices
		{
			get
			{
				SortedSet<int> indices = Collections.synchronizedSortedSet(new SortedSet<int>());
				for (int i = 0; i < nodes.Count; i++)
				{
					indices.Add(i);
				}
				return indices;
			}
		}

		public DependencyNode DependencyRoot
		{
			get
			{
				return nodes[0];
			}
		}

		public bool hasLabeledDependency(int index)
		{
			if (index < 0 || index >= nodes.Count)
			{
				return false;
			}
			if (!nodes[index].hasHead())
			{
				return false;
			}
			return nodes[index].HeadLabeled;
		}

		public bool Connected
		{
			get
			{
				int[] components = findComponents();
				int tmp = components[0];
				for (int i = 1; i < components.Length; i++)
				{
					if (tmp != components[i])
					{
						return false;
					}
				}
				return true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public boolean isProjective() throws org.maltparser.core.exception.MaltChainedException
		public bool Projective
		{
			get
			{
				for (int i = 1; i < nodes.Count; i++)
				{
					if (!nodes[i].Projective)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool SingleHeaded
		{
			get
			{
				return true;
			}
		}

		public bool Tree
		{
			get
			{
				return Connected && SingleHeaded;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int nNonProjectiveEdges() throws org.maltparser.core.exception.MaltChainedException
		public int nNonProjectiveEdges()
		{
			int c = 0;
			for (int i = 1; i < nodes.Count; i++)
			{
				if (!nodes[i].Projective)
				{
					c++;
				}
			}
			return c;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.LabelSet getDefaultRootEdgeLabels() throws org.maltparser.core.exception.MaltChainedException
		public LabelSet DefaultRootEdgeLabels
		{
			get
			{
				return rootLabels.DefaultRootLabels;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public String getDefaultRootEdgeLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public string getDefaultRootEdgeLabelSymbol(SymbolTable table)
		{
			return rootLabels.getDefaultRootLabelSymbol(table);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int getDefaultRootEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public int getDefaultRootEdgeLabelCode(SymbolTable table)
		{
			return rootLabels.getDefaultRootLabelCode(table).Value;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setDefaultRootEdgeLabel(org.maltparser.core.symbol.SymbolTable table, String defaultRootSymbol) throws org.maltparser.core.exception.MaltChainedException
		public void setDefaultRootEdgeLabel(SymbolTable table, string defaultRootSymbol)
		{
			rootLabels.setDefaultRootLabel(table, defaultRootSymbol);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setDefaultRootEdgeLabels(String rootLabelOption, java.util.SortedMap<String, org.maltparser.core.symbol.SymbolTable> edgeSymbolTables) throws org.maltparser.core.exception.MaltChainedException
		public void setDefaultRootEdgeLabels(string rootLabelOption, SortedDictionary<string, SymbolTable> edgeSymbolTables)
		{
			rootLabels.setRootLabels(rootLabelOption, edgeSymbolTables);
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (LWNode node in nodes)
			{
				sb.Append(node.ToString().Trim());
				sb.Append('\n');
			}
			sb.Append('\n');
			return sb.ToString();
		}
	}

}