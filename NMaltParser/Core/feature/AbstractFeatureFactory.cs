namespace org.maltparser.core.feature
{
	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature.function;

	public interface AbstractFeatureFactory
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.function.Function makeFunction(String subFunctionName, FeatureRegistry registry) throws org.maltparser.core.exception.MaltChainedException;
		Function makeFunction(string subFunctionName, FeatureRegistry registry);
	}

}