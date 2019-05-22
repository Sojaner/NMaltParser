namespace org.maltparser.parser
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using SymbolTable = org.maltparser.core.symbol.SymbolTable;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;
	using DependencyNode = org.maltparser.core.syntaxgraph.node.DependencyNode;
	using ClassifierGuide = org.maltparser.parser.guide.ClassifierGuide;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class ParsingAlgorithm : AlgoritmInterface
	{
		protected internal readonly DependencyParserConfig manager;
		protected internal readonly ParserRegistry registry;
		protected internal ClassifierGuide classifierGuide;
		protected internal readonly ParserState parserState;
		protected internal ParserConfiguration currentParserConfiguration;

		/// <summary>
		/// Creates a parsing algorithm
		/// </summary>
		/// <param name="_manager"> a reference to the single malt configuration </param>
		/// <param name="symbolTableHandler"> a reference to the symbol table handler </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParsingAlgorithm(DependencyParserConfig _manager, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public ParsingAlgorithm(DependencyParserConfig _manager, SymbolTableHandler symbolTableHandler)
		{
			this.manager = _manager;
			this.registry = new ParserRegistry();
			registry.SymbolTableHandler = symbolTableHandler;
			registry.DataFormatInstance = manager.DataFormatInstance;
			registry.setAbstractParserFeatureFactory(manager.ParserFactory);
			parserState = new ParserState(manager, symbolTableHandler, manager.ParserFactory);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void terminate() throws org.maltparser.core.exception.MaltChainedException;
		public abstract void terminate();

		public virtual ParserRegistry ParserRegistry
		{
			get
			{
				return registry;
			}
		}

		/// <summary>
		/// Returns the classifier guide.
		/// </summary>
		/// <returns> the classifier guide </returns>
		public virtual ClassifierGuide Guide
		{
			get
			{
				return classifierGuide;
			}
			set
			{
				this.classifierGuide = value;
			}
		}


		/// <summary>
		/// Returns the current active parser configuration
		/// </summary>
		/// <returns> the current active parser configuration </returns>
		public virtual ParserConfiguration CurrentParserConfiguration
		{
			get
			{
				return currentParserConfiguration;
			}
			set
			{
				this.currentParserConfiguration = value;
			}
		}


		/// <summary>
		/// Returns the parser state
		/// </summary>
		/// <returns> the parser state </returns>
		public virtual ParserState ParserState
		{
			get
			{
				return parserState;
			}
		}


		/// <summary>
		/// Returns the single malt configuration
		/// </summary>
		/// <returns> the single malt configuration </returns>
		public virtual DependencyParserConfig Manager
		{
			get
			{
				return manager;
			}
		}


		/// <summary>
		/// Copies the edges of the source dependency structure to the target dependency structure
		/// </summary>
		/// <param name="source"> a source dependency structure </param>
		/// <param name="target"> a target dependency structure </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void copyEdges(org.maltparser.core.syntaxgraph.DependencyStructure source, org.maltparser.core.syntaxgraph.DependencyStructure target) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void copyEdges(DependencyStructure source, DependencyStructure target)
		{
			foreach (int index in source.TokenIndices)
			{
				DependencyNode snode = source.getDependencyNode(index);

				if (snode.hasHead())
				{
					Edge s = snode.HeadEdge;
					Edge t = target.addDependencyEdge(s.Source.Index, s.Target.Index);

					foreach (SymbolTable table in s.LabelTypes)
					{
						t.addLabel(table, s.getLabelSymbol(table));
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void copyDynamicInput(org.maltparser.core.syntaxgraph.DependencyStructure source, org.maltparser.core.syntaxgraph.DependencyStructure target) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void copyDynamicInput(DependencyStructure source, DependencyStructure target)
		{
			foreach (int index in source.TokenIndices)
			{
				DependencyNode snode = source.getDependencyNode(index);
				DependencyNode tnode = target.getDependencyNode(index);
				foreach (SymbolTable table in snode.LabelTypes)
				{
					if (!tnode.hasLabel(table))
					{
						tnode.addLabel(table,snode.getLabelSymbol(table));
					}
				}
			}
		}
	}

}