using NMaltParser.Core.Feature;
using NMaltParser.Core.Feature.Function;
using NMaltParser.Parser.Guide;
using NMaltParser.Parser.History;

namespace NMaltParser.Parser.Algorithm.Covington
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class CovingtonFactory : AbstractParserFactory
	{
		public abstract OracleGuide makeOracleGuide(GuideUserHistory history);
		public abstract TransitionSystem makeTransitionSystem();
		protected internal readonly IDependencyParserConfig manager;

		public CovingtonFactory(IDependencyParserConfig _manager)
		{
			manager = _manager;
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