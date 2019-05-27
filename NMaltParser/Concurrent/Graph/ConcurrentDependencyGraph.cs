using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Concurrent.Graph.DataFormat;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Utilities;

namespace NMaltParser.Concurrent.Graph
{
    /// <summary>
	/// Immutable and tread-safe dependency graph implementation.
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class ConcurrentDependencyGraph
	{
		private const string TAB_SIGN = "\t";
		private readonly DataFormat dataFormat;
		private readonly ConcurrentDependencyNode[] nodes;

		/// <summary>
		/// Creates a copy of a dependency graph
		/// </summary>
		/// <param name="graph"> a dependency graph </param>
		/// <exception cref="ConcurrentGraphException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ConcurrentDependencyGraph(ConcurrentDependencyGraph graph) throws ConcurrentGraphException
		public ConcurrentDependencyGraph(ConcurrentDependencyGraph graph)
		{
			dataFormat = graph.dataFormat;
			nodes = new ConcurrentDependencyNode[graph.nodes.Length + 1];

			for (int i = 0; i < graph.nodes.Length; i++)
			{
				nodes[i] = new ConcurrentDependencyNode(this, (ConcurrentDependencyNode)graph.nodes[i]);
			}
		}

		/// <summary>
		/// Creates a immutable dependency graph
		/// </summary>
		/// <param name="dataFormat"> a data format that describes the label types (or the columns in the input and output) </param>
		/// <param name="inputTokens"> a string array of tokens. Each label is separated by a tab-character and must follow the order in the data format </param>
		/// <exception cref="ConcurrentGraphException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ConcurrentDependencyGraph(org.maltparser.concurrent.graph.dataformat.DataFormat dataFormat, String[] inputTokens) throws ConcurrentGraphException
		public ConcurrentDependencyGraph(DataFormat dataFormat, string[] inputTokens)
		{
			this.dataFormat = dataFormat;
			nodes = new ConcurrentDependencyNode[inputTokens.Length + 1];

			// Add nodes
			nodes[0] = new ConcurrentDependencyNode(this, 0, null); // ROOT
			for (int i = 0; i < inputTokens.Length; i++)
			{
				string[] columns = inputTokens[i].Split(TAB_SIGN, true);
				nodes[i + 1] = new ConcurrentDependencyNode(this, i + 1, columns);
			}

			// Check graph
			for (int i = 0; i < nodes.Length; i++)
			{
				if (nodes[i].HeadIndex >= nodes.Length)
				{
					throw new ConcurrentGraphException("Not allowed to add a head node that doesn't exists");
				}
			}
		}

		/// <summary>
		/// Creates a immutable dependency graph
		/// </summary>
		/// <param name="dataFormat"> a data format that describes the label types (or the columns in the input and output) </param>
		/// <param name="sourceGraph"> a dependency graph that implements the interface org.maltparser.core.syntaxgraph.DependencyStructure </param>
		/// <param name="defaultRootLabel"> the default root label </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ConcurrentDependencyGraph(org.maltparser.concurrent.graph.dataformat.DataFormat dataFormat, org.maltparser.core.syntaxgraph.DependencyStructure sourceGraph, String defaultRootLabel) throws org.maltparser.core.exception.MaltChainedException
		public ConcurrentDependencyGraph(DataFormat dataFormat, IDependencyStructure sourceGraph, string defaultRootLabel)
		{
			this.dataFormat = dataFormat;
			nodes = new ConcurrentDependencyNode[sourceGraph.NDependencyNode()];

			// Add nodes
			nodes[0] = new ConcurrentDependencyNode(this, 0, null); // ROOT

			foreach (int index in sourceGraph.TokenIndices)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode gnode = sourceGraph.getDependencyNode(index);
				DependencyNode gnode = sourceGraph.GetDependencyNode(index);
				string[] columns = new string[dataFormat.numberOfColumns()];
				for (int i = 0; i < dataFormat.numberOfColumns(); i++)
				{
					ColumnDescription column = dataFormat.getColumnDescription(i);
					if (!column.Internal)
					{
						if (column.Category == ColumnDescription.Input)
						{
							columns[i] = gnode.getLabelSymbol(sourceGraph.SymbolTables.getSymbolTable(column.Name));
						}
						else if (column.Category == ColumnDescription.Head)
						{
							if (gnode.hasHead())
							{
								columns[i] = Convert.ToString(gnode.HeadEdge.Source.Index);
							}
							else
							{
								columns[i] = Convert.ToString(-1);
							}
						}
						else if (column.Category == ColumnDescription.DependencyEdgeLabel)
						{
							SymbolTable sourceTable = sourceGraph.SymbolTables.getSymbolTable(column.Name);
							if (gnode.HeadEdge.hasLabel(sourceTable))
							{
								columns[i] = gnode.HeadEdge.getLabelSymbol(sourceTable);
							}
							else
							{
								columns[i] = defaultRootLabel;
							}
						}
						else
						{
							columns[i] = "_";
						}
					}
				}
				nodes[index] = new ConcurrentDependencyNode(this, index, columns);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ConcurrentDependencyGraph(org.maltparser.concurrent.graph.dataformat.DataFormat dataFormat, ConcurrentDependencyNode[] inputNodes) throws ConcurrentGraphException
		protected internal ConcurrentDependencyGraph(DataFormat dataFormat, ConcurrentDependencyNode[] inputNodes)
		{
			this.dataFormat = dataFormat;
			nodes = new ConcurrentDependencyNode[inputNodes.Length];

			// Add nodes
			for (int i = 0; i < inputNodes.Length; i++)
			{
				nodes[i] = inputNodes[i];
			}

			// Check graph
			for (int i = 0; i < nodes.Length; i++)
			{
				if (nodes[i].HeadIndex >= nodes.Length)
				{
					throw new ConcurrentGraphException("Not allowed to add a head node that doesn't exists");
				}
			}
		}

		/// <summary>
		/// Returns the data format that describes the label types (or the columns in the input and output)
		/// </summary>
		/// <returns> the data format that describes the label types </returns>
		public DataFormat DataFormat
		{
			get
			{
				return dataFormat;
			}
		}

		/// <summary>
		/// Returns the root node
		/// </summary>
		/// <returns> the root node </returns>
		public ConcurrentDependencyNode Root
		{
			get
			{
				return nodes[0];
			}
		}

		/// <summary>
		/// Returns a dependency node specified by the node index
		/// </summary>
		/// <param name="nodeIndex"> the index of the node </param>
		/// <returns> a dependency node specified by the node index </returns>
	//	public ConcurrentDependencyNode getNode(int nodeIndex) {
	//		if (nodeIndex < 0 || nodeIndex >= nodes.length) {
	//			return null;
	//		}
	//		return nodes[nodeIndex];
	//	}

		/// <summary>
		/// Returns a dependency node specified by the node index. Index 0 equals the root node
		/// </summary>
		/// <param name="index"> the index of the node </param>
		/// <returns> a dependency node specified by the node index, if out of range <i>null</i> is returned. </returns>
		public ConcurrentDependencyNode getDependencyNode(int index)
		{
			if (index < 0 || index >= nodes.Length)
			{
				return null;
			}
			return nodes[index];
		}

		/// <summary>
		/// Returns a dependency node specified by the node index. If index is equals to 0 (the root node) then null will be returned because this node
		/// is not a token node.
		/// </summary>
		/// <param name="index"> the index of the node </param>
		/// <returns> a dependency node specified by the node index, if out of range and root node then <i>null</i> is returned. </returns>
		public ConcurrentDependencyNode getTokenNode(int index)
		{
			if (index <= 0 || index >= nodes.Length)
			{
				return null;
			}
			return nodes[index];
		}

		/// <summary>
		/// Returns the number of dependency nodes (including the root node)
		/// </summary>
		/// <returns> the number of dependency nodes </returns>
		public int nDependencyNodes()
		{
			return nodes.Length;
		}

		/// <summary>
		/// Returns the number of token nodes in the dependency graph (Number of dependency nodes - the root node).
		/// </summary>
		/// <returns> the number of token nodes in the dependency graph. </returns>
		public int nTokenNodes()
		{
			return nodes.Length - 1;
		}

		/// <summary>
		/// Returns the index of the last dependency node.
		/// </summary>
		/// <returns> the index of the last dependency node. </returns>
		public int HighestDependencyNodeIndex
		{
			get
			{
				return nodes.Length - 1;
			}
		}

		/// <summary>
		/// Returns the index of the last token node.
		/// </summary>
		/// <returns> the index of the last token node. If there are no token nodes then -1 is returned. </returns>
		public int HighestTokenIndex
		{
			get
			{
				if (nodes.Length == 1)
				{
					return - 1;
				}
				return nodes.Length - 1;
			}
		}


		/// <summary>
		///  Returns <i>true</i> if the dependency graph has any token nodes, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the dependency graph has any token nodes, otherwise <i>false</i>. </returns>
		public bool hasTokens()
		{
			return nodes.Length > 1;
		}

		/// <summary>
		/// Returns the number of edges
		/// </summary>
		/// <returns> the number of edges </returns>
		public int nEdges()
		{
			int n = 0;
			for (int i = 1; i < nodes.Length; i++)
			{
				if (nodes[i].hasHead())
				{
					n++;
				}
			}
			return n;
		}
		/// <summary>
		/// Returns a sorted set of edges. If no edges are found an empty set is returned
		/// </summary>
		/// <returns> a sorted set of edges. </returns>
		/// <exception cref="ConcurrentGraphException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.SortedSet<ConcurrentDependencyEdge> getEdges() throws ConcurrentGraphException
		public SortedSet<ConcurrentDependencyEdge> Edges
		{
			get
			{
				SortedSet<ConcurrentDependencyEdge> edges = Collections.SynchronizedSortedSet(new SortedSet<ConcurrentDependencyEdge>());
				for (int i = 1; i < nodes.Length; i++)
				{
					ConcurrentDependencyEdge edge = nodes[i].HeadEdge;
					if (edge != null)
					{
						edges.Add(edge);
					}
				}
				return edges;
			}
		}

		/// <summary>
		/// Returns a sorted set of integers {0,s,..n} , where each index i identifies a dependency node. Index 0
		/// should always be the root dependency node and index s is the first terminal node and index n is the
		/// last terminal node.  
		/// </summary>
		/// <returns> a sorted set of integers </returns>
		public SortedSet<int> DependencyIndices
		{
			get
			{
				SortedSet<int> indices = Collections.SynchronizedSortedSet(new SortedSet<int>());
				for (int i = 0; i < nodes.Length; i++)
				{
					indices.Add(i);
				}
				return indices;
			}
		}

		/// <summary>
		/// Returns a sorted set of integers {s,...,n}, where each index i identifies a token node. Index <i>s</i> 
		/// is the first token node and index <i>n</i> is the last token node. 
		/// </summary>
		/// <returns> a sorted set of integers {s,...,n}. If there are no token nodes then an empty set is returned. </returns>
		public SortedSet<int> TokenIndices
		{
			get
			{
				SortedSet<int> indices = Collections.SynchronizedSortedSet(new SortedSet<int>());
				for (int i = 1; i < nodes.Length; i++)
				{
					indices.Add(i);
				}
				return indices;
			}
		}

		/// <summary>
		/// Returns <i>true</i> if the head edge of the dependency node with <i>index</i> is labeled, otherwise <i>false</i>.
		/// </summary>
		/// <param name="index"> the index of the dependency node </param>
		/// <returns> <i>true</i> if the head edge of the dependency node with <i>index</i> is labeled, otherwise <i>false</i>. </returns>
		public bool hasLabeledDependency(int index)
		{
			if (index < 0 || index >= nodes.Length)
			{
				return false;
			}
			if (!nodes[index].hasHead())
			{
				return false;
			}
			return nodes[index].HeadLabeled;
		}

		/// <summary>
		/// Returns <i>true</i> if all nodes in the dependency structure are connected, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if all nodes in the dependency structure are connected, otherwise <i>false</i>. </returns>
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

		/// <summary>
		/// Returns <i>true</i> if all edges in the dependency structure are projective, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if all edges in the dependency structure are projective, otherwise <i>false</i>. </returns>
		public bool Projective
		{
			get
			{
				for (int i = 1; i < nodes.Length; i++)
				{
					if (!nodes[i].Projective)
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Returns <i>true</i> if all dependency nodes have at most one incoming edge, otherwise <i>false</i>.
		/// 
		/// Note: In this implementation this will always be <i>true</i>
		/// </summary>
		/// <returns>  <i>true</i> if all dependency nodes have at most one incoming edge, otherwise <i>false</i>. </returns>
		public bool SingleHeaded
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Returns <i>true</i> if the dependency structure are a tree (isConnected() && isSingleHeaded()), otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the dependency structure are a tree (isConnected() && isSingleHeaded()), otherwise <i>false</i>. </returns>
		public bool Tree
		{
			get
			{
				return Connected && SingleHeaded;
			}
		}

		/// <summary>
		/// Returns the number of non-projective edges in the dependency structure.
		/// </summary>
		/// <returns> the number of non-projective edges in the dependency structure. </returns>
		public int nNonProjectiveEdges()
		{
			int c = 0;
			for (int i = 1; i < nodes.Length; i++)
			{
				if (!nodes[i].Projective)
				{
					c++;
				}
			}
			return c;
		}

		protected internal bool hasDependent(int nodeIndex)
		{
			for (int i = 1; i < nodes.Length; i++)
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
			for (int i = nodeIndex + 1; i < nodes.Length; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					return true;
				}
			}
			return false;
		}

		protected internal IList<ConcurrentDependencyNode> getListOfLeftDependents(int nodeIndex)
		{
			IList<ConcurrentDependencyNode> leftDependents = Collections.synchronizedList(new List<ConcurrentDependencyNode>());
			for (int i = 1; i < nodeIndex; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					leftDependents.Add(nodes[i]);
				}
			}
			return leftDependents;
		}

		protected internal SortedSet<ConcurrentDependencyNode> getSortedSetOfLeftDependents(int nodeIndex)
		{
			SortedSet<ConcurrentDependencyNode> leftDependents = Collections.SynchronizedSortedSet(new SortedSet<ConcurrentDependencyNode>());
			for (int i = 1; i < nodeIndex; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					leftDependents.Add(nodes[i]);
				}
			}
			return leftDependents;
		}

		protected internal IList<ConcurrentDependencyNode> getListOfRightDependents(int nodeIndex)
		{
			IList<ConcurrentDependencyNode> rightDependents = Collections.synchronizedList(new List<ConcurrentDependencyNode>());
			for (int i = nodeIndex + 1; i < nodes.Length; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					rightDependents.Add(nodes[i]);
				}
			}
			return rightDependents;
		}

		protected internal SortedSet<ConcurrentDependencyNode> getSortedSetOfRightDependents(int nodeIndex)
		{
			SortedSet<ConcurrentDependencyNode> rightDependents = Collections.SynchronizedSortedSet(new SortedSet<ConcurrentDependencyNode>());
			for (int i = nodeIndex + 1; i < nodes.Length; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					rightDependents.Add(nodes[i]);
				}
			}
			return rightDependents;
		}

		protected internal IList<ConcurrentDependencyNode> getListOfDependents(int nodeIndex)
		{
			IList<ConcurrentDependencyNode> dependents = Collections.synchronizedList(new List<ConcurrentDependencyNode>());
			for (int i = 1; i < nodes.Length; i++)
			{
				if (nodeIndex == nodes[i].HeadIndex)
				{
					dependents.Add(nodes[i]);
				}
			}
			return dependents;
		}

		protected internal SortedSet<ConcurrentDependencyNode> getSortedSetOfDependents(int nodeIndex)
		{
			SortedSet<ConcurrentDependencyNode> dependents = Collections.SynchronizedSortedSet(new SortedSet<ConcurrentDependencyNode>());
			for (int i = 1; i < nodes.Length; i++)
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
			int[] components = new int[nodes.Length];
			int[] ranks = new int[nodes.Length];
			for (int i = 0; i < components.Length; i++)
			{
				components[i] = i;
				ranks[i] = 0;
			}
			for (int i = 1; i < nodes.Length; i++)
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

		protected internal ConcurrentDependencyNode findComponent(int nodeIndex)
		{
			int[] components = new int[nodes.Length];
			int[] ranks = new int[nodes.Length];
			for (int i = 0; i < components.Length; i++)
			{
				components[i] = i;
				ranks[i] = 0;
			}
			for (int i = 1; i < nodes.Length; i++)
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
			int[] components = new int[nodes.Length];
			int[] ranks = new int[nodes.Length];
			for (int i = 0; i < components.Length; i++)
			{
				components[i] = i;
				ranks[i] = 0;
			}
			for (int i = 1; i < nodes.Length; i++)
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

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((dataFormat == null) ? 0 : dataFormat.GetHashCode());
			result = prime * result + Arrays.HashCode(nodes);
			return result;
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
			ConcurrentDependencyGraph other = (ConcurrentDependencyGraph) obj;
			if (dataFormat == null)
			{
				if (other.dataFormat != null)
				{
					return false;
				}
			}
			else if (!dataFormat.Equals(other.dataFormat))
			{
				return false;
			}
			if (!Arrays.Equals(nodes, other.nodes))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			for (int i = 1; i < nodes.Length; i++)
			{
				sb.Append(nodes[i]);
				sb.Append('\n');
			}
			return sb.ToString();
		}
	}

}