using NMaltParser.Core.Exception;
using NMaltParser.Core.Symbol;

namespace NMaltParser.Core.SyntaxGraph
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface ILabeledStructure
	{
		/// <summary>
		/// Returns the symbol table handler. 
		/// </summary>
		/// <returns> the symbol table handler.  </returns>
		SymbolTableHandler SymbolTables {get;set;}

		/// <summary>
		/// Adds a label <i>label</i> to the graph element <i>element</i>
		/// </summary>
		/// <param name="element"> a graph element <i>element</i> (a node or a edge). </param>
		/// <param name="tableName"> the name of the symbol table. </param>
		/// <param name="label"> the string value of the label. </param>
		/// <exception cref="MaltChainedException"> </exception>
		void AddLabel(Element element, string tableName, string label);

		/// <summary>
		/// Checks out a new label set from the structure.
		/// </summary>
		/// <returns> a new label set. </returns>
		/// <exception cref="MaltChainedException"> </exception>
		LabelSet CheckOutNewLabelSet();

		/// <summary>
		/// Checks in a label set. 
		/// </summary>
		/// <param name="labelSet"> a label set. </param>
		/// <exception cref="MaltChainedException"> </exception>
		void CheckInLabelSet(LabelSet labelSet);

		/// <summary>
		/// Resets the structure.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
		void Clear();
	}

}