using System;
using System.Text;

namespace org.maltparser.core.syntaxgraph.edge
{
	using  exception;
	using  node;


	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class GraphEdge : GraphElement, Edge, IComparable<GraphEdge>
	{
		private Node source = null;
		private Node target = null;
		private int type;

		public GraphEdge()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GraphEdge(org.maltparser.core.syntaxgraph.node.Node source, org.maltparser.core.syntaxgraph.node.Node target, int type) throws org.maltparser.core.exception.MaltChainedException
		public GraphEdge(Node source, Node target, int type) : base()
		{
			clear();
			setEdge(source, target, type);
		}

		/// <summary>
		/// Sets the edge with a source node, a target node and a type (DEPENDENCY_EDGE, PHRASE_STRUCTURE_EDGE
		/// or SECONDARY_EDGE). 
		/// </summary>
		/// <param name="source"> a source node </param>
		/// <param name="target"> a target node </param>
		/// <param name="type"> a type (DEPENDENCY_EDGE, PHRASE_STRUCTURE_EDGE or SECONDARY_EDGE) </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setEdge(org.maltparser.core.syntaxgraph.node.Node source, org.maltparser.core.syntaxgraph.node.Node target, int type) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setEdge(Node source, Node target, int type)
		{
			this.source = source;
			this.target = target;
			if (type >= Edge_Fields.DEPENDENCY_EDGE && type <= Edge_Fields.SECONDARY_EDGE)
			{
				this.type = type;
			}
			this.source.addOutgoingEdge(this);
			this.target.addIncomingEdge(this);
			setChanged();
			notifyObservers(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			base.clear();
			if (source != null)
			{
				source.removeOutgoingEdge(this);
			}
			if (target != null)
			{
				target.removeIncomingEdge(this);
			}
			source = null;
			target = null;
			type = -1;
		}

		/// <summary>
		/// Returns the source node of the edge.
		/// </summary>
		/// <returns> the source node of the edge. </returns>
		public virtual Node Source
		{
			get
			{
				return source;
			}
		}

		/// <summary>
		/// Returns the target node of the edge.
		/// </summary>
		/// <returns> the target node of the edge. </returns>
		public virtual Node Target
		{
			get
			{
				return target;
			}
		}

		/// <summary>
		/// Returns the edge type (DEPENDENCY_EDGE, PHRASE_STRUCTURE_EDGE or SECONDARY_EDGE).
		/// </summary>
		/// <returns> the edge type (DEPENDENCY_EDGE, PHRASE_STRUCTURE_EDGE or SECONDARY_EDGE). </returns>
		public virtual int Type
		{
			get
			{
				return type;
			}
		}

		public virtual int CompareTo(GraphEdge that)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;

			if (this == that)
			{
				return EQUAL;
			}

			if (target.CompareToIndex < that.target.CompareToIndex)
			{
				return BEFORE;
			}
			if (target.CompareToIndex > that.target.CompareToIndex)
			{
				return AFTER;
			}

			if (source.CompareToIndex < that.source.CompareToIndex)
			{
				return BEFORE;
			}
			if (source.CompareToIndex > that.source.CompareToIndex)
			{
				return AFTER;
			}

			if (type < that.type)
			{
				return BEFORE;
			}
			if (type > that.type)
			{
				return AFTER;
			}

			return base.compareTo(that);
		}


		public override bool Equals(object obj)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final GraphEdge e = (GraphEdge)obj;
			GraphEdge e = (GraphEdge)obj;
			return type == e.Type && source.Equals(e.Source) && target.Equals(e.Target) && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			int hash = 7;
			hash = 31 * hash + type;
			hash = 31 * hash + (null == source ? 0 : source.GetHashCode());
			hash = 31 * hash + (null == target ? 0 : target.GetHashCode());
			return 31 * hash + base.GetHashCode();
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(source.Index);
			sb.Append("->");
			sb.Append(target.Index);
			sb.Append(' ');
			sb.Append(base.ToString());
			return sb.ToString();
		}
	}

}