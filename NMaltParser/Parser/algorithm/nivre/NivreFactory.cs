namespace org.maltparser.parser.algorithm.nivre
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureRegistry = org.maltparser.core.feature.FeatureRegistry;
	using Function = org.maltparser.core.feature.function.Function;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class NivreFactory : AbstractParserFactory
	{
		public abstract org.maltparser.parser.guide.OracleGuide makeOracleGuide(org.maltparser.parser.history.GuideUserHistory history);
		public abstract TransitionSystem makeTransitionSystem();
		protected internal readonly DependencyParserConfig manager;

		public NivreFactory(DependencyParserConfig _manager)
		{
			this.manager = _manager;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.ParserConfiguration makeParserConfiguration() throws org.maltparser.core.exception.MaltChainedException
		public virtual ParserConfiguration makeParserConfiguration()
		{
			bool allowRoot = (bool?)manager.getOptionValue("nivre", "allow_root").Value;
			bool allowReduce = (bool?)manager.getOptionValue("nivre", "allow_reduce").Value;
			bool enforceTree = (bool?)manager.getOptionValue("nivre", "enforce_tree") && manager.getOptionValueString("config", "flowchart").Equals("parse");
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Parser configuration : Nivre with allow_root=" + allowRoot + ", allow_reduce=" + allowReduce + " and enforce_tree=" + enforceTree + "\n");
			}
			return new NivreConfig(allowRoot, allowReduce, enforceTree);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.function.Function makeFunction(String subFunctionName, org.maltparser.core.feature.FeatureRegistry registry) throws org.maltparser.core.exception.MaltChainedException
		public virtual Function makeFunction(string subFunctionName, FeatureRegistry registry)
		{
			AlgoritmInterface algorithm = ((ParserRegistry)registry).Algorithm;
			return new NivreAddressFunction(subFunctionName, algorithm);
		}
	}


}