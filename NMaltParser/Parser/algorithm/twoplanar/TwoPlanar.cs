using System.Collections.Generic;
using NMaltParser.Core.Propagation;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Parser.History;
using NMaltParser.Parser.History.Action;
using NMaltParser.Parser.Transition;

namespace NMaltParser.Parser.Algorithm.TwoPlanar
{
    /// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public class TwoPlanar : TransitionSystem
	{
		protected internal const int SHIFT = 1;
		protected internal const int SWITCH = 2;
		protected internal const int RIGHTARC = 3;
		protected internal const int LEFTARC = 4;
		protected internal const int REDUCE = 5;
		protected internal const int REDUCEBOTH = 6;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TwoPlanar(org.maltparser.core.propagation.PropagationManager propagationManager) throws org.maltparser.core.exception.MaltChainedException
		public TwoPlanar(PropagationManager propagationManager) : base(propagationManager)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void apply(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override void apply(GuideUserAction currentAction, ParserConfiguration config)
		{
			TwoPlanarConfig planarConfig = (TwoPlanarConfig)config;
			Stack<DependencyNode> activeStack = planarConfig.ActiveStack;
			Stack<DependencyNode> inactiveStack = planarConfig.InactiveStack;
			Stack<DependencyNode> input = planarConfig.Input;
			currentAction.getAction(actionContainers);
			Edge e = null;
			int actionCode = transActionContainer.ActionCode;
			switch (actionCode)
			{
			case LEFTARC:
				e = planarConfig.DependencyStructure.AddDependencyEdge(input.Peek().Index, activeStack.Peek().Index);
				addEdgeLabels(e);
				break;
			case RIGHTARC:
				e = planarConfig.DependencyStructure.AddDependencyEdge(activeStack.Peek().Index, input.Peek().Index);
				addEdgeLabels(e);
				break;
			case SWITCH:
				planarConfig.switchStacks();
				if (planarConfig.reduceAfterSwitch())
				{
					planarConfig.ActiveStack.Pop();
				}
				break;
			case REDUCE:
				activeStack.Pop();
				break;
			case REDUCEBOTH:
				activeStack.Pop();
				inactiveStack.Pop();
				break;
			default: //SHIFT
				DependencyNode n = input.Pop();
				activeStack.Push(n);
				inactiveStack.Push(n);
				break;
			}
			planarConfig.LastAction = actionCode;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction getDeterministicAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction getDeterministicAction(GuideUserHistory history, ParserConfiguration config)
		{
			TwoPlanarConfig theConfig = (TwoPlanarConfig)config;
			if (theConfig.getRootHandling() != TwoPlanarConfig.NORMAL && theConfig.ActiveStack.Peek().Root)
			{
				return updateActionContainers(history, SHIFT, null);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void addAvailableTransitionToTable(org.maltparser.parser.transition.TransitionTable ttable) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void addAvailableTransitionToTable(TransitionTable ttable)
		{
			ttable.addTransition(SHIFT, "SH", false, null);
			ttable.addTransition(SWITCH, "SW", false, null);
			ttable.addTransition(REDUCE, "RE", false, null);
			ttable.addTransition(REDUCEBOTH, "RB", false, null);
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
			transActionContainer.Action = SWITCH; //TODO it seems like a good idea to do this, but I don't know what it actually does
			transActionContainer.Action = REDUCEBOTH; //TODO same as above
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
				return "two-planar arc-eager";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean permissible(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override bool permissible(GuideUserAction currentAction, ParserConfiguration config)
		{
			currentAction.getAction(actionContainers);
			int trans = transActionContainer.ActionCode;
			TwoPlanarConfig planarConfig = (TwoPlanarConfig)config;
			DependencyNode activeStackPeek = planarConfig.ActiveStack.Peek();
			DependencyNode inactiveStackPeek = planarConfig.InactiveStack.Peek();
			DependencyNode inputPeek = planarConfig.Input.Peek();
			IDependencyStructure dg = planarConfig.DependencyGraph;
			//int rootHandling = planarConfig.getRootHandling();
			bool singleHeadConstraint = planarConfig.requiresSingleHead();
			bool noCoveredRootsConstraint = planarConfig.requiresNoCoveredRoots();
			bool acyclicityConstraint = planarConfig.requiresAcyclicity();
			//boolean connectednessConstraintOnReduce = planarConfig.requiresConnectednessCheckOnReduce();
			//boolean connectednessConstraintOnShift = planarConfig.requiresConnectednessCheckOnShift();
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
				if (activeStackPeek.Root)
				{
					return false;
				}
				//enforce single-head constraint if present
				if (activeStackPeek.hasHead() && singleHeadConstraint)
				{
					return false;
				}
				//avoid two links being created from and to the same node
				if (activeStackPeek.hasHead() && dg.GetTokenNode(activeStackPeek.Index).Head.Index == inputPeek.Index)
				{
					return false;
				}
				//enforce acyclicity constraint if present
				if (acyclicityConstraint && activeStackPeek.findComponent().Index == inputPeek.findComponent().Index)
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
				if (inputPeek.hasHead() && dg.GetTokenNode(inputPeek.Index).Head.Index == activeStackPeek.Index)
				{
					return false;
				}
				//enforce acyclicity constraint if present
				if (acyclicityConstraint && activeStackPeek.findComponent().Index == inputPeek.findComponent().Index)
				{
					return false;
				}
			}
			if (trans == REDUCE)
			{
				//do not reduce the dummy root
				if (activeStackPeek.Root)
				{
					return false;
				}
				//enforce no-covered-roots constraint if present
				if (!activeStackPeek.hasHead() && noCoveredRootsConstraint)
				{
					return false;
				}
				//TODO does this line still make sense? (from Nivre arc-eager)
				//if ( !stackPeek.hasHead() && rootHandling == PlanarConfig.STRICT ) 
				//	return false;
				//enforce connectedness constraint if present
				/*
				if ( connectednessConstraintOnReduce )
				{
					boolean path1 = ( stackPeek.findComponent().getIndex() == inputPeek.findComponent().getIndex() );
					boolean path2;
					if ( planarConfig.getStack().size() < 2 ) path2=false;
					else
					{
						DependencyNode stackPrev = planarConfig.getStack().get(planarConfig.getStack().size()-2);
						path2 = stackPrev.findComponent().getIndex() == stackPeek.findComponent().getIndex();
					}
					return path1 || path2;
				}
				*/
			}
			if (trans == SHIFT)
			{
				/*
				if ( connectednessConstraintOnShift && planarConfig.getInput().size() == 1 ) //last word
				{
					boolean path = ( planarConfig.getDependencyGraph().getTokenNode(1).findComponent().getIndex() == inputPeek.findComponent().getIndex() ); //require connection to 1st
					return path;
				}
				*/
			}
			if (trans == REDUCEBOTH)
			{
				//do not reduce the dummy root
				if (activeStackPeek.Root || inactiveStackPeek.Root)
				{
					return false;
				}
				//enforce no-covered-roots constraint if present
				if ((!activeStackPeek.hasHead() || inactiveStackPeek.hasHead()) && noCoveredRootsConstraint)
				{
					return false;
				}

				//TODO remove this:
				//not using this transition at the moment, so
				return false;
			}
			if (trans == SWITCH)
			{
				if (planarConfig.reduceAfterSwitch())
				{
					if (inactiveStackPeek.Root)
					{
						return false;
					}
					//enforce no-covered-roots constraint if present
					if (!inactiveStackPeek.hasHead() && noCoveredRootsConstraint)
					{
						return false;
					}
				}
				else
				{
					if (planarConfig.LastAction == SWITCH)
					{
						return false;
					}
				}
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction defaultAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction defaultAction(GuideUserHistory history, ParserConfiguration configuration)
		{
			return updateActionContainers(history, SHIFT, null);
		}


	}
}