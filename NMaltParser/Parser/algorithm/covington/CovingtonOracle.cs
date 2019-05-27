using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Parser.History;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser.Algorithm.Covington
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class CovingtonOracle : Oracle
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CovingtonOracle(org.maltparser.parser.DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public CovingtonOracle(IDependencyParserConfig manager, GuideUserHistory history) : base(manager, history)
		{
			GuideName = "NonProjective";
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction predict(IDependencyStructure gold, ParserConfiguration config)
		{
			CovingtonConfig covingtonConfig = (CovingtonConfig)config;
			DependencyNode leftTarget = covingtonConfig.LeftTarget;
			int leftTargetIndex = leftTarget.Index;
			int rightTargetIndex = covingtonConfig.RightTarget.Index;

			if (!leftTarget.Root && gold.GetTokenNode(leftTargetIndex).Head.Index == rightTargetIndex)
			{
				return updateActionContainers(NonProjective.LEFTARC, gold.GetTokenNode(leftTargetIndex).HeadEdge.LabelSet);
			}
			else if (gold.GetTokenNode(rightTargetIndex).Head.Index == leftTargetIndex)
			{
				return updateActionContainers(NonProjective.RIGHTARC, gold.GetTokenNode(rightTargetIndex).HeadEdge.LabelSet);
			}
			else if (covingtonConfig.AllowShift == true && (!(gold.GetTokenNode(rightTargetIndex).hasLeftDependent() && gold.GetTokenNode(rightTargetIndex).LeftmostDependent.Index < leftTargetIndex) && !(gold.GetTokenNode(rightTargetIndex).Head.Index < leftTargetIndex && (!gold.GetTokenNode(rightTargetIndex).Head.Root || covingtonConfig.Leftstop == 0))))
			{
				return updateActionContainers(NonProjective.SHIFT, null);
			}
			else
			{
				return updateActionContainers(NonProjective.NOARC, null);
			}
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