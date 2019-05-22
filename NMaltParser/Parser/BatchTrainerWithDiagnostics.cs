using NMaltParser.Core.Exception;
using NMaltParser.Core.Feature;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Parser.Guide;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser
{
    public class BatchTrainerWithDiagnostics : Trainer
	{
		private readonly Diagnostics diagnostics;
		private readonly OracleGuide oracleGuide;
		private int parseCount;
		private readonly FeatureModel featureModel;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BatchTrainerWithDiagnostics(DependencyParserConfig manager, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler) throws org.maltparser.core.exception.MaltChainedException
		public BatchTrainerWithDiagnostics(DependencyParserConfig manager, SymbolTableHandler symbolTableHandler) : base(manager,symbolTableHandler)
		{
			diagnostics = new Diagnostics(manager.getOptionValue("singlemalt", "diafile").ToString());
			registry.Algorithm = this;
			Guide = new SingleGuide(this, ClassifierGuide_GuideMode.BATCH);
			string featureModelFileName = manager.getOptionValue("guide", "features").ToString().Trim();
			if (manager.LoggerInfoEnabled)
			{
				manager.logDebugMessage("  Feature model        : " + featureModelFileName + "\n");
				manager.logDebugMessage("  Learner              : " + manager.getOptionValueString("guide", "learner").ToString() + "\n");
			}
			string dataSplitColumn = manager.getOptionValue("guide", "data_split_column").ToString().Trim();
			string dataSplitStructure = manager.getOptionValue("guide", "data_split_structure").ToString().Trim();
			featureModel = manager.FeatureModelManager.getFeatureModel(SingleGuide.findURL(featureModelFileName, manager), 0, ParserRegistry, dataSplitColumn, dataSplitStructure);

			manager.writeInfoToConfigFile("\nFEATURE MODEL\n");
			manager.writeInfoToConfigFile(featureModel.ToString());
			oracleGuide = parserState.Factory.makeOracleGuide(parserState.History);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.DependencyStructure parse(org.maltparser.core.syntaxgraph.DependencyStructure goldDependencyGraph, org.maltparser.core.syntaxgraph.DependencyStructure parseDependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public override DependencyStructure parse(DependencyStructure goldDependencyGraph, DependencyStructure parseDependencyGraph)
		{
			parserState.clear();
			parserState.initialize(parseDependencyGraph);
			currentParserConfiguration = parserState.Configuration;
			parseCount++;

			diagnostics.writeToDiaFile(parseCount + "");

			TransitionSystem transitionSystem = parserState.TransitionSystem;
			while (!parserState.TerminalState)
			{
				GuideUserAction action = transitionSystem.getDeterministicAction(parserState.History, currentParserConfiguration);
				if (action == null)
				{
					action = oracleGuide.predict(goldDependencyGraph, currentParserConfiguration);
					try
					{
						classifierGuide.addInstance(featureModel,(GuideDecision)action);
					}
					catch (System.NullReferenceException e)
					{
						throw new MaltChainedException("The guide cannot be found. ", e);
					}
				}
				else
				{
					diagnostics.writeToDiaFile(" *");
				}

				diagnostics.writeToDiaFile(" " + transitionSystem.getActionString(action));

				parserState.apply(action);
			}
			copyEdges(currentParserConfiguration.DependencyGraph, parseDependencyGraph);
			parseDependencyGraph.linkAllTreesToRoot();
			oracleGuide.finalizeSentence(parseDependencyGraph);

			diagnostics.writeToDiaFile("\n");

			return parseDependencyGraph;
		}

		public override OracleGuide OracleGuide
		{
			get
			{
				return oracleGuide;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void train() throws org.maltparser.core.exception.MaltChainedException
		public override void train()
		{
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
			diagnostics.closeDiaWriter();
		}
	}

}