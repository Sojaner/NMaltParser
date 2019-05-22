using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Helper;
using NMaltParser.Core.Pool;
using NMaltParser.Core.Symbol;
using NMaltParser.Parser.History.Action;
using NMaltParser.Parser.History.Container;

namespace NMaltParser.Parser.History
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class History : GuideUserHistory
	{
		private readonly ObjectPoolList<ComplexDecisionAction> actionPool;
		private readonly int kBestSize;
		private readonly string separator;
		private readonly string decisionSettings;
		private readonly List<TableContainer> decisionTables;
		private readonly List<TableContainer> actionTables;
		private readonly HashMap<string, TableHandler> tableHandlers;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public History(String _decisionSettings, String _separator, org.maltparser.core.helper.HashMap<String, org.maltparser.core.symbol.TableHandler> _tableHandlers, int _kBestSize) throws org.maltparser.core.exception.MaltChainedException
		public History(string _decisionSettings, string _separator, HashMap<string, TableHandler> _tableHandlers, int _kBestSize)
		{
			tableHandlers = _tableHandlers;
			if (ReferenceEquals(_separator, null) || _separator.Length < 1)
			{
				separator = "~";
			}
			else
			{
				separator = _separator;
			}
			kBestSize = _kBestSize;
			decisionTables = new List<TableContainer>();
			actionTables = new List<TableContainer>();
			decisionSettings = _decisionSettings;
			initDecisionSettings();
			actionPool = new ObjectPoolListAnonymousInnerClass(this);
			clear();
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<ComplexDecisionAction>
		{
			private readonly History outerInstance;

			public ObjectPoolListAnonymousInnerClass(History outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.parser.history.action.ComplexDecisionAction create() throws org.maltparser.core.exception.MaltChainedException
			protected internal override ComplexDecisionAction create()
			{
				return new ComplexDecisionAction(outerInstance.This);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetObject(org.maltparser.parser.history.action.ComplexDecisionAction o) throws org.maltparser.core.exception.MaltChainedException
			public void resetObject(ComplexDecisionAction o)
			{
				o.clear();
			}
		}

		private History This
		{
			get
			{
				return this;
			}
		}

		/* GuideUserHistory interface */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction getEmptyGuideUserAction() throws org.maltparser.core.exception.MaltChainedException
		public virtual GuideUserAction EmptyGuideUserAction
		{
			get
			{
				return (GuideUserAction)EmptyActionObject;
			}
		}

		public virtual List<ActionContainer> ActionContainers
		{
			get
			{
				List<ActionContainer> actionContainers = new List<ActionContainer>();
				for (int i = 0; i < actionTables.Count; i++)
				{
					actionContainers.Add(new ActionContainer(actionTables[i]));
				}
				return actionContainers;
			}
		}

		public virtual ActionContainer[] ActionContainerArray
		{
			get
			{
				ActionContainer[] actionContainers = new ActionContainer[actionTables.Count];
				for (int i = 0; i < actionTables.Count; i++)
				{
					actionContainers[i] = new ActionContainer(actionTables[i]);
				}
				return actionContainers;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public virtual void clear()
		{
			actionPool.checkInAll();
		}

		/* GuideHistory interface */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideDecision getEmptyGuideDecision() throws org.maltparser.core.exception.MaltChainedException
		public virtual GuideDecision EmptyGuideDecision
		{
			get
			{
				return (GuideDecision)EmptyActionObject;
			}
		}

		public virtual int NumberOfDecisions
		{
			get
			{
				return decisionTables.Count;
			}
		}

		public virtual TableHandler getTableHandler(string name)
		{
			return tableHandlers[name];
		}

		public virtual int KBestSize
		{
			get
			{
				return kBestSize;
			}
		}

		public virtual int NumberOfActions
		{
			get
			{
				return actionTables.Count;
			}
		}

		public virtual List<TableContainer> DecisionTables
		{
			get
			{
				return decisionTables;
			}
		}

		public virtual List<TableContainer> ActionTables
		{
			get
			{
				return actionTables;
			}
		}

		public virtual HashMap<string, TableHandler> TableHandlers
		{
			get
			{
				return tableHandlers;
			}
		}

		public virtual string Separator
		{
			get
			{
				return separator;
			}
		}

		public virtual string DecisionSettings
		{
			get
			{
				return decisionSettings;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.parser.history.action.ActionDecision getEmptyActionObject() throws org.maltparser.core.exception.MaltChainedException
		private ActionDecision EmptyActionObject
		{
			get
			{
				return actionPool.checkOut();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initDecisionSettings() throws org.maltparser.core.exception.MaltChainedException
		private void initDecisionSettings()
		{
			int start = 0;
			int k = 0;
			char prevDecisionSeparator = ' ';
			TableContainer tmp = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sbTableHandler = new StringBuilder();
			StringBuilder sbTableHandler = new StringBuilder();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sbTable = new StringBuilder();
			StringBuilder sbTable = new StringBuilder();
			int state = 0;
			for (int i = 0; i < decisionSettings.Length; i++)
			{
				switch (decisionSettings[i])
				{
				case '.':
					state = 1;
					break;
				case '+':
					tmp = new TableContainer(tableHandlers[sbTableHandler.ToString()].getSymbolTable(sbTable.ToString()), sbTableHandler.ToString() + "." + sbTable.ToString(), '+');
					actionTables.Add(tmp);
					k++;
					sbTableHandler.Length = 0;
					sbTable.Length = 0;
					state = 0;
					break;
				case '#':
					state = 2;
					break;
				case ';':
					state = 2;
					break;
				case ',':
					state = 2;
					break;
				default:
					if (state == 0)
					{
						sbTableHandler.Append(decisionSettings[i]);
					}
					else if (state == 1)
					{
						sbTable.Append(decisionSettings[i]);
					}
				break;
				}
				if (state == 2 || i == decisionSettings.Length - 1)
				{
					char decisionSeparator = decisionSettings[i];
					if (i == decisionSettings.Length - 1)
					{
						decisionSeparator = prevDecisionSeparator;
					}
					tmp = new TableContainer(tableHandlers[sbTableHandler.ToString()].getSymbolTable(sbTable.ToString()), sbTableHandler.ToString() + "." + sbTable.ToString(), decisionSeparator);
					actionTables.Add(tmp);
					k++;
					if (k - start > 1)
					{
						decisionTables.Add(new CombinedTableContainer(getTableHandler("A"), separator, actionTables.subList(start, k), decisionSeparator));
					}
					else
					{
						decisionTables.Add(tmp);
					}
					sbTableHandler.Length = 0;
					sbTable.Length = 0;
					state = 0;
					start = k;
					prevDecisionSeparator = decisionSeparator;
				}
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			return sb.ToString();
		}
	}

}