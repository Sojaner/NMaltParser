using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConcurrencyFactory factory = new ConcurrencyFactory();

        private const string tabSign = "\t";

        private readonly ConcurrentDependencyNode[] nodes;

        /// <summary>
        /// Creates a copy of a dependency graph
        /// </summary>
        /// <param name="graph"> a dependency graph </param>
        /// <exception cref="ConcurrentGraphException"> </exception>
        public ConcurrentDependencyGraph(ConcurrentDependencyGraph graph)
        {
            DataFormat = graph.DataFormat;

            nodes = new ConcurrentDependencyNode[graph.nodes.Length + 1];

            for (int i = 0; i < graph.nodes.Length; i++)
            {
                nodes[i] = new ConcurrentDependencyNode(this, graph.nodes[i]);
            }
        }

        /// <summary>
        /// Creates a immutable dependency graph
        /// </summary>
        /// <param name="dataFormat"> a data format that describes the label types (or the columns in the input and output) </param>
        /// <param name="inputTokens"> a string array of tokens. Each label is separated by a tab-character and must follow the order in the data format </param>
        /// <exception cref="ConcurrentGraphException"> </exception>
        public ConcurrentDependencyGraph(DataFormat.DataFormat dataFormat, IReadOnlyList<string> inputTokens)
        {
            DataFormat = dataFormat;

            nodes = new ConcurrentDependencyNode[inputTokens.Count + 1];

            // Add nodes
            nodes[0] = new ConcurrentDependencyNode(this, 0, null); // ROOT

            for (int i = 0; i < inputTokens.Count; i++)
            {
                string[] columns = inputTokens[i].Split(tabSign, true);

                nodes[i + 1] = new ConcurrentDependencyNode(this, i + 1, columns);
            }

            // Check graph
            if (nodes.Any(t => t.HeadIndex >= nodes.Length))
            {
                throw new ConcurrentGraphException("Not allowed to add a head node that doesn't exists");
            }
        }

        /// <summary>
        /// Creates a immutable dependency graph
        /// </summary>
        /// <param name="dataFormat"> a data format that describes the label types (or the columns in the input and output) </param>
        /// <param name="sourceGraph"> a dependency graph that implements the interface org.maltparser.core.syntaxgraph.DependencyStructure </param>
        /// <param name="defaultRootLabel"> the default root label </param>
        /// <exception cref="MaltChainedException"> </exception>
        public ConcurrentDependencyGraph(DataFormat.DataFormat dataFormat, IDependencyStructure sourceGraph, string defaultRootLabel)
        {
            DataFormat = dataFormat;

            nodes = new ConcurrentDependencyNode[sourceGraph.NDependencyNode()];

            // Add nodes
            nodes[0] = new ConcurrentDependencyNode(this, 0, null); // ROOT

            foreach (int index in sourceGraph.TokenIndices)
            {
                DependencyNode gNode = sourceGraph.GetDependencyNode(index);

                string[] columns = new string[dataFormat.NumberOfColumns()];

                for (int i = 0; i < dataFormat.NumberOfColumns(); i++)
                {
                    ColumnDescription column = dataFormat.GetColumnDescription(i);

                    if (!column.Internal)
                    {
                        if (column.Category == ColumnDescription.Input)
                        {
                            columns[i] = gNode.getLabelSymbol(sourceGraph.SymbolTables.getSymbolTable(column.Name));
                        }
                        else if (column.Category == ColumnDescription.Head)
                        {
                            if (gNode.hasHead())
                            {
                                columns[i] = Convert.ToString(gNode.HeadEdge.Source.Index);
                            }
                            else
                            {
                                columns[i] = Convert.ToString(-1);
                            }
                        }
                        else if (column.Category == ColumnDescription.DependencyEdgeLabel)
                        {
                            SymbolTable sourceTable = sourceGraph.SymbolTables.getSymbolTable(column.Name);

                            if (gNode.HeadEdge.hasLabel(sourceTable))
                            {
                                columns[i] = gNode.HeadEdge.getLabelSymbol(sourceTable);
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

        internal ConcurrentDependencyGraph(DataFormat.DataFormat dataFormat, IReadOnlyList<ConcurrentDependencyNode> inputNodes)
        {
            DataFormat = dataFormat;

            nodes = new ConcurrentDependencyNode[inputNodes.Count];

            // Add nodes
            for (int i = 0; i < inputNodes.Count; i++)
            {
                nodes[i] = inputNodes[i];
            }

            // Check graph
            if (nodes.Any(t => t.HeadIndex >= nodes.Length))
            {
                throw new ConcurrentGraphException("Not allowed to add a head node that doesn't exists");
            }
        }

        /// <summary>
        /// Returns the data format that describes the label types (or the columns in the input and output)
        /// </summary>
        /// <returns> the data format that describes the label types </returns>
        public DataFormat.DataFormat DataFormat { get; }

        /// <summary>
		/// Returns the root node
		/// </summary>
		/// <returns> the root node </returns>
		public ConcurrentDependencyNode Root => nodes[0];

        /// <summary>
		/// Returns a dependency node specified by the node index. Index 0 equals the root node
		/// </summary>
		/// <param name="index"> the index of the node </param>
		/// <returns> a dependency node specified by the node index, if out of range <i>null</i> is returned. </returns>
		public ConcurrentDependencyNode GetDependencyNode(int index)
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
        public ConcurrentDependencyNode GetTokenNode(int index)
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
        public int NDependencyNodes()
        {
            return nodes.Length;
        }

        /// <summary>
        /// Returns the number of token nodes in the dependency graph (Number of dependency nodes - the root node).
        /// </summary>
        /// <returns> the number of token nodes in the dependency graph. </returns>
        public int NTokenNodes()
        {
            return nodes.Length - 1;
        }

        /// <summary>
        /// Returns the index of the last dependency node.
        /// </summary>
        /// <returns> the index of the last dependency node. </returns>
        public int HighestDependencyNodeIndex => nodes.Length - 1;

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
                    return -1;
                }

                return nodes.Length - 1;
            }
        }


        /// <summary>
        ///  Returns <i>true</i> if the dependency graph has any token nodes, otherwise <i>false</i>.
        /// </summary>
        /// <returns> <i>true</i> if the dependency graph has any token nodes, otherwise <i>false</i>. </returns>
        public bool HasTokens()
        {
            return nodes.Length > 1;
        }

        /// <summary>
        /// Returns the number of edges
        /// </summary>
        /// <returns> the number of edges </returns>
        public int NEdges()
        {
            int n = 0;

            for (int i = 1; i < nodes.Length; i++)
            {
                if (nodes[i].HasHead())
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
        public SortedSet<ConcurrentDependencyEdge> Edges
        {
            get
            {
                return factory.Get<SortedSet<ConcurrentDependencyEdge>>(() =>
                {
                    SortedSet<ConcurrentDependencyEdge> sortedSet = new SortedSet<ConcurrentDependencyEdge>();

                    for (int i = 1; i < nodes.Length; i++)
                    {
                        ConcurrentDependencyEdge edge = nodes[i].HeadEdge;

                        if (edge != null)
                        {
                            sortedSet.Add(edge);
                        }
                    }

                    return sortedSet;
                });
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
                return factory.Get<SortedSet<int>>(() =>
                {
                    SortedSet<int> sortedSet = new SortedSet<int>();

                    for (int i = 0; i < nodes.Length; i++)
                    {
                        sortedSet.Add(i);
                    }

                    return sortedSet;
                });
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
                return factory.Get<SortedSet<int>>(() =>
                {
                    SortedSet<int> sortedSet = new SortedSet<int>();

                    for (int i = 1; i < nodes.Length; i++)
                    {
                        sortedSet.Add(i);
                    }

                    return sortedSet;
                });
            }
        }

        /// <summary>
        /// Returns <i>true</i> if the head edge of the dependency node with <i>index</i> is labeled, otherwise <i>false</i>.
        /// </summary>
        /// <param name="index"> the index of the dependency node </param>
        /// <returns> <i>true</i> if the head edge of the dependency node with <i>index</i> is labeled, otherwise <i>false</i>. </returns>
        public bool HasLabeledDependency(int index)
        {
            if (index < 0 || index >= nodes.Length)
            {
                return false;
            }

            return nodes[index].HasHead() && nodes[index].HeadLabeled;
        }

        /// <summary>
        /// Returns <i>true</i> if all nodes in the dependency structure are connected, otherwise <i>false</i>.
        /// </summary>
        /// <returns> <i>true</i> if all nodes in the dependency structure are connected, otherwise <i>false</i>. </returns>
        public bool Connected
        {
            get
            {
                int[] components = FindComponents();

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
        public bool SingleHeaded => true;

        /// <summary>
        /// Returns <i>true</i> if the dependency structure are a tree (isConnected() && isSingleHeaded()), otherwise <i>false</i>.
        /// </summary>
        /// <returns> <i>true</i> if the dependency structure are a tree (isConnected() && isSingleHeaded()), otherwise <i>false</i>. </returns>
        public bool Tree => Connected && SingleHeaded;

        /// <summary>
        /// Returns the number of non-projective edges in the dependency structure.
        /// </summary>
        /// <returns> the number of non-projective edges in the dependency structure. </returns>
        public int NNonProjectiveEdges()
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

        internal bool HasDependent(int nodeIndex)
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

        internal bool HasLeftDependent(int nodeIndex)
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

        internal bool HasRightDependent(int nodeIndex)
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

        internal IList<ConcurrentDependencyNode> GetListOfLeftDependents(int nodeIndex)
        {
            return factory.Get<IList<ConcurrentDependencyNode>>(() =>
            {
                IList<ConcurrentDependencyNode> leftDependents = new List<ConcurrentDependencyNode>();

                for (int i = 1; i < nodeIndex; i++)
                {
                    if (nodeIndex == nodes[i].HeadIndex)
                    {
                        leftDependents.Add(nodes[i]);
                    }
                }

                return leftDependents;
            });
        }

        internal SortedSet<ConcurrentDependencyNode> GetSortedSetOfLeftDependents(int nodeIndex)
        {
            return factory.Get<SortedSet<ConcurrentDependencyNode>>(() => {

                SortedSet<ConcurrentDependencyNode> leftDependents = new SortedSet<ConcurrentDependencyNode>();

                for (int i = 1; i < nodeIndex; i++)
                {
                    if (nodeIndex == nodes[i].HeadIndex)
                    {
                        leftDependents.Add(nodes[i]);
                    }
                }

                return leftDependents;
            });
        }

        internal IList<ConcurrentDependencyNode> GetListOfRightDependents(int nodeIndex)
        {
            return factory.Get<IList<ConcurrentDependencyNode>>(() => {

                IList<ConcurrentDependencyNode> rightDependents = new List<ConcurrentDependencyNode>();

                for (int i = nodeIndex + 1; i < nodes.Length; i++)
                {
                    if (nodeIndex == nodes[i].HeadIndex)
                    {
                        rightDependents.Add(nodes[i]);
                    }
                }

                return rightDependents;
            });
        }

        internal SortedSet<ConcurrentDependencyNode> GetSortedSetOfRightDependents(int nodeIndex)
        {
            return factory.Get<SortedSet<ConcurrentDependencyNode>>(() => {

                SortedSet<ConcurrentDependencyNode> rightDependents = new SortedSet<ConcurrentDependencyNode>();

                for (int i = nodeIndex + 1; i < nodes.Length; i++)
                {
                    if (nodeIndex == nodes[i].HeadIndex)
                    {
                        rightDependents.Add(nodes[i]);
                    }
                }

                return rightDependents;
            });
        }

        internal IList<ConcurrentDependencyNode> GetListOfDependents(int nodeIndex)
        {
            return factory.Get<IList<ConcurrentDependencyNode>>(() => {

                IList<ConcurrentDependencyNode> dependents = new List<ConcurrentDependencyNode>();

                for (int i = 1; i < nodes.Length; i++)
                {
                    if (nodeIndex == nodes[i].HeadIndex)
                    {
                        dependents.Add(nodes[i]);
                    }
                }

                return dependents;
            });
        }

        internal SortedSet<ConcurrentDependencyNode> GetSortedSetOfDependents(int nodeIndex)
        {
            return factory.Get<SortedSet<ConcurrentDependencyNode>>(() => {

                SortedSet<ConcurrentDependencyNode> dependents = new SortedSet<ConcurrentDependencyNode>();

                for (int i = 1; i < nodes.Length; i++)
                {
                    if (nodeIndex == nodes[i].HeadIndex)
                    {
                        dependents.Add(nodes[i]);
                    }
                }

                return dependents;
            });
        }

        internal int GetRank(int nodeIndex)
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
                if (nodes[i].HasHead())
                {
                    int hcIndex = FindComponent(nodes[i].Head.Index, components);

                    int dcIndex = FindComponent(nodes[i].Index, components);

                    if (hcIndex != dcIndex)
                    {
                        Link(hcIndex, dcIndex, components, ranks);
                    }
                }
            }

            return ranks[nodeIndex];
        }

        internal ConcurrentDependencyNode FindComponent(int nodeIndex)
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
                if (nodes[i].HasHead())
                {
                    int hcIndex = FindComponent(nodes[i].Head.Index, components);

                    int dcIndex = FindComponent(nodes[i].Index, components);

                    if (hcIndex != dcIndex)
                    {
                        Link(hcIndex, dcIndex, components, ranks);
                    }
                }
            }

            return nodes[FindComponent(nodeIndex, components)];
        }

        private int[] FindComponents()
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
                if (nodes[i].HasHead())
                {
                    int hcIndex = FindComponent(nodes[i].Head.Index, components);

                    int dcIndex = FindComponent(nodes[i].Index, components);

                    if (hcIndex != dcIndex)
                    {
                        Link(hcIndex, dcIndex, components, ranks);
                    }
                }
            }

            return components;
        }

        private static int FindComponent(int xIndex, IList<int> components)
        {
            if (xIndex != components[xIndex])
            {
                components[xIndex] = FindComponent(components[xIndex], components);
            }

            return components[xIndex];
        }

        private static void Link(int xIndex, int yIndex, IList<int> components, IList<int> ranks)
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
            }
        }

        public override int GetHashCode()
        {
            const int prime = 31;

            int result = 1;

            result = prime * result + ((DataFormat == null) ? 0 : DataFormat.GetHashCode());

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

            ConcurrentDependencyGraph other = (ConcurrentDependencyGraph)obj;

            if (DataFormat == null)
            {
                if (other.DataFormat != null)
                {
                    return false;
                }
            }
            else if (!DataFormat.Equals(other.DataFormat))
            {
                return false;
            }

            return Arrays.Equals(nodes, other.nodes);
        }

        public override string ToString()
        {
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