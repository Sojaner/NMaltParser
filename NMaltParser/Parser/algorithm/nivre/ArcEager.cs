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
	public class ArcEager : TransitionSystem
	{
		protected internal const int SHIFT = 1;
		protected internal const int REDUCE = 2;
		protected internal const int RIGHTARC = 3;
		protected internal const int LEFTARC = 4;
		protected internal const int UNSHIFT = 5;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArcEager(org.maltparser.core.propagation.PropagationManager propagationManager) throws org.maltparser.core.exception.MaltChainedException
		public ArcEager(PropagationManager propagationManager) : base(propagationManager)
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
			if (!nivreConfig.EnforceTree)
			{
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
					stack.Push(input.Pop());
					break;
				case REDUCE:
					stack.Pop();
					break;
				default:
					stack.Push(input.Pop());
					break;
				}
			}
			else
			{
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
					stack.Push(input.Pop());
					if (input.Count == 0 && !nivreConfig.End)
					{
						nivreConfig.End = true;
					}
					break;
				case REDUCE:
					stack.Pop();
					break;
				case UNSHIFT:
					input.Push(stack.Pop());
					break;
				default:
					stack.Push(input.Pop());

					if (input.Count == 0 && !nivreConfig.End)
					{
						nivreConfig.End = true;
					}

					break;
				}

			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction getDeterministicAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction getDeterministicAction(GuideUserHistory history, ParserConfiguration config)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NivreConfig nivreConfig = (NivreConfig)config;
			NivreConfig nivreConfig = (NivreConfig)config;
			if (!nivreConfig.EnforceTree)
			{
			  if (!nivreConfig.AllowRoot && nivreConfig.Stack.Peek().Root)
			  {
				  return updateActionContainers(history, SHIFT, null);
			  }
			}
			else
			{
				//Added
				if (!nivreConfig.AllowRoot && nivreConfig.Stack.Peek().Root && !nivreConfig.End)
				{
					return updateActionContainers(history, SHIFT, null);
				}

				if (nivreConfig.Input.Count == 0 && nivreConfig.Stack.Peek().hasHead())
				{
					return updateActionContainers(history, REDUCE, null);
				}

				if (nivreConfig.Input.Count == 0 && !nivreConfig.Stack.Peek().hasHead())
				{
					return updateActionContainers(history, UNSHIFT, null);
				}
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
			ttable.addTransition(UNSHIFT, "USH", false, null); //Added
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
				return "nivreeager";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean permissible(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override bool permissible(GuideUserAction currentAction, ParserConfiguration config)
		{
			currentAction.getAction(actionContainers);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int trans = transActionContainer.getActionCode();
			int trans = transActionContainer.ActionCode;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NivreConfig nivreConfig = (NivreConfig)config;
			NivreConfig nivreConfig = (NivreConfig)config;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.syntaxgraph.node.DependencyNode stackPeek = nivreConfig.getStack().peek();
			DependencyNode stackPeek = nivreConfig.Stack.Peek();
			if ((trans == LEFTARC || trans == RIGHTARC) && !ActionContainersLabeled)
			{
				return false;
			}
			if ((trans == LEFTARC || trans == REDUCE) && stackPeek.Root)
			{
				return false;
			}
			if (trans == LEFTARC && stackPeek.hasHead())
			{
				return false;
			}
			if (trans == REDUCE && !stackPeek.hasHead() && !nivreConfig.AllowReduce)
			{
				return false;
			}
			//Added                                                                                                                                                   
			if (trans == SHIFT && nivreConfig.EnforceTree && nivreConfig.End)
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