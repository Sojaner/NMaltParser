namespace org.maltparser.parser.algorithm.stack
{
    using  core.feature;
	using  core.feature.function;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class StackFactory : AbstractParserFactory
	{
		public abstract guide.OracleGuide makeOracleGuide(history.GuideUserHistory history);
		public abstract TransitionSystem makeTransitionSystem();
		protected internal readonly DependencyParserConfig manager;

		public StackFactory(DependencyParserConfig _manager)
		{
			manager = _manager;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.ParserConfiguration makeParserConfiguration() throws org.maltparser.core.exception.MaltChainedException
		public virtual ParserConfiguration makeParserConfiguration()
		{
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Parser configuration : Stack\n");
			}
			return new StackConfig();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.function.Function makeFunction(String subFunctionName, org.maltparser.core.feature.FeatureRegistry registry) throws org.maltparser.core.exception.MaltChainedException
		public virtual Function makeFunction(string subFunctionName, FeatureRegistry registry)
		{
			AlgoritmInterface algorithm = ((ParserRegistry)registry).Algorithm;
			return new StackAddressFunction(subFunctionName, algorithm);
		}
	}

}