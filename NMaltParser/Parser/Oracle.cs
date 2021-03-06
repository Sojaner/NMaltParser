﻿using System;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Parser.Guide;
using NMaltParser.Parser.History;
using NMaltParser.Parser.History.Action;
using NMaltParser.Parser.History.Container;

namespace NMaltParser.Parser
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class Oracle : OracleGuide
	{
		public abstract void terminate();
		public abstract void finalizeSentence(IDependencyStructure dependencyGraph);
		public abstract GuideUserAction predict(IDependencyStructure gold, ParserConfiguration config);
		private readonly IDependencyParserConfig manager;
		private readonly GuideUserHistory history;
		private string name;
		protected internal readonly ActionContainer[] actionContainers;
		protected internal ActionContainer transActionContainer;
		protected internal readonly ActionContainer[] arcLabelActionContainers;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Oracle(DependencyParserConfig manager, org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public Oracle(IDependencyParserConfig manager, GuideUserHistory history)
		{
			this.manager = manager;
			this.history = history;
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
			arcLabelActionContainers = new ActionContainer[nLabels];
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

		public virtual IDependencyParserConfig Configuration
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
				name = value;
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