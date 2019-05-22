using System.Collections.Generic;

namespace org.maltparser.core.syntaxgraph
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;
	using PhraseStructureNode = org.maltparser.core.syntaxgraph.node.PhraseStructureNode;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface PhraseStructure : TokenStructure, SecEdgeStructure
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addTerminalNode() throws org.maltparser.core.exception.MaltChainedException;
		PhraseStructureNode addTerminalNode();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException;
		PhraseStructureNode addTerminalNode(int index);
		PhraseStructureNode getTerminalNode(int index);
		int nTerminalNode();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge addPhraseStructureEdge(org.maltparser.core.syntaxgraph.node.PhraseStructureNode source, org.maltparser.core.syntaxgraph.node.PhraseStructureNode target) throws org.maltparser.core.exception.MaltChainedException;
		Edge addPhraseStructureEdge(PhraseStructureNode source, PhraseStructureNode target);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removePhraseStructureEdge(org.maltparser.core.syntaxgraph.node.PhraseStructureNode source, org.maltparser.core.syntaxgraph.node.PhraseStructureNode target) throws org.maltparser.core.exception.MaltChainedException;
		void removePhraseStructureEdge(PhraseStructureNode source, PhraseStructureNode target);
		int nEdges();
		PhraseStructureNode PhraseStructureRoot {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode getNonTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException;
		PhraseStructureNode getNonTerminalNode(int index);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addNonTerminalNode() throws org.maltparser.core.exception.MaltChainedException;
		PhraseStructureNode addNonTerminalNode();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.PhraseStructureNode addNonTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException;
		PhraseStructureNode addNonTerminalNode(int index);
		int HighestNonTerminalIndex {get;}
		ISet<int> NonTerminalIndices {get;}
		bool hasNonTerminals();
		int nNonTerminals();
		bool Continuous {get;}
		bool ContinuousExcludeTerminalsAttachToRoot {get;}
	//	public void makeContinuous() throws MaltChainedException;
	}

}