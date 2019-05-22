namespace NMaltParser.Core.SyntaxGraph.Node
{
	public interface TokenNode : DependencyNode, PhraseStructureNode
	{
		/// <summary>
		/// Sets the predecessor token node in the linear order of the token nodes.
		/// </summary>
		/// <param name="predecessor"> the predecessor token node </param>
		TokenNode Predecessor {set;}
		/// <summary>
		/// Sets the predecessor token node in the linear order of the token nodes.
		/// </summary>
		/// <param name="successor"> the successor token node </param>
		TokenNode Successor {set;}
		/// <summary>
		/// Returns the predecessor token node in the linear order of the token nodes.
		/// </summary>
		/// <returns> the predecessor token node in the linear order of the token nodes. </returns>
		TokenNode TokenNodePredecessor {get;}
		/// <summary>
		/// Returns the successor token node in the linear order of the token nodes.
		/// </summary>
		/// <returns> the successor token node in the linear order of the token nodes. </returns>
		TokenNode TokenNodeSuccessor {get;}
	}

}