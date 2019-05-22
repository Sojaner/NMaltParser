namespace org.maltparser.core.feature.function
{
	using  org.maltparser.core.exception;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface Modifiable : FeatureFunction
	{
		/// <summary>
		/// Override the feature value of the feature function.
		/// </summary>
		/// <param name="code"> an integer representation of the symbol </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setFeatureValue(int code) throws org.maltparser.core.exception.MaltChainedException;
		int FeatureValue {set;}
		/// <summary>
		/// Override the feature value of the feature function.
		/// </summary>
		/// <param name="symbol"> an string representation of the symbol </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setFeatureValue(String symbol) throws org.maltparser.core.exception.MaltChainedException;
		string FeatureValue {set;}
	}

}