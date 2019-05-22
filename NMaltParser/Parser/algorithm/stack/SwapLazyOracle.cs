﻿using System.Collections.Generic;

namespace org.maltparser.parser.algorithm.stack
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using DependencyNode = org.maltparser.core.syntaxgraph.node.DependencyNode;
	using GuideUserHistory = org.maltparser.parser.history.GuideUserHistory;
	using GuideUserAction = org.maltparser.parser.history.action.GuideUserAction;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class SwapLazyOracle : Oracle
	{
		private List<int> swapArray;
		private bool swapArrayActive = false;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SwapLazyOracle(org.maltparser.parser.DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public SwapLazyOracle(DependencyParserConfig manager, GuideUserHistory history) : base(manager, history)
		{
			GuideName = "swaplazy";
			swapArray = new List<int>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction predict(DependencyStructure gold, ParserConfiguration configuration)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StackConfig config = (StackConfig)configuration;
			StackConfig config = (StackConfig)configuration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> stack = config.getStack();
			Stack<DependencyNode> stack = config.Stack;

			if (!swapArrayActive)
			{
				createSwapArray(gold);
				swapArrayActive = true;
			}
			if (stack.Count < 2)
			{
				return updateActionContainers(NonProjective.SHIFT, null);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode left = stack.get(stack.size()-2);
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				DependencyNode left = stack.get(stack.Count - 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode right = stack.get(stack.size() - 1);
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
				DependencyNode right = stack.get(stack.Count - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int leftIndex = left.getIndex();
				int leftIndex = left.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rightIndex = right.getIndex();
				int rightIndex = right.Index;
				if (swapArray[leftIndex] > swapArray[rightIndex] && necessarySwap(gold, config.DependencyGraph, right, config.Input))
				{
					return updateActionContainers(NonProjective.SWAP, null);
				}
				else if (!left.Root && gold.getTokenNode(leftIndex).Head.Index == rightIndex && nodeComplete(gold, config.DependencyGraph, leftIndex))
				{
					return updateActionContainers(NonProjective.LEFTARC, gold.getTokenNode(leftIndex).HeadEdge.LabelSet);
				}
				else if (gold.getTokenNode(rightIndex).Head.Index == leftIndex && nodeComplete(gold, config.DependencyGraph, rightIndex))
				{
					return updateActionContainers(NonProjective.RIGHTARC, gold.getTokenNode(rightIndex).HeadEdge.LabelSet);
				}
				else
				{
					return updateActionContainers(NonProjective.SHIFT, null);
				}
			}
		}

		private bool nodeComplete(DependencyStructure gold, DependencyStructure parseDependencyGraph, int nodeIndex)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode goldNode = gold.getTokenNode(nodeIndex);
			DependencyNode goldNode = gold.getTokenNode(nodeIndex);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode parseNode = parseDependencyGraph.getTokenNode(nodeIndex);
			DependencyNode parseNode = parseDependencyGraph.getTokenNode(nodeIndex);
			if (goldNode.hasLeftDependent())
			{
				if (!parseNode.hasLeftDependent())
				{
					return false;
				}
				else if (goldNode.LeftmostDependent.Index != parseNode.LeftmostDependent.Index)
				{
					return false;
				}
			}
			if (goldNode.hasRightDependent())
			{
				if (!parseNode.hasRightDependent())
				{
					return false;
				}
				else if (goldNode.RightmostDependent.Index != parseNode.RightmostDependent.Index)
				{
					return false;
				}
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean necessarySwap(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.core.syntaxgraph.DependencyStructure parse, org.maltparser.core.syntaxgraph.node.DependencyNode node, java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> input) throws org.maltparser.core.exception.MaltChainedException
		private bool necessarySwap(DependencyStructure gold, DependencyStructure parse, DependencyNode node, Stack<DependencyNode> input)
		{
			DependencyNode left = node;
			int index = input.Count - 1;
			if (index < 0)
			{
				return true;
			}
			DependencyNode right = input.Peek();

			int rc = -1;
			while (projectiveInterval(parse, left, right))
			{
				if (rc == right.Index)
				{
					return false;
				}
				if (gold.getDependencyNode(node.Index).Head.Index == right.Index)
				{
					return !leftComplete(gold, node);
				}
				if (gold.getDependencyNode(right.Index).Head.Index == node.Index)
				{
					if (gold.getDependencyNode(right.Index).hasRightDependent())
					{
						  rc = gold.getDependencyNode(right.Index).RightmostProperDescendantIndex;
					}
					else
					{
					  return false;
					}
				}
				if (index > 0)
				{
					left = right;
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
					right = input.get(--index);
				}
				else
				{
					break;
				}
			}

			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean projectiveInterval(org.maltparser.core.syntaxgraph.DependencyStructure parse, org.maltparser.core.syntaxgraph.node.DependencyNode left, org.maltparser.core.syntaxgraph.node.DependencyNode right) throws org.maltparser.core.exception.MaltChainedException
		private bool projectiveInterval(DependencyStructure parse, DependencyNode left, DependencyNode right)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l = swapArray.get(left.getIndex());
			int l = swapArray[left.Index];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = swapArray.get(right.getIndex());
			int r = swapArray[right.Index];
			DependencyNode node = null;
			if (l > r)
			{
				return false;
			}
			else
			{
				for (int i = l + 1; i < r; i++)
				{
					for (int j = 0; j < swapArray.Count; j++)
					{
						if (swapArray[j] == i)
						{
							node = parse.getDependencyNode(j);
							break;
						}
					}
					while (node.hasHead())
					{
						node = node.Head;
					}
					if (!(node == left || node == right))
					{
						return false;
					}
				}
				return true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean leftComplete(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.core.syntaxgraph.node.DependencyNode right) throws org.maltparser.core.exception.MaltChainedException
		private bool leftComplete(DependencyStructure gold, DependencyNode right)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode goldNode = gold.getDependencyNode(right.getIndex());
			DependencyNode goldNode = gold.getDependencyNode(right.Index);
			if (!goldNode.hasLeftDependent())
			{
				return true;
			}
			else if (!right.hasLeftDependent())
			{
				return false;
			}
			else if (goldNode.LeftmostDependent.Index == right.LeftmostDependent.Index)
			{
				return true;
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public override void finalizeSentence(DependencyStructure dependencyGraph)
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
		private void createSwapArray(DependencyStructure goldDependencyGraph)
		{
			swapArray.Clear();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = goldDependencyGraph.getHighestDependencyNodeIndex();
			int n = goldDependencyGraph.HighestDependencyNodeIndex;
			for (int i = 0; i <= n; i++)
			{
				swapArray.Add(new int?(i));
			}
			createSwapArray(goldDependencyGraph.DependencyRoot, 0);
		}

		private int createSwapArray(DependencyNode node, int order)
		{
			int o = order;
			if (node != null)
			{
				for (int i = 0; i < node.LeftDependentCount; i++)
				{
					o = createSwapArray(node.getLeftDependent(i), o);
				}
				swapArray[node.Index] = o++;
				for (int i = node.RightDependentCount; i >= 0; i--)
				{
					o = createSwapArray(node.getRightDependent(i), o);
				}
			}
			return o;
		}
	}

}