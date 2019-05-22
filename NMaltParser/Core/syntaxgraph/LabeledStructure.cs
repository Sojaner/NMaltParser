namespace org.maltparser.core.syntaxgraph
{
	using  exception;
	using  symbol;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface LabeledStructure
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(Element element, String tableName, String label) throws org.maltparser.core.exception.MaltChainedException;
		void addLabel(Element element, string tableName, string label);
		/// <summary>
		/// Checks out a new label set from the structure.
		/// </summary>
		/// <returns> a new label set. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LabelSet checkOutNewLabelSet() throws org.maltparser.core.exception.MaltChainedException;
		LabelSet checkOutNewLabelSet();
		/// <summary>
		/// Checks in a label set. 
		/// </summary>
		/// <param name="labelSet"> a label set. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkInLabelSet(LabelSet labelSet) throws org.maltparser.core.exception.MaltChainedException;
		void checkInLabelSet(LabelSet labelSet);
		/// <summary>
		/// Resets the structure.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException;
		void clear();
	}

}