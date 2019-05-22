using System.Collections.Generic;

namespace org.maltparser.core.syntaxgraph
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.syntaxgraph.node;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface TokenStructure : LabeledStructure
	{
		/// <summary>
		/// Adds a token node with index <i>n + 1</i>, where <i>n</i> is the index of the last token node. 
		/// </summary>
		/// <returns> the added token node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.TokenNode addTokenNode() throws org.maltparser.core.exception.MaltChainedException;
		TokenNode addTokenNode();
		/// <summary>
		/// Adds a token node with index <i>index</i>.
		/// </summary>
		/// <param name="index"> the index of the token node. </param>
		/// <returns> the added token node. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.TokenNode addTokenNode(int index) throws org.maltparser.core.exception.MaltChainedException;
		TokenNode addTokenNode(int index);

		void addComment(string comment, int at_index);
		List<string> getComment(int at_index);
		bool hasComments();
		/// <summary>
		/// Returns the token node with index <i>index</i>.
		/// </summary>
		/// <param name="index"> the index of the token node. </param>
		/// <returns> a token node with index <i>index</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		TokenNode getTokenNode(int index);
		/// <summary>
		/// Returns the number of token nodes in the token structure (sentence).
		/// </summary>
		/// <returns> the number of token nodes in the token structure (sentence). </returns>
		int nTokenNode();
		/// <summary>
		/// Returns a sorted set of integers {s,...,n}, where each index i identifies a token node. Index <i>s</i> 
		/// is the first token node and index <i>n</i> is the last token node. 
		/// </summary>
		/// <returns> a sorted set of integers {s,...,n}. </returns>
		SortedSet<int> TokenIndices {get;}
		/// <summary>
		/// Returns the index of the last token node.
		/// </summary>
		/// <returns> the index of the last token node. </returns>
		int HighestTokenIndex {get;}
		/// <summary>
		///  Returns <i>true</i> if the token structure (sentence) has any token nodes, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the token structure (sentence) has any token nodes, otherwise <i>false</i>. </returns>
		bool hasTokens();
		/// <summary>
		/// Returns the sentence ID
		/// </summary>
		/// <returns> the sentence ID </returns>
		int SentenceID {get;set;}
	}

}