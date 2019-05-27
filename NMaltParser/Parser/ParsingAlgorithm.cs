using NMaltParser.Core.Exception;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Parser.Guide;

namespace NMaltParser.Parser
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class ParsingAlgorithm : AlgoritmInterface
	{
		/// <summary>
		/// Creates a parsing algorithm
		/// </summary>
		/// <param name="manager"> a reference to the single malt configuration </param>
		/// <param name="symbolTableHandler"> a reference to the symbol table handler </param>
		/// <exception cref="MaltChainedException"> </exception>
        protected ParsingAlgorithm(IDependencyParserConfig manager, SymbolTableHandler symbolTableHandler)
		{
			Manager = manager;

            ParserRegistry = new ParserRegistry
            {
                SymbolTableHandler = symbolTableHandler,

                DataFormatInstance = Manager.DataFormatInstance
            };

            ParserRegistry.setAbstractParserFeatureFactory(Manager.ParserFactory);

            ParserState = new ParserState(Manager, symbolTableHandler, Manager.ParserFactory);
		}

		public abstract void Terminate();

		public ParserRegistry ParserRegistry { get; }

        /// <summary>
		/// Returns the classifier guide.
		/// </summary>
		/// <returns> the classifier guide </returns>
		public ClassifierGuide Guide
        {
            get;
            set;
        }


		/// <summary>
		/// Returns the current active parser configuration
		/// </summary>
		/// <returns> the current active parser configuration </returns>
		public ParserConfiguration CurrentParserConfiguration
		{
			get;
            set;
        }


		/// <summary>
		/// Returns the parser state
		/// </summary>
		/// <returns> the parser state </returns>
		public ParserState ParserState { get; }


        /// <summary>
        /// Returns the single malt configuration
        /// </summary>
        /// <returns> the single malt configuration </returns>
        public IDependencyParserConfig Manager { get; }


        /// <summary>
		/// Copies the edges of the source dependency structure to the target dependency structure
		/// </summary>
		/// <param name="source"> a source dependency structure </param>
		/// <param name="target"> a target dependency structure </param>
		/// <exception cref="MaltChainedException"> </exception>
		protected internal virtual void CopyEdges(IDependencyStructure source, IDependencyStructure target)
		{
			foreach (int index in source.TokenIndices)
			{
				DependencyNode sNode = source.GetDependencyNode(index);

				if (sNode.hasHead())
				{
					Edge s = sNode.HeadEdge;

					Edge t = target.AddDependencyEdge(s.Source.Index, s.Target.Index);

					foreach (SymbolTable table in s.LabelTypes)
					{
						t.addLabel(table, s.getLabelSymbol(table));
					}
				}
			}
		}
        
		protected internal virtual void CopyDynamicInput(IDependencyStructure source, IDependencyStructure target)
		{
			foreach (int index in source.TokenIndices)
			{
				DependencyNode sNode = source.GetDependencyNode(index);

				DependencyNode tNode = target.GetDependencyNode(index);

				foreach (SymbolTable table in sNode.LabelTypes)
				{
					if (!tNode.hasLabel(table))
					{
						tNode.addLabel(table,sNode.getLabelSymbol(table));
					}
				}
			}
		}
	}
}