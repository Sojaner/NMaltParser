using System.Collections.Generic;
using NMaltParser.Core.Propagation;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Parser.History;
using NMaltParser.Parser.History.Action;
using NMaltParser.Parser.Transition;

namespace NMaltParser.Parser.Algorithm.Nivre
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ArcStandard : TransitionSystem
	{
		protected internal const int SHIFT = 1;
		protected internal const int RIGHTARC = 2;
		protected internal const int LEFTARC = 3;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArcStandard(org.maltparser.core.propagation.PropagationManager propagationManager) throws org.maltparser.core.exception.MaltChainedException
		public ArcStandard(PropagationManager propagationManager) : base(propagationManager)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void apply(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override void apply(GuideUserAction currentAction, ParserConfiguration config)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NivreConfig nivreConfig = (NivreConfig)config;
			NivreConfig nivreConfig = (NivreConfig)config;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> stack = nivreConfig.getStack();
			Stack<DependencyNode> stack = nivreConfig.Stack;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack<org.maltparser.core.syntaxgraph.node.DependencyNode> input = nivreConfig.getInput();
			Stack<DependencyNode> input = nivreConfig.Input;
			currentAction.getAction(actionContainers);
			Edge e = null;
			switch (transActionContainer.ActionCode)
			{
			case LEFTARC:
				e = nivreConfig.DependencyStructure.addDependencyEdge(input.Peek().Index, stack.Peek().Index);
				addEdgeLabels(e);
				stack.Pop();
				break;
			case RIGHTARC:
				e = nivreConfig.DependencyStructure.addDependencyEdge(stack.Peek().Index, input.Peek().Index);
				addEdgeLabels(e);
				input.Pop();
				if (!stack.Peek().Root)
				{
					input.Push(stack.Pop());
				}
				break;
			default:
				stack.Push(input.Pop()); // SHIFT
				break;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction getDeterministicAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction getDeterministicAction(GuideUserHistory history, ParserConfiguration config)
		{
			NivreConfig nivreConfig = (NivreConfig)config;
			if (!nivreConfig.AllowRoot && nivreConfig.Stack.Peek().Root)
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
				return "nivrestandard";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean permissible(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override bool permissible(GuideUserAction currentAction, ParserConfiguration config)
		{
			currentAction.getAction(actionContainers);
			int trans = transActionContainer.ActionCode;
			if ((trans == LEFTARC || trans == RIGHTARC) && !ActionContainersLabeled)
			{
				return false;
			}
			DependencyNode stackTop = ((NivreConfig)config).Stack.Peek();
			if (!((NivreConfig)config).AllowRoot && stackTop.Root && trans != SHIFT)
			{
				return false;
			}
			if (trans == LEFTARC && stackTop.Root)
			{
				return false;
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