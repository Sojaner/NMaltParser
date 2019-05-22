using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.parser.history.action
{

	using  core.exception;
	using  container;
    using  kbest;

	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ComplexDecisionAction : GuideUserAction, MultipleDecision
	{
		private readonly GuideUserHistory history;
		private readonly List<SimpleDecisionAction> decisions;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ComplexDecisionAction(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public ComplexDecisionAction(GuideUserHistory history)
		{
			this.history = history;
			decisions = new List<SimpleDecisionAction>(history.DecisionTables.Count);
			for (int i = 0, n = history.DecisionTables.Count; i < n; i++)
			{
				decisions.Add(new SimpleDecisionAction(history.KBestSize, history.DecisionTables[i]));
			}
		}

		/* GuideUserAction interface */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAction(java.util.ArrayList<org.maltparser.parser.history.container.ActionContainer> actionContainers) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addAction(List<ActionContainer> actionContainers)
		{
			if (actionContainers == null || actionContainers.Count != history.ActionTables.Count)
			{
				throw new HistoryException("The action containers does not exist or is not of the same size as the action table. ");
			}
			int j = 0;
			for (int i = 0, n = history.DecisionTables.Count; i < n; i++)
			{
				if (history.DecisionTables[i] is CombinedTableContainer)
				{
					CombinedTableContainer tableContainer = (CombinedTableContainer)history.DecisionTables[i];
					int nContainers = tableContainer.NumberContainers;
					decisions[i].addDecision(tableContainer.getCombinedCode(actionContainers.subList(j, j + nContainers)));
					j = j + nContainers;
				}
				else
				{
					decisions[i].addDecision(actionContainers[j].ActionCode);
					j++;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getAction(java.util.ArrayList<org.maltparser.parser.history.container.ActionContainer> actionContainers) throws org.maltparser.core.exception.MaltChainedException
		public virtual void getAction(List<ActionContainer> actionContainers)
		{
			if (actionContainers == null || actionContainers.Count != history.ActionTables.Count)
			{
				throw new HistoryException("The action containers does not exist or is not of the same size as the action table. ");
			}
			int j = 0;
			for (int i = 0, n = history.DecisionTables.Count; i < n; i++)
			{
				if (history.DecisionTables[i] is CombinedTableContainer)
				{
					CombinedTableContainer tableContainer = (CombinedTableContainer)history.DecisionTables[i];
					int nContainers = tableContainer.NumberContainers;
					tableContainer.setActionContainer(actionContainers.subList(j, j + nContainers), decisions[i].DecisionCode);
					j = j + nContainers;
				}
				else
				{
					actionContainers[j].Action = decisions[i].DecisionCode;
					j++;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAction(org.maltparser.parser.history.container.ActionContainer[] actionContainers) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addAction(ActionContainer[] actionContainers)
		{
			if (actionContainers == null || actionContainers.Length != history.ActionTables.Count)
			{
				throw new HistoryException("The action containers does not exist or is not of the same size as the action table. ");
			}
			int j = 0;
			for (int i = 0, n = history.DecisionTables.Count; i < n; i++)
			{
				if (history.DecisionTables[i] is CombinedTableContainer)
				{
					CombinedTableContainer tableContainer = (CombinedTableContainer)history.DecisionTables[i];
					int nContainers = tableContainer.NumberContainers;
					decisions[i].addDecision(tableContainer.getCombinedCode(actionContainers, j));
					j = j + nContainers;
				}
				else
				{
					decisions[i].addDecision(actionContainers[j].ActionCode);
					j++;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getAction(org.maltparser.parser.history.container.ActionContainer[] actionContainers) throws org.maltparser.core.exception.MaltChainedException
		public virtual void getAction(ActionContainer[] actionContainers)
		{
			if (actionContainers == null || actionContainers.Length != history.ActionTables.Count)
			{
				throw new HistoryException("The action containers does not exist or is not of the same size as the action table. ");
			}
			int j = 0;
			for (int i = 0, n = history.DecisionTables.Count; i < n; i++)
			{
				if (history.DecisionTables[i] is CombinedTableContainer)
				{
					CombinedTableContainer tableContainer = (CombinedTableContainer)history.DecisionTables[i];
					int nContainers = tableContainer.NumberContainers;
					tableContainer.setActionContainer(actionContainers, j, decisions[i].DecisionCode);
					j = j + nContainers;
				}
				else
				{
					actionContainers[j].Action = decisions[i].DecisionCode;
					j++;
				}
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getKBestLists(java.util.ArrayList<org.maltparser.parser.history.kbest.ScoredKBestList> kbestListContainers) throws org.maltparser.core.exception.MaltChainedException
		public virtual void getKBestLists(List<ScoredKBestList> kbestListContainers)
		{
	//		if (kbestListContainers == null || kbestListContainers.size() != history.getActionTables().size()) {
	//			throw new HistoryException("The action containers does not exist or is not of the same size as the action table. ");
	//		}
			kbestListContainers.Clear();
			for (int i = 0, n = decisions.Count; i < n; i++)
			{
				kbestListContainers.Add((ScoredKBestList)decisions[i].KBestList);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getKBestLists(org.maltparser.parser.history.kbest.ScoredKBestList[] kbestListContainers) throws org.maltparser.core.exception.MaltChainedException
		public virtual void getKBestLists(ScoredKBestList[] kbestListContainers)
		{
			for (int i = 0, n = decisions.Count; i < n; i++)
			{
				kbestListContainers[0] = (ScoredKBestList)decisions[i].KBestList;
			}
		}

		public virtual int numberOfActions()
		{
			return history.ActionTables.Count;
		}

		public virtual void clear()
		{
			for (int i = 0, n = decisions.Count; i < n;i++)
			{
				decisions[i].clear();
			}
		}

		/* MultipleDecision */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SingleDecision getSingleDecision(int decisionIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual SingleDecision getSingleDecision(int decisionIndex)
		{
			return decisions[decisionIndex];
		}

		/* GuideDecision */
		public virtual int numberOfDecisions()
		{
			return history.DecisionTables.Count;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			ComplexDecisionAction other = (ComplexDecisionAction) obj;
			if (decisions == null)
			{
				if (other.decisions != null)
				{
					return false;
				}
			}
			else if (decisions.Count != other.decisions.Count)
			{
				return false;
			}
			else
			{
				for (int i = 0; i < decisions.Count; i++)
				{
					try
					{
						if (decisions[i].DecisionCode != other.decisions[i].DecisionCode)
						{
							return false;
						}
					}
					catch (MaltChainedException)
					{
						Console.Error.WriteLine("Error in equals. ");
					}
				}
			}

			return true;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0, n = decisions.Count; i < n; i++)
			{
				sb.Append(decisions[i]);
				sb.Append(';');
			}
			if (sb.Length > 0)
			{
				sb.Length = sb.Length - 1;
			}
			return sb.ToString();
		}
	}

}