using System;

namespace org.maltparser.parser
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using LabelSet = org.maltparser.core.syntaxgraph.LabelSet;
	using OracleGuide = org.maltparser.parser.guide.OracleGuide;
	using GuideUserHistory = org.maltparser.parser.history.GuideUserHistory;
	using GuideUserAction = org.maltparser.parser.history.action.GuideUserAction;
	using ActionContainer = org.maltparser.parser.history.container.ActionContainer;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class Oracle : OracleGuide
	{
		public abstract void terminate();
		public abstract void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph);
		public abstract GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, ParserConfiguration config);
		private readonly DependencyParserConfig manager;
		private readonly GuideUserHistory history;
		private string name;
		protected internal readonly ActionContainer[] actionContainers;
		protected internal ActionContainer transActionContainer;
		protected internal readonly ActionContainer[] arcLabelActionContainers;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Oracle(DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public Oracle(DependencyParserConfig manager, GuideUserHistory history)
		{
			this.manager = manager;
			this.history = history;
			this.actionContainers = history.ActionContainerArray;

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
			this.arcLabelActionContainers = new ActionContainer[nLabels];
			for (int i = 0; i < actionContainers.Length; i++)
			{
				if (actionContainers[i].TableContainerName.Equals("T.TRANS"))
				{
					transActionContainer = actionContainers[i];
				}
				else if (actionContainers[i].TableContainerName.StartsWith("A.", StringComparison.Ordinal))
				{
					arcLabelActionContainers[j++] = actionContainers[i];
				}
			}
		}

		public virtual GuideUserHistory History
		{
			get
			{
				return history;
			}
		}

		public virtual DependencyParserConfig Configuration
		{
			get
			{
				return manager;
			}
		}

		public virtual string GuideName
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.parser.history.action.GuideUserAction updateActionContainers(int transition, org.maltparser.core.syntaxgraph.LabelSet arcLabels) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual GuideUserAction updateActionContainers(int transition, LabelSet arcLabels)
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
					arcLabelActionContainers[i].Action = arcLabels.get(arcLabelActionContainers[i].Table).shortValue();
				}
			}
			GuideUserAction oracleAction = history.EmptyGuideUserAction;
			oracleAction.addAction(actionContainers);
			return oracleAction;
		}
	}

}