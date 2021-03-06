﻿using NMaltParser.Core.Exception;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Parser.Guide;

namespace NMaltParser.Parser
{
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
		public Trainer(IDependencyParserConfig manager, SymbolTableHandler symbolTableHandler) : base(manager, symbolTableHandler)
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
		public abstract IDependencyStructure parse(IDependencyStructure goldDependencyGraph, IDependencyStructure parseDependencyGraph);
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