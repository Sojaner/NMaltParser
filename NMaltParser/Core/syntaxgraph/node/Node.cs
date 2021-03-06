﻿using System.Collections.Generic;

namespace NMaltParser.Core.SyntaxGraph.Node
{
    public interface Node : ComparableNode, Element
    {
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void addIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException;
        void addIncomingEdge(Edge.Edge @in);
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void addOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException;
        void addOutgoingEdge(Edge.Edge @out);
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void removeIncomingEdge(org.maltparser.core.syntaxgraph.edge.Edge in) throws org.maltparser.core.exception.MaltChainedException;
        void removeIncomingEdge(Edge.Edge @in);
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void removeOutgoingEdge(org.maltparser.core.syntaxgraph.edge.Edge out) throws org.maltparser.core.exception.MaltChainedException;
        void removeOutgoingEdge(Edge.Edge @out);
        IEnumerator<Edge.Edge> IncomingEdgeIterator { get; }
        IEnumerator<Edge.Edge> OutgoingEdgeIterator { get; }
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void setIndex(int index) throws org.maltparser.core.exception.MaltChainedException;
        int Index { get; set; }
    }

}