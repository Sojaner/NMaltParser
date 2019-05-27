using NMaltParser.Core.Exception;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;

namespace NMaltParser.Parser
{
    /// <inheritdoc />
    /// <summary>
    /// @author Johan Hall
    /// </summary>
	public abstract class Parser : ParsingAlgorithm
	{
        /// <inheritdoc />
        /// <summary>
        /// Creates a parser
        /// </summary>
        /// <param name="manager"> a reference to the single malt configuration </param>
        /// <param name="symbolTableHandler"></param>
        /// <exception cref="T:NMaltParser.Core.Exception.MaltChainedException"> </exception>
        protected Parser(IDependencyParserConfig manager, SymbolTableHandler symbolTableHandler) : base(manager, symbolTableHandler)
		{
		}
		/// <summary>
		/// Parses the empty dependency graph
		/// </summary>
		/// <param name="parseDependencyGraph"> a dependency graph </param>
		/// <returns> a parsed dependency graph </returns>
		/// <exception cref="MaltChainedException"> </exception>
		public abstract IDependencyStructure Parse(IDependencyStructure parseDependencyGraph);
	}

}