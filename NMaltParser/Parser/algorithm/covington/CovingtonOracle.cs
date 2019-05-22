namespace org.maltparser.parser.algorithm.covington
{
    using  core.syntaxgraph;
	using  core.syntaxgraph.node;
	using  history;
	using  history.action;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class CovingtonOracle : Oracle
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CovingtonOracle(org.maltparser.parser.DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public CovingtonOracle(DependencyParserConfig manager, GuideUserHistory history) : base(manager, history)
		{
			GuideName = "NonProjective";
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction predict(DependencyStructure gold, ParserConfiguration config)
		{
			CovingtonConfig covingtonConfig = (CovingtonConfig)config;
			DependencyNode leftTarget = covingtonConfig.LeftTarget;
			int leftTargetIndex = leftTarget.Index;
			int rightTargetIndex = covingtonConfig.RightTarget.Index;

			if (!leftTarget.Root && gold.getTokenNode(leftTargetIndex).Head.Index == rightTargetIndex)
			{
				return updateActionContainers(NonProjective.LEFTARC, gold.getTokenNode(leftTargetIndex).HeadEdge.LabelSet);
			}
			else if (gold.getTokenNode(rightTargetIndex).Head.Index == leftTargetIndex)
			{
				return updateActionContainers(NonProjective.RIGHTARC, gold.getTokenNode(rightTargetIndex).HeadEdge.LabelSet);
			}
			else if (covingtonConfig.AllowShift == true && (!(gold.getTokenNode(rightTargetIndex).hasLeftDependent() && gold.getTokenNode(rightTargetIndex).LeftmostDependent.Index < leftTargetIndex) && !(gold.getTokenNode(rightTargetIndex).Head.Index < leftTargetIndex && (!gold.getTokenNode(rightTargetIndex).Head.Root || covingtonConfig.Leftstop == 0))))
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