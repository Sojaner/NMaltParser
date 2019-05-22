using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.syntaxgraph
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.symbol;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public abstract class GraphElement : Observable, Element
	{
		private LabeledStructure belongsToGraph;
		private LabelSet labelSet;

		public GraphElement()
		{
			belongsToGraph = null;
			labelSet = null;
		}

		/// <summary>
		/// Adds a label (a string value) to the symbol table and to the graph element. 
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <param name="symbol"> a label symbol </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.symbol.SymbolTable table, String symbol) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addLabel(SymbolTable table, string symbol)
		{
			table.addSymbol(symbol);
			addLabel(table, table.getSymbolStringToCode(symbol));
		}

		/// <summary>
		/// Adds a label (an integer value) to the symbol table and to the graph element.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <param name="code"> a label code </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.symbol.SymbolTable table, int code) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addLabel(SymbolTable table, int code)
		{
			if (!string.ReferenceEquals(table.getSymbolCodeToString(code), null))
			{
				if (labelSet == null)
				{
					if (belongsToGraph == null)
					{
						throw new SyntaxGraphException("The graph element doesn't belong to any graph. ");
					}
					labelSet = belongsToGraph.checkOutNewLabelSet();
				}
				labelSet.put(table, code);
				setChanged();
				notifyObservers(table);
			}
		}

		/// <summary>
		/// Adds the labels of the label set to the label set of the graph element.
		/// </summary>
		/// <param name="labels"> a label set. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(LabelSet labels) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addLabel(LabelSet labels)
		{
			if (labels != null)
			{
				foreach (SymbolTable table in labels.Keys)
				{
					addLabel(table, labels.get(table));
				}
			}
		}

		/// <summary>
		/// Returns <i>true</i> if the graph element has a label for the symbol table, otherwise <i>false</i>.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <returns> <i>true</i> if the graph element has a label for the symbol table, otherwise <i>false</i>. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool hasLabel(SymbolTable table)
		{
			if (labelSet != null)
			{
				return labelSet.containsKey(table);
			}
			return false;
		}

		/// <summary>
		/// Returns the label symbol(a string representation) of the symbol table if it exists, otherwise 
		/// an exception is thrown.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <returns> the label (a string representation) of the symbol table if it exists. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getLabelSymbol(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getLabelSymbol(SymbolTable table)
		{
			int? code = labelSet.get(table);
			if (code == null)
			{
				throw new SyntaxGraphException("No label symbol available for label '" + table.Name + "'.");
			}
			return table.getSymbolCodeToString(code.Value);
		}

		/// <summary>
		/// Returns the label code (an integer representation) of the symbol table if it exists, otherwise 
		/// an exception is thrown.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <returns> the label code (an integer representation) of the symbol table if it exists </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getLabelCode(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual int getLabelCode(SymbolTable table)
		{
			int? code = labelSet.get(table);
			if (code == null)
			{
				throw new SyntaxGraphException("No label symbol available for label '" + table.Name + "'.");
			}
	//		if (code != null) {
	//			return -1;
	//		}
			return code.Value;
		}

		/// <summary>
		/// Returns <i>true</i> if the graph element has one or more labels, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the graph element has one or more labels, otherwise <i>false</i>. </returns>
		public virtual bool Labeled
		{
			get
			{
				if (labelSet == null)
				{
					return false;
				}
				return labelSet.size() > 0;
			}
		}

		/// <summary>
		/// Returns the number of labels of the graph element.
		/// </summary>
		/// <returns> the number of labels of the graph element. </returns>
		public virtual int nLabels()
		{
			if (labelSet == null)
			{
				return 0;
			}
			return labelSet.size();
		}

		/// <summary>
		/// Returns a set of symbol tables (labeling functions or label types) that labels the graph element.
		/// </summary>
		/// <returns> a set of symbol tables (labeling functions or label types) </returns>
		public virtual ISet<SymbolTable> LabelTypes
		{
			get
			{
				if (labelSet == null)
				{
					return new LinkedHashSet<SymbolTable>();
				}
				return labelSet.Keys;
			}
		}

		/// <summary>
		/// Returns the label set.
		/// </summary>
		/// <returns> the label set. </returns>
		public virtual LabelSet LabelSet
		{
			get
			{
				return labelSet;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public virtual void removeLabel(SymbolTable table)
		{
			if (labelSet != null)
			{
				labelSet.remove(table);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeLabels() throws org.maltparser.core.exception.MaltChainedException
		public virtual void removeLabels()
		{
			if (labelSet != null && belongsToGraph != null)
			{
				belongsToGraph.checkInLabelSet(labelSet);
			}
			labelSet = null;
		}

		/// <summary>
		/// Returns the graph (structure) in which the graph element belongs to. 
		/// </summary>
		/// <returns> the graph (structure) in which the graph element belongs to.  </returns>
		public virtual LabeledStructure BelongsToGraph
		{
			get
			{
				return belongsToGraph;
			}
			set
			{
				this.belongsToGraph = value;
				addObserver((SyntaxGraph)value);
			}
		}



		/// <summary>
		/// Resets the graph element.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public virtual void clear()
		{
			if (labelSet != null && belongsToGraph != null)
			{
				belongsToGraph.checkInLabelSet(labelSet);
			}
			labelSet = null;
			deleteObserver((SyntaxGraph)belongsToGraph);
			belongsToGraph = null;
		}

		public override bool Equals(object obj)
		{
			GraphElement ge = (GraphElement)obj;
			return belongsToGraph == ge.BelongsToGraph && labelSet == null ? ge.LabelSet == null : labelSet.Equals(ge.LabelSet);
		}

		public override int GetHashCode()
		{
			int hash = 7;
			hash = 31 * hash + (null == belongsToGraph ? 0 : belongsToGraph.GetHashCode());
			return 31 * hash + (null == labelSet ? 0 : labelSet.GetHashCode());
		}

		public virtual int compareTo(GraphElement o)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;
			if (this == o)
			{
				return EQUAL;
			}

			if (this.labelSet == null && o.labelSet != null)
			{
				return BEFORE;
			}
			if (this.labelSet != null && o.labelSet == null)
			{
				return AFTER;
			}
			if (this.labelSet == null && o.labelSet == null)
			{
				return EQUAL;
			}

			int comparison = EQUAL;
			foreach (SymbolTable table in labelSet.Keys)
			{
				int? ocode = o.labelSet.get(table);
				int? tcode = labelSet.get(table);
				if (ocode != null && tcode != null)
				{
					if (!ocode.Equals(tcode))
					{
						try
						{
							comparison = table.getSymbolCodeToString(tcode.Value).CompareTo(table.getSymbolCodeToString(ocode.Value));
							if (comparison != EQUAL)
							{
								return comparison;
							}
						}
						catch (MaltChainedException)
						{
						}
					}
				}
			}
			return EQUAL;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			if (labelSet != null)
			{
				foreach (SymbolTable table in labelSet.Keys)
				{
					try
					{
						sb.Append(table.Name);
						sb.Append(':');
						sb.Append(getLabelSymbol(table));
					}
					catch (MaltChainedException e)
					{
						Console.Error.WriteLine("Print error : " + e.MessageChain);
					}
					sb.Append(' ');
				}
			}
			return sb.ToString();
		}
	}

}