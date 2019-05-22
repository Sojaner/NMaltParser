﻿namespace org.maltparser.parser.algorithm.twoplanar
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureRegistry = org.maltparser.core.feature.FeatureRegistry;
	using Function = org.maltparser.core.feature.function.Function;
	/// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public abstract class TwoPlanarFactory : AbstractParserFactory
	{
		public abstract org.maltparser.parser.guide.OracleGuide makeOracleGuide(org.maltparser.parser.history.GuideUserHistory history);
		public abstract TransitionSystem makeTransitionSystem();
		protected internal readonly DependencyParserConfig manager;

		public TwoPlanarFactory(DependencyParserConfig _manager)
		{
			this.manager = _manager;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.ParserConfiguration makeParserConfiguration() throws org.maltparser.core.exception.MaltChainedException
		public virtual ParserConfiguration makeParserConfiguration()
		{
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Parser configuration : Two-Planar with no_covered_roots = " + manager.getOptionValue("planar", "no_covered_roots").ToString().ToUpper() + ", " + "acyclicity = " + manager.getOptionValue("planar", "acyclicity").ToString().ToUpper() + ", planar root handling = " + manager.getOptionValue("2planar", "planar_root_handling").ToString().ToUpper() + ", reduce on switch = " + manager.getOptionValue("2planar", "reduceonswitch").ToString().ToUpper() + "\n");
			}
			return new TwoPlanarConfig(manager.getOptionValue("planar", "no_covered_roots").ToString(), manager.getOptionValue("planar", "acyclicity").ToString(), manager.getOptionValue("2planar", "reduceonswitch").ToString(), manager.getOptionValue("multiplanar", "planar_root_handling").ToString());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.function.Function makeFunction(String subFunctionName, org.maltparser.core.feature.FeatureRegistry registry) throws org.maltparser.core.exception.MaltChainedException
		public virtual Function makeFunction(string subFunctionName, FeatureRegistry registry)
		{
			AlgoritmInterface algorithm = ((ParserRegistry)registry).Algorithm;
			return new TwoPlanarAddressFunction(subFunctionName, algorithm);
		}
	}

}