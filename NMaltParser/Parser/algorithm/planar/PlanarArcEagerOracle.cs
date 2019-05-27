using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Parser.History;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser.Algorithm.Planar
{
    /// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public class PlanarArcEagerOracle : Oracle
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PlanarArcEagerOracle(org.maltparser.parser.DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public PlanarArcEagerOracle(IDependencyParserConfig manager, GuideUserHistory history) : base(manager, history)
		{
			GuideName = "Planar";
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction predict(IDependencyStructure gold, ParserConfiguration config)
		{
			PlanarConfig planarConfig = (PlanarConfig)config;
			IDependencyStructure dg = planarConfig.DependencyGraph;
			DependencyNode stackPeek = planarConfig.Stack.Peek();
			int stackPeekIndex = stackPeek.Index;
			int inputPeekIndex = planarConfig.Input.Peek().Index;

			if (!stackPeek.Root && gold.GetTokenNode(stackPeekIndex).Head.Index == inputPeekIndex && !checkIfArcExists(dg, inputPeekIndex, stackPeekIndex))
			{
				return updateActionContainers(Planar.LEFTARC, gold.GetTokenNode(stackPeekIndex).HeadEdge.LabelSet);
			}
			else if (gold.GetTokenNode(inputPeekIndex).Head.Index == stackPeekIndex && !checkIfArcExists(dg, stackPeekIndex, inputPeekIndex))
			{
				return updateActionContainers(Planar.RIGHTARC, gold.GetTokenNode(inputPeekIndex).HeadEdge.LabelSet);
			}
			else if (gold.GetTokenNode(inputPeekIndex).hasLeftDependent() && gold.GetTokenNode(inputPeekIndex).LeftmostDependent.Index < stackPeekIndex)
			{
				return updateActionContainers(Planar.REDUCE, null);
			}
			else if (gold.GetTokenNode(inputPeekIndex).Head.Index < stackPeekIndex && (!gold.GetTokenNode(inputPeekIndex).Head.Root || planarConfig.getRootHandling() == PlanarConfig.NORMAL))
			{
				return updateActionContainers(Planar.REDUCE, null);
			}
			else
			{
				return updateActionContainers(Planar.SHIFT, null);
			}

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean checkIfArcExists(org.maltparser.core.syntaxgraph.DependencyStructure dg, int index1, int index2) throws org.maltparser.core.exception.MaltChainedException
		private bool checkIfArcExists(IDependencyStructure dg, int index1, int index2)
		{
			return dg.GetTokenNode(index2).hasHead() && dg.GetTokenNode(index2).Head.Index == index1;
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