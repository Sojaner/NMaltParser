using System.Collections.Generic;

namespace org.maltparser.parser.algorithm.planar
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.propagation;
	using  org.maltparser.core.syntaxgraph;
	using  org.maltparser.core.syntaxgraph.edge;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.parser.history;
	using  org.maltparser.parser.history.action;
	using  org.maltparser.parser.history.action;
	using  org.maltparser.parser.transition;
	/// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public class Planar : TransitionSystem
	{
		protected internal const int SHIFT = 1;
		protected internal const int REDUCE = 2;
		protected internal const int RIGHTARC = 3;
		protected internal const int LEFTARC = 4;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Planar(org.maltparser.core.propagation.PropagationManager propagationManager) throws org.maltparser.core.exception.MaltChainedException
		public Planar(PropagationManager propagationManager) : base(propagationManager)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void apply(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override void apply(GuideUserAction currentAction, ParserConfiguration config)
		{
			PlanarConfig planarConfig = (PlanarConfig)config;
			Stack<DependencyNode> stack = planarConfig.Stack;
			Stack<DependencyNode> input = planarConfig.Input;
			currentAction.getAction(actionContainers);
			Edge e = null;
			switch (transActionContainer.ActionCode)
			{
			case LEFTARC:
				e = planarConfig.DependencyStructure.addDependencyEdge(input.Peek().Index, stack.Peek().Index);
				addEdgeLabels(e);
				break;
			case RIGHTARC:
				e = planarConfig.DependencyStructure.addDependencyEdge(stack.Peek().Index, input.Peek().Index);
				addEdgeLabels(e);
				break;
			case REDUCE:
				stack.Pop();
				break;
			default: //SHIFT
				stack.Push(input.Pop());
				break;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction getDeterministicAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction getDeterministicAction(GuideUserHistory history, ParserConfiguration config)
		{
			PlanarConfig planarConfig = (PlanarConfig)config;
			if (planarConfig.getRootHandling() != PlanarConfig.NORMAL && planarConfig.Stack.Peek().Root)
			{
				return updateActionContainers(history, Planar.SHIFT, null);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void addAvailableTransitionToTable(org.maltparser.parser.transition.TransitionTable ttable) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void addAvailableTransitionToTable(TransitionTable ttable)
		{
			ttable.addTransition(SHIFT, "SH", false, null);
			ttable.addTransition(REDUCE, "RE", false, null);
			ttable.addTransition(RIGHTARC, "RA", true, null);
			ttable.addTransition(LEFTARC, "LA", true, null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initWithDefaultTransitions(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void initWithDefaultTransitions(GuideUserHistory history)
		{
			GuideUserAction currentAction = new ComplexDecisionAction(history);

			transActionContainer.Action = SHIFT;
			transActionContainer.Action = REDUCE;
			for (int i = 0; i < arcLabelActionContainers.Length; i++)
			{
				arcLabelActionContainers[i].Action = -1;
			}
			currentAction.addAction(actionContainers);
		}

		public override string Name
		{
			get
			{
				return "planar arc-eager";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean permissible(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override bool permissible(GuideUserAction currentAction, ParserConfiguration config)
		{
			currentAction.getAction(actionContainers);
			int trans = transActionContainer.ActionCode;
			PlanarConfig planarConfig = (PlanarConfig)config;
			DependencyNode stackPeek = planarConfig.Stack.Peek();
			DependencyNode inputPeek = planarConfig.Input.Peek();
			DependencyStructure dg = planarConfig.DependencyGraph;
			//int rootHandling = planarConfig.getRootHandling();
			bool singleHeadConstraint = planarConfig.requiresSingleHead();
			bool noCoveredRootsConstraint = planarConfig.requiresNoCoveredRoots();
			bool acyclicityConstraint = planarConfig.requiresAcyclicity();
			bool connectednessConstraintOnReduce = planarConfig.requiresConnectednessCheckOnReduce();
			bool connectednessConstraintOnShift = planarConfig.requiresConnectednessCheckOnShift();
			if ((trans == LEFTARC || trans == RIGHTARC) && !ActionContainersLabeled)
			{
				return false;
			}
			//if ((trans == LEFTARC || trans == REDUCE) && stackPeek.isRoot()) { 
			//	return false;
			//}
			if (trans == LEFTARC)
			{
				//avoid making root child of something
				if (stackPeek.Root)
				{
					return false;
				}
				//enforce single-head constraint if present
				if (stackPeek.hasHead() && singleHeadConstraint)
				{
					return false;
				}
				//avoid two links being created from and to the same node
				if (stackPeek.hasHead() && dg.getTokenNode(stackPeek.Index).Head.Index == inputPeek.Index)
				{
					return false;
				}
				//enforce acyclicity constraint if present
				if (acyclicityConstraint && stackPeek.findComponent().Index == inputPeek.findComponent().Index)
				{
					return false;
				}
			}
			if (trans == RIGHTARC)
			{
				//enforce single-head constraint if present
				if (inputPeek.hasHead() && singleHeadConstraint)
				{
					return false;
				}
				//avoid two links being created from and to the same node
				if (inputPeek.hasHead() && dg.getTokenNode(inputPeek.Index).Head.Index == stackPeek.Index)
				{
					return false;
				}
				//enforce acyclicity constraint if present
				if (acyclicityConstraint && stackPeek.findComponent().Index == inputPeek.findComponent().Index)
				{
					return false;
				}
			}
			if (trans == REDUCE)
			{
				//do not reduce the dummy root
				if (stackPeek.Root)
				{
					return false;
				}
				//enforce no-covered-roots constraint if present
				if (!stackPeek.hasHead() && noCoveredRootsConstraint)
				{
					return false;
				}
				//TODO does this line still make sense? (from Nivre arc-eager)
				//if ( !stackPeek.hasHead() && rootHandling == PlanarConfig.STRICT ) 
				//	return false;
				//enforce connectedness constraint if present
				if (connectednessConstraintOnReduce)
				{
					bool path1 = (stackPeek.findComponent().Index == inputPeek.findComponent().Index);
					bool path2;
					if (planarConfig.Stack.Count < 2)
					{
						path2 = false;
					}
					else
					{
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
						DependencyNode stackPrev = planarConfig.Stack.get(planarConfig.Stack.Count - 2);
						path2 = stackPrev.findComponent().Index == stackPeek.findComponent().Index;
					}
					return path1 || path2;
				}
			}
			if (trans == SHIFT)
			{
				if (connectednessConstraintOnShift && planarConfig.Input.Count == 1) //last word
				{
					bool path = (planarConfig.DependencyGraph.getTokenNode(1).findComponent().Index == inputPeek.findComponent().Index); //require connection to 1st
					return path;
				}
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction defaultAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction defaultAction(GuideUserHistory history, ParserConfiguration configuration)
		{
			return updateActionContainers(history, Planar.SHIFT, null);
		}
	}
}