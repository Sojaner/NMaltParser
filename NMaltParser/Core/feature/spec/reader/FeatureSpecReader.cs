namespace org.maltparser.core.feature.spec.reader
{

	using  org.maltparser.core.exception;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface FeatureSpecReader
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.net.URL specModelURL, org.maltparser.core.feature.spec.SpecificationModels featureSpecModels) throws org.maltparser.core.exception.MaltChainedException;
		void load(URL specModelURL, SpecificationModels featureSpecModels);
	}

}