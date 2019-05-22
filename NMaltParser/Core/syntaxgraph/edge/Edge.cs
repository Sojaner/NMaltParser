namespace org.maltparser.core.syntaxgraph.edge
{
	using  exception;
	using  node;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface Edge : Element
	{

		/// <summary>
		/// Sets the edge with a source node, a target node and a type (DEPENDENCY_EDGE, PHRASE_STRUCTURE_EDGE
		/// or SECONDARY_EDGE). 
		/// </summary>
		/// <param name="source"> a source node </param>
		/// <param name="target"> a target node </param>
		/// <param name="type"> a type (DEPENDENCY_EDGE, PHRASE_STRUCTURE_EDGE or SECONDARY_EDGE) </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setEdge(org.maltparser.core.syntaxgraph.node.Node source, org.maltparser.core.syntaxgraph.node.Node target, int type) throws org.maltparser.core.exception.MaltChainedException;
		void setEdge(Node source, Node target, int type);
		/// <summary>
		/// Returns the source node of the edge.
		/// </summary>
		/// <returns> the source node of the edge. </returns>
		Node Source {get;}
		/// <summary>
		/// Returns the target node of the edge.
		/// </summary>
		/// <returns> the target node of the edge. </returns>
		Node Target {get;}
		/// <summary>
		/// Returns the edge type (DEPENDENCY_EDGE, PHRASE_STRUCTURE_EDGE or SECONDARY_EDGE).
		/// </summary>
		/// <returns> the edge type (DEPENDENCY_EDGE, PHRASE_STRUCTURE_EDGE or SECONDARY_EDGE). </returns>
		int Type {get;}
	}

	public static class Edge_Fields
	{
		public const int DEPENDENCY_EDGE = 1;
		public const int PHRASE_STRUCTURE_EDGE = 2;
		public const int SECONDARY_EDGE = 3;
	}

}