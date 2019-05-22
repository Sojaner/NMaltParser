using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Concurrent.Graph.DataFormat;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Helper;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.LW.Graph
{
    /// <summary>
	/// A lightweight version of org.maltparser.core.syntaxgraph.node.{Token,Root}
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class LWNode : DependencyNode, Node
	{
		private readonly LWDependencyGraph graph;
		private int index;
	//	private final SortedMap<Integer, String> labels;
		private readonly IDictionary<int, string> labels;
		private Edge headEdge;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected LWNode(LWNode node) throws LWGraphException
		protected internal LWNode(LWNode node) : this(node.graph, node)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected LWNode(LWDependencyGraph _graph, LWNode node) throws LWGraphException
		protected internal LWNode(LWDependencyGraph _graph, LWNode node)
		{
			if (_graph == null)
			{
				throw new LWGraphException("The graph node must belong to a dependency graph.");
			}
			graph = _graph;
			index = node.index;
	//		this.labels = new TreeMap<Integer, String>(node.labels);
			labels = new HashMap<int, string>(node.labels);
			headEdge = node.headEdge;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected LWNode(LWDependencyGraph _graph, int _index) throws LWGraphException
		protected internal LWNode(LWDependencyGraph _graph, int _index)
		{
			if (_graph == null)
			{
				throw new LWGraphException("The graph node must belong to a dependency graph.");
			}
			if (_index < 0)
			{
				throw new LWGraphException("Not allowed to have negative node index");
			}
			graph = _graph;
			index = _index;
	//		this.labels = new TreeMap<Integer, String>();
			labels = new HashMap<int, string>();
			headEdge = null;
		}

	//	public void setHeadIndex(int _headIndex) throws LWGraphException {
	//		if (this.index == 0 && _headIndex != -1) {
	//			throw new LWGraphException("Not allowed to add head to a root node.");
	//		}
	//		if (this.index == _headIndex) {
	//			throw new LWGraphException("Not allowed to add head to itself");
	//		}
	//		this.headIndex = _headIndex;
	//	}
	//	
	//	public void removeHeadIndex() throws LWGraphException {
	//		this.headIndex = -1;
	//		for (Integer i : labels.keySet()) {
	//			if (graph.getDataFormat().getColumnDescription(i).getCategory() == ColumnDescription.DEPENDENCY_EDGE_LABEL) {
	//				this.labels.remove(i);
	//			}
	//		}
	//	}

		protected internal DependencyStructure Graph
		{
			get
			{
				return graph;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}


		public string getLabel(int columnPosition)
		{
			if (labels.ContainsKey(columnPosition))
			{
				return labels[columnPosition];
			}
			else if (graph.DataFormat.getColumnDescription(columnPosition).Category == ColumnDescription.IGNORE)
			{
				return graph.DataFormat.getColumnDescription(columnPosition).DefaultOutput;
			}
			return "";
		}

		public string getLabel(string columnName)
		{
			ColumnDescription column = graph.DataFormat.getColumnDescription(columnName);
			if (column != null)
			{
				return getLabel(column.Position);
			}
			return "";
		}

		public string getLabel(ColumnDescription column)
		{
			return getLabel(column.Position);
		}

		public bool hasLabel(int columnPosition)
		{
			return labels.ContainsKey(columnPosition);
		}

		public bool hasLabel(string columnName)
		{
			ColumnDescription column = graph.DataFormat.getColumnDescription(columnName);
			if (column != null)
			{
				return hasLabel(column.Position);
			}
			return false;
		}

		public bool hasLabel(ColumnDescription column)
		{
			return labels.ContainsKey(column.Position);
		}

		public bool Labeled
		{
			get
			{
				foreach (int? key in labels.Keys)
				{
					if (graph.DataFormat.getColumnDescription(key).Category == ColumnDescription.INPUT)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool HeadLabeled
		{
			get
			{
				if (headEdge == null)
				{
					return false;
				}
				return headEdge.Labeled;
			}
		}

		public int HeadIndex
		{
			get
			{
				if (headEdge == null)
				{
					return -1;
				}
				return headEdge.Source.Index;
			}
		}

		public SortedDictionary<ColumnDescription, string> Labels
		{
			get
			{
				SortedDictionary<ColumnDescription, string> nodeLabels = Collections.synchronizedSortedMap(new SortedDictionary<ColumnDescription, string>());
				foreach (int? key in labels.Keys)
				{
					nodeLabels[graph.DataFormat.getColumnDescription(key)] = labels[key];
				}
				return nodeLabels;
			}
		}

	//	public SortedMap<ColumnDescription, String> getEdgeLabels() {
	//		SortedMap<ColumnDescription, String> edgeLabels = Collections.synchronizedSortedMap(new TreeMap<ColumnDescription, String>());
	//		for (Integer key : labels.keySet()) {
	//			if (graph.getDataFormat().getColumnDescription(key).getCategory() == ColumnDescription.DEPENDENCY_EDGE_LABEL) {
	//				edgeLabels.put(graph.getDataFormat().getColumnDescription(key), labels.get(key));
	//			}
	//		}
	//		return edgeLabels;
	//	}

		public DependencyNode Predecessor
		{
			get
			{
				return index > 1 ? graph.getNode(index - 1) : null;
			}
		}

		public DependencyNode Successor
		{
			get
			{
				return graph.getNode(index + 1);
			}
		}

		public bool Root
		{
			get
			{
				return index == 0;
			}
		}

		public bool hasAtMostOneHead()
		{
			return true;
		}

		public bool hasHead()
		{
			return headEdge != null;
		}

		public bool hasDependent()
		{
			return graph.hasDependent(index);
		}

		public bool hasLeftDependent()
		{
			return graph.hasLeftDependent(index);
		}

		public bool hasRightDependent()
		{
			return graph.hasRightDependent(index);
		}

		public SortedSet<DependencyNode> Heads
		{
			get
			{
				SortedSet<DependencyNode> heads = Collections.synchronizedSortedSet(new SortedSet<DependencyNode>());
				DependencyNode head = Head;
				if (head != null)
				{
					heads.Add(head);
				}
				return heads;
			}
		}

		public DependencyNode Head
		{
			get
			{
				if (headEdge == null)
				{
					return null;
				}
				return graph.getNode(HeadIndex);
			}
		}

		public DependencyNode getLeftDependent(int leftDependentIndex)
		{
			IList<DependencyNode> leftDependents = graph.getListOfLeftDependents(index);
			if (leftDependentIndex >= 0 && leftDependentIndex < leftDependents.Count)
			{
				return leftDependents[leftDependentIndex];
			}
			return null;
		}

		public int LeftDependentCount
		{
			get
			{
				return graph.getListOfLeftDependents(index).Count;
			}
		}

		public SortedSet<DependencyNode> LeftDependents
		{
			get
			{
				return graph.getSortedSetOfLeftDependents(index);
			}
		}

		public IList<DependencyNode> ListOfLeftDependents
		{
			get
			{
				return graph.getListOfLeftDependents(index);
			}
		}

		public DependencyNode LeftSibling
		{
			get
			{
				if (headEdge == null)
				{
					return null;
				}
    
				int nodeDepedentPosition = 0;
				IList<DependencyNode> headDependents = Head.ListOfDependents;
				for (int i = 0; i < headDependents.Count; i++)
				{
					if (headDependents[i].Index == index)
					{
						nodeDepedentPosition = i;
						break;
					}
				}
    
				return (nodeDepedentPosition > 0) ? headDependents[nodeDepedentPosition - 1] : null;
			}
		}

		public DependencyNode SameSideLeftSibling
		{
			get
			{
				if (headEdge == null)
				{
					return null;
				}
    
				IList<DependencyNode> headDependents;
				if (index < HeadIndex)
				{
					headDependents = Head.ListOfLeftDependents;
				}
				else
				{ //(index > headIndex)
					headDependents = Head.ListOfRightDependents;
				}
				int nodeDepedentPosition = 0;
				for (int i = 0; i < headDependents.Count; i++)
				{
					if (headDependents[i].Index == index)
					{
						nodeDepedentPosition = i;
						break;
					}
				}
				return (nodeDepedentPosition > 0) ? headDependents[nodeDepedentPosition - 1] : null;
			}
		}

		public DependencyNode ClosestLeftDependent
		{
			get
			{
				IList<DependencyNode> leftDependents = graph.getListOfLeftDependents(index);
				return (leftDependents.Count > 0) ? leftDependents[leftDependents.Count - 1] : null;
			}
		}

		public DependencyNode LeftmostDependent
		{
			get
			{
				IList<DependencyNode> leftDependents = graph.getListOfLeftDependents(index);
				return (leftDependents.Count > 0) ? leftDependents[0] : null;
			}
		}

		public DependencyNode getRightDependent(int rightDependentIndex)
		{
			IList<DependencyNode> rightDependents = graph.getListOfRightDependents(index);
			if (rightDependentIndex >= 0 && rightDependentIndex < rightDependents.Count)
			{
				return rightDependents[rightDependents.Count - 1 - rightDependentIndex];
			}
			return null;
		}

		public int RightDependentCount
		{
			get
			{
				return graph.getListOfRightDependents(index).Count;
			}
		}

		public SortedSet<DependencyNode> RightDependents
		{
			get
			{
				return graph.getSortedSetOfRightDependents(index);
			}
		}

		public IList<DependencyNode> ListOfRightDependents
		{
			get
			{
				return graph.getListOfRightDependents(index);
			}
		}

		public DependencyNode RightSibling
		{
			get
			{
				if (headEdge == null)
				{
					return null;
				}
    
				IList<DependencyNode> headDependents = Head.ListOfDependents;
				int nodeDepedentPosition = headDependents.Count - 1;
				for (int i = headDependents.Count - 1; i >= 0 ; i--)
				{
					if (headDependents[i].Index == index)
					{
						nodeDepedentPosition = i;
						break;
					}
				}
    
				return (nodeDepedentPosition < headDependents.Count - 1) ? headDependents[nodeDepedentPosition + 1] : null;
			}
		}

		public DependencyNode SameSideRightSibling
		{
			get
			{
				if (headEdge == null)
				{
					return null;
				}
    
				IList<DependencyNode> headDependents;
				if (index < HeadIndex)
				{
					headDependents = Head.ListOfLeftDependents;
				}
				else
				{
					headDependents = Head.ListOfRightDependents;
				}
				int nodeDepedentPosition = headDependents.Count - 1;
				for (int i = headDependents.Count - 1; i >= 0 ; i--)
				{
					if (headDependents[i].Index == index)
					{
						nodeDepedentPosition = i;
						break;
					}
				}
    
				return (nodeDepedentPosition < headDependents.Count - 1) ? headDependents[nodeDepedentPosition + 1] : null;
			}
		}

		public DependencyNode ClosestRightDependent
		{
			get
			{
				IList<DependencyNode> rightDependents = graph.getListOfRightDependents(index);
				return (rightDependents.Count > 0) ? rightDependents[0] : null;
			}
		}

		public DependencyNode RightmostDependent
		{
			get
			{
				IList<DependencyNode> rightDependents = graph.getListOfRightDependents(index);
				return (rightDependents.Count > 0) ? rightDependents[rightDependents.Count - 1] : null;
			}
		}

		public SortedSet<DependencyNode> Dependents
		{
			get
			{
				return graph.getSortedSetOfDependents(index);
			}
		}

		public IList<DependencyNode> ListOfDependents
		{
			get
			{
				return graph.getListOfDependents(index);
			}
		}

		public int InDegree
		{
			get
			{
				if (hasHead())
				{
					return 1;
				}
				return 0;
			}
		}

		public int OutDegree
		{
			get
			{
				return graph.getListOfDependents(index).Count;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getAncestor() throws org.maltparser.core.exception.MaltChainedException
		public DependencyNode Ancestor
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
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.DependencyNode getProperAncestor() throws org.maltparser.core.exception.MaltChainedException
		public DependencyNode ProperAncestor
		{
			get
			{
				if (!hasHead())
				{
					return null;
				}
    
				DependencyNode tmp = this;
				while (tmp.hasHead() && !tmp.Root)
				{
					tmp = tmp.Head;
				}
				return tmp;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasAncestorInside(int left, int right) throws org.maltparser.core.exception.MaltChainedException
		public bool hasAncestorInside(int left, int right)
		{
			if (index == 0)
			{
				return false;
			}
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
//ORIGINAL LINE: public boolean isProjective() throws org.maltparser.core.exception.MaltChainedException
		public bool Projective
		{
			get
			{
				int headIndex = HeadIndex;
				if (headIndex > 0)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode head = getHead();
					DependencyNode head = Head;
					if (headIndex < index)
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
		public int DependencyNodeDepth
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

		public int CompareToIndex
		{
			get
			{
				return index;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.node.ComparableNode getLeftmostProperDescendant() throws org.maltparser.core.exception.MaltChainedException
		public ComparableNode LeftmostProperDescendant
		{
			get
			{
				ComparableNode candidate = null;
				IList<DependencyNode> dependents = graph.getListOfDependents(index);
				for (int i = 0; i < dependents.Count; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode dep = dependents.get(i);
					DependencyNode dep = dependents[i];
					if (candidate == null || dep.Index < candidate.Index)
					{
						candidate = dep;
					}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.ComparableNode tmp = dep.getLeftmostProperDescendant();
					ComparableNode tmp = dep.LeftmostProperDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null || tmp.Index < candidate.Index)
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
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.node.ComparableNode getRightmostProperDescendant() throws org.maltparser.core.exception.MaltChainedException
		public ComparableNode RightmostProperDescendant
		{
			get
			{
				ComparableNode candidate = null;
				IList<DependencyNode> dependents = graph.getListOfDependents(index);
				for (int i = 0; i < dependents.Count; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode dep = dependents.get(i);
					DependencyNode dep = dependents[i];
					if (candidate == null || dep.Index > candidate.Index)
					{
						candidate = dep;
					}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.ComparableNode tmp = dep.getRightmostProperDescendant();
					ComparableNode tmp = dep.RightmostProperDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (candidate == null || tmp.Index > candidate.Index)
					{
						candidate = tmp;
					}
				}
				return candidate;
			}
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int getLeftmostProperDescendantIndex() throws org.maltparser.core.exception.MaltChainedException
		public int LeftmostProperDescendantIndex
		{
			get
			{
				ComparableNode node = LeftmostProperDescendant;
				return (node != null)?node.Index:-1;
			}
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int getRightmostProperDescendantIndex() throws org.maltparser.core.exception.MaltChainedException
		public int RightmostProperDescendantIndex
		{
			get
			{
				ComparableNode node = RightmostProperDescendant;
				return (node != null)?node.Index:-1;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.node.ComparableNode getLeftmostDescendant() throws org.maltparser.core.exception.MaltChainedException
		public ComparableNode LeftmostDescendant
		{
			get
			{
				ComparableNode candidate = this;
				IList<DependencyNode> dependents = graph.getListOfDependents(index);
				for (int i = 0; i < dependents.Count; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode dep = dependents.get(i);
					DependencyNode dep = dependents[i];
					if (dep.Index < candidate.Index)
					{
						candidate = dep;
					}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.ComparableNode tmp = dep.getLeftmostDescendant();
					ComparableNode tmp = dep.LeftmostDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (tmp.Index < candidate.Index)
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
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.node.ComparableNode getRightmostDescendant() throws org.maltparser.core.exception.MaltChainedException
		public ComparableNode RightmostDescendant
		{
			get
			{
				ComparableNode candidate = this;
				IList<DependencyNode> dependents = graph.getListOfDependents(index);
				for (int i = 0; i < dependents.Count; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode dep = dependents.get(i);
					DependencyNode dep = dependents[i];
					if (dep.Index > candidate.Index)
					{
						candidate = dep;
					}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.ComparableNode tmp = dep.getRightmostDescendant();
					ComparableNode tmp = dep.RightmostDescendant;
					if (tmp == null)
					{
						continue;
					}
					if (tmp.Index > candidate.Index)
					{
						candidate = tmp;
					}
				}
				return candidate;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int getLeftmostDescendantIndex() throws org.maltparser.core.exception.MaltChainedException
		public int LeftmostDescendantIndex
		{
			get
			{
				ComparableNode node = LeftmostDescendant;
				return (node != null)?node.Index:Index;
			}
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int getRightmostDescendantIndex() throws org.maltparser.core.exception.MaltChainedException
		public int RightmostDescendantIndex
		{
			get
			{
				ComparableNode node = RightmostDescendant;
				return (node != null)?node.Index:Index;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.SortedSet<org.maltparser.core.syntaxgraph.edge.Edge> getIncomingSecondaryEdges() throws org.maltparser.core.exception.MaltChainedException
		public SortedSet<Edge> IncomingSecondaryEdges
		{
			get
			{
				throw new LWGraphException("Not implemented in the light-weight dependency graph package");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.SortedSet<org.maltparser.core.syntaxgraph.edge.Edge> getOutgoingSecondaryEdges() throws org.maltparser.core.exception.MaltChainedException
		public SortedSet<Edge> OutgoingSecondaryEdges
		{
			get
			{
				throw new LWGraphException("Not implemented in the light-weight dependency graph package");
			}
		}

		public ISet<Edge> HeadEdges
		{
			get
			{
				SortedSet<Edge> edges = Collections.synchronizedSortedSet(new SortedSet<Edge>());
				if (hasHead())
				{
					edges.Add(headEdge);
				}
				return edges;
			}
		}

		public Edge HeadEdge
		{
			get
			{
				if (!hasHead())
				{
					return null;
				}
				return headEdge;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table, String symbol) throws org.maltparser.core.exception.MaltChainedException
		public void addHeadEdgeLabel(SymbolTable table, string symbol)
		{
			if (headEdge != null)
			{
				headEdge.addLabel(table, symbol);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table, int code) throws org.maltparser.core.exception.MaltChainedException
		public void addHeadEdgeLabel(SymbolTable table, int code)
		{
			if (headEdge != null)
			{
				headEdge.addLabel(table, code);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addHeadEdgeLabel(org.maltparser.core.syntaxgraph.LabelSet labelSet) throws org.maltparser.core.exception.MaltChainedException
		public void addHeadEdgeLabel(LabelSet labelSet)
		{
			if (headEdge != null)
			{
				headEdge.addLabel(labelSet);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public boolean hasHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public bool hasHeadEdgeLabel(SymbolTable table)
		{
			if (headEdge != null)
			{
				return headEdge.hasLabel(table);
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public String getHeadEdgeLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public string getHeadEdgeLabelSymbol(SymbolTable table)
		{
			if (headEdge != null)
			{
				return headEdge.getLabelSymbol(table);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int getHeadEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public int getHeadEdgeLabelCode(SymbolTable table)
		{
			if (headEdge != null)
			{
				return headEdge.getLabelCode(table);
			}
			return 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.util.Set<org.maltparser.core.symbol.SymbolTable> getHeadEdgeLabelTypes() throws org.maltparser.core.exception.MaltChainedException
		public ISet<SymbolTable> HeadEdgeLabelTypes
		{
			get
			{
				if (headEdge != null)
				{
					return headEdge.LabelTypes;
				}
				return new System.Collections.Generic.HashSet<SymbolTable>();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.maltparser.core.syntaxgraph.LabelSet getHeadEdgeLabelSet() throws org.maltparser.core.exception.MaltChainedException
		public LabelSet HeadEdgeLabelSet
		{
			get
			{
				if (headEdge != null)
				{
					return headEdge.LabelSet;
				}
				return new LabelSet();
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException
		public void addIncomingEdge(Edge @in)
		{
			headEdge = @in;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public void addOutgoingEdge(Edge @out)
		{
			throw new LWGraphException("Not implemented in the light-weight dependency graph package");

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void removeIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException
		public void removeIncomingEdge(Edge @in)
		{
			if (headEdge.Equals(@in))
			{
				headEdge = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void removeOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public void removeOutgoingEdge(Edge @out)
		{
			throw new LWGraphException("Not implemented in the light-weight dependency graph package");
		}

		public IEnumerator<Edge> IncomingEdgeIterator
		{
			get
			{
				return HeadEdges.GetEnumerator();
			}
		}

		public IEnumerator<Edge> OutgoingEdgeIterator
		{
			get
			{
				IList<DependencyNode> dependents = ListOfDependents;
				IList<Edge> outEdges = new List<Edge>(dependents.Count);
				for (int i = 0; i < dependents.Count; i++)
				{
					try
					{
						outEdges.Add(dependents[i].HeadEdge);
					}
					catch (MaltChainedException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
				return outEdges.GetEnumerator();
			}
		}

		public int Rank
		{
			set
			{
			}
			get
			{
				return graph.getRank(index);
			}
		}
		public DependencyNode Component
		{
			get
			{
				return null;
			}
			set
			{
			}
		}


		public DependencyNode findComponent()
		{
			return graph.findComponent(index);
		}


		public bool HeadEdgeLabeled
		{
			get
			{
				if (headEdge != null)
				{
					return headEdge.Labeled;
				}
				return false;
			}
		}

		public int nHeadEdgeLabels()
		{
			if (headEdge != null)
			{
				return headEdge.nLabels();
			}
			return 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addColumnLabels(String[] columnLabels) throws org.maltparser.core.exception.MaltChainedException
		public void addColumnLabels(string[] columnLabels)
		{
			addColumnLabels(columnLabels, true);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addColumnLabels(String[] columnLabels, boolean addEdges) throws org.maltparser.core.exception.MaltChainedException
		public void addColumnLabels(string[] columnLabels, bool addEdges)
		{
			if (addEdges == true)
			{
				SortedDictionary<ColumnDescription, string> edgeLabels = new SortedDictionary<ColumnDescription, string>();
				int tmpHeadIndex = -1;
				if (columnLabels != null)
				{
					for (int i = 0; i < columnLabels.Length; i++)
					{
						ColumnDescription column = graph.DataFormat.getColumnDescription(i);
						if (column.Category == ColumnDescription.HEAD)
						{
							tmpHeadIndex = int.Parse(columnLabels[i]);
						}
						else if (column.Category == ColumnDescription.INPUT)
						{
							addLabel(graph.SymbolTables.addSymbolTable(column.Name), columnLabels[i]);
						}
						else if (column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
						{
							edgeLabels[column] = columnLabels[i];
						}
					}
				}
				if (tmpHeadIndex == -1)
				{
					headEdge = null;
				}
				else
				{
					if (tmpHeadIndex < -1)
					{
						throw new LWGraphException("Not allowed to have head index less than -1.");
					}
					if (index == 0 && tmpHeadIndex != -1)
					{
						throw new LWGraphException("Not allowed to add head to a root node.");
					}
					if (index == tmpHeadIndex)
					{
						throw new LWGraphException("Not allowed to add head to itself");
					}
					headEdge = new LWEdge(graph.getNode(tmpHeadIndex), this, edgeLabels);
				}
			}
			else
			{
				if (columnLabels != null)
				{
					for (int i = 0; i < columnLabels.Length; i++)
					{
						ColumnDescription column = graph.DataFormat.getColumnDescription(i);
						if (column.Category == ColumnDescription.INPUT)
						{
							addLabel(graph.SymbolTables.addSymbolTable(column.Name), columnLabels[i]);
						}
					}
				}
				headEdge = null;
			}
		}

		/// <summary>
		/// Adds a label (a string value) to the symbol table and to the graph element. 
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <param name="symbol"> a label symbol </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.symbol.SymbolTable table, String symbol) throws org.maltparser.core.exception.MaltChainedException
		public void addLabel(SymbolTable table, string symbol)
		{
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			table.addSymbol(symbol);
			labels[column.Position] = symbol;
		}

		/// <summary>
		/// Adds a label (an integer value) to the symbol table and to the graph element.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <param name="code"> a label code </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.symbol.SymbolTable table, int code) throws org.maltparser.core.exception.MaltChainedException
		public void addLabel(SymbolTable table, int code)
		{
			addLabel(table, table.getSymbolCodeToString(code));
		}

		/// <summary>
		/// Adds the labels of the label set to the label set of the graph element.
		/// </summary>
		/// <param name="labels"> a label set. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.syntaxgraph.LabelSet labels) throws org.maltparser.core.exception.MaltChainedException
		public void addLabel(LabelSet labels)
		{
			foreach (SymbolTable table in labels.Keys)
			{
				addLabel(table, labels.get(table));
			}
		}

		/// <summary>
		/// Returns <i>true</i> if the graph element has a label for the symbol table, otherwise <i>false</i>.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <returns> <i>true</i> if the graph element has a label for the symbol table, otherwise <i>false</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public bool hasLabel(SymbolTable table)
		{
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			return labels.ContainsKey(column.Position);
		}

		/// <summary>
		/// Returns the label symbol(a string representation) of the symbol table if it exists, otherwise 
		/// an exception is thrown.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <returns> the label (a string representation) of the symbol table if it exists. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public string getLabelSymbol(SymbolTable table)
		{
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			return labels[column.Position];
		}

		/// <summary>
		/// Returns the label code (an integer representation) of the symbol table if it exists, otherwise 
		/// an exception is thrown.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <returns> the label code (an integer representation) of the symbol table if it exists </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public int getLabelCode(SymbolTable table)
		{
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			return table.getSymbolStringToCode(labels[column.Position]);
		}

		/// <summary>
		/// Returns the number of labels of the graph element.
		/// </summary>
		/// <returns> the number of labels of the graph element. </returns>
		public int nLabels()
		{
			return labels.Count;
		}

		/// <summary>
		/// Returns a set of symbol tables (labeling functions or label types) that labels the graph element.
		/// </summary>
		/// <returns> a set of symbol tables (labeling functions or label types) </returns>
		public ISet<SymbolTable> LabelTypes
		{
			get
			{
				ISet<SymbolTable> labelTypes = new System.Collections.Generic.HashSet<SymbolTable>();
				SymbolTableHandler symbolTableHandler = BelongsToGraph.SymbolTables;
				SortedSet<ColumnDescription> selectedColumns = graph.DataFormat.getSelectedColumnDescriptions(labels.Keys);
				foreach (ColumnDescription column in selectedColumns)
				{
					try
					{
						labelTypes.Add(symbolTableHandler.getSymbolTable(column.Name));
					}
					catch (MaltChainedException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
				return labelTypes;
			}
		}

		/// <summary>
		/// Returns the label set.
		/// </summary>
		/// <returns> the label set. </returns>
		public LabelSet LabelSet
		{
			get
			{
				SymbolTableHandler symbolTableHandler = BelongsToGraph.SymbolTables;
				LabelSet labelSet = new LabelSet();
				SortedSet<ColumnDescription> selectedColumns = graph.DataFormat.getSelectedColumnDescriptions(labels.Keys);
				foreach (ColumnDescription column in selectedColumns)
				{
					try
					{
						SymbolTable table = symbolTableHandler.getSymbolTable(column.Name);
						int code = table.getSymbolStringToCode(labels[column.Position]);
						labelSet.put(table, code);
					}
					catch (MaltChainedException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
				return labelSet;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public void removeLabel(SymbolTable table)
		{
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			labels.Remove(column.Position);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeLabels() throws org.maltparser.core.exception.MaltChainedException
		public void removeLabels()
		{
			labels.Clear();
		}

		/// <summary>
		/// Returns the graph (structure) in which the graph element belongs to. 
		/// </summary>
		/// <returns> the graph (structure) in which the graph element belongs to.  </returns>
		public LabeledStructure BelongsToGraph
		{
			get
			{
				return graph;
			}
			set
			{
			}
		}



		/// <summary>
		/// Resets the graph element.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public void clear()
		{
			labels.Clear();
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
			if (index < that.Index)
			{
				return BEFORE;
			}
			if (index > that.Index)
			{
				return AFTER;
			}
			return EQUAL;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((headEdge == null) ? 0 : headEdge.GetHashCode());
			result = prime * result + index;
			result = prime * result + ((labels == null) ? 0 : labels.GetHashCode());
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
			LWNode other = (LWNode) obj;
			if (headEdge == null)
			{
				if (other.headEdge != null)
				{
					return false;
				}
			}
			else if (!headEdge.Equals(other.headEdge))
			{
				return false;
			}
			if (index != other.index)
			{
				return false;
			}
			if (labels == null)
			{
				if (other.labels != null)
				{
					return false;
				}
			}
			else if (!labels.Equals(other.labels))
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
			for (int i = 0; i < graph.DataFormat.numberOfColumns(); i++)
			{
				ColumnDescription column = graph.DataFormat.getColumnDescription(i);
				if (!column.Internal)
				{
					if (column.Category == ColumnDescription.HEAD)
					{
						sb.Append(HeadIndex);
					}
					else if (column.Category == ColumnDescription.INPUT)
					{
						sb.Append(labels[column.Position]);
					}
					else if (column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
					{
						if (headEdge != null)
						{
							sb.Append(((LWEdge)headEdge).getLabel(column));
						}
						else
						{
							sb.Append(column.DefaultOutput);
						}
					}
					else if (column.Category == ColumnDescription.IGNORE)
					{
						sb.Append(column.DefaultOutput);
					}
					sb.Append('\t');
				}
			}
			sb.Length = (sb.Length > 0)?sb.Length - 1:0;
			return sb.ToString();
		}
	}

}