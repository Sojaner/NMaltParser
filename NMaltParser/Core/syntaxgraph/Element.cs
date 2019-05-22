using System.Collections.Generic;

namespace org.maltparser.core.syntaxgraph
{

	using  exception;
	using  symbol;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface Element
	{
		/// <summary>
		/// Adds a label (a string value) to the symbol table and to the graph element. 
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <param name="symbol"> a label symbol </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.symbol.SymbolTable table, String symbol) throws org.maltparser.core.exception.MaltChainedException;
		void addLabel(SymbolTable table, string symbol);
		/// <summary>
		/// Adds a label (an integer value) to the symbol table and to the graph element.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <param name="code"> a label code </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.symbol.SymbolTable table, int code) throws org.maltparser.core.exception.MaltChainedException;
		void addLabel(SymbolTable table, int code);
		/// <summary>
		/// Adds the labels of the label set to the label set of the graph element.
		/// </summary>
		/// <param name="labelSet"> a label set. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(LabelSet labelSet) throws org.maltparser.core.exception.MaltChainedException;
		void addLabel(LabelSet labelSet);
		/// <summary>
		/// Returns <i>true</i> if the graph element has a label for the symbol table, otherwise <i>false</i>.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <returns> <i>true</i> if the graph element has a label for the symbol table, otherwise <i>false</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		bool hasLabel(SymbolTable table);
		/// <summary>
		/// Returns the label symbol(a string representation) of the symbol table if it exists, otherwise 
		/// an exception is thrown.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <returns> the label (a string representation) of the symbol table if it exists. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		string getLabelSymbol(SymbolTable table);
		/// <summary>
		/// Returns the label code (an integer representation) of the symbol table if it exists, otherwise 
		/// an exception is thrown.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <returns> the label code (an integer representation) of the symbol table if it exists </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		int getLabelCode(SymbolTable table);
		/// <summary>
		/// Returns <i>true</i> if the graph element has one or more labels, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the graph element has one or more labels, otherwise <i>false</i>. </returns>
		bool Labeled {get;}
		/// <summary>
		/// Returns the number of labels of the graph element.
		/// </summary>
		/// <returns> the number of labels of the graph element. </returns>
		int nLabels();
		/// <summary>
		/// Returns a set of symbol tables (labeling functions or label types) that labels the graph element.
		/// </summary>
		/// <returns> a set of symbol tables (labeling functions or label types) </returns>
		ISet<SymbolTable> LabelTypes {get;}
		/// <summary>
		/// Returns the label set.
		/// </summary>
		/// <returns> the label set. </returns>
		LabelSet LabelSet {get;}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException;
		void removeLabel(SymbolTable table);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeLabels() throws org.maltparser.core.exception.MaltChainedException;
		void removeLabels();

		/// <summary>
		/// Returns the graph (structure) in which the graph element belongs to. 
		/// </summary>
		/// <returns> the graph (structure) in which the graph element belongs to.  </returns>
		LabeledStructure BelongsToGraph {get;set;}
		/// <summary>
		/// Resets the graph element.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException;
		void clear();
	}

}