namespace org.maltparser.parser.guide
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature;
	using  org.maltparser.core.syntaxgraph;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public interface Model
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException;
		void finalizeSentence(DependencyStructure dependencyGraph);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances(org.maltparser.core.feature.FeatureModel featureModel) throws org.maltparser.core.exception.MaltChainedException;
		void noMoreInstances(FeatureModel featureModel);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException;
		void terminate();

		ClassifierGuide Guide {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getModelName() throws org.maltparser.core.exception.MaltChainedException;
		string ModelName {get;}
	}

}