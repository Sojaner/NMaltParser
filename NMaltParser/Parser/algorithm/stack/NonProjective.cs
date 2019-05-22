using System.Collections.Generic;

namespace org.maltparser.parser.algorithm.stack
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.propagation;
	using  org.maltparser.core.syntaxgraph.edge;
	using  org.maltparser.core.syntaxgraph.node;
	using  org.maltparser.parser.history;
	using  org.maltparser.parser.history.action;
	using  org.maltparser.parser.history.action;
	using  org.maltparser.parser.transition;


	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class NonProjective : TransitionSystem
	{
		protected internal const int SHIFT = 1;
		protected internal const int SWAP = 2;
		protected internal const int RIGHTARC = 3;
		protected internal const int LEFTARC = 4;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NonProjective(org.maltparser.core.propagation.PropagationManager propagationManager) throws org.maltparser.core.exception.MaltChainedException
		public NonProjective(PropagationManager propagationManager) : base(propagationManager)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void apply(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override void apply(GuideUserAction currentAction, ParserConfiguration configuration)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StackConfig config = (StackConfig)configuration;
			StackConfig config = (StackConfig)configuration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> stack = config.getStack();
			Stack<DependencyNode> stack = config.Stack;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> input = config.getInput();
			Stack<DependencyNode> input = config.Input;
			currentAction.getAction(actionContainers);
			Edge e = null;
			DependencyNode head = null;
			DependencyNode dep = null;
			switch (transActionContainer.ActionCode)
			{
			case LEFTARC:
				head = stack.Pop();
				dep = stack.Pop();
				e = config.DependencyStructure.addDependencyEdge(head.Index, dep.Index);
				addEdgeLabels(e);
				stack.Push(head);
				break;
			case RIGHTARC:
				dep = stack.Pop();
				e = config.DependencyStructure.addDependencyEdge(stack.Peek().Index, dep.Index);
				addEdgeLabels(e);
				break;
			case SWAP:
				dep = stack.Pop();
				input.Push(stack.Pop());
				stack.Push(dep);
				config.lookaheadIncrement();
				break;
			default:
				if (input.Count == 0)
				{
					stack.Pop();
				}
				else
				{
					stack.Push(input.Pop()); // SHIFT
				}
				config.lookaheadDecrement();
				break;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean permissible(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override bool permissible(GuideUserAction currentAction, ParserConfiguration configuration)
		{
			currentAction.getAction(actionContainers);
			int trans = transActionContainer.ActionCode;
			if ((trans == LEFTARC || trans == RIGHTARC) && !ActionContainersLabeled)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StackConfig config = (StackConfig)configuration;
			StackConfig config = (StackConfig)configuration;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> stack = config.getStack();
			Stack<DependencyNode> stack = config.Stack;
			if ((trans == LEFTARC || trans == RIGHTARC || trans == SWAP) && stack.Count < 2)
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
			if ((trans == LEFTARC || trans == SWAP) && stack.get(stack.Count - 2).Root)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> input = config.getInput();
			Stack<DependencyNode> input = config.Input;
			if (trans == SHIFT && input.Count == 0)
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
			if (trans == SWAP && stack.get(stack.Count - 2).Index > stack.get(stack.Count - 1).Index)
			{
				return false;
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction getDeterministicAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction getDeterministicAction(GuideUserHistory history, ParserConfiguration config)
		{
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void addAvailableTransitionToTable(org.maltparser.parser.transition.TransitionTable ttable) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void addAvailableTransitionToTable(TransitionTable ttable)
		{
			ttable.addTransition(SHIFT, "SH", false, null);
			ttable.addTransition(SWAP, "SW", false, null);
			ttable.addTransition(RIGHTARC, "RA", true, null);
			ttable.addTransition(LEFTARC, "LA", true, null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initWithDefaultTransitions(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void initWithDefaultTransitions(GuideUserHistory history)
		{
			GuideUserAction currentAction = new ComplexDecisionAction(history);

			transActionContainer.Action = SHIFT;
			//transActionContainer.setAction(SWAP);
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
				return "nonprojective";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction defaultAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction defaultAction(GuideUserHistory history, ParserConfiguration configuration)
		{
			if (((StackConfig)configuration).Input.Count == 0)
			{
				return updateActionContainers(history, RIGHTARC, ((StackConfig)configuration).DependencyGraph.DefaultRootEdgeLabels);
			}
			return updateActionContainers(history, SHIFT, null);
		}
	}

}