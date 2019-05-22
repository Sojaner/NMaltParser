using NMaltParser.Core.Exception;
using NMaltParser.Core.Feature;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Parser.Guide;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class DeterministicParser : Parser
	{
		private readonly FeatureModel featureModel;
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DeterministicParser(DependencyParserConfig manager, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public DeterministicParser(DependencyParserConfig manager, SymbolTableHandler symbolTableHandler) : base(manager,symbolTableHandler)
		{
			registry.Algorithm = this;
			Guide = new SingleGuide(this, ClassifierGuide_GuideMode.CLASSIFY);
			string featureModelFileName = manager.getOptionValue("guide", "features").ToString().Trim();
			if (manager.LoggerInfoEnabled)
			{
				manager.logDebugMessage("  Feature model        : " + featureModelFileName + "\n");
				manager.logDebugMessage("  Classifier           : " + manager.getOptionValueString("guide", "learner") + "\n");
			}
			string dataSplitColumn = manager.getOptionValue("guide", "data_split_column").ToString().Trim();
			string dataSplitStructure = manager.getOptionValue("guide", "data_split_structure").ToString().Trim();
			featureModel = manager.FeatureModelManager.getFeatureModel(SingleGuide.findURL(featureModelFileName, manager), 0, ParserRegistry, dataSplitColumn, dataSplitStructure);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.DependencyStructure parse(org.maltparser.core.syntaxgraph.DependencyStructure parseDependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public override DependencyStructure parse(DependencyStructure parseDependencyGraph)
		{
			parserState.clear();
			parserState.initialize(parseDependencyGraph);
			currentParserConfiguration = parserState.Configuration;
			TransitionSystem ts = parserState.TransitionSystem;
			while (!parserState.TerminalState)
			{
				GuideUserAction action = ts.getDeterministicAction(parserState.History, currentParserConfiguration);
				if (action == null)
				{
					action = predict();
				}
				parserState.apply(action);
			}
	//		copyEdges(currentParserConfiguration.getDependencyGraph(), parseDependencyGraph);
	//		copyDynamicInput(currentParserConfiguration.getDependencyGraph(), parseDependencyGraph);
			parseDependencyGraph.linkAllTreesToRoot();
			return parseDependencyGraph;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.parser.history.action.GuideUserAction predict() throws org.maltparser.core.exception.MaltChainedException
		private GuideUserAction predict()
		{
			GuideUserAction currentAction = parserState.History.EmptyGuideUserAction;
			try
			{
				classifierGuide.predict(featureModel,(GuideDecision)currentAction);
				while (!parserState.permissible(currentAction))
				{
					if (classifierGuide.predictFromKBestList(featureModel,(GuideDecision)currentAction) == false)
					{
						currentAction = ParserState.TransitionSystem.defaultAction(parserState.History, currentParserConfiguration);
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
		}
	}

}