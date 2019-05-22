namespace org.maltparser.parser.algorithm.covington
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureRegistry = org.maltparser.core.feature.FeatureRegistry;
	using Function = org.maltparser.core.feature.function.Function;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class CovingtonFactory : AbstractParserFactory
	{
		public abstract org.maltparser.parser.guide.OracleGuide makeOracleGuide(org.maltparser.parser.history.GuideUserHistory history);
		public abstract TransitionSystem makeTransitionSystem();
		protected internal readonly DependencyParserConfig manager;

		public CovingtonFactory(DependencyParserConfig _manager)
		{
			this.manager = _manager;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.ParserConfiguration makeParserConfiguration() throws org.maltparser.core.exception.MaltChainedException
		public virtual ParserConfiguration makeParserConfiguration()
		{
			bool allowRoot = (bool?)manager.getOptionValue("covington", "allow_root").Value;
			bool allowShift = (bool?)manager.getOptionValue("covington", "allow_shift").Value;
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Parser configuration : Covington with allow_root=" + allowRoot + " and allow_shift=" + allowShift + "\n");
			}
			CovingtonConfig config = new CovingtonConfig(allowRoot, allowShift);
			return config;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.function.Function makeFunction(String subFunctionName, org.maltparser.core.feature.FeatureRegistry registry) throws org.maltparser.core.exception.MaltChainedException
		public virtual Function makeFunction(string subFunctionName, FeatureRegistry registry)
		{
			AlgoritmInterface algorithm = ((ParserRegistry)registry).Algorithm;
			return new CovingtonAddressFunction(subFunctionName, algorithm);
		}
	}

}