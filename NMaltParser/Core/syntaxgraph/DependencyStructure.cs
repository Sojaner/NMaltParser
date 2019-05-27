using System.Collections.Generic;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph
{
    /// <summary>
    /// @author Johan Hall
    /// </summary>
	public interface IDependencyStructure : ITokenStructure, SecEdgeStructure
	{
        DependencyNode AddDependencyNode();

		DependencyNode AddDependencyNode(int index);
		/// <summary>
		/// Returns the dependency node identified by <i>index</i> if it exists, otherwise <i>null</i>.
		/// </summary>
		/// <param name="index"> the index of the dependency node </param>
		/// <returns> the dependency node identified by <i>index</i> if it exists, otherwise <i>null</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		DependencyNode GetDependencyNode(int index);

		int NDependencyNode();


		int HighestDependencyNodeIndex {get;}

		/// <summary>
		/// Adds an edge from the head to the dependent identified by the indices of the dependency nodes.
		/// </summary>
		/// <param name="headIndex"> the index of the head dependency node </param>
		/// <param name="dependentIndex"> the index of the dependent dependency node </param>
		/// <returns> the edge that have been added. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		Edge.Edge AddDependencyEdge(int headIndex, int dependentIndex);

		/// <summary>
		/// Replace the head of the dependent with a new head. The labels are not affected.
		/// </summary>
		/// <param name="newHeadIndex"> the index of the new head dependency node </param>
		/// <param name="dependentIndex"> the index of the dependent dependency node </param>
		/// <returns> the edge that have been moved. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		Edge.Edge MoveDependencyEdge(int newHeadIndex, int dependentIndex);

		/// <summary>
		/// Remove an edge from the head to the dependent identified by the indices of the dependency nodes. </summary>
		/// <param name="headIndex"> the index of the head dependency node </param>
		/// <param name="dependentIndex"> the index of the dependent dependency node </param>
		/// <exception cref="MaltChainedException"> </exception>
		void RemoveDependencyEdge(int headIndex, int dependentIndex);

		/// <summary>
		/// Returns the number of edges
		/// </summary>
		/// <returns>  the number of edges </returns>
		int NEdges();

		SortedSet<Edge.Edge> Edges {get;}

		/// <summary>
		/// Returns a sorted set of integers {0,s,..n} , where each index i identifies a dependency node. Index 0
		/// should always be the root dependency node and index s is the first terminal node and index n is the
		/// last terminal node.  
		/// </summary>
		/// <returns> a sorted set of integers </returns>
		SortedSet<int> DependencyIndices {get;}

		/// <summary>
		/// Returns the root of the dependency structure.
		/// </summary>
		/// <returns> the root of the dependency structure. </returns>
		DependencyNode DependencyRoot {get;}

		/// <summary>
		/// Returns <i>true</i> if the head edge of the dependency node with <i>index</i> is labeled, otherwise <i>false</i>.
		/// </summary>
		/// <param name="index"> the index of the dependency node </param>
		/// <returns> <i>true</i> if the head edge of the dependency node with <i>index</i> is labeled, otherwise <i>false</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		bool HasLabeledDependency(int index);

		/// <summary>
		/// Returns <i>true</i> if all nodes in the dependency structure are connected, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if all nodes in the dependency structure are connected, otherwise <i>false</i>. </returns>
		bool Connected {get;}

		/// <summary>
		/// Returns <i>true</i> if all edges in the dependency structure are projective, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if all edges in the dependency structure are projective, otherwise <i>false</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		bool Projective {get;}

		/// <summary>
		/// Returns <i>true</i> if all dependency nodes have at most one incoming edge, otherwise <i>false</i>.
		/// </summary>
		/// <returns>  <i>true</i> if all dependency nodes have at most one incoming edge, otherwise <i>false</i>. </returns>
		bool SingleHeaded {get;}

		/// <summary>
		/// Returns <i>true</i> if the dependency structure are a tree (isConnected() && isSingleHeaded()), otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the dependency structure are a tree (isConnected() && isSingleHeaded()), otherwise <i>false</i>. </returns>
		bool Tree {get;}

		/// <summary>
		/// Returns the number of non-projective edges in the dependency structure.
		/// </summary>
		/// <returns> the number of non-projective edges in the dependency structure. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		int NNonProjectiveEdges();

		/// <summary>
		/// Links all subtrees to the root of the dependency structure.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
		void LinkAllTreesToRoot();
        
		LabelSet DefaultRootEdgeLabels {get;}

		/// <summary>
		/// Returns the default edge label of the root as a string value.
		/// </summary>
		/// <param name="table"> the symbol table that identifies the label type. </param>
		/// <returns> the default edge label of the root. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		string GetDefaultRootEdgeLabelSymbol(SymbolTable table);

		/// <summary>
		/// Returns the default edge label of the root as an integer value.
		/// </summary>
		/// <param name="table"> the symbol table that identifies the label type. </param>
		/// <returns> the default edge label of the root as an integer value. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		int GetDefaultRootEdgeLabelCode(SymbolTable table);

		/// <summary>
		/// Sets the default edge label of the root.
		/// </summary>
		/// <param name="table"> the symbol table that identifies the label type. </param>
		/// <param name="defaultRootSymbol"> the default root edge label </param>
		/// <exception cref="MaltChainedException"> </exception>
		void SetDefaultRootEdgeLabel(SymbolTable table, string defaultRootSymbol);

		/// <summary>
		/// Sets the default edge label of the root according to the default root label option
		/// </summary>
		/// <param name="rootLabelOption"> the default root label option </param>
		/// <param name="edgeSymbolTables"> a sorted map that maps the symbol table name to the symbol table object. </param>
		/// <exception cref="MaltChainedException"> </exception>
		void SetDefaultRootEdgeLabels(string rootLabelOption, SortedDictionary<string, SymbolTable> edgeSymbolTables);
	}

}