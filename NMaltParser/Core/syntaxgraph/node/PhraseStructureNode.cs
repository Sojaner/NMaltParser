﻿using NMaltParser.Core.Symbol;

namespace NMaltParser.Core.SyntaxGraph.Node
{
    public interface PhraseStructureNode : ComparableNode
	{
		PhraseStructureNode Parent {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge getParentEdge() throws org.maltparser.core.exception.MaltChainedException;
		Edge.Edge ParentEdge {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getParentEdgeLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		string getParentEdgeLabelSymbol(SymbolTable table);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getParentEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		int getParentEdgeLabelCode(SymbolTable table);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasParentEdgeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		bool hasParentEdgeLabel(SymbolTable table);
	}

}