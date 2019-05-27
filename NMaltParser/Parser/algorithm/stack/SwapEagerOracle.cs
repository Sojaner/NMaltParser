using System.Collections.Generic;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Parser.History;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser.Algorithm.Stack
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class SwapEagerOracle : Oracle
	{
		private List<int> swapArray;
		private bool swapArrayActive = false;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SwapEagerOracle(org.maltparser.parser.DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public SwapEagerOracle(IDependencyParserConfig manager, GuideUserHistory history) : base(manager, history)
		{
			GuideName = "swapeager";
			swapArray = new List<int>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction predict(IDependencyStructure gold, ParserConfiguration configuration)
		{
			StackConfig config = (StackConfig)configuration;
			Stack<DependencyNode> stack = config.Stack;

			if (!swapArrayActive)
			{
				createSwapArray(gold);
				swapArrayActive = true;
			}
			GuideUserAction action = null;
			if (stack.Count < 2)
			{
				action = updateActionContainers(NonProjective.SHIFT, null);
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				DependencyNode left = stack.get(stack.Count - 2);
				int leftIndex = left.Index;
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				int rightIndex = stack.get(stack.Count - 1).Index;
				if (swapArray[leftIndex] > swapArray[rightIndex])
				{
					action = updateActionContainers(NonProjective.SWAP, null);
				}
				else if (!left.Root && gold.GetTokenNode(leftIndex).Head.Index == rightIndex && nodeComplete(gold, config.DependencyGraph, leftIndex))
				{
					action = updateActionContainers(NonProjective.LEFTARC, gold.GetTokenNode(leftIndex).HeadEdge.LabelSet);
				}
				else if (gold.GetTokenNode(rightIndex).Head.Index == leftIndex && nodeComplete(gold, config.DependencyGraph, rightIndex))
				{
					action = updateActionContainers(NonProjective.RIGHTARC, gold.GetTokenNode(rightIndex).HeadEdge.LabelSet);
				}
				else
				{
					action = updateActionContainers(NonProjective.SHIFT, null);
				}
			}
			return action;
		}

		private bool nodeComplete(IDependencyStructure gold, IDependencyStructure parseDependencyGraph, int nodeIndex)
		{
			if (gold.GetTokenNode(nodeIndex).hasLeftDependent())
			{
				if (!parseDependencyGraph.GetTokenNode(nodeIndex).hasLeftDependent())
				{
					return false;
				}
				else if (gold.GetTokenNode(nodeIndex).LeftmostDependent.Index != parseDependencyGraph.GetTokenNode(nodeIndex).LeftmostDependent.Index)
				{
					return false;
				}
			}
			if (gold.GetTokenNode(nodeIndex).hasRightDependent())
			{
				if (!parseDependencyGraph.GetTokenNode(nodeIndex).hasRightDependent())
				{
					return false;
				}
				else if (gold.GetTokenNode(nodeIndex).RightmostDependent.Index != parseDependencyGraph.GetTokenNode(nodeIndex).RightmostDependent.Index)
				{
					return false;
				}
			}
			return true;
		}

	//	private boolean checkRightDependent(DependencyStructure gold, DependencyStructure parseDependencyGraph, int index) throws MaltChainedException {
	//		if (gold.getTokenNode(index).getRightmostDependent() == null) {
	//			return true;
	//		} else if (parseDependencyGraph.getTokenNode(index).getRightmostDependent() != null) {
	//			if (gold.getTokenNode(index).getRightmostDependent().getIndex() == parseDependencyGraph.getTokenNode(index).getRightmostDependent().getIndex()) {
	//				return true;
	//			}
	//		}
	//		return false;
	//	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public override void finalizeSentence(IDependencyStructure dependencyGraph)
		{
			swapArrayActive = false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void createSwapArray(org.maltparser.core.syntaxgraph.DependencyStructure goldDependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		private void createSwapArray(IDependencyStructure goldDependencyGraph)
		{
			swapArray.Clear();
			for (int i = 0; i <= goldDependencyGraph.HighestDependencyNodeIndex; i++)
			{
				swapArray.Add(new int?(i));
			}
			createSwapArray(goldDependencyGraph.DependencyRoot, 0);
		}

		private int createSwapArray(DependencyNode n, int order)
		{
			int o = order;
			if (n != null)
			{
				for (int i = 0; i < n.LeftDependentCount; i++)
				{
					o = createSwapArray(n.getLeftDependent(i), o);
				}
				swapArray[n.Index] = o++;
				for (int i = n.RightDependentCount; i >= 0; i--)
				{
					o = createSwapArray(n.getRightDependent(i), o);
				}
			}
			return o;
		}
	}

}