using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.concurrent.graph
{

	using  org.maltparser.concurrent.graph.dataformat;
	using  org.maltparser.concurrent.graph.dataformat;

	/// <summary>
	/// Immutable and tread-safe dependency node implementation.
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class ConcurrentDependencyNode : IComparable<ConcurrentDependencyNode>
	{
		private readonly ConcurrentDependencyGraph graph;
		private readonly int index;
		private readonly SortedDictionary<int, string> labels;
		private readonly int headIndex;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ConcurrentDependencyNode(ConcurrentDependencyNode node) throws ConcurrentGraphException
		protected internal ConcurrentDependencyNode(ConcurrentDependencyNode node) : this(node.graph, node)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ConcurrentDependencyNode(ConcurrentDependencyGraph _graph, ConcurrentDependencyNode node) throws ConcurrentGraphException
		protected internal ConcurrentDependencyNode(ConcurrentDependencyGraph _graph, ConcurrentDependencyNode node)
		{
			if (_graph == null)
			{
				throw new ConcurrentGraphException("The graph node must belong to a dependency graph.");
			}
			this.graph = _graph;
			this.index = node.index;
			this.labels = new SortedDictionary<int, string>(node.labels);
			this.headIndex = node.headIndex;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ConcurrentDependencyNode(ConcurrentDependencyGraph _graph, int _index, java.util.SortedMap<int, String> _labels, int _headIndex) throws ConcurrentGraphException
		protected internal ConcurrentDependencyNode(ConcurrentDependencyGraph _graph, int _index, SortedDictionary<int, string> _labels, int _headIndex)
		{
			if (_graph == null)
			{
				throw new ConcurrentGraphException("The graph node must belong to a dependency graph.");
			}
			if (_index < 0)
			{
				throw new ConcurrentGraphException("Not allowed to have negative node index");
			}
			if (_headIndex < -1)
			{
				throw new ConcurrentGraphException("Not allowed to have head index less than -1.");
			}
			if (_index == 0 && _headIndex != -1)
			{
				throw new ConcurrentGraphException("Not allowed to add head to a root node.");
			}
			if (_index == _headIndex)
			{
				throw new ConcurrentGraphException("Not allowed to add head to itself");
			}
			this.graph = _graph;
			this.index = _index;
			this.labels = new SortedDictionary<int, string>(_labels);
			this.headIndex = _headIndex;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ConcurrentDependencyNode(ConcurrentDependencyGraph _graph, int _index, String[] _labels) throws ConcurrentGraphException
		protected internal ConcurrentDependencyNode(ConcurrentDependencyGraph _graph, int _index, string[] _labels)
		{
			if (_graph == null)
			{
				throw new ConcurrentGraphException("The graph node must belong to a dependency graph.");
			}
			if (_index < 0)
			{
				throw new ConcurrentGraphException("Not allowed to have negative node index");
			}
			this.graph = _graph;
			this.index = _index;
			this.labels = new SortedDictionary<int, string>();

			int tmpHeadIndex = -1;
			if (_labels != null)
			{
				for (int i = 0; i < _labels.Length; i++)
				{
					int columnCategory = graph.DataFormat.getColumnDescription(i).Category;
					if (columnCategory == ColumnDescription.HEAD)
					{
						tmpHeadIndex = int.Parse(_labels[i]);
					}
					else if (columnCategory == ColumnDescription.INPUT || columnCategory == ColumnDescription.DEPENDENCY_EDGE_LABEL)
					{
						this.labels[i] = _labels[i];
					}
				}
			}
			this.headIndex = tmpHeadIndex;
			if (this.headIndex < -1)
			{
				throw new ConcurrentGraphException("Not allowed to have head index less than -1.");
			}
			if (this.index == 0 && this.headIndex != -1)
			{
				throw new ConcurrentGraphException("Not allowed to add head to a root node.");
			}
			if (this.index == this.headIndex)
			{
				throw new ConcurrentGraphException("Not allowed to add head to itself");
			}
		}

		/// <summary>
		/// Returns the index of the node.
		/// </summary>
		/// <returns> the index of the node. </returns>
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		/// <summary>
		/// Returns a label
		/// </summary>
		/// <param name="columnPosition"> the column position of the column that describes the label </param>
		/// <returns> a label. An empty string is returned if the label is not found. </returns>
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

		/// <summary>
		/// Returns a label
		/// </summary>
		/// <param name="columnName"> the name of the column that describes the label. </param>
		/// <returns> a label. An empty string is returned if the label is not found. </returns>
		public string getLabel(string columnName)
		{
			ColumnDescription column = graph.DataFormat.getColumnDescription(columnName);
			if (column != null)
			{
				return getLabel(column.Position);
			}
			return "";
		}

		/// <summary>
		/// Returns a label
		/// </summary>
		/// <param name="column"> a column description that describes the label </param>
		/// <returns> a label described by the column description. An empty string is returned if the label is not found.  </returns>
		public string getLabel(ColumnDescription column)
		{
			return getLabel(column.Position);
		}

		/// <summary>
		/// Returns <i>true</i> if the label exists, otherwise <i>false<i/>
		/// </summary>
		/// <param name="columnPosition"> the column position of the column that describes the label </param>
		/// <returns> <i>true</i> if the label exists, otherwise <i>false<i/> </returns>
		public bool hasLabel(int columnPosition)
		{
			return labels.ContainsKey(columnPosition);
		}

		/// <summary>
		/// Returns <i>true</i> if the label exists, otherwise <i>false<i/>
		/// </summary>
		/// <param name="columnName"> the name of the column that describes the label. </param>
		/// <returns> <i>true</i> if the label exists, otherwise <i>false<i/> </returns>
		public bool hasLabel(string columnName)
		{
			ColumnDescription column = graph.DataFormat.getColumnDescription(columnName);
			if (column != null)
			{
				return hasLabel(column.Position);
			}
			return false;
		}

		/// <summary>
		/// Returns <i>true</i> if the label exists, otherwise <i>false<i/>
		/// </summary>
		/// <param name="column"> a column description that describes the label </param>
		/// <returns> <i>true</i> if the label exists, otherwise <i>false<i/> </returns>
		public bool hasLabel(ColumnDescription column)
		{
			return labels.ContainsKey(column.Position);
		}

		/// <summary>
		/// Returns <i>true</i> if the node has one or more labels, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more labels, otherwise <i>false</i>. </returns>
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

		/// <summary>
		/// Returns <i>true</i> if the head edge has one or more labels, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the head edge has one or more labels, otherwise <i>false</i>. </returns>
		public bool HeadLabeled
		{
			get
			{
				foreach (int? key in labels.Keys)
				{
					if (graph.DataFormat.getColumnDescription(key).Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Returns the index of the head node
		/// </summary>
		/// <returns> the index of the head node </returns>
		public int HeadIndex
		{
			get
			{
				return headIndex;
			}
		}

		/// <summary>
		/// Returns a sorted map that maps column descriptions to node labels
		/// </summary>
		/// <returns> a sorted map that maps column descriptions to node labels </returns>
		public SortedDictionary<ColumnDescription, string> NodeLabels
		{
			get
			{
				SortedDictionary<ColumnDescription, string> nodeLabels = Collections.synchronizedSortedMap(new SortedDictionary<ColumnDescription, string>());
				foreach (int? key in labels.Keys)
				{
					if (graph.DataFormat.getColumnDescription(key).Category == ColumnDescription.INPUT)
					{
						nodeLabels[graph.DataFormat.getColumnDescription(key)] = labels[key];
					}
				}
				return nodeLabels;
			}
		}

		/// <summary>
		/// Returns a sorted map that maps column descriptions to head edge labels
		/// </summary>
		/// <returns> a sorted map that maps column descriptions to head edge labels </returns>
		public SortedDictionary<ColumnDescription, string> EdgeLabels
		{
			get
			{
				SortedDictionary<ColumnDescription, string> edgeLabels = Collections.synchronizedSortedMap(new SortedDictionary<ColumnDescription, string>());
				foreach (int? key in labels.Keys)
				{
					if (graph.DataFormat.getColumnDescription(key).Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
					{
						edgeLabels[graph.DataFormat.getColumnDescription(key)] = labels[key];
					}
				}
				return edgeLabels;
			}
		}

		/// <summary>
		/// Returns the predecessor dependency node in the linear order of the token nodes.
		/// </summary>
		/// <returns> the predecessor dependency node in the linear order of the token nodes. </returns>
		public ConcurrentDependencyNode Predecessor
		{
			get
			{
				return index > 1 ? graph.getDependencyNode(index - 1) : null;
			}
		}

		/// <summary>
		/// Returns the successor dependency node in the linear order of the token nodes.
		/// </summary>
		/// <returns> the successor dependency node in the linear order of the token nodes. </returns>
		public ConcurrentDependencyNode Successor
		{
			get
			{
				return graph.getDependencyNode(index + 1);
			}
		}

		/// <summary>
		/// Returns <i>true</i> if the node is a root node, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node is a root node, otherwise <i>false</i>. </returns>
		public bool Root
		{
			get
			{
				return index == 0;
			}
		}

		/// <summary>
		/// Returns <i>true</i> if the node has at most one head, otherwise <i>false</i>.
		/// 
		/// Note: this method will always return true because the concurrent dependency graph implementation only supports one or zero head.
		/// </summary>
		/// <returns> <i>true</i> if the node has at most one head, otherwise <i>false</i>. </returns>
		public bool hasAtMostOneHead()
		{
			return true;
		}

		/// <summary>
		/// Returns <i>true</i> if the node has one or more head(s), otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more head(s), otherwise <i>false</i>. </returns>
		public bool hasHead()
		{
			return headIndex != -1;
		}

		/// <summary>
		/// Returns <i>true</i> if the node has one or more dependents, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more dependents, otherwise <i>false</i>. </returns>
		public bool hasDependent()
		{
			return graph.hasDependent(index);
		}

		/// <summary>
		/// Returns <i>true</i> if the node has one or more left dependents, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more left dependents, otherwise <i>false</i>. </returns>
		public bool hasLeftDependent()
		{
			return graph.hasLeftDependent(index);
		}

		/// <summary>
		/// Returns <i>true</i> if the node has one or more right dependents, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more right dependents, otherwise <i>false</i>. </returns>
		public bool hasRightDependent()
		{
			return graph.hasRightDependent(index);
		}


		/// <summary>
		/// Returns a sorted set of head nodes. If the head is missing a empty set is returned. 
		/// 
		/// Note: In this implementation there will at most be one head.
		/// </summary>
		/// <returns> a sorted set of head nodes. </returns>
		public SortedSet<ConcurrentDependencyNode> Heads
		{
			get
			{
				SortedSet<ConcurrentDependencyNode> heads = Collections.synchronizedSortedSet(new SortedSet<ConcurrentDependencyNode>());
				ConcurrentDependencyNode head = Head;
				if (head != null)
				{
					heads.Add(head);
				}
				return heads;
			}
		}

		/// <summary>
		/// Returns the head dependency node if it exists, otherwise <i>null</i>. 
		/// </summary>
		/// <returns> the head dependency node if it exists, otherwise <i>null</i>. </returns>
		public ConcurrentDependencyNode Head
		{
			get
			{
				return graph.getDependencyNode(headIndex);
			}
		}

		/// <summary>
		/// Returns the left dependent at the position <i>leftDependentIndex</i>, where <i>leftDependentIndex==0</i> equals the left most dependent.
		/// </summary>
		/// <param name="leftDependentIndex"> the index of the left dependent </param>
		/// <returns> the left dependent at the position <i>leftDependentIndex</i>, where <i>leftDependentIndex==0</i> equals the left most dependent </returns>
		public ConcurrentDependencyNode getLeftDependent(int leftDependentIndex)
		{
			IList<ConcurrentDependencyNode> leftDependents = graph.getListOfLeftDependents(index);
			if (leftDependentIndex >= 0 && leftDependentIndex < leftDependents.Count)
			{
				return leftDependents[leftDependentIndex];
			}
			return null;
		}

		/// <summary>
		/// Return the number of left dependents
		/// </summary>
		/// <returns> the number of left dependents </returns>
		public int LeftDependentCount
		{
			get
			{
				return graph.getListOfLeftDependents(index).Count;
			}
		}

		/// <summary>
		/// Returns a sorted set of left dependents.
		/// </summary>
		/// <returns> a sorted set of left dependents. </returns>
		public SortedSet<ConcurrentDependencyNode> LeftDependents
		{
			get
			{
				return graph.getSortedSetOfLeftDependents(index);
			}
		}

		/// <summary>
		/// Returns a list of left dependents.
		/// </summary>
		/// <returns> a list of left dependents. </returns>
		public IList<ConcurrentDependencyNode> ListOfLeftDependents
		{
			get
			{
				return graph.getListOfLeftDependents(index);
			}
		}

		/// <summary>
		/// Returns the left sibling if it exists, otherwise <code>null</code>
		/// </summary>
		/// <returns> the left sibling if it exists, otherwise <code>null</code> </returns>
		public ConcurrentDependencyNode LeftSibling
		{
			get
			{
				if (headIndex == -1)
				{
					return null;
				}
    
				int nodeDepedentPosition = 0;
				IList<ConcurrentDependencyNode> headDependents = Head.ListOfDependents;
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

		/// <summary>
		/// Returns the left sibling at the same side of head as the node it self. If not found <code>null</code is returned
		/// </summary>
		/// <returns> the left sibling at the same side of head as the node it self. If not found <code>null</code is returned </returns>
		public ConcurrentDependencyNode SameSideLeftSibling
		{
			get
			{
				if (headIndex == -1)
				{
					return null;
				}
    
				IList<ConcurrentDependencyNode> headDependents;
				if (index < headIndex)
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

		/// <summary>
		/// Returns the closest left dependent of the node
		/// </summary>
		/// <returns> the closest left dependent of the node </returns>
		public ConcurrentDependencyNode ClosestLeftDependent
		{
			get
			{
				IList<ConcurrentDependencyNode> leftDependents = graph.getListOfLeftDependents(index);
				return (leftDependents.Count > 0) ? leftDependents[leftDependents.Count - 1] : null;
			}
		}

		/// <summary>
		/// Returns the leftmost dependent
		/// </summary>
		/// <returns> the leftmost dependent </returns>
		public ConcurrentDependencyNode LeftmostDependent
		{
			get
			{
				IList<ConcurrentDependencyNode> leftDependents = graph.getListOfLeftDependents(index);
				return (leftDependents.Count > 0) ? leftDependents[0] : null;
			}
		}

		/// <summary>
		/// Returns the right dependent at the position <i>rightDependentIndex</i>, where <i>rightDependentIndex==0</i> equals the right most dependent
		/// </summary>
		/// <param name="rightDependentIndex"> the index of the right dependent </param>
		/// <returns> the right dependent at the position <i>rightDependentIndex</i>, where <i>rightDependentIndex==0</i> equals the right most dependent </returns>
		public ConcurrentDependencyNode getRightDependent(int rightDependentIndex)
		{
			IList<ConcurrentDependencyNode> rightDependents = graph.getListOfRightDependents(index);
			if (rightDependentIndex >= 0 && rightDependentIndex < rightDependents.Count)
			{
				return rightDependents[rightDependents.Count - 1 - rightDependentIndex];
			}
			return null;
		}

		/// <summary>
		/// Return the number of right dependents
		/// </summary>
		/// <returns> the number of right dependents </returns>
		public int RightDependentCount
		{
			get
			{
				return graph.getListOfRightDependents(index).Count;
			}
		}

		/// <summary>
		/// Returns a sorted set of right dependents.
		/// </summary>
		/// <returns> a sorted set of right dependents. </returns>
		public SortedSet<ConcurrentDependencyNode> RightDependents
		{
			get
			{
				return graph.getSortedSetOfRightDependents(index);
			}
		}

		/// <summary>
		/// Returns a list of right dependents.
		/// </summary>
		/// <returns> a list of right dependents. </returns>
		public IList<ConcurrentDependencyNode> ListOfRightDependents
		{
			get
			{
				return graph.getListOfRightDependents(index);
			}
		}

		/// <summary>
		/// Returns the right sibling if it exists, otherwise <code>null</code>
		/// </summary>
		/// <returns> the right sibling if it exists, otherwise <code>null</code> </returns>
		public ConcurrentDependencyNode RightSibling
		{
			get
			{
				if (headIndex == -1)
				{
					return null;
				}
    
				IList<ConcurrentDependencyNode> headDependents = Head.ListOfDependents;
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

		/// <summary>
		/// Returns the right sibling at the same side of head as the node it self. If not found <code>null</code is returned
		/// </summary>
		/// <returns> the right sibling at the same side of head as the node it self. If not found <code>null</code is returned </returns>
		public ConcurrentDependencyNode SameSideRightSibling
		{
			get
			{
				if (headIndex == -1)
				{
					return null;
				}
    
				IList<ConcurrentDependencyNode> headDependents;
				if (index < headIndex)
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

		/// <summary>
		/// Returns the closest right dependent of the node
		/// </summary>
		/// <returns> the closest right dependent of the node </returns>
		public ConcurrentDependencyNode ClosestRightDependent
		{
			get
			{
				IList<ConcurrentDependencyNode> rightDependents = graph.getListOfRightDependents(index);
				return (rightDependents.Count > 0) ? rightDependents[0] : null;
			}
		}

		/// <summary>
		/// Returns the rightmost dependent
		/// </summary>
		/// <returns> the rightmost dependent </returns>
		public ConcurrentDependencyNode RightmostDependent
		{
			get
			{
				IList<ConcurrentDependencyNode> rightDependents = graph.getListOfRightDependents(index);
				return (rightDependents.Count > 0) ? rightDependents[rightDependents.Count - 1] : null;
			}
		}

		/// <summary>
		/// Returns a sorted set of dependents.
		/// </summary>
		/// <returns> a sorted set of dependents. </returns>
		public SortedSet<ConcurrentDependencyNode> Dependents
		{
			get
			{
				return graph.getSortedSetOfDependents(index);
			}
		}

		/// <summary>
		/// Returns a list of dependents.
		/// </summary>
		/// <returns> a list of dependents. </returns>
		public IList<ConcurrentDependencyNode> ListOfDependents
		{
			get
			{
				return graph.getListOfDependents(index);
			}
		}

		/// <summary>
		/// Returns the in degree of the node (number of incoming edges).
		/// </summary>
		/// <returns> the in degree of the node (number of incoming edges). </returns>
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

		/// <summary>
		/// Returns the out degree of the node (number of outgoing edges).
		/// </summary>
		/// <returns> the out degree of the node (number of outgoing edges). </returns>
		public int OutDegree
		{
			get
			{
				return graph.getListOfDependents(index).Count;
			}
		}

		public ConcurrentDependencyNode Ancestor
		{
			get
			{
				if (!this.hasHead())
				{
					return this;
				}
    
				ConcurrentDependencyNode tmp = this;
				while (tmp.hasHead())
				{
					tmp = tmp.Head;
				}
				return tmp;
			}
		}

		public ConcurrentDependencyNode ProperAncestor
		{
			get
			{
				if (!this.hasHead())
				{
					return null;
				}
    
				ConcurrentDependencyNode tmp = this;
				while (tmp.hasHead() && !tmp.Root)
				{
					tmp = tmp.Head;
				}
				return tmp;
			}
		}

		public bool hasAncestorInside(int left, int right)
		{
			if (index == 0)
			{
				return false;
			}
			ConcurrentDependencyNode tmp = this;
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

		/// <summary>
		/// Returns <i>true</i> if the head edge is projective, otherwise <i>false</i>. 
		/// </summary>
		/// <returns> <i>true</i> if the head edge is projective, otherwise <i>false</i>. </returns>
		public bool Projective
		{
			get
			{
				if (headIndex > 0)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ConcurrentDependencyNode head = getHead();
					ConcurrentDependencyNode head = Head;
					if (headIndex < index)
					{
						ConcurrentDependencyNode terminals = head;
						ConcurrentDependencyNode tmp = null;
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
						ConcurrentDependencyNode terminals = this;
						ConcurrentDependencyNode tmp = null;
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

		/// <summary>
		/// Returns the depth of the node. The root node has the depth 0. </summary>
		/// <returns> the depth of the node. </returns>
		public int DependencyNodeDepth
		{
			get
			{
				ConcurrentDependencyNode tmp = this;
				int depth = 0;
				while (tmp.hasHead())
				{
					depth++;
					tmp = tmp.Head;
				}
				return depth;
			}
		}

		/// <summary>
		/// Returns the left-most proper terminal descendant node (excluding itself). 
		/// </summary>
		/// <returns> the left-most proper terminal descendant node.  </returns>
		public ConcurrentDependencyNode LeftmostProperDescendant
		{
			get
			{
				ConcurrentDependencyNode candidate = null;
				IList<ConcurrentDependencyNode> dependents = graph.getListOfDependents(index);
				for (int i = 0; i < dependents.Count; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ConcurrentDependencyNode dep = dependents.get(i);
					ConcurrentDependencyNode dep = dependents[i];
					if (candidate == null || dep.Index < candidate.Index)
					{
						candidate = dep;
					}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ConcurrentDependencyNode tmp = dep.getLeftmostProperDescendant();
					ConcurrentDependencyNode tmp = dep.LeftmostProperDescendant;
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

		/// <summary>
		/// Returns the right-most proper terminal descendant node (excluding itself). 
		/// </summary>
		/// <returns> the right-most proper terminal descendant node.  </returns>
		public ConcurrentDependencyNode RightmostProperDescendant
		{
			get
			{
				ConcurrentDependencyNode candidate = null;
				IList<ConcurrentDependencyNode> dependents = graph.getListOfDependents(index);
				for (int i = 0; i < dependents.Count; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ConcurrentDependencyNode dep = dependents.get(i);
					ConcurrentDependencyNode dep = dependents[i];
					if (candidate == null || dep.Index > candidate.Index)
					{
						candidate = dep;
					}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ConcurrentDependencyNode tmp = dep.getRightmostProperDescendant();
					ConcurrentDependencyNode tmp = dep.RightmostProperDescendant;
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

		/// <summary>
		/// Returns the index of the left-most proper terminal descendant node (excluding itself). 
		/// </summary>
		/// <returns> the index of the left-most proper terminal descendant node.  </returns>
		public int LeftmostProperDescendantIndex
		{
			get
			{
				ConcurrentDependencyNode node = LeftmostProperDescendant;
				return (node != null)?node.Index:-1;
			}
		}

		/// <summary>
		/// Returns the index of the right-most proper terminal descendant node (excluding itself). 
		/// </summary>
		/// <returns> the index of the right-most proper terminal descendant node.  </returns>
		public int RightmostProperDescendantIndex
		{
			get
			{
				ConcurrentDependencyNode node = RightmostProperDescendant;
				return (node != null)?node.Index:-1;
			}
		}

		/// <summary>
		/// Returns the left-most terminal descendant node. 
		/// </summary>
		/// <returns> the left-most terminal descendant node.  </returns>
		public ConcurrentDependencyNode LeftmostDescendant
		{
			get
			{
				ConcurrentDependencyNode candidate = this;
				IList<ConcurrentDependencyNode> dependents = graph.getListOfDependents(index);
				for (int i = 0; i < dependents.Count; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ConcurrentDependencyNode dep = dependents.get(i);
					ConcurrentDependencyNode dep = dependents[i];
					if (dep.Index < candidate.Index)
					{
						candidate = dep;
					}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ConcurrentDependencyNode tmp = dep.getLeftmostDescendant();
					ConcurrentDependencyNode tmp = dep.LeftmostDescendant;
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

		/// <summary>
		/// Returns the right-most terminal descendant node. 
		/// </summary>
		/// <returns> the right-most terminal descendant node.  </returns>
		public ConcurrentDependencyNode RightmostDescendant
		{
			get
			{
				ConcurrentDependencyNode candidate = this;
				IList<ConcurrentDependencyNode> dependents = graph.getListOfDependents(index);
				for (int i = 0; i < dependents.Count; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ConcurrentDependencyNode dep = dependents.get(i);
					ConcurrentDependencyNode dep = dependents[i];
					if (dep.Index > candidate.Index)
					{
						candidate = dep;
					}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ConcurrentDependencyNode tmp = dep.getRightmostDescendant();
					ConcurrentDependencyNode tmp = dep.RightmostDescendant;
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

		/// <summary>
		/// Returns the index of the left-most terminal descendant node. 
		/// </summary>
		/// <returns> the index of the left-most terminal descendant node.  </returns>
		public int LeftmostDescendantIndex
		{
			get
			{
				ConcurrentDependencyNode node = LeftmostDescendant;
				return (node != null)?node.Index:this.Index;
			}
		}

		/// <summary>
		/// Returns the index of the right-most terminal descendant node. 
		/// </summary>
		/// <returns> the index of the right-most terminal descendant node.  </returns>
		public int RightmostDescendantIndex
		{
			get
			{
				ConcurrentDependencyNode node = RightmostDescendant;
				return (node != null)?node.Index:this.Index;
			}
		}

		public ConcurrentDependencyNode findComponent()
		{
			return graph.findComponent(index);
		}

		public int Rank
		{
			get
			{
				return graph.getRank(index);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ConcurrentDependencyEdge getHeadEdge() throws ConcurrentGraphException
		public ConcurrentDependencyEdge HeadEdge
		{
			get
			{
				if (!hasHead())
				{
					return null;
				}
				return new ConcurrentDependencyEdge(graph.DataFormat, Head, this, labels);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.SortedSet<ConcurrentDependencyEdge> getHeadEdges() throws ConcurrentGraphException
		public SortedSet<ConcurrentDependencyEdge> HeadEdges
		{
			get
			{
				SortedSet<ConcurrentDependencyEdge> edges = Collections.synchronizedSortedSet(new SortedSet<ConcurrentDependencyEdge>());
				if (hasHead())
				{
					edges.Add(new ConcurrentDependencyEdge(graph.DataFormat, Head, this, labels));
				}
				return edges;
			}
		}

		public bool HeadEdgeLabeled
		{
			get
			{
				return EdgeLabels.Count > 0;
			}
		}

		public int nHeadEdgeLabels()
		{
			return EdgeLabels.Count;
		}

		public DataFormat DataFormat
		{
			get
			{
				return graph.DataFormat;
			}
		}

		public int CompareTo(ConcurrentDependencyNode that)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;
			if (this == that)
			{
				return EQUAL;
			}
			if (this.index < that.Index)
			{
				return BEFORE;
			}
			if (this.index > that.Index)
			{
				return AFTER;
			}
			return EQUAL;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + headIndex;
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
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			ConcurrentDependencyNode other = (ConcurrentDependencyNode) obj;
			if (headIndex != other.headIndex)
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
						sb.Append(headIndex);
					}
					else if (column.Category == ColumnDescription.INPUT || column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
					{
						sb.Append(labels[column.Position]);
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