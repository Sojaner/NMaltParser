using System.Collections.Generic;
using NMaltParser.Core.Exception;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph
{
    /// <inheritdoc />
    /// <summary>
    /// @author Johan Hall
    /// </summary>
	public interface ITokenStructure : ILabeledStructure
	{
		/// <summary>
		/// Adds a token node with index <i>n + 1</i>, where <i>n</i> is the index of the last token node. 
		/// </summary>
		/// <returns> the added token node. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		TokenNode AddTokenNode();

		/// <summary>
		/// Adds a token node with index <i>index</i>.
		/// </summary>
		/// <param name="index"> the index of the token node. </param>
		/// <returns> the added token node. </returns>
		TokenNode AddTokenNode(int index);

		void AddComment(string comment, int atIndex);

		List<string> GetComment(int atIndex);

		bool HasComments();

		/// <summary>
		/// Returns the token node with index <i>index</i>.
		/// </summary>
		/// <param name="index"> the index of the token node. </param>
		/// <returns> a token node with index <i>index</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		TokenNode GetTokenNode(int index);

		/// <summary>
		/// Returns the number of token nodes in the token structure (sentence).
		/// </summary>
		/// <returns> the number of token nodes in the token structure (sentence). </returns>
		int NTokenNode();

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
		bool HasTokens();

		/// <summary>
		/// Returns the sentence ID
		/// </summary>
		/// <returns> the sentence ID </returns>
		int SentenceID {get;set;}
	}

}