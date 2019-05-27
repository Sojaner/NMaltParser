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
	public class ProjectiveOracle : Oracle
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ProjectiveOracle(org.maltparser.parser.DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public ProjectiveOracle(IDependencyParserConfig manager, GuideUserHistory history) : base(manager, history)
		{
			GuideName = "projective";
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction predict(IDependencyStructure gold, ParserConfiguration configuration)
		{
			StackConfig config = (StackConfig)configuration;
			Stack<DependencyNode> stack = config.Stack;

			if (stack.Count < 2)
			{
				return updateActionContainers(Projective.SHIFT, null);
			}
			else
			{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				DependencyNode left = stack.get(stack.Count - 2);
				int leftIndex = left.Index;
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				int rightIndex = stack.get(stack.Count - 1).Index;
				if (!left.Root && gold.GetTokenNode(leftIndex).Head.Index == rightIndex)
				{
					return updateActionContainers(Projective.LEFTARC, gold.GetTokenNode(leftIndex).HeadEdge.LabelSet);
				}
				else if (gold.GetTokenNode(rightIndex).Head.Index == leftIndex && checkRightDependent(gold, config.DependencyGraph, rightIndex))
				{
					return updateActionContainers(Projective.RIGHTARC, gold.GetTokenNode(rightIndex).HeadEdge.LabelSet);
				}
				else
				{
					return updateActionContainers(Projective.SHIFT, null);
				} // Solve the problem with non-projective input.
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean checkRightDependent(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.core.syntaxgraph.DependencyStructure parseDependencyGraph, int index) throws org.maltparser.core.exception.MaltChainedException
		private bool checkRightDependent(IDependencyStructure gold, IDependencyStructure parseDependencyGraph, int index)
		{
			if (gold.GetTokenNode(index).RightmostDependent == null)
			{
				return true;
			}
			else if (parseDependencyGraph.GetTokenNode(index).RightmostDependent != null)
			{
				if (gold.GetTokenNode(index).RightmostDependent.Index == parseDependencyGraph.GetTokenNode(index).RightmostDependent.Index)
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