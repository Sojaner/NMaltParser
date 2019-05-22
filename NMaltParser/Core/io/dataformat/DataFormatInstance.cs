using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.io.dataformat
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class DataFormatInstance : IEnumerable<ColumnDescription>
	{
		private readonly SortedSet<ColumnDescription> columnDescriptions;
		private SortedDictionary<string, ColumnDescription> headColumnDescriptions;
		private SortedDictionary<string, ColumnDescription> dependencyEdgeLabelColumnDescriptions;
		private SortedDictionary<string, ColumnDescription> phraseStructureEdgeLabelColumnDescriptions;
		private SortedDictionary<string, ColumnDescription> phraseStructureNodeLabelColumnDescriptions;
		private SortedDictionary<string, ColumnDescription> secondaryEdgeLabelColumnDescriptions;
		private SortedDictionary<string, ColumnDescription> inputColumnDescriptions;
		private SortedDictionary<string, ColumnDescription> ignoreColumnDescriptions;

		private SortedSet<ColumnDescription> headColumnDescriptionSet;
		private SortedSet<ColumnDescription> dependencyEdgeLabelColumnDescriptionSet;
		private SortedSet<ColumnDescription> phraseStructureEdgeLabelColumnDescriptionSet;
		private SortedSet<ColumnDescription> phraseStructureNodeLabelColumnDescriptionSet;
		private SortedSet<ColumnDescription> secondaryEdgeLabelColumnDescriptionSet;
		private SortedSet<ColumnDescription> inputColumnDescriptionSet;
		private SortedSet<ColumnDescription> ignoreColumnDescriptionSet;

		private SortedDictionary<string, SymbolTable> dependencyEdgeLabelSymbolTables;
		private SortedDictionary<string, SymbolTable> phraseStructureEdgeLabelSymbolTables;
		private SortedDictionary<string, SymbolTable> phraseStructureNodeLabelSymbolTables;
		private SortedDictionary<string, SymbolTable> secondaryEdgeLabelSymbolTables;
		private SortedDictionary<string, SymbolTable> inputSymbolTables;

		// Internal
		private SortedDictionary<string, ColumnDescription> internalColumnDescriptions;
		private SortedSet<ColumnDescription> internalColumnDescriptionSet;

		private readonly DataFormatSpecification dataFormarSpec;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DataFormatInstance(java.util.Map<String, DataFormatEntry> entries, org.maltparser.core.symbol.SymbolTableHandler symbolTables, String nullValueStrategy, DataFormatSpecification dataFormarSpec) throws org.maltparser.core.exception.MaltChainedException
		public DataFormatInstance(IDictionary<string, DataFormatEntry> entries, SymbolTableHandler symbolTables, string nullValueStrategy, DataFormatSpecification dataFormarSpec)
		{
			this.columnDescriptions = new SortedSet<ColumnDescription>();
			this.dataFormarSpec = dataFormarSpec;
			createColumnDescriptions(symbolTables, entries, nullValueStrategy);

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ColumnDescription addInternalColumnDescription(org.maltparser.core.symbol.SymbolTableHandler symbolTables, String name, String category, String type, String defaultOutput, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException
		public virtual ColumnDescription addInternalColumnDescription(SymbolTableHandler symbolTables, string name, string category, string type, string defaultOutput, string nullValueStrategy)
		{
			if (internalColumnDescriptions == null)
			{
				internalColumnDescriptions = new SortedDictionary<string, ColumnDescription>();
				internalColumnDescriptionSet = new SortedSet<ColumnDescription>();
			}

			if (!internalColumnDescriptions.ContainsKey(name))
			{
				ColumnDescription internalColumn = new ColumnDescription(name, ColumnDescription.getCategory(category), ColumnDescription.getType(type), defaultOutput, nullValueStrategy, true);
				symbolTables.addSymbolTable(internalColumn.Name, internalColumn.Category, internalColumn.Type, internalColumn.NullValueStrategy);
				internalColumnDescriptions[name] = internalColumn;
				internalColumnDescriptionSet.Add(internalColumn);
				return internalColumn;
			}
			else
			{
				return internalColumnDescriptions[name];
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ColumnDescription addInternalColumnDescription(org.maltparser.core.symbol.SymbolTableHandler symbolTables, String name, int category, int type, String defaultOutput, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException
		public virtual ColumnDescription addInternalColumnDescription(SymbolTableHandler symbolTables, string name, int category, int type, string defaultOutput, string nullValueStrategy)
		{
			if (internalColumnDescriptions == null)
			{
				internalColumnDescriptions = new SortedDictionary<string, ColumnDescription>();
				internalColumnDescriptionSet = new SortedSet<ColumnDescription>();
			}

			if (!internalColumnDescriptions.ContainsKey(name))
			{
				ColumnDescription internalColumn = new ColumnDescription(name, category, type, defaultOutput, nullValueStrategy, true);
				symbolTables.addSymbolTable(internalColumn.Name, internalColumn.Category, internalColumn.Type, internalColumn.NullValueStrategy);
				internalColumnDescriptions[name] = internalColumn;
				internalColumnDescriptionSet.Add(internalColumn);
				return internalColumn;
			}
			else
			{
				return internalColumnDescriptions[name];
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ColumnDescription addInternalColumnDescription(org.maltparser.core.symbol.SymbolTableHandler symbolTables, String name, ColumnDescription column) throws org.maltparser.core.exception.MaltChainedException
		public virtual ColumnDescription addInternalColumnDescription(SymbolTableHandler symbolTables, string name, ColumnDescription column)
		{
			return addInternalColumnDescription(symbolTables, name, column.Category, column.Type, column.DefaultOutput, column.NullValueStrategy);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void createColumnDescriptions(org.maltparser.core.symbol.SymbolTableHandler symbolTables, java.util.Map<String, DataFormatEntry> entries, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException
		private void createColumnDescriptions(SymbolTableHandler symbolTables, IDictionary<string, DataFormatEntry> entries, string nullValueStrategy)
		{
			foreach (DataFormatEntry entry in entries.Values)
			{
				ColumnDescription column = new ColumnDescription(entry.DataFormatEntryName, ColumnDescription.getCategory(entry.Category), ColumnDescription.getType(entry.Type), entry.DefaultOutput, nullValueStrategy, false);
				symbolTables.addSymbolTable(column.Name, column.Category, column.Type, column.NullValueStrategy);
				columnDescriptions.Add(column);

			}
		}

		public virtual ColumnDescription getColumnDescriptionByName(string name)
		{
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Name.Equals(name))
				{
					return column;
				}
			}
			if (internalColumnDescriptionSet != null)
			{
				foreach (ColumnDescription internalColumn in internalColumnDescriptionSet)
				{
					if (internalColumn.Name.Equals(name))
					{
						return internalColumn;
					}
				}
			}
			return null;
		}

	//	public int getNumberOfColumnDescriptions() {
	//		return columnDescriptions.size();
	//	}

		public virtual IEnumerator<ColumnDescription> GetEnumerator()
		{
			return columnDescriptions.GetEnumerator();
		}

		public virtual DataFormatSpecification DataFormarSpec
		{
			get
			{
				return dataFormarSpec;
			}
		}

		protected internal virtual void createHeadColumnDescriptions()
		{
			headColumnDescriptions = new SortedDictionary<string, ColumnDescription>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.HEAD)
				{
					headColumnDescriptions[column.Name] = column;
				}
			}
		}

		public virtual ColumnDescription HeadColumnDescription
		{
			get
			{
				if (headColumnDescriptions == null)
				{
					createHeadColumnDescriptions();
				}
				return headColumnDescriptions[headColumnDescriptions.firstKey()];
			}
		}

		public virtual SortedDictionary<string, ColumnDescription> HeadColumnDescriptions
		{
			get
			{
				if (headColumnDescriptions == null)
				{
					createHeadColumnDescriptions();
				}
				return headColumnDescriptions;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createDependencyEdgeLabelSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void createDependencyEdgeLabelSymbolTables(SymbolTableHandler symbolTables)
		{
			dependencyEdgeLabelSymbolTables = new SortedDictionary<string, SymbolTable>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
				{
					dependencyEdgeLabelSymbolTables[column.Name] = symbolTables.getSymbolTable(column.Name);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.SortedMap<String,org.maltparser.core.symbol.SymbolTable> getDependencyEdgeLabelSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public virtual SortedDictionary<string, SymbolTable> getDependencyEdgeLabelSymbolTables(SymbolTableHandler symbolTables)
		{
			if (dependencyEdgeLabelSymbolTables == null)
			{
				createDependencyEdgeLabelSymbolTables(symbolTables);
			}
			return dependencyEdgeLabelSymbolTables;
		}

		protected internal virtual void createDependencyEdgeLabelColumnDescriptions()
		{
			dependencyEdgeLabelColumnDescriptions = new SortedDictionary<string, ColumnDescription>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
				{
					dependencyEdgeLabelColumnDescriptions[column.Name] = column;
				}
			}
		}

		public virtual SortedDictionary<string, ColumnDescription> DependencyEdgeLabelColumnDescriptions
		{
			get
			{
				if (dependencyEdgeLabelColumnDescriptions == null)
				{
					createDependencyEdgeLabelColumnDescriptions();
				}
				return dependencyEdgeLabelColumnDescriptions;
			}
		}



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createPhraseStructureEdgeLabelSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void createPhraseStructureEdgeLabelSymbolTables(SymbolTableHandler symbolTables)
		{
			phraseStructureEdgeLabelSymbolTables = new SortedDictionary<string, SymbolTable>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.PHRASE_STRUCTURE_EDGE_LABEL)
				{
					phraseStructureEdgeLabelSymbolTables[column.Name] = symbolTables.getSymbolTable(column.Name);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.SortedMap<String,org.maltparser.core.symbol.SymbolTable> getPhraseStructureEdgeLabelSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public virtual SortedDictionary<string, SymbolTable> getPhraseStructureEdgeLabelSymbolTables(SymbolTableHandler symbolTables)
		{
			if (phraseStructureEdgeLabelSymbolTables == null)
			{
				createPhraseStructureEdgeLabelSymbolTables(symbolTables);
			}
			return phraseStructureEdgeLabelSymbolTables;
		}

		protected internal virtual void createPhraseStructureEdgeLabelColumnDescriptions()
		{
			phraseStructureEdgeLabelColumnDescriptions = new SortedDictionary<string, ColumnDescription>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.PHRASE_STRUCTURE_EDGE_LABEL)
				{
					phraseStructureEdgeLabelColumnDescriptions[column.Name] = column;
				}
			}
		}

		public virtual SortedDictionary<string, ColumnDescription> PhraseStructureEdgeLabelColumnDescriptions
		{
			get
			{
				if (phraseStructureEdgeLabelColumnDescriptions == null)
				{
					createPhraseStructureEdgeLabelColumnDescriptions();
				}
				return phraseStructureEdgeLabelColumnDescriptions;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createPhraseStructureNodeLabelSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void createPhraseStructureNodeLabelSymbolTables(SymbolTableHandler symbolTables)
		{
			phraseStructureNodeLabelSymbolTables = new SortedDictionary<string, SymbolTable>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.PHRASE_STRUCTURE_NODE_LABEL)
				{
					phraseStructureNodeLabelSymbolTables[column.Name] = symbolTables.getSymbolTable(column.Name);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.SortedMap<String,org.maltparser.core.symbol.SymbolTable> getPhraseStructureNodeLabelSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public virtual SortedDictionary<string, SymbolTable> getPhraseStructureNodeLabelSymbolTables(SymbolTableHandler symbolTables)
		{
			if (phraseStructureNodeLabelSymbolTables == null)
			{
				createPhraseStructureNodeLabelSymbolTables(symbolTables);
			}
			return phraseStructureNodeLabelSymbolTables;
		}

		protected internal virtual void createPhraseStructureNodeLabelColumnDescriptions()
		{
			phraseStructureNodeLabelColumnDescriptions = new SortedDictionary<string, ColumnDescription>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.PHRASE_STRUCTURE_NODE_LABEL)
				{
					phraseStructureNodeLabelColumnDescriptions[column.Name] = column;
				}
			}
		}

		public virtual SortedDictionary<string, ColumnDescription> PhraseStructureNodeLabelColumnDescriptions
		{
			get
			{
				if (phraseStructureNodeLabelColumnDescriptions == null)
				{
					createPhraseStructureNodeLabelColumnDescriptions();
				}
				return phraseStructureNodeLabelColumnDescriptions;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createSecondaryEdgeLabelSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void createSecondaryEdgeLabelSymbolTables(SymbolTableHandler symbolTables)
		{
			secondaryEdgeLabelSymbolTables = new SortedDictionary<string, SymbolTable>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.PHRASE_STRUCTURE_EDGE_LABEL)
				{
					secondaryEdgeLabelSymbolTables[column.Name] = symbolTables.getSymbolTable(column.Name);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.SortedMap<String,org.maltparser.core.symbol.SymbolTable> getSecondaryEdgeLabelSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public virtual SortedDictionary<string, SymbolTable> getSecondaryEdgeLabelSymbolTables(SymbolTableHandler symbolTables)
		{
			if (secondaryEdgeLabelSymbolTables == null)
			{
				createSecondaryEdgeLabelSymbolTables(symbolTables);
			}
			return secondaryEdgeLabelSymbolTables;
		}

		protected internal virtual void createSecondaryEdgeLabelColumnDescriptions()
		{
			secondaryEdgeLabelColumnDescriptions = new SortedDictionary<string, ColumnDescription>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.PHRASE_STRUCTURE_EDGE_LABEL)
				{
					secondaryEdgeLabelColumnDescriptions[column.Name] = column;
				}
			}
		}

		public virtual SortedDictionary<string, ColumnDescription> SecondaryEdgeLabelColumnDescriptions
		{
			get
			{
				if (secondaryEdgeLabelColumnDescriptions == null)
				{
					createSecondaryEdgeLabelColumnDescriptions();
				}
				return secondaryEdgeLabelColumnDescriptions;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createInputSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void createInputSymbolTables(SymbolTableHandler symbolTables)
		{
			inputSymbolTables = new SortedDictionary<string, SymbolTable>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.INPUT)
				{
					inputSymbolTables[column.Name] = symbolTables.getSymbolTable(column.Name);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.SortedMap<String,org.maltparser.core.symbol.SymbolTable> getInputSymbolTables(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public virtual SortedDictionary<string, SymbolTable> getInputSymbolTables(SymbolTableHandler symbolTables)
		{
			if (inputSymbolTables == null)
			{
				createInputSymbolTables(symbolTables);
			}
			return inputSymbolTables;
		}

		protected internal virtual void createInputColumnDescriptions()
		{
			inputColumnDescriptions = new SortedDictionary<string, ColumnDescription>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.INPUT)
				{
					inputColumnDescriptions[column.Name] = column;
				}
			}
		}

		public virtual SortedDictionary<string, ColumnDescription> InputColumnDescriptions
		{
			get
			{
				if (inputColumnDescriptions == null)
				{
					createInputColumnDescriptions();
				}
				return inputColumnDescriptions;
			}
		}

		protected internal virtual void createIgnoreColumnDescriptions()
		{
			ignoreColumnDescriptions = new SortedDictionary<string, ColumnDescription>();
			foreach (ColumnDescription column in columnDescriptions)
			{
				if (column.Category == ColumnDescription.IGNORE)
				{
	//			if (column.getType() == ColumnDescription.IGNORE) { 
					ignoreColumnDescriptions[column.Name] = column;
				}
			}
		}

		public virtual SortedDictionary<string, ColumnDescription> IgnoreColumnDescriptions
		{
			get
			{
				if (ignoreColumnDescriptions == null)
				{
					createIgnoreColumnDescriptions();
				}
				return ignoreColumnDescriptions;
			}
		}

		public virtual SortedSet<ColumnDescription> HeadColumnDescriptionSet
		{
			get
			{
				if (headColumnDescriptionSet == null)
				{
					headColumnDescriptionSet = new SortedSet<ColumnDescription>();
					foreach (ColumnDescription column in columnDescriptions)
					{
						if (column.Category == ColumnDescription.HEAD)
						{
							headColumnDescriptionSet.Add(column);
						}
					}
				}
				return headColumnDescriptionSet;
			}
		}

		public virtual SortedSet<ColumnDescription> DependencyEdgeLabelColumnDescriptionSet
		{
			get
			{
				if (dependencyEdgeLabelColumnDescriptionSet == null)
				{
					dependencyEdgeLabelColumnDescriptionSet = new SortedSet<ColumnDescription>();
					foreach (ColumnDescription column in columnDescriptions)
					{
						if (column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
						{
							dependencyEdgeLabelColumnDescriptionSet.Add(column);
						}
					}
				}
				return dependencyEdgeLabelColumnDescriptionSet;
			}
		}

		public virtual SortedSet<ColumnDescription> PhraseStructureEdgeLabelColumnDescriptionSet
		{
			get
			{
				if (phraseStructureEdgeLabelColumnDescriptionSet == null)
				{
					phraseStructureEdgeLabelColumnDescriptionSet = new SortedSet<ColumnDescription>();
					foreach (ColumnDescription column in columnDescriptions)
					{
						if (column.Category == ColumnDescription.PHRASE_STRUCTURE_EDGE_LABEL)
						{
							phraseStructureEdgeLabelColumnDescriptionSet.Add(column);
						}
					}
				}
				return phraseStructureEdgeLabelColumnDescriptionSet;
			}
		}

		public virtual SortedSet<ColumnDescription> PhraseStructureNodeLabelColumnDescriptionSet
		{
			get
			{
				if (phraseStructureNodeLabelColumnDescriptionSet == null)
				{
					phraseStructureNodeLabelColumnDescriptionSet = new SortedSet<ColumnDescription>();
					foreach (ColumnDescription column in columnDescriptions)
					{
						if (column.Category == ColumnDescription.PHRASE_STRUCTURE_NODE_LABEL)
						{
							phraseStructureNodeLabelColumnDescriptionSet.Add(column);
						}
					}
				}
				return phraseStructureNodeLabelColumnDescriptionSet;
			}
		}

		public virtual SortedSet<ColumnDescription> SecondaryEdgeLabelColumnDescriptionSet
		{
			get
			{
				if (secondaryEdgeLabelColumnDescriptionSet == null)
				{
					secondaryEdgeLabelColumnDescriptionSet = new SortedSet<ColumnDescription>();
					foreach (ColumnDescription column in columnDescriptions)
					{
						if (column.Category == ColumnDescription.SECONDARY_EDGE_LABEL)
						{
							secondaryEdgeLabelColumnDescriptionSet.Add(column);
						}
					}
				}
				return secondaryEdgeLabelColumnDescriptionSet;
			}
		}

		public virtual SortedSet<ColumnDescription> InputColumnDescriptionSet
		{
			get
			{
				if (inputColumnDescriptionSet == null)
				{
					inputColumnDescriptionSet = new SortedSet<ColumnDescription>();
					foreach (ColumnDescription column in columnDescriptions)
					{
						if (column.Category == ColumnDescription.INPUT)
						{
							inputColumnDescriptionSet.Add(column);
						}
					}
				}
				return inputColumnDescriptionSet;
			}
		}

		public virtual SortedSet<ColumnDescription> IgnoreColumnDescriptionSet
		{
			get
			{
				if (ignoreColumnDescriptionSet == null)
				{
					ignoreColumnDescriptionSet = new SortedSet<ColumnDescription>();
					foreach (ColumnDescription column in columnDescriptions)
					{
						if (column.Category == ColumnDescription.IGNORE)
						{
							ignoreColumnDescriptionSet.Add(column);
						}
					}
				}
				return ignoreColumnDescriptionSet;
			}
		}

	//	public SymbolTableHandler getSymbolTables() {
	//		return symbolTables;
	//	}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (ColumnDescription column in columnDescriptions)
			{
				sb.Append(column);
				sb.Append('\n');
			}
			return sb.ToString();
		}
	}

}