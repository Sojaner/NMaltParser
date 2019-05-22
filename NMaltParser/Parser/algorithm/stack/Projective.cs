using System.Collections.Generic;

namespace org.maltparser.parser.algorithm.stack
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
	/// @author Johan Hall
	/// 
	/// </summary>
	public class Projective : TransitionSystem
	{
		protected internal const int SHIFT = 1;
		protected internal const int RIGHTARC = 2;
		protected internal const int LEFTARC = 3;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Projective(org.maltparser.core.propagation.PropagationManager propagationManager) throws org.maltparser.core.exception.MaltChainedException
		public Projective(PropagationManager propagationManager) : base(propagationManager)
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
			default:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> input = config.getInput();
				Stack<DependencyNode> input = config.Input;
				if (input.Count == 0)
				{
					stack.Pop();
				}
				else
				{
					stack.Push(input.Pop()); // SHIFT
				}
				break;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean permissible(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override bool permissible(GuideUserAction currentAction, ParserConfiguration configuration)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StackConfig config = (StackConfig)configuration;
			StackConfig config = (StackConfig)configuration;
			currentAction.getAction(actionContainers);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int trans = transActionContainer.getActionCode();
			int trans = transActionContainer.ActionCode;
			if ((trans == LEFTARC || trans == RIGHTARC) && !ActionContainersLabeled)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> stack = config.getStack();
			Stack<DependencyNode> stack = config.Stack;
			if ((trans == LEFTARC || trans == RIGHTARC) && stack.Count < 2)
			{
				return false;
			}
//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
			if (trans == LEFTARC && stack.get(stack.Count - 2).Root)
			{
				return false;
			}
			if (trans == SHIFT && config.Input.Count == 0)
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
			ttable.addTransition(RIGHTARC, "RA", true, null);
			ttable.addTransition(LEFTARC, "LA", true, null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initWithDefaultTransitions(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void initWithDefaultTransitions(GuideUserHistory history)
		{
			GuideUserAction currentAction = new ComplexDecisionAction(history);

			transActionContainer.Action = SHIFT;
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
				return "projective";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction defaultAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction defaultAction(GuideUserHistory history, ParserConfiguration configuration)
		{
			if (((StackConfig)configuration).Input.Count == 0)
			{
				LabelSet labelSet = ((StackConfig)configuration).DependencyGraph.DefaultRootEdgeLabels;
				return updateActionContainers(history, RIGHTARC, labelSet);
			}
			return updateActionContainers(history, SHIFT, null);
		}
	}

}