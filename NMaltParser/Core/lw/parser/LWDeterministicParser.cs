﻿using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.lw.parser
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureModel = org.maltparser.core.feature.FeatureModel;
	using FeatureModelManager = org.maltparser.core.feature.FeatureModelManager;
	using HashMap = org.maltparser.core.helper.HashMap;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using TableHandler = org.maltparser.core.symbol.TableHandler;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using AlgoritmInterface = org.maltparser.parser.AlgoritmInterface;
	using DependencyParserConfig = org.maltparser.parser.DependencyParserConfig;
	using ParserConfiguration = org.maltparser.parser.ParserConfiguration;
	using ParserRegistry = org.maltparser.parser.ParserRegistry;
	using TransitionSystem = org.maltparser.parser.TransitionSystem;
	using GuideUserHistory = org.maltparser.parser.history.GuideUserHistory;
	using ComplexDecisionAction = org.maltparser.parser.history.action.ComplexDecisionAction;
	using GuideUserAction = org.maltparser.parser.history.action.GuideUserAction;
	using ActionContainer = org.maltparser.parser.history.container.ActionContainer;
	using CombinedTableContainer = org.maltparser.parser.history.container.CombinedTableContainer;
	using TableContainer = org.maltparser.parser.history.container.TableContainer;

	/// <summary>
	/// A lightweight version of org.maltparser.parser.DeterministicParser. This class also implements a lightweight version of 
	/// org.maltparser.parser.history.History and reduces the need of org.maltparser.parser.ParserState. 
	/// 
	/// The class must be used in the same thread.
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class LWDeterministicParser : AlgoritmInterface, GuideUserHistory
	{
		private readonly LWSingleMalt manager;
		private readonly ParserRegistry registry;

		private readonly TransitionSystem transitionSystem;
		private readonly ParserConfiguration config;
		private readonly FeatureModel featureModel;
		private readonly ComplexDecisionAction currentAction;

		private readonly int kBestSize;
		private readonly List<TableContainer> decisionTables;
		private readonly List<TableContainer> actionTables;
		private readonly HashMap<string, TableHandler> tableHandlers;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LWDeterministicParser(LWSingleMalt lwSingleMalt, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler, org.maltparser.core.feature.FeatureModel _featureModel) throws org.maltparser.core.exception.MaltChainedException
		public LWDeterministicParser(LWSingleMalt lwSingleMalt, SymbolTableHandler symbolTableHandler, FeatureModel _featureModel)
		{
			this.manager = lwSingleMalt;
			this.registry = new ParserRegistry();
			this.registry.SymbolTableHandler = symbolTableHandler;
			this.registry.DataFormatInstance = manager.DataFormatInstance;
			this.registry.setAbstractParserFeatureFactory(manager.ParserFactory);
			this.registry.Algorithm = this;
			this.transitionSystem = manager.ParserFactory.makeTransitionSystem();
			this.transitionSystem.initTableHandlers(lwSingleMalt.DecisionSettings, symbolTableHandler);

			this.tableHandlers = transitionSystem.TableHandlers;
			this.kBestSize = lwSingleMalt.getkBestSize();
			this.decisionTables = new List<TableContainer>();
			this.actionTables = new List<TableContainer>();
			initDecisionSettings(lwSingleMalt.DecisionSettings, lwSingleMalt.Classitem_separator);
			this.transitionSystem.initTransitionSystem(this);
			this.config = manager.ParserFactory.makeParserConfiguration();
			this.featureModel = _featureModel;
			this.currentAction = new ComplexDecisionAction(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LWDeterministicParser(LWSingleMalt lwSingleMalt, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public LWDeterministicParser(LWSingleMalt lwSingleMalt, SymbolTableHandler symbolTableHandler)
		{
			this.manager = lwSingleMalt;
			this.registry = new ParserRegistry();
			this.registry.SymbolTableHandler = symbolTableHandler;
			this.registry.DataFormatInstance = manager.DataFormatInstance;
			this.registry.setAbstractParserFeatureFactory(manager.ParserFactory);
			this.registry.Algorithm = this;
			this.transitionSystem = manager.ParserFactory.makeTransitionSystem();
			this.transitionSystem.initTableHandlers(lwSingleMalt.DecisionSettings, symbolTableHandler);

			this.tableHandlers = transitionSystem.TableHandlers;
			this.kBestSize = lwSingleMalt.getkBestSize();
			this.decisionTables = new List<TableContainer>();
			this.actionTables = new List<TableContainer>();
			initDecisionSettings(lwSingleMalt.DecisionSettings, lwSingleMalt.Classitem_separator);
			this.transitionSystem.initTransitionSystem(this);
			this.config = manager.ParserFactory.makeParserConfiguration();
			this.featureModel = manager.FeatureModelManager.getFeatureModel(lwSingleMalt.FeatureModelURL, 0, registry, manager.DataSplitColumn, manager.DataSplitStructure);
			this.currentAction = new ComplexDecisionAction(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.DependencyStructure parse(org.maltparser.core.syntaxgraph.DependencyStructure parseDependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public DependencyStructure parse(DependencyStructure parseDependencyGraph)
		{
			config.clear();
			config.DependencyGraph = parseDependencyGraph;
			config.initialize();

			while (!config.TerminalState)
			{
				GuideUserAction action = transitionSystem.getDeterministicAction(this, config);
				if (action == null)
				{
					action = predict();
				}
				transitionSystem.apply(action, config);
			}
			parseDependencyGraph.linkAllTreesToRoot();
			return parseDependencyGraph;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.parser.history.action.GuideUserAction predict() throws org.maltparser.core.exception.MaltChainedException
		private GuideUserAction predict()
		{
			currentAction.clear();
			try
			{
				manager.DecisionModel.predict(featureModel, currentAction, true);

				while (!transitionSystem.permissible(currentAction, config))
				{
					if (manager.DecisionModel.predictFromKBestList(featureModel, currentAction) == false)
					{
						GuideUserAction defaultAction = transitionSystem.defaultAction(this, config);
						ActionContainer[] actionContainers = this.ActionContainerArray;
						defaultAction.getAction(actionContainers);
						currentAction.addAction(actionContainers);
						break;
					}
				}
			}
			catch (System.NullReferenceException e)
			{
				throw new MaltChainedException("The guide cannot be found. ", e);
			}
			return currentAction;
		}

		public ParserRegistry ParserRegistry
		{
			get
			{
				return registry;
			}
		}

		public ParserConfiguration CurrentParserConfiguration
		{
			get
			{
				return config;
			}
		}

		public DependencyParserConfig Manager
		{
			get
			{
				return manager;
			}
		}

		public string GuideName
		{
			get
			{
				return null;
			}
			set
			{
			}
		}


		// GuideUserHistory interface
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction getEmptyGuideUserAction() throws org.maltparser.core.exception.MaltChainedException
		public GuideUserAction EmptyGuideUserAction
		{
			get
			{
				return new ComplexDecisionAction(this);
			}
		}

		public List<ActionContainer> ActionContainers
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

		public ActionContainer[] ActionContainerArray
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
		public void clear()
		{
		}

		public int NumberOfDecisions
		{
			get
			{
				return decisionTables.Count;
			}
		}

		public int KBestSize
		{
			get
			{
				return kBestSize;
			}
		}

		public int NumberOfActions
		{
			get
			{
				return actionTables.Count;
			}
		}

		public List<TableContainer> DecisionTables
		{
			get
			{
				return decisionTables;
			}
		}

		public List<TableContainer> ActionTables
		{
			get
			{
				return actionTables;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initDecisionSettings(String decisionSettings, String separator) throws org.maltparser.core.exception.MaltChainedException
		private void initDecisionSettings(string decisionSettings, string separator)
		{
			if (decisionSettings.Equals("T.TRANS+A.DEPREL"))
			{
				actionTables.Add(new TableContainer(tableHandlers["T"].getSymbolTable("TRANS"), "T.TRANS", '+'));
				actionTables.Add(new TableContainer(tableHandlers["A"].getSymbolTable("DEPREL"), "A.DEPREL", ' '));
				decisionTables.Add(new CombinedTableContainer(tableHandlers["A"], separator, actionTables, ' '));
			}
			else if (decisionSettings.Equals("T.TRANS,A.DEPREL"))
			{
				TableContainer transTableContainer = new TableContainer(tableHandlers["T"].getSymbolTable("TRANS"), "T.TRANS", ',');
				TableContainer deprelTableContainer = new TableContainer(tableHandlers["A"].getSymbolTable("DEPREL"), "A.DEPREL", ',');
				actionTables.Add(transTableContainer);
				actionTables.Add(deprelTableContainer);
				decisionTables.Add(transTableContainer);
				decisionTables.Add(deprelTableContainer);
			}
			else if (decisionSettings.Equals("T.TRANS#A.DEPREL") || decisionSettings.Equals("T.TRANS;A.DEPREL"))
			{
				TableContainer transTableContainer = new TableContainer(tableHandlers["T"].getSymbolTable("TRANS"), "T.TRANS", '#');
				TableContainer deprelTableContainer = new TableContainer(tableHandlers["A"].getSymbolTable("DEPREL"), "A.DEPREL", '#');
				actionTables.Add(transTableContainer);
				actionTables.Add(deprelTableContainer);
				decisionTables.Add(transTableContainer);
				decisionTables.Add(deprelTableContainer);
			}
			else
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
							decisionTables.Add(new CombinedTableContainer(tableHandlers["A"], separator, actionTables.subList(start, k), decisionSeparator));
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
		}
	}

}