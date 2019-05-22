namespace org.maltparser.core.feature
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using Function = org.maltparser.core.feature.function.Function;

	public interface AbstractFeatureFactory
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.function.Function makeFunction(String subFunctionName, FeatureRegistry registry) throws org.maltparser.core.exception.MaltChainedException;
		Function makeFunction(string subFunctionName, FeatureRegistry registry);
	}

}