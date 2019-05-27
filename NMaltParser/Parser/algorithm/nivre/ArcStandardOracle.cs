using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Parser.History;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser.Algorithm.Nivre
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ArcStandardOracle : Oracle
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArcStandardOracle(org.maltparser.parser.DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public ArcStandardOracle(IDependencyParserConfig manager, GuideUserHistory history) : base(manager, history)
		{
			GuideName = "ArcStandard";
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction predict(IDependencyStructure gold, ParserConfiguration config)
		{
			NivreConfig nivreConfig = (NivreConfig)config;
			DependencyNode stackPeek = nivreConfig.Stack.Peek();
			int stackPeekIndex = stackPeek.Index;
			int inputPeekIndex = nivreConfig.Input.Peek().Index;

			if (!nivreConfig.AllowRoot && stackPeek.Root)
			{
				return updateActionContainers(ArcStandard.SHIFT, null);
			}
			if (!stackPeek.Root && gold.GetTokenNode(stackPeekIndex).Head.Index == inputPeekIndex)
			{
				return updateActionContainers(ArcStandard.LEFTARC, gold.GetTokenNode(stackPeekIndex).HeadEdge.LabelSet);
			}
			else if (gold.GetTokenNode(inputPeekIndex).Head.Index == stackPeekIndex && checkRightDependent(gold, nivreConfig.DependencyGraph, inputPeekIndex))
			{
				return updateActionContainers(ArcStandard.RIGHTARC, gold.GetTokenNode(inputPeekIndex).HeadEdge.LabelSet);
			}
			else
			{
				return updateActionContainers(ArcStandard.SHIFT, null);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean checkRightDependent(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.core.syntaxgraph.DependencyStructure parseDependencyGraph, int inputPeekIndex) throws org.maltparser.core.exception.MaltChainedException
		private bool checkRightDependent(IDependencyStructure gold, IDependencyStructure parseDependencyGraph, int inputPeekIndex)
		{
			if (gold.GetTokenNode(inputPeekIndex).RightmostDependent == null)
			{
				return true;
			}
			else if (parseDependencyGraph.GetTokenNode(inputPeekIndex).RightmostDependent != null)
			{
				if (gold.GetTokenNode(inputPeekIndex).RightmostDependent.Index == parseDependencyGraph.GetTokenNode(inputPeekIndex).RightmostDependent.Index)
				{
					return true;
				}
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public override void finalizeSentence(IDependencyStructure dependencyGraph)
		{

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{

		}
	}

}