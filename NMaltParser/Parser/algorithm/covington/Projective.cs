using NMaltParser.Core.Propagation;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Parser.History;
using NMaltParser.Parser.History.Action;
using NMaltParser.Parser.Transition;

namespace NMaltParser.Parser.Algorithm.Covington
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class Projective : TransitionSystem
	{
		protected internal const int SHIFT = 1;
		protected internal const int NOARC = 2;
		protected internal const int RIGHTARC = 3;
		protected internal const int LEFTARC = 4;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Projective(org.maltparser.core.propagation.PropagationManager propagationManager) throws org.maltparser.core.exception.MaltChainedException
		public Projective(PropagationManager propagationManager) : base(propagationManager)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void apply(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override void apply(GuideUserAction currentAction, ParserConfiguration config)
		{
			CovingtonConfig covingtonConfig = (CovingtonConfig)config;
			currentAction.getAction(actionContainers);

			Edge e = null;
			switch (transActionContainer.ActionCode)
			{
			case LEFTARC:
				e = covingtonConfig.DependencyGraph.addDependencyEdge(covingtonConfig.RightTarget.Index, covingtonConfig.LeftTarget.Index);
				addEdgeLabels(e);
	//			config.setArcParent(covingtonConfig.getRightTarget());
	//			config.setArcChild(covingtonConfig.getLeftTarget());
				break;
			case RIGHTARC:
				e = covingtonConfig.DependencyGraph.addDependencyEdge(covingtonConfig.LeftTarget.Index, covingtonConfig.RightTarget.Index);
				addEdgeLabels(e);
	//			config.setArcParent(covingtonConfig.getLeftTarget());
	//			config.setArcChild(covingtonConfig.getRightTarget());
				break;
			default:
	//			config.setArcParent(null);
	//			config.setArcChild(null);
				break;
			}
			update(covingtonConfig, transActionContainer.ActionCode);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void update(CovingtonConfig covingtonConfig, int trans) throws org.maltparser.core.exception.MaltChainedException
		private void update(CovingtonConfig covingtonConfig, int trans)
		{
			if (trans == SHIFT || trans == RIGHTARC)
			{
				covingtonConfig.Right = covingtonConfig.Right + 1;
				covingtonConfig.Left = covingtonConfig.Right - 1;
			}
			else
			{
				int leftstop = covingtonConfig.Leftstop;
				int left = covingtonConfig.Left;
				if (trans == NOARC)
				{
					DependencyStructure dg = covingtonConfig.DependencyStructure;
					DependencyNode leftNode = covingtonConfig.Input[covingtonConfig.Left];
					if (dg.getTokenNode(leftNode.Index) != null && dg.getTokenNode(leftNode.Index).hasHead())
					{
						left = dg.getTokenNode(leftNode.Index).Head.Index;
					}
					else
					{
						left = leftstop - 1;
					}
				}
				else
				{
					DependencyNode rightNode = covingtonConfig.RightTarget;
					left--;
					DependencyNode leftNode = null;
					while (left >= leftstop)
					{
						leftNode = covingtonConfig.Input[left];
						if (rightNode.findComponent().Index != leftNode.findComponent().Index)
						{
							break;
						}
						left--;
					}
				}

				if (left < leftstop)
				{
					covingtonConfig.Right = covingtonConfig.Right + 1;
					covingtonConfig.Left = covingtonConfig.Right - 1;
				}
				else
				{
					covingtonConfig.Left = left;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction getDeterministicAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction getDeterministicAction(GuideUserHistory history, ParserConfiguration config)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CovingtonConfig covingtonConfig = (CovingtonConfig)config;
			CovingtonConfig covingtonConfig = (CovingtonConfig)config;
			if (!covingtonConfig.AllowRoot && covingtonConfig.LeftTarget.Root)
			{
					return updateActionContainers(history, NOARC, null);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void addAvailableTransitionToTable(org.maltparser.parser.transition.TransitionTable ttable) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void addAvailableTransitionToTable(TransitionTable ttable)
		{
			ttable.addTransition(SHIFT, "SH", false, null);
			ttable.addTransition(NOARC, "NA", false, null);
			ttable.addTransition(RIGHTARC, "RA", true, null);
			ttable.addTransition(LEFTARC, "LA", true, null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initWithDefaultTransitions(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void initWithDefaultTransitions(GuideUserHistory history)
		{
			GuideUserAction currentAction = new ComplexDecisionAction(history);

			transActionContainer.Action = SHIFT;
			transActionContainer.Action = NOARC;
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
				return "covproj";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean permissible(org.maltparser.parser.history.action.GuideUserAction currentAction, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException
		public override bool permissible(GuideUserAction currentAction, ParserConfiguration config)
		{
			CovingtonConfig covingtonConfig = (CovingtonConfig)config;
			DependencyNode leftTarget = covingtonConfig.LeftTarget;
			DependencyNode rightTarget = covingtonConfig.RightTarget;
			DependencyStructure dg = covingtonConfig.DependencyGraph;
			currentAction.getAction(actionContainers);
			int trans = transActionContainer.ActionCode;

			if (trans == SHIFT && covingtonConfig.AllowShift == false)
			{
				return false;
			}
			if ((trans == LEFTARC || trans == RIGHTARC) && !ActionContainersLabeled)
			{
				return false;
			}
			if (trans == LEFTARC && leftTarget.Root)
			{
				return false;
			}
			if (trans == LEFTARC && dg.hasLabeledDependency(leftTarget.Index))
			{
				return false;
			}
			if (trans == RIGHTARC && dg.hasLabeledDependency(rightTarget.Index))
			{
				return false;
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction defaultAction(org.maltparser.parser.history.GuideUserHistory history, org.maltparser.parser.ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException
		public override GuideUserAction defaultAction(GuideUserHistory history, ParserConfiguration configuration)
		{
			return updateActionContainers(history, NOARC, null);
		}
	}


}