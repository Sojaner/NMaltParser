using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Parser.History;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class ParserState
	{
		private readonly AbstractParserFactory factory;
		private readonly GuideUserHistory history;
		private readonly TransitionSystem transitionSystem;
		private readonly HistoryStructure historyStructure;
		private readonly ParserConfiguration config;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParserState(DependencyParserConfig manager, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler, AbstractParserFactory factory) throws org.maltparser.core.exception.MaltChainedException
		public ParserState(IDependencyParserConfig manager, SymbolTableHandler symbolTableHandler, AbstractParserFactory factory)
		{
			this.factory = factory;
			historyStructure = new HistoryList();
			transitionSystem = factory.makeTransitionSystem();
			string decisionSettings = manager.getOptionValue("guide", "decision_settings").ToString().Trim();
			TransitionSystem.initTableHandlers(decisionSettings, symbolTableHandler);
			int kBestSize = ((int?)manager.getOptionValue("guide", "kbest")).Value;
			string classitem_separator = manager.getOptionValue("guide", "classitem_separator").ToString();
			history = new History.History(decisionSettings, classitem_separator, TransitionSystem.TableHandlers, kBestSize);
			TransitionSystem.initTransitionSystem(history);
			config = factory.makeParserConfiguration();
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public virtual void clear()
		{
			history.clear();
			historyStructure.clear();
		}

		public virtual GuideUserHistory History
		{
			get
			{
				return history;
			}
		}

		public virtual TransitionSystem TransitionSystem
		{
			get
			{
				return transitionSystem;
			}
		}

		public virtual HistoryStructure HistoryStructure
		{
			get
			{
				return historyStructure;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(org.maltparser.core.syntaxgraph.DependencyStructure dependencyStructure) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initialize(IDependencyStructure dependencyStructure)
		{
			config.clear();
			config.DependencyGraph = dependencyStructure;
			config.initialize();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isTerminalState() throws org.maltparser.core.exception.MaltChainedException
		public virtual bool TerminalState
		{
			get
			{
				return config.TerminalState;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean permissible(org.maltparser.parser.history.action.GuideUserAction currentAction) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool permissible(GuideUserAction currentAction)
		{
			return transitionSystem.permissible(currentAction, config);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void apply(org.maltparser.parser.history.action.GuideUserAction currentAction) throws org.maltparser.core.exception.MaltChainedException
		public virtual void apply(GuideUserAction currentAction)
		{
			transitionSystem.apply(currentAction, config);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nConfigurations() throws org.maltparser.core.exception.MaltChainedException
		public virtual int nConfigurations()
		{
			return 1;
		}

		public virtual ParserConfiguration Configuration
		{
			get
			{
				return config;
			}
		}

		public virtual AbstractParserFactory Factory
		{
			get
			{
				return factory;
			}
		}
	}

}