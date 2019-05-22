using NMaltParser.Core.Exception;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;

namespace NMaltParser.Parser
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class Parser : ParsingAlgorithm
	{

		/// <summary>
		/// Creates a parser
		/// </summary>
		/// <param name="manager"> a reference to the single malt configuration </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Parser(DependencyParserConfig manager, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public Parser(DependencyParserConfig manager, SymbolTableHandler symbolTableHandler) : base(manager, symbolTableHandler)
		{
		}
		/// <summary>
		/// Parses the empty dependency graph
		/// </summary>
		/// <param name="parseDependencyGraph"> a dependency graph </param>
		/// <returns> a parsed dependency graph </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract org.maltparser.core.syntaxgraph.DependencyStructure parse(org.maltparser.core.syntaxgraph.DependencyStructure parseDependencyGraph) throws org.maltparser.core.exception.MaltChainedException;
		public abstract DependencyStructure parse(DependencyStructure parseDependencyGraph);
	}

}