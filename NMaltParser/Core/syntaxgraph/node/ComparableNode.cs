using System;
using System.Collections.Generic;

namespace org.maltparser.core.syntaxgraph.node
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;

	public interface ComparableNode : Element, IComparable<ComparableNode>
	{
		/// <summary>
		/// Returns the index of the node.
		/// </summary>
		/// <returns> the index of the node. </returns>
		int Index {get;}
		/// <summary>
		/// Returns the index of the node (only used internal by compareTo).
		/// </summary>
		/// <returns> the index of the node (only used internal by compareTo). </returns>
		int CompareToIndex {get;}
		/// <summary>
		/// Returns <i>true</i> if the node is a root node, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node is a root node, otherwise <i>false</i>. </returns>
		bool Root {get;}
		/// <summary>
		/// Returns the left-most proper terminal descendant node (excluding itself). 
		/// </summary>
		/// <returns> the left-most proper terminal descendant node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getLeftmostProperDescendant() throws org.maltparser.core.exception.MaltChainedException;
		ComparableNode LeftmostProperDescendant {get;}
		/// <summary>
		/// Returns the right-most proper terminal descendant node (excluding itself). 
		/// </summary>
		/// <returns> the right-most proper terminal descendant node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getRightmostProperDescendant() throws org.maltparser.core.exception.MaltChainedException;
		ComparableNode RightmostProperDescendant {get;}
		/// <summary>
		/// Returns the index of the left-most proper terminal descendant node (excluding itself). 
		/// </summary>
		/// <returns> the index of the left-most proper terminal descendant node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getLeftmostProperDescendantIndex() throws org.maltparser.core.exception.MaltChainedException;
		int LeftmostProperDescendantIndex {get;}
		/// <summary>
		/// Returns the index of the right-most proper terminal descendant node (excluding itself). 
		/// </summary>
		/// <returns> the index of the right-most proper terminal descendant node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getRightmostProperDescendantIndex() throws org.maltparser.core.exception.MaltChainedException;
		int RightmostProperDescendantIndex {get;}
		/// <summary>
		/// Returns the left-most terminal descendant node. 
		/// </summary>
		/// <returns> the left-most terminal descendant node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getLeftmostDescendant() throws org.maltparser.core.exception.MaltChainedException;
		ComparableNode LeftmostDescendant {get;}
		/// <summary>
		/// Returns the right-most terminal descendant node. 
		/// </summary>
		/// <returns> the right-most terminal descendant node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComparableNode getRightmostDescendant() throws org.maltparser.core.exception.MaltChainedException;
		ComparableNode RightmostDescendant {get;}
		/// <summary>
		/// Returns the index of the left-most terminal descendant node. 
		/// </summary>
		/// <returns> the index of the left-most terminal descendant node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getLeftmostDescendantIndex() throws org.maltparser.core.exception.MaltChainedException;
		int LeftmostDescendantIndex {get;}
		/// <summary>
		/// Returns the index of the right-most terminal descendant node. 
		/// </summary>
		/// <returns> the index of the right-most terminal descendant node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getRightmostDescendantIndex() throws org.maltparser.core.exception.MaltChainedException;
		int RightmostDescendantIndex {get;}
		/// <summary>
		/// Returns the in degree of the node (number of incoming edges of all types of edges).
		/// </summary>
		/// <returns> the in degree of the node (number of incoming edges of all types of edges). </returns>
		int InDegree {get;}
		/// <summary>
		/// Returns the out degree of the node (number of outgoing edges of all types of edges).
		/// </summary>
		/// <returns> the out degree of the node (number of outgoing edges of all types of edges). </returns>
		int OutDegree {get;}
		/// <summary>
		/// Returns a sorted set of incoming secondary edges.
		/// </summary>
		/// <returns> a sorted set of incoming secondary edges. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.SortedSet<org.maltparser.core.syntaxgraph.edge.Edge> getIncomingSecondaryEdges() throws org.maltparser.core.exception.MaltChainedException;
		SortedSet<Edge> IncomingSecondaryEdges {get;}
		/// <summary>
		/// Returns a sorted set of outgoing secondary edges.
		/// </summary>
		/// <returns> a sorted set of outgoing secondary edges. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.SortedSet<org.maltparser.core.syntaxgraph.edge.Edge> getOutgoingSecondaryEdges() throws org.maltparser.core.exception.MaltChainedException;
		SortedSet<Edge> OutgoingSecondaryEdges {get;}
	}

}