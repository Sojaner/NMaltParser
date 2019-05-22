using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.lw.graph
{

	using ColumnDescription = org.maltparser.concurrent.graph.dataformat.ColumnDescription;
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using LabelSet = org.maltparser.core.syntaxgraph.LabelSet;
	using LabeledStructure = org.maltparser.core.syntaxgraph.LabeledStructure;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;
	using Node = org.maltparser.core.syntaxgraph.node.Node;

	/// <summary>
	/// A lightweight version of org.maltparser.core.syntaxgraph.edge.GraphEdge.
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class LWEdge : Edge, IComparable<LWEdge>
	{
		private readonly Node source;
		private readonly Node target;
		private readonly SortedDictionary<ColumnDescription, string> labels;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected LWEdge(LWEdge edge) throws LWGraphException
		protected internal LWEdge(LWEdge edge)
		{
			this.source = edge.source;
			this.target = edge.target;
			this.labels = new SortedDictionary<ColumnDescription, string>(edge.labels);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected LWEdge(org.maltparser.core.syntaxgraph.node.Node _source, org.maltparser.core.syntaxgraph.node.Node _target, java.util.SortedMap<org.maltparser.concurrent.graph.dataformat.ColumnDescription, String> _labels) throws org.maltparser.core.exception.MaltChainedException
		protected internal LWEdge(Node _source, Node _target, SortedDictionary<ColumnDescription, string> _labels)
		{
			if (_source.BelongsToGraph != _target.BelongsToGraph)
			{
			throw new LWGraphException("The source node and target node must belong to the same dependency graph.");
			}
			this.source = _source;
			this.target = _target;
			this.labels = _labels;
			SymbolTableHandler symbolTableHandler = BelongsToGraph.SymbolTables;
			foreach (ColumnDescription column in labels.Keys)
			{
				SymbolTable table = symbolTableHandler.addSymbolTable(column.Name);
				table.addSymbol(labels[column]);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected LWEdge(org.maltparser.core.syntaxgraph.node.Node _source, org.maltparser.core.syntaxgraph.node.Node _target) throws org.maltparser.core.exception.MaltChainedException
		protected internal LWEdge(Node _source, Node _target)
		{
			if (_source.BelongsToGraph != _target.BelongsToGraph)
			{
				throw new LWGraphException("The source node and target node must belong to the same dependency graph.");
			}
			this.source = _source;
			this.target = _target;
			this.labels = new SortedDictionary<ColumnDescription, string>();
		}

		public Node Source
		{
			get
			{
				return source;
			}
		}

		public Node Target
		{
			get
			{
				return target;
			}
		}

		public string getLabel(ColumnDescription column)
		{
			if (labels.ContainsKey(column))
			{
				return labels[column];
			}
			else if (column.Category == ColumnDescription.IGNORE)
			{
				return column.DefaultOutput;
			}
			return "";
		}

		public int nLabels()
		{
			return labels.Count;
		}

		public bool Labeled
		{
			get
			{
				return labels.Count > 0;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setEdge(org.maltparser.core.syntaxgraph.node.Node source, org.maltparser.core.syntaxgraph.node.Node target, int type) throws org.maltparser.core.exception.MaltChainedException
		public void setEdge(Node source, Node target, int type)
		{
			throw new LWGraphException("Not implemented in light-weight dependency graph");
		}

		public int Type
		{
			get
			{
				return org.maltparser.core.syntaxgraph.edge.Edge_Fields.DEPENDENCY_EDGE;
			}
		}


		/// <summary>
		/// Adds a label (a string value) to the symbol table and to the graph element. 
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <param name="symbol"> a label symbol </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.symbol.SymbolTable table, String symbol) throws org.maltparser.core.exception.MaltChainedException
		public void addLabel(SymbolTable table, string symbol)
		{
			LWDependencyGraph graph = (LWDependencyGraph)BelongsToGraph;
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			table.addSymbol(symbol);
			labels[column] = symbol;
		}

		/// <summary>
		/// Adds a label (an integer value) to the symbol table and to the graph element.
		/// </summary>
		/// <param name="table"> the symbol table </param>
		/// <param name="code"> a label code </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.symbol.SymbolTable table, int code) throws org.maltparser.core.exception.MaltChainedException
		public void addLabel(SymbolTable table, int code)
		{
			addLabel(table, table.getSymbolCodeToString(code));
		}

		/// <summary>
		/// Adds the labels of the label set to the label set of the graph element.
		/// </summary>
		/// <param name="labelSet"> a label set. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLabel(org.maltparser.core.syntaxgraph.LabelSet labelSet) throws org.maltparser.core.exception.MaltChainedException
		public void addLabel(LabelSet labelSet)
		{
			foreach (SymbolTable table in labelSet.Keys)
			{
				addLabel(table, labelSet.get(table));
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
		public bool hasLabel(SymbolTable table)
		{
			if (table == null)
			{
				return false;
			}
			LWDependencyGraph graph = (LWDependencyGraph)BelongsToGraph;
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			return labels.ContainsKey(column);
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
		public string getLabelSymbol(SymbolTable table)
		{
			LWDependencyGraph graph = (LWDependencyGraph)BelongsToGraph;
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			return labels[column];
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
		public int getLabelCode(SymbolTable table)
		{
			LWDependencyGraph graph = (LWDependencyGraph)BelongsToGraph;
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			return table.getSymbolStringToCode(labels[column]);
		}

		/// <summary>
		/// Returns a set of symbol tables (labeling functions or label types) that labels the graph element.
		/// </summary>
		/// <returns> a set of symbol tables (labeling functions or label types) </returns>
		public ISet<SymbolTable> LabelTypes
		{
			get
			{
				ISet<SymbolTable> labelTypes = new System.Collections.Generic.HashSet<SymbolTable>();
				SymbolTableHandler symbolTableHandler = BelongsToGraph.SymbolTables;
				foreach (ColumnDescription column in labels.Keys)
				{
					try
					{
						labelTypes.Add(symbolTableHandler.getSymbolTable(column.Name));
					}
					catch (MaltChainedException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
				return labelTypes;
			}
		}

		/// <summary>
		/// Returns the label set.
		/// </summary>
		/// <returns> the label set. </returns>
		public LabelSet LabelSet
		{
			get
			{
				SymbolTableHandler symbolTableHandler = BelongsToGraph.SymbolTables;
				LabelSet labelSet = new LabelSet();
    
				foreach (ColumnDescription column in labels.Keys)
				{
					try
					{
						SymbolTable table = symbolTableHandler.getSymbolTable(column.Name);
						int code = table.getSymbolStringToCode(labels[column]);
						labelSet.put(table, code);
					}
					catch (MaltChainedException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
				return labelSet;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeLabel(org.maltparser.core.symbol.SymbolTable table) throws org.maltparser.core.exception.MaltChainedException
		public void removeLabel(SymbolTable table)
		{
			LWDependencyGraph graph = (LWDependencyGraph)BelongsToGraph;
			ColumnDescription column = graph.DataFormat.getColumnDescription(table.Name);
			labels.Remove(column);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeLabels() throws org.maltparser.core.exception.MaltChainedException
		public void removeLabels()
		{
			labels.Clear();
		}

		/// <summary>
		/// Returns the graph (structure) in which the graph element belongs to. 
		/// </summary>
		/// <returns> the graph (structure) in which the graph element belongs to.  </returns>
		public LabeledStructure BelongsToGraph
		{
			get
			{
				return target.BelongsToGraph;
			}
			set
			{
			}
		}



		/// <summary>
		/// Resets the graph element.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public void clear()
		{
			labels.Clear();
		}

		public int CompareTo(LWEdge that)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;

			if (this == that)
			{
				return EQUAL;
			}

			if (this.target.Index < that.target.Index)
			{
				return BEFORE;
			}
			if (this.target.Index > that.target.Index)
			{
				return AFTER;
			}

			if (this.source.Index < that.source.Index)
			{
				return BEFORE;
			}
			if (this.source.Index > that.source.Index)
			{
				return AFTER;
			}


			if (this.labels.Equals(that.labels))
			{
				return EQUAL;
			}

			IEnumerator<ColumnDescription> itthis = this.labels.Keys.GetEnumerator();
			IEnumerator<ColumnDescription> itthat = that.labels.Keys.GetEnumerator();
			while (itthis.MoveNext() && itthat.MoveNext())
			{
				ColumnDescription keythis = itthis.Current;
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ColumnDescription keythat = itthat.next();
				if (keythis.Position < keythat.Position)
				{
					return BEFORE;
				}
				if (keythis.Position > keythat.Position)
				{
					return AFTER;
				}
				if (this.labels[keythis].compareTo(that.labels[keythat]) != EQUAL)
				{
					return this.labels[keythis].compareTo(that.labels[keythat]);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (itthis.hasNext() == false && itthat.hasNext() == true)
			{
				return BEFORE;
			}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (itthis.hasNext() == true && itthat.hasNext() == false)
			{
				return AFTER;
			}


			return EQUAL;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + source.Index;
			result = prime * result + target.Index;
			result = prime * result + ((labels == null) ? 0 : labels.GetHashCode());
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			LWEdge other = (LWEdge) obj;
			if (source.Index != other.source.Index)
			{
				return false;
			}
			if (target.Index != other.target.Index)
			{
				return false;
			}
			if (labels == null)
			{
				if (other.labels != null)
				{
					return false;
				}
			}
			else if (!labels.Equals(other.labels))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(source);
			sb.Append(" -> ");
			sb.Append(target);
			if (labels.Count > 0)
			{
				int i = 1;
				sb.Append(" {");
				foreach (ColumnDescription column in labels.Keys)
				{
					sb.Append(column.Name);
					sb.Append('=');
					sb.Append(labels[column]);
					if (i < labels.Count)
					{
						sb.Append(',');
					}
					i++;
				}
				sb.Append(" }");
			}
			return sb.ToString();
		}
	}

}