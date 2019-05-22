namespace org.maltparser.core.syntaxgraph
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.syntaxgraph.edge;
	using  org.maltparser.core.syntaxgraph.node;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface SecEdgeStructure
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge addSecondaryEdge(org.maltparser.core.syntaxgraph.node.ComparableNode source, org.maltparser.core.syntaxgraph.node.ComparableNode target) throws org.maltparser.core.exception.MaltChainedException;
		Edge addSecondaryEdge(ComparableNode source, ComparableNode target);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeSecondaryEdge(org.maltparser.core.syntaxgraph.node.ComparableNode source, org.maltparser.core.syntaxgraph.node.ComparableNode target) throws org.maltparser.core.exception.MaltChainedException;
		void removeSecondaryEdge(ComparableNode source, ComparableNode target);
	}

}