using System;
using System.Text;

namespace org.maltparser.parser
{

	using  core.exception;
	using  core.helper;
	using  core.propagation;
	using  core.symbol;
    using  core.syntaxgraph;
    using  core.syntaxgraph.edge;
	using  history;
	using  history.action;
	using  history.container;
	using  transition;

    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class TransitionSystem
	{
		public static readonly Pattern decisionSettingsSplitPattern = Pattern.compile(",|#|;|\\+");
		private readonly HashMap<string, TableHandler> tableHandlers;
		private readonly PropagationManager propagationManager;
		protected internal readonly TransitionTableHandler transitionTableHandler;
		protected internal ActionContainer[] actionContainers;
		protected internal ActionContainer transActionContainer;
		protected internal ActionContainer[] arcLabelActionContainers;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TransitionSystem(org.maltparser.core.propagation.PropagationManager _propagationManager) throws org.maltparser.core.exception.MaltChainedException
		public TransitionSystem(PropagationManager _propagationManager)
		{
			transitionTableHandler = new TransitionTableHandler();
			tableHandlers = new HashMap<string, TableHandler>();
			propagationManager = _propagationManager;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void apply(org.maltparser.parser.history.action.GuideUserAction currentAction, ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException;
		public abstract void apply(GuideUserAction currentAction, ParserConfiguration config);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract boolean permissible(org.maltparser.parser.history.action.GuideUserAction currentAction, ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException;
		public abstract bool permissible(GuideUserAction currentAction, ParserConfiguration config);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract org.maltparser.parser.history.action.GuideUserAction getDeterministicAction(org.maltparser.parser.history.GuideUserHistory history, ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException;
		public abstract GuideUserAction getDeterministicAction(GuideUserHistory history, ParserConfiguration config);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void addAvailableTransitionToTable(org.maltparser.parser.transition.TransitionTable ttable) throws org.maltparser.core.exception.MaltChainedException;
		protected internal abstract void addAvailableTransitionToTable(TransitionTable ttable);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void initWithDefaultTransitions(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException;
		protected internal abstract void initWithDefaultTransitions(GuideUserHistory history);
		public abstract string Name {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract org.maltparser.parser.history.action.GuideUserAction defaultAction(org.maltparser.parser.history.GuideUserHistory history, ParserConfiguration configuration) throws org.maltparser.core.exception.MaltChainedException;
		public abstract GuideUserAction defaultAction(GuideUserHistory history, ParserConfiguration configuration);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.parser.history.action.GuideUserAction updateActionContainers(org.maltparser.parser.history.GuideUserHistory history, int transition, org.maltparser.core.syntaxgraph.LabelSet arcLabels) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual GuideUserAction updateActionContainers(GuideUserHistory history, int transition, LabelSet arcLabels)
		{
			transActionContainer.Action = transition;

			if (arcLabels == null)
			{
				for (int i = 0; i < arcLabelActionContainers.Length; i++)
				{
					arcLabelActionContainers[i].Action = -1;
				}
			}
			else
			{
				for (int i = 0; i < arcLabelActionContainers.Length; i++)
				{
					if (arcLabelActionContainers[i] == null)
					{
						throw new MaltChainedException("arcLabelActionContainer " + i + " is null when doing transition " + transition);
					}

					int? code = arcLabels.get(arcLabelActionContainers[i].Table);
					if (code != null)
					{
						arcLabelActionContainers[i].Action = code.Value;
					}
					else
					{
						arcLabelActionContainers[i].Action = -1;
					}
				}
			}
			GuideUserAction oracleAction = history.EmptyGuideUserAction;
			oracleAction.addAction(actionContainers);
			return oracleAction;
		}

		protected internal virtual bool ActionContainersLabeled
		{
			get
			{
				for (int i = 0; i < arcLabelActionContainers.Length; i++)
				{
					if (arcLabelActionContainers[i].ActionCode < 0)
					{
						return false;
					}
				}
				return true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void addEdgeLabels(org.maltparser.core.syntaxgraph.edge.Edge e) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void addEdgeLabels(Edge e)
		{
			if (e != null)
			{
				for (int i = 0; i < arcLabelActionContainers.Length; i++)
				{
					if (arcLabelActionContainers[i].ActionCode != -1)
					{
						e.addLabel((SymbolTable)arcLabelActionContainers[i].Table, arcLabelActionContainers[i].ActionCode);
					}
					else
					{
						e.addLabel((SymbolTable)arcLabelActionContainers[i].Table, ((DependencyStructure)e.BelongsToGraph).getDefaultRootEdgeLabelCode((SymbolTable)arcLabelActionContainers[i].Table));
					}
				}
				if (propagationManager != null)
				{
					propagationManager.propagate(e);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initTransitionSystem(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initTransitionSystem(GuideUserHistory history)
		{
			actionContainers = history.ActionContainerArray;
			if (actionContainers.Length < 1)
			{
				throw new ParsingException("Problem when initialize the history (sequence of actions). There are no action containers. ");
			}
			int nLabels = 0;
			for (int i = 0; i < actionContainers.Length; i++)
			{
				if (actionContainers[i].TableContainerName.StartsWith("A.", StringComparison.Ordinal))
				{
					nLabels++;
				}
			}
			int j = 0;
			for (int i = 0; i < actionContainers.Length; i++)
			{
				if (actionContainers[i].TableContainerName.Equals("T.TRANS"))
				{
					transActionContainer = actionContainers[i];
				}
				else if (actionContainers[i].TableContainerName.StartsWith("A.", StringComparison.Ordinal))
				{
					if (arcLabelActionContainers == null)
					{
						arcLabelActionContainers = new ActionContainer[nLabels];
					}
					arcLabelActionContainers[j++] = actionContainers[i];
				}
			}
			initWithDefaultTransitions(history);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initTableHandlers(String decisionSettings, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initTableHandlers(string decisionSettings, SymbolTableHandler symbolTableHandler)
		{
			if (decisionSettings.Equals("T.TRANS+A.DEPREL") || decisionSettings.Equals("T.TRANS#A.DEPREL") || decisionSettings.Equals("T.TRANS,A.DEPREL") || decisionSettings.Equals("T.TRANS;A.DEPREL"))
			{
				tableHandlers["T"] = transitionTableHandler;
				addAvailableTransitionToTable((TransitionTable)transitionTableHandler.addSymbolTable("TRANS"));
				tableHandlers["A"] = symbolTableHandler;
				return;
			}
			initTableHandlers(decisionSettingsSplitPattern.split(decisionSettings), decisionSettings, symbolTableHandler);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initTableHandlers(String[] decisionElements, String decisionSettings, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initTableHandlers(string[] decisionElements, string decisionSettings, SymbolTableHandler symbolTableHandler)
		{
			int nTrans = 0;
			for (int i = 0; i < decisionElements.Length; i++)
			{
				int index = decisionElements[i].IndexOf('.');
				if (index == -1)
				{
					throw new ParsingException("Decision settings '" + decisionSettings + "' contain an item '" + decisionElements[i] + "' that does not follow the format {TableHandler}.{Table}. ");
				}
				if (decisionElements[i].Substring(0,index).Equals("T"))
				{
					if (!tableHandlers.ContainsKey("T"))
					{
						tableHandlers["T"] = transitionTableHandler;
					}
					if (decisionElements[i].Substring(index + 1).Equals("TRANS"))
					{
						if (nTrans == 0)
						{
							addAvailableTransitionToTable((TransitionTable)transitionTableHandler.addSymbolTable("TRANS"));
						}
						else
						{
							throw new ParsingException("Illegal decision settings '" + decisionSettings + "'");
						}
						nTrans++;
					}
				}
				else if (decisionElements[i].Substring(0,index).Equals("A"))
				{
					if (!tableHandlers.ContainsKey("A"))
					{
						tableHandlers["A"] = symbolTableHandler;
					}
				}
				else
				{
					throw new ParsingException("The decision settings '" + decisionSettings + "' contains an unknown table handler '" + decisionElements[i].Substring(0,index) + "'. " + "Only T (Transition table handler) and A (ArcLabel table handler) is allowed. ");
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copyAction(org.maltparser.parser.history.action.GuideUserAction source, org.maltparser.parser.history.action.GuideUserAction target) throws org.maltparser.core.exception.MaltChainedException
		public virtual void copyAction(GuideUserAction source, GuideUserAction target)
		{
			source.getAction(actionContainers);
			target.addAction(actionContainers);
		}

		public virtual HashMap<string, TableHandler> TableHandlers
		{
			get
			{
				return tableHandlers;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getActionString(org.maltparser.parser.history.action.GuideUserAction action) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getActionString(GuideUserAction action)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			action.getAction(actionContainers);
			Table ttable = transitionTableHandler.getSymbolTable("TRANS");
			sb.Append(ttable.getSymbolCodeToString(transActionContainer.ActionCode));
			for (int i = 0; i < arcLabelActionContainers.Length; i++)
			{
				if (arcLabelActionContainers[i].ActionCode != -1)
				{
					sb.Append("+");
					sb.Append(arcLabelActionContainers[i].Table.getSymbolCodeToString(arcLabelActionContainers[i].ActionCode));
				}
			}
			return sb.ToString();
		}
	}

}