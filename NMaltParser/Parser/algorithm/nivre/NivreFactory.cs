﻿using NMaltParser.Core.Feature;
using NMaltParser.Core.Feature.Function;
using NMaltParser.Parser.Guide;
using NMaltParser.Parser.History;

namespace NMaltParser.Parser.Algorithm.Nivre
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class NivreFactory : AbstractParserFactory
	{
		public abstract OracleGuide makeOracleGuide(GuideUserHistory history);
		public abstract TransitionSystem makeTransitionSystem();
		protected internal readonly IDependencyParserConfig manager;

		public NivreFactory(IDependencyParserConfig _manager)
		{
			manager = _manager;
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