namespace org.maltparser.parser.algorithm.nivre
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.syntaxgraph;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.parser.history;
	using  org.maltparser.parser.history.action;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ArcEagerOracle : Oracle
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArcEagerOracle(org.maltparser.parser.DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public ArcEagerOracle(DependencyParserConfig manager, GuideUserHistory history) : base(manager, history)
		{
			GuideName = "ArcEager";
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction predict(DependencyStructure gold, ParserConfiguration config)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NivreConfig nivreConfig = (NivreConfig)config;
			NivreConfig nivreConfig = (NivreConfig)config;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode stackPeek = nivreConfig.getStack().peek();
			DependencyNode stackPeek = nivreConfig.Stack.Peek();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stackPeekIndex = stackPeek.getIndex();
			int stackPeekIndex = stackPeek.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int inputPeekIndex = nivreConfig.getInput().peek().getIndex();
			int inputPeekIndex = nivreConfig.Input.Peek().Index;

			if (!stackPeek.Root && gold.getTokenNode(stackPeekIndex).Head.Index == inputPeekIndex)
			{
				return updateActionContainers(ArcEager.LEFTARC, gold.getTokenNode(stackPeekIndex).HeadEdge.LabelSet);
			}
			else if (gold.getTokenNode(inputPeekIndex).Head.Index == stackPeekIndex)
			{
				return updateActionContainers(ArcEager.RIGHTARC, gold.getTokenNode(inputPeekIndex).HeadEdge.LabelSet);
			}
			else if (!nivreConfig.AllowReduce && !stackPeek.hasHead())
			{
				return updateActionContainers(ArcEager.SHIFT, null);
			}
			else if (gold.getTokenNode(inputPeekIndex).hasLeftDependent() && gold.getTokenNode(inputPeekIndex).LeftmostDependent.Index < stackPeekIndex)
			{
				return updateActionContainers(ArcEager.REDUCE, null);
			}
			else if (gold.getTokenNode(inputPeekIndex).Head.Index < stackPeekIndex && (!gold.getTokenNode(inputPeekIndex).Head.Root || nivreConfig.AllowRoot))
			{
				return updateActionContainers(ArcEager.REDUCE, null);
			}
			else
			{
				return updateActionContainers(ArcEager.SHIFT, null);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public override void finalizeSentence(DependencyStructure dependencyGraph)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
		}
	}

}