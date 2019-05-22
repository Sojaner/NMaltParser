namespace org.maltparser.parser.guide
{
    using  core.syntaxgraph;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.1
	/// 
	/// </summary>
	public interface Guide
	{
	//	public enum GuideMode { BATCH, ONLINE, CLASSIFY}

	//	public void addInstance(GuideDecision decision) throws MaltChainedException;
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException;
		void finalizeSentence(DependencyStructure dependencyGraph);
	//	public void noMoreInstances() throws MaltChainedException;
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException;
		void terminate();

	//	public void predict(GuideDecision decision) throws MaltChainedException;
	//	public boolean predictFromKBestList(GuideDecision decision) throws MaltChainedException;

		DependencyParserConfig Configuration {get;}
	//	public GuideMode getGuideMode();
	//	public GuideHistory getHistory();
	//	public FeatureModelManager getFeatureModelManager();
		string GuideName {get;set;}
	}

}