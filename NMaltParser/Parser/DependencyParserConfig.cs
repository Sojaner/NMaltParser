namespace org.maltparser.parser
{
	using  core.config;
	using  core.io.dataformat;
    using  core.feature;
	using  core.propagation;
	using  core.syntaxgraph;

	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public interface DependencyParserConfig : Configuration
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parse(org.maltparser.core.syntaxgraph.DependencyStructure graph) throws org.maltparser.core.exception.MaltChainedException;
		void parse(DependencyStructure graph);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void oracleParse(org.maltparser.core.syntaxgraph.DependencyStructure goldGraph, org.maltparser.core.syntaxgraph.DependencyStructure oracleGraph) throws org.maltparser.core.exception.MaltChainedException;
		void oracleParse(DependencyStructure goldGraph, DependencyStructure oracleGraph);
		DataFormatInstance DataFormatInstance {get;}
		FeatureModelManager FeatureModelManager {get;}
		PropagationManager PropagationManager {get;}
		AbstractParserFactory ParserFactory {get;}
	}

}