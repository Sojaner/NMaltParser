using System.Collections.Generic;

namespace org.maltparser.core.syntaxgraph.node
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;



	public interface DependencyNode : ComparableNode
	{
		/// <summary>
		/// Returns <i>true</i> if the node has at most one head, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has at most one head, otherwise <i>false</i>. </returns>
		bool hasAtMostOneHead();
		/// <summary>
		/// Returns <i>true</i> if the node has one or more head(s), otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more head(s), otherwise <i>false</i>. </returns>
		bool hasHead();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<DependencyNode> getHeads() throws org.maltparser.core.exception.MaltChainedException;
		ISet<DependencyNode> Heads {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<org.maltparser.core.syntaxgraph.edge.Edge> getHeadEdges() throws org.maltparser.core.exception.MaltChainedException;
		ISet<Edge> HeadEdges {get;}


		/// <summary>
		/// Returns the predecessor dependency node in the linear order of the token nodes.
		/// </summary>
		/// <returns> the predecessor dependency node in the linear order of the token nodes. </returns>
		DependencyNode Predecessor {get;}
		/// <summary>
		/// Returns the successor dependency node in the linear order of the token nodes.
		/// </summary>
		/// <returns> the successor dependency node in the linear order of the token nodes. </returns>
		DependencyNode Successor {get;}

		/// <summary>
		/// Returns the head dependency node if it exists, otherwise <i>null</i>. If there exists more
		/// than one head the first head is returned according to the linear order of the terminals 
		/// or the root if it is one of the heads.
		/// </summary>
		/// <returns> the head dependency node if it exists, otherwise <i>null</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getHead() throws org.maltparser.core.exception.MaltChainedException;
		DependencyNode Head {get;}
		/// <summary>
		/// Returns the edge between the head and the node if it exists, otherwise <i>null</i>. If there exists more
		/// than one head edge the first head edge is returned according to the linear order of the terminals 
		/// or the root if it is one of the heads.
		/// </summary>
		/// <returns> the edge between the head and the node if it exists, otherwise <i>null</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.edge.Edge getHeadEdge() throws org.maltparser.core.exception.MaltChainedException;
		Edge HeadEdge {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasAncestorInside(int left, int right) throws org.maltparser.core.exception.MaltChainedException;
		bool hasAncestorInside(int left, int right);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table, String symbol) throws org.maltparser.core.exception.MaltChainedException;
		void addHeadEdgeLabel(SymbolTable table, string symbol);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table, int code) throws org.maltparser.core.exception.MaltChainedException;
		void addHeadEdgeLabel(SymbolTable table, int code);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addHeadEdgeLabel(org.maltparser.core.syntaxgraph.LabelSet labelSet) throws org.maltparser.core.exception.MaltChainedException;
		void addHeadEdgeLabel(LabelSet labelSet);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasHeadEdgeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		bool hasHeadEdgeLabel(SymbolTable table);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getHeadEdgeLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		string getHeadEdgeLabelSymbol(SymbolTable table);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getHeadEdgeLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		int getHeadEdgeLabelCode(SymbolTable table);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isHeadEdgeLabeled() throws org.maltparser.core.exception.MaltChainedException;
		bool HeadEdgeLabeled {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nHeadEdgeLabels() throws org.maltparser.core.exception.MaltChainedException;
		int nHeadEdgeLabels();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Set<org.maltparser.core.symbol.SymbolTable> getHeadEdgeLabelTypes() throws org.maltparser.core.exception.MaltChainedException;
		ISet<SymbolTable> HeadEdgeLabelTypes {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.LabelSet getHeadEdgeLabelSet() throws org.maltparser.core.exception.MaltChainedException;
		LabelSet HeadEdgeLabelSet {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getAncestor() throws org.maltparser.core.exception.MaltChainedException;
		DependencyNode Ancestor {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getProperAncestor() throws org.maltparser.core.exception.MaltChainedException;
		DependencyNode ProperAncestor {get;}

		bool hasDependent();
		/// <summary>
		/// Returns <i>true</i> if the node has one or more left dependents, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the node has one or more left dependents, otherwise <i>false</i>. </returns>
		bool hasLeftDependent();
		/// <summary>
		/// Returns the left dependent at the position <i>index</i>, where <i>index==0</i> equals the left most dependent.
		/// </summary>
		/// <param name="index"> the index </param>
		/// <returns> the left dependent at the position <i>index</i>, where <i>index==0</i> equals the left most dependent </returns>
		DependencyNode getLeftDependent(int index);
		/// <summary>
		/// Return the number of left dependents
		/// </summary>
		/// <returns> the number of left dependents </returns>
		int LeftDependentCount {get;}
		/// <summary>
		/// Returns a sorted set of left dependents.
		/// </summary>
		/// <returns> a sorted set of left dependents. </returns>
		SortedSet<DependencyNode> LeftDependents {get;}
		/// <summary>
		/// Returns the left sibling if it exists, otherwise <code>null</code>
		/// </summary>
		/// <returns> the left sibling if it exists, otherwise <code>null</code> </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getLeftSibling() throws org.maltparser.core.exception.MaltChainedException;
		DependencyNode LeftSibling {get;}
		/// <summary>
		/// Returns the left sibling at the same side of head as the node it self. If not found <code>null</code> is returned
		/// </summary>
		/// <returns> the left sibling at the same side of head as the node it self. If not found <code>null</code> is returned </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getSameSideLeftSibling() throws org.maltparser.core.exception.MaltChainedException;
		DependencyNode SameSideLeftSibling {get;}
		/// <summary>
		/// Returns the closest left dependent to the node it self, if not found <code>null</code> is returned.
		/// </summary>
		/// <returns> the closest left dependent to the node it self, if not found <code>null</code> is returned. </returns>
		DependencyNode ClosestLeftDependent {get;}
		DependencyNode LeftmostDependent {get;}
		DependencyNode getRightDependent(int index);
		/// <summary>
		/// Return the number of right dependents
		/// </summary>
		/// <returns> the number of right dependents </returns>
		int RightDependentCount {get;}
		/// <summary>
		/// Returns a sorted set of right dependents.
		/// </summary>
		/// <returns> a sorted set of right dependents. </returns>
		SortedSet<DependencyNode> RightDependents {get;}
		/// <summary>
		/// Returns the right sibling if it exists, otherwise <code>null</code>
		/// </summary>
		/// <returns> the right sibling if it exists, otherwise <code>null</code> </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getRightSibling() throws org.maltparser.core.exception.MaltChainedException;
		DependencyNode RightSibling {get;}
		/// <summary>
		/// Returns the right sibling at the same side of head as the node it self. If not found <code>null</code> is returned
		/// </summary>
		/// <returns> the right sibling at the same side of head as the node it self. If not found <code>null</code> is returned </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DependencyNode getSameSideRightSibling() throws org.maltparser.core.exception.MaltChainedException;
		DependencyNode SameSideRightSibling {get;}
		/// <summary>
		/// Returns the closest right dependent to the node it self, if not found <code>null</code> is returned.
		/// </summary>
		/// <returns> the closest right dependent to the node it self, if not found <code>null</code> is returned. </returns>
		DependencyNode ClosestRightDependent {get;}
		DependencyNode RightmostDependent {get;}
		bool hasRightDependent();

		IList<DependencyNode> ListOfDependents {get;}
		IList<DependencyNode> ListOfLeftDependents {get;}
		IList<DependencyNode> ListOfRightDependents {get;}

		/// <summary>
		/// Returns <i>true</i> if the head edge is projective, otherwise <i>false</i>. Undefined if the node has 
		/// more than one head.
		/// </summary>
		/// <returns> <i>true</i> if the head edge is projective, otherwise <i>false</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isProjective() throws org.maltparser.core.exception.MaltChainedException;
		bool Projective {get;}
		/// <summary>
		/// Returns the depth of the node. The root node has the depth 0. </summary>
		/// <returns> the depth of the node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDependencyNodeDepth() throws org.maltparser.core.exception.MaltChainedException;
		int DependencyNodeDepth {get;}
		int Rank {get;set;}
		DependencyNode findComponent();
		DependencyNode Component {get;set;}
	}

}