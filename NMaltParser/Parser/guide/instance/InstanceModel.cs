namespace org.maltparser.parser.guide.instance
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureVector = org.maltparser.core.feature.FeatureVector;
	using SingleDecision = org.maltparser.parser.history.action.SingleDecision;

	public interface InstanceModel : Model
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException;
		void addInstance(FeatureVector featureVector, SingleDecision decision);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException;
		bool predict(FeatureVector featureVector, SingleDecision decision);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector predictExtract(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException;
		FeatureVector predictExtract(FeatureVector featureVector, SingleDecision decision);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector extract(org.maltparser.core.feature.FeatureVector featureVector) throws org.maltparser.core.exception.MaltChainedException;
		FeatureVector extract(FeatureVector featureVector);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void train() throws org.maltparser.core.exception.MaltChainedException;
		void train();
		void increaseFrequency();
		void decreaseFrequency();
	}

}