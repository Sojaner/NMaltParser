using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.syntaxgraph.node
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;



	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class GraphNode : GraphElement, Node
	{
		public abstract ComparableNode RightmostDescendant {get;}
		public abstract ComparableNode LeftmostDescendant {get;}
		public abstract ComparableNode RightmostProperDescendant {get;}
		public abstract ComparableNode LeftmostProperDescendant {get;}
		public abstract int CompareToIndex {get;}
		protected internal SortedSet<Edge> incomingEdges;
		protected internal SortedSet<Edge> outgoingEdges;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GraphNode() throws org.maltparser.core.exception.MaltChainedException
		public GraphNode() : base()
		{
			incomingEdges = new SortedSet<Edge>();
			outgoingEdges = new SortedSet<Edge>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addIncomingEdge(Edge @in)
		{
			if (@in.Target != this)
			{
				throw new SyntaxGraphException("The incoming edge's 'to' reference is not correct.");
			}
			incomingEdges.Add(@in);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addOutgoingEdge(Edge @out)
		{
			if (@out.Source != this)
			{
				throw new SyntaxGraphException("The outgoing edge's 'from' reference is not correct");
			}
			outgoingEdges.Add(@out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException
		public virtual void removeIncomingEdge(Edge @in)
		{
			if (@in.Target != this)
			{
				throw new SyntaxGraphException("The incoming edge's 'to' reference is not correct");
			}
			incomingEdges.remove(@in);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException
		public virtual void removeOutgoingEdge(Edge @out)
		{
			if (@out.Source != this)
			{
				throw new SyntaxGraphException("The outgoing edge's 'from' reference is not correct");
			}
			outgoingEdges.remove(@out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getLeftmostProperDescendantIndex() throws org.maltparser.core.exception.MaltChainedException
		public virtual int LeftmostProperDescendantIndex
		{
			get
			{
				ComparableNode node = LeftmostProperDescendant;
				return (node != null)?node.Index:-1;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getRightmostProperDescendantIndex() throws org.maltparser.core.exception.MaltChainedException
		public virtual int RightmostProperDescendantIndex
		{
			get
			{
				ComparableNode node = RightmostProperDescendant;
				return (node != null)?node.Index:-1;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getLeftmostDescendantIndex() throws org.maltparser.core.exception.MaltChainedException
		public virtual int LeftmostDescendantIndex
		{
			get
			{
				ComparableNode node = LeftmostProperDescendant;
				return (node != null)?node.Index:this.Index;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getRightmostDescendantIndex() throws org.maltparser.core.exception.MaltChainedException
		public virtual int RightmostDescendantIndex
		{
			get
			{
				ComparableNode node = RightmostProperDescendant;
				return (node != null)?node.Index:this.Index;
			}
		}

		public virtual IEnumerator<Edge> IncomingEdgeIterator
		{
			get
			{
				return incomingEdges.GetEnumerator();
			}
		}

		public virtual IEnumerator<Edge> OutgoingEdgeIterator
		{
			get
			{
				return outgoingEdges.GetEnumerator();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			base.clear();
			incomingEdges.Clear();
			outgoingEdges.Clear();
		}

		public virtual int InDegree
		{
			get
			{
				return incomingEdges.Count;
			}
		}

		public virtual int OutDegree
		{
			get
			{
				return outgoingEdges.Count;
			}
		}

		public virtual SortedSet<Edge> IncomingSecondaryEdges
		{
			get
			{
				SortedSet<Edge> inSecEdges = new SortedSet<Edge>();
				foreach (Edge e in incomingEdges)
				{
					if (e.Type == org.maltparser.core.syntaxgraph.edge.Edge_Fields.SECONDARY_EDGE)
					{
						inSecEdges.Add(e);
					}
				}
				return inSecEdges;
			}
		}

		public virtual SortedSet<Edge> OutgoingSecondaryEdges
		{
			get
			{
				SortedSet<Edge> outSecEdges = new SortedSet<Edge>();
				foreach (Edge e in outgoingEdges)
				{
					if (e.Type == org.maltparser.core.syntaxgraph.edge.Edge_Fields.SECONDARY_EDGE)
					{
						outSecEdges.Add(e);
					}
				}
				return outSecEdges;
			}
		}

		public virtual int compareTo(ComparableNode o)
		{
			return base.compareTo((GraphElement)o);
		}

		public abstract int Index {get;set;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setIndex(int index) throws org.maltparser.core.exception.MaltChainedException;
		public abstract bool Root {get;}

		public override bool Equals(object obj)
		{
			GraphNode v = (GraphNode)obj;
			return base.Equals(obj) && incomingEdges.Equals(v.incomingEdges) && outgoingEdges.Equals(v.outgoingEdges);
		}

		public override int GetHashCode()
		{
			int hash = 7;
			hash = 31 * hash + base.GetHashCode();
			hash = 31 * hash + (null == incomingEdges ? 0 : incomingEdges.GetHashCode());
			hash = 31 * hash + (null == outgoingEdges ? 0 : outgoingEdges.GetHashCode());
			return hash;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(Index);
			sb.Append(" [I:");
			foreach (Edge e in incomingEdges)
			{
				sb.Append(e.Source.Index);
				sb.Append("(");
				sb.Append(e.ToString());
				sb.Append(")");
				if (incomingEdges.Max != e)
				{
					sb.Append(",");
				}
			}
			sb.Append("][O:");
			foreach (Edge e in outgoingEdges)
			{
				sb.Append(e.Target.Index);
				if (outgoingEdges.Max != e)
				{
					sb.Append(",");
				}
			}
			sb.Append("]");
			sb.Append(base.ToString());
			return sb.ToString();
		}
	}

}