namespace org.maltparser.parser
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.symbol;
	using  org.maltparser.core.syntaxgraph;
	using  org.maltparser.parser.guide;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class Trainer : ParsingAlgorithm
	{
		/// <summary>
		/// Creates a parser trainer
		/// </summary>
		/// <param name="manager"> a reference to the single malt configuration </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Trainer(DependencyParserConfig manager, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public Trainer(DependencyParserConfig manager, SymbolTableHandler symbolTableHandler) : base(manager, symbolTableHandler)
		{
		}

		/// <summary>
		/// Trains a parser using the gold-standard dependency graph and returns a parsed dependency graph
		/// </summary>
		/// <param name="goldDependencyGraph"> a old-standard dependency graph </param>
		/// <param name="parseDependencyGraph"> a empty dependency graph </param>
		/// <returns> a parsed dependency graph </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract org.maltparser.core.syntaxgraph.DependencyStructure parse(org.maltparser.core.syntaxgraph.DependencyStructure goldDependencyGraph, org.maltparser.core.syntaxgraph.DependencyStructure parseDependencyGraph) throws org.maltparser.core.exception.MaltChainedException;
		public abstract DependencyStructure parse(DependencyStructure goldDependencyGraph, DependencyStructure parseDependencyGraph);
		/// <summary>
		/// Returns the oracle guide.
		/// </summary>
		/// <returns> the oracle guide. </returns>
		public abstract OracleGuide OracleGuide {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void train() throws org.maltparser.core.exception.MaltChainedException;
		public abstract void train();

	}

}