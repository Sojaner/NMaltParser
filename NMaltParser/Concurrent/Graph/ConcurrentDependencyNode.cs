using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Concurrent.Graph.DataFormat;
using NMaltParser.Utilities;

namespace NMaltParser.Concurrent.Graph
{
    /// <summary>
	/// Immutable and tread-safe dependency node implementation.
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class ConcurrentDependencyNode : IComparable<ConcurrentDependencyNode>, IEquatable<ConcurrentDependencyNode>
	{
        private readonly ConcurrencyFactory factory = new ConcurrencyFactory();

        private readonly ConcurrentDependencyGraph graph;

        private readonly SortedDictionary<int, string> labels;

        internal ConcurrentDependencyNode(ConcurrentDependencyNode node) : this(node.graph, node)
		{
		}

        internal ConcurrentDependencyNode(ConcurrentDependencyGraph graph, ConcurrentDependencyNode node)
		{
            this.graph = graph ?? throw new ConcurrentGraphException("The graph node must belong to a dependency graph.");

			Index = node.Index;

			labels = new SortedDictionary<int, string>(node.labels);

			HeadIndex = node.HeadIndex;
		}

        internal ConcurrentDependencyNode(ConcurrentDependencyGraph graph, int index, IDictionary<int, string> labels, int headIndex)
		{
            if (index < 0)
			{
				throw new ConcurrentGraphException("Not allowed to have negative node index");
			}

			if (headIndex < -1)
			{
				throw new ConcurrentGraphException("Not allowed to have head index less than -1.");
			}

			if (index == 0 && headIndex != -1)
			{
				throw new ConcurrentGraphException("Not allowed to add head to a root node.");
			}

			if (index == headIndex)
			{
				throw new ConcurrentGraphException("Not allowed to add head to itself");
			}

			this.graph = graph ?? throw new ConcurrentGraphException("The graph node must belong to a dependency graph.");

			Index = index;

			this.labels = new SortedDictionary<int, string>(labels);

			HeadIndex = headIndex;
		}

        internal ConcurrentDependencyNode(ConcurrentDependencyGraph graph, int index, IReadOnlyList<string> labels)
		{
            if (index < 0)
			{
				throw new ConcurrentGraphException("Not allowed to have negative node index");
			}

			this.graph = graph ?? throw new ConcurrentGraphException("The graph node must belong to a dependency graph.");

			Index = index;

			this.labels = new SortedDictionary<int, string>();

			int tmpHeadIndex = -1;

			if (labels != null)
			{
				for (int i = 0; i < labels.Count; i++)
				{
					int columnCategory = this.graph.DataFormat.GetColumnDescription(i).Category;

					if (columnCategory == ColumnDescription.Head)
					{
						tmpHeadIndex = int.Parse(labels[i]);
					}
					else if (columnCategory == ColumnDescription.Input || columnCategory == ColumnDescription.DependencyEdgeLabel)
					{
						this.labels[i] = labels[i];
					}
				}
			}

			HeadIndex = tmpHeadIndex;

			if (HeadIndex < -1)
			{
				throw new ConcurrentGraphException("Not allowed to have head index less than -1.");
			}

			if (Index == 0 && HeadIndex != -1)
			{
				throw new ConcurrentGraphException("Not allowed to add head to a root node.");
			}

			if (Index == HeadIndex)
			{
				throw new ConcurrentGraphException("Not allowed to add head to itself");
			}
		}

		/// <summary>
		/// Returns the index of the node.
		/// </summary>
		/// <returns> the index of the node. </returns>
		public int Index { get; }

        /// <summary>
		/// Returns a label
		/// </summary>
		/// <param name="columnPosition"> the column position of the column that describes the label </param>
		/// <returns> a label. An empty string is returned if the label is not found. </returns>
		public string GetLabel(int columnPosition)
		{
			if (labels.ContainsKey(columnPosition))
			{
				return labels[columnPosition];
			}
			else if (graph.DataFormat.GetColumnDescription(columnPosition).Category == ColumnDescription.Ignore)
			{
				return graph.DataFormat.GetColumnDescription(columnPosition).DefaultOutput;
			}

			return "";
		}

		/// <summary>
		/// Returns a label
		/// </summary>
		/// <param name="columnName"> the name of the column that describes the label. </param>
		/// <returns> a label. An empty string is returned if the label is not found. </returns>
		public string GetLabel(string columnName)
		{
			ColumnDescription column = graph.DataFormat.GetColumnDescription(columnName);

			return column != null ? GetLabel(column.Position) : "";
        }

		/// <summary>
		/// Returns a label
		/// </summary>
		/// <param name="column"> a column description that describes the label </param>
		/// <returns> a label described by the column description. An empty string is returned if the label is not found.  </returns>
		public string GetLabel(ColumnDescription column)
		{
			return GetLabel(column.Position);
		}

		/// <summary>
		/// Returns <i>true</i> if the label exists, otherwise <i>false</i>
		/// </summary>
		/// <param name="columnPosition"> the column position of the column that describes the label </param>
		/// <returns> <i>true</i> if the label exists, otherwise <i>false</i> </returns>
		public bool HasLabel(int columnPosition)
		{
			return labels.ContainsKey(columnPosition);
		}

		/// <summary>
		/// Returns <i>true</i> if the label exists, otherwise <i>false</i>
		/// </summary>
		/// <param name="columnName"> the name of the column that describes the label. </param>
		/// <returns> <i>true</i> if the label exists, otherwise <i>false</i> </returns>
		public bool HasLabel(string columnName)
		{
			ColumnDescription column = graph.DataFormat.GetColumnDescription(columnName);

			return column != null && HasLabel(column.Position);
        }

		/// <summary>
		/// Returns <i>true</i> if the label exists, otherwise <i>false</i>
		/// </summary>
		/// <param name="column"> a column description that describes the label </param>
		/// <returns> <i>true</i> if the label exists, otherwise <i>false</i> </returns>
		public bool HasLabel(ColumnDescription column)
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
				foreach (int key in labels.Keys)
				{
					if (graph.DataFormat.GetColumnDescription(key).Category == ColumnDescription.Input)
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
				foreach (int key in labels.Keys)
				{
					if (graph.DataFormat.GetColumnDescription(key).Category == ColumnDescription.DependencyEdgeLabel)
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
		public int HeadIndex { get; }

        /// <summary>
		/// Returns a sorted map that maps column descriptions to node labels
		/// </summary>
		/// <returns> a sorted map that maps column descriptions to node labels </returns>
		public SortedDictionary<ColumnDescription, string> NodeLabels
		{
			get
            {
                return factory.Get<SortedDictionary<ColumnDescription, string>>(() =>
                {
                    SortedDictionary<ColumnDescription, string> nodeLabels = new SortedDictionary<ColumnDescription, string>();

                    foreach (int key in labels.Keys)
                    {
                        if (graph.DataFormat.GetColumnDescription(key).Category == ColumnDescription.Input)
                        {
                            nodeLabels[graph.DataFormat.GetColumnDescription(key)] = labels[key];
                        }
                    }

                    return nodeLabels;
                });
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
                return factory.Get<SortedDictionary<ColumnDescription, string>>(() =>
                {
                    SortedDictionary<ColumnDescription, string> edgeLabels = new SortedDictionary<ColumnDescription, string>();

                    foreach (int key in labels.Keys)
                    {
                        if (graph.DataFormat.GetColumnDescription(key).Category == ColumnDescription.DependencyEdgeLabel)
                        {
                            edgeLabels[graph.DataFormat.GetColumnDescription(key)] = labels[key];
                        }
                    }

                    return edgeLabels;
                });
            }
		}

		/// <summary>
		/// Returns the predecessor dependency node in the linear order of the token nodes.
		/// </summary>
		/// <returns> the predecessor dependency node in the linear order of the token nodes. </returns>
		public ConcurrentDependencyNode Predecessor => Index > 1 ? graph.GetDependencyNode(Index - 1) : null;

        /// <summary>
		/// Returns the successor dependency node in the linear order of the token nodes.
		/// </summary>
		/// <returns> the successor dependency node in the linear order of the token nodes. </returns>
		public ConcurrentDependencyNode Successor => graph.GetDependencyNode(Index + 1);

        /// <summary>
		/// Returns <i>true</i> if the node is a root node, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node is a root node, otherwise <i>false</i>. </returns>
		public bool Root => Index == 0;

        /// <summary>
		/// Returns <i>true</i> if the node has at most one head, otherwise <i>false</i>.
		/// 
		/// Note: this method will always return true because the concurrent dependency graph implementation only supports one or zero head.
		/// </summary>
		/// <returns> <i>true</i> if the node has at most one head, otherwise <i>false</i>. </returns>
		public bool HasAtMostOneHead()
		{
			return true;
		}

		/// <summary>
		/// Returns <i>true</i> if the node has one or more head(s), otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more head(s), otherwise <i>false</i>. </returns>
		public bool HasHead()
		{
			return HeadIndex != -1;
		}

		/// <summary>
		/// Returns <i>true</i> if the node has one or more dependents, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more dependents, otherwise <i>false</i>. </returns>
		public bool HasDependent()
		{
			return graph.HasDependent(Index);
		}

		/// <summary>
		/// Returns <i>true</i> if the node has one or more left dependents, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more left dependents, otherwise <i>false</i>. </returns>
		public bool HasLeftDependent()
		{
			return graph.HasLeftDependent(Index);
		}

		/// <summary>
		/// Returns <i>true</i> if the node has one or more right dependents, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more right dependents, otherwise <i>false</i>. </returns>
		public bool HasRightDependent()
		{
			return graph.HasRightDependent(Index);
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
                return factory.Get<SortedSet<ConcurrentDependencyNode>>(() =>
                {
                    SortedSet<ConcurrentDependencyNode> heads = new SortedSet<ConcurrentDependencyNode>();

                    ConcurrentDependencyNode head = Head;

                    if (head != null)
                    {
                        heads.Add(head);
                    }

                    return heads;
                });
            }
		}

		/// <summary>
		/// Returns the head dependency node if it exists, otherwise <i>null</i>. 
		/// </summary>
		/// <returns> the head dependency node if it exists, otherwise <i>null</i>. </returns>
		public ConcurrentDependencyNode Head => graph.GetDependencyNode(HeadIndex);

        /// <summary>
		/// Returns the left dependent at the position <i>leftDependentIndex</i>, where <i>leftDependentIndex==0</i> equals the left most dependent.
		/// </summary>
		/// <param name="leftDependentIndex"> the index of the left dependent </param>
		/// <returns> the left dependent at the position <i>leftDependentIndex</i>, where <i>leftDependentIndex==0</i> equals the left most dependent </returns>
		public ConcurrentDependencyNode GetLeftDependent(int leftDependentIndex)
		{
			IList<ConcurrentDependencyNode> leftDependents = graph.GetListOfLeftDependents(Index);

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
		public int LeftDependentCount => graph.GetListOfLeftDependents(Index).Count;

        /// <summary>
		/// Returns a sorted set of left dependents.
		/// </summary>
		/// <returns> a sorted set of left dependents. </returns>
		public SortedSet<ConcurrentDependencyNode> LeftDependents => graph.GetSortedSetOfLeftDependents(Index);

        /// <summary>
		/// Returns a list of left dependents.
		/// </summary>
		/// <returns> a list of left dependents. </returns>
		public IList<ConcurrentDependencyNode> ListOfLeftDependents => graph.GetListOfLeftDependents(Index);

        /// <summary>
		/// Returns the left sibling if it exists, otherwise <code>null</code>
		/// </summary>
		/// <returns> the left sibling if it exists, otherwise <code>null</code> </returns>
		public ConcurrentDependencyNode LeftSibling
		{
			get
			{
				if (HeadIndex == -1)
				{
					return null;
				}
    
				int nodeDependentPosition = 0;

				IList<ConcurrentDependencyNode> headDependents = Head.ListOfDependents;

				for (int i = 0; i < headDependents.Count; i++)
				{
					if (headDependents[i].Index == Index)
					{
						nodeDependentPosition = i;

						break;
					}
				}
    
				return (nodeDependentPosition > 0) ? headDependents[nodeDependentPosition - 1] : null;
			}
		}

		/// <summary>
		/// Returns the left sibling at the same side of head as the node it self. If not found <code>null</code> is returned
		/// </summary>
		/// <returns> the left sibling at the same side of head as the node it self. If not found <code>null</code> is returned </returns>
		public ConcurrentDependencyNode SameSideLeftSibling
		{
			get
			{
				if (HeadIndex == -1)
				{
					return null;
				}

                IList<ConcurrentDependencyNode> headDependents = Index < HeadIndex ? Head.ListOfLeftDependents : Head.ListOfRightDependents;

				int nodeDependentPosition = 0;

				for (int i = 0; i < headDependents.Count; i++)
				{
					if (headDependents[i].Index == Index)
					{
						nodeDependentPosition = i;

						break;
					}
				}

				return (nodeDependentPosition > 0) ? headDependents[nodeDependentPosition - 1] : null;
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
				IList<ConcurrentDependencyNode> leftDependents = graph.GetListOfLeftDependents(Index);

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
				IList<ConcurrentDependencyNode> leftDependents = graph.GetListOfLeftDependents(Index);

				return (leftDependents.Count > 0) ? leftDependents[0] : null;
			}
		}

		/// <summary>
		/// Returns the right dependent at the position <i>rightDependentIndex</i>, where <i>rightDependentIndex==0</i> equals the right most dependent
		/// </summary>
		/// <param name="rightDependentIndex"> the index of the right dependent </param>
		/// <returns> the right dependent at the position <i>rightDependentIndex</i>, where <i>rightDependentIndex==0</i> equals the right most dependent </returns>
		public ConcurrentDependencyNode GetRightDependent(int rightDependentIndex)
		{
			IList<ConcurrentDependencyNode> rightDependents = graph.GetListOfRightDependents(Index);

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
		public int RightDependentCount => graph.GetListOfRightDependents(Index).Count;

        /// <summary>
		/// Returns a sorted set of right dependents.
		/// </summary>
		/// <returns> a sorted set of right dependents. </returns>
		public SortedSet<ConcurrentDependencyNode> RightDependents => graph.GetSortedSetOfRightDependents(Index);

        /// <summary>
		/// Returns a list of right dependents.
		/// </summary>
		/// <returns> a list of right dependents. </returns>
		public IList<ConcurrentDependencyNode> ListOfRightDependents => graph.GetListOfRightDependents(Index);

        /// <summary>
		/// Returns the right sibling if it exists, otherwise <code>null</code>
		/// </summary>
		/// <returns> the right sibling if it exists, otherwise <code>null</code> </returns>
		public ConcurrentDependencyNode RightSibling
		{
			get
			{
				if (HeadIndex == -1)
				{
					return null;
				}
    
				IList<ConcurrentDependencyNode> headDependents = Head.ListOfDependents;

				int nodeDependentPosition = headDependents.Count - 1;

				for (int i = headDependents.Count - 1; i >= 0 ; i--)
				{
					if (headDependents[i].Index == Index)
					{
						nodeDependentPosition = i;

						break;
					}
				}
    
				return (nodeDependentPosition < headDependents.Count - 1) ? headDependents[nodeDependentPosition + 1] : null;
			}
		}

		/// <summary>
		/// Returns the right sibling at the same side of head as the node it self. If not found <code>null</code> is returned
		/// </summary>
		/// <returns> the right sibling at the same side of head as the node it self. If not found <code>null</code> is returned </returns>
		public ConcurrentDependencyNode SameSideRightSibling
		{
			get
			{
				if (HeadIndex == -1)
				{
					return null;
				}

                IList<ConcurrentDependencyNode> headDependents = Index < HeadIndex ? Head.ListOfLeftDependents : Head.ListOfRightDependents;

				int nodeDependentPosition = headDependents.Count - 1;

				for (int i = headDependents.Count - 1; i >= 0 ; i--)
				{
					if (headDependents[i].Index == Index)
					{
						nodeDependentPosition = i;

						break;
					}
				}
    
				return (nodeDependentPosition < headDependents.Count - 1) ? headDependents[nodeDependentPosition + 1] : null;
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
				IList<ConcurrentDependencyNode> rightDependents = graph.GetListOfRightDependents(Index);

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
				IList<ConcurrentDependencyNode> rightDependents = graph.GetListOfRightDependents(Index);

				return (rightDependents.Count > 0) ? rightDependents[rightDependents.Count - 1] : null;
			}
		}

		/// <summary>
		/// Returns a sorted set of dependents.
		/// </summary>
		/// <returns> a sorted set of dependents. </returns>
		public SortedSet<ConcurrentDependencyNode> Dependents => graph.GetSortedSetOfDependents(Index);

        /// <summary>
		/// Returns a list of dependents.
		/// </summary>
		/// <returns> a list of dependents. </returns>
		public IList<ConcurrentDependencyNode> ListOfDependents => graph.GetListOfDependents(Index);

        /// <summary>
		/// Returns the in degree of the node (number of incoming edges).
		/// </summary>
		/// <returns> the in degree of the node (number of incoming edges). </returns>
		public int InDegree => HasHead() ? 1 : 0;

        /// <summary>
		/// Returns the out degree of the node (number of outgoing edges).
		/// </summary>
		/// <returns> the out degree of the node (number of outgoing edges). </returns>
		public int OutDegree => graph.GetListOfDependents(Index).Count;

        public ConcurrentDependencyNode Ancestor
		{
			get
			{
				if (!HasHead())
				{
					return this;
				}
    
				ConcurrentDependencyNode tmp = this;

				while (tmp.HasHead())
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
				if (!HasHead())
				{
					return null;
				}
    
				ConcurrentDependencyNode tmp = this;

				while (tmp.HasHead() && !tmp.Root)
				{
					tmp = tmp.Head;
				}

				return tmp;
			}
		}

		public bool HasAncestorInside(int left, int right)
		{
			if (Index == 0)
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
				if (HeadIndex > 0)
				{
					ConcurrentDependencyNode head = Head;

					if (HeadIndex < Index)
					{
						ConcurrentDependencyNode terminals = head;

                        while (true)
						{
							if (terminals?.Successor == null)
							{
								return false;
							}

							if (terminals.Successor.Equals(this))
							{
								break;
							}

							ConcurrentDependencyNode tmp = terminals = terminals.Successor;

							while (!tmp.Equals(this) && !tmp.Equals(head))
							{
								if (!tmp.HasHead())
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

                        while (true)
						{
							if (terminals?.Successor == null)
							{
								return false;
							}
							if (terminals.Successor.Equals(head))
							{
								break;
							}

							ConcurrentDependencyNode tmp = terminals = terminals.Successor;

							while (!tmp.Equals(this) && !tmp.Equals(head))
							{
								if (!tmp.HasHead())
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

				while (tmp.HasHead())
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

				IList<ConcurrentDependencyNode> dependents = graph.GetListOfDependents(Index);

				foreach (ConcurrentDependencyNode dep in dependents)
                {
                    if (candidate == null || dep.Index < candidate.Index)
                    {
                        candidate = dep;
                    }

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

				IList<ConcurrentDependencyNode> dependents = graph.GetListOfDependents(Index);

				foreach (ConcurrentDependencyNode dep in dependents)
                {
                    if (candidate == null || dep.Index > candidate.Index)
                    {
                        candidate = dep;
                    }

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

				return node?.Index ?? -1;
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

				return node?.Index ?? -1;
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

				IList<ConcurrentDependencyNode> dependents = graph.GetListOfDependents(Index);

				foreach (ConcurrentDependencyNode dep in dependents)
                {
                    if (dep.Index < candidate.Index)
                    {
                        candidate = dep;
                    }

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

				IList<ConcurrentDependencyNode> dependents = graph.GetListOfDependents(Index);

				foreach (ConcurrentDependencyNode dep in dependents)
                {
                    if (dep.Index > candidate.Index)
                    {
                        candidate = dep;
                    }

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

				return node?.Index ?? Index;
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

				return node?.Index ?? Index;
			}
		}

		public ConcurrentDependencyNode FindComponent()
		{
			return graph.FindComponent(Index);
		}

		public int Rank => graph.GetRank(Index);
        
		public ConcurrentDependencyEdge HeadEdge => !HasHead() ? null : new ConcurrentDependencyEdge(graph.DataFormat, Head, this, labels);

        public SortedSet<ConcurrentDependencyEdge> HeadEdges
		{
			get
            {
                return factory.Get<SortedSet<ConcurrentDependencyEdge>>(() =>
                {
                    SortedSet<ConcurrentDependencyEdge> edges = new SortedSet<ConcurrentDependencyEdge>();

                    if (HasHead())
                    {
                        edges.Add(new ConcurrentDependencyEdge(graph.DataFormat, Head, this, labels));
                    }

                    return edges;
                });
            }
		}

		public bool HeadEdgeLabeled => EdgeLabels.Count > 0;

        public int NHeadEdgeLabels()
		{
			return EdgeLabels.Count;
		}

		public DataFormat.DataFormat DataFormat => graph.DataFormat;

        public int CompareTo(ConcurrentDependencyNode that)
		{
			const int before = -1;

			const int equal = 0;

			const int after = 1;

			if (Equals(that))
			{
				return equal;
			}

			if (Index < that.Index)
			{
				return before;
			}

			return Index > that.Index ? after : equal;
        }

		public override int GetHashCode()
		{
			const int prime = 31;

			int result = 1;

			result = prime * result + HeadIndex;

			result = prime * result + Index;

			result = prime * result + ((labels == null) ? 0 : labels.GetHashCode());

			return result;
		}

        public bool Equals(ConcurrentDependencyNode other)
        {
            return Equals((object)other);
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

			ConcurrentDependencyNode other = (ConcurrentDependencyNode) obj;

			if (HeadIndex != other.HeadIndex)
			{
				return false;
			}

			if (Index != other.Index)
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
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < graph.DataFormat.NumberOfColumns(); i++)
			{
				ColumnDescription column = graph.DataFormat.GetColumnDescription(i);

				if (!column.Internal)
				{
					if (column.Category == ColumnDescription.Head)
					{
						sb.Append(HeadIndex);
					}
					else if (column.Category == ColumnDescription.Input || column.Category == ColumnDescription.DependencyEdgeLabel)
					{
						sb.Append(labels[column.Position]);
					}
					else if (column.Category == ColumnDescription.Ignore)
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