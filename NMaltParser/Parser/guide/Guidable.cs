namespace org.maltparser.parser.guide
{
    using  history.action;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.1
	/// 
	/// </summary>
	public interface Guidable
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setInstance(org.maltparser.parser.history.action.GuideUserAction action) throws org.maltparser.core.exception.MaltChainedException;
		GuideUserAction Instance {set;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void predict(org.maltparser.parser.history.action.GuideUserAction action) throws org.maltparser.core.exception.MaltChainedException;
		void predict(GuideUserAction action);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predictFromKBestList(org.maltparser.parser.history.action.GuideUserAction action) throws org.maltparser.core.exception.MaltChainedException;
		bool predictFromKBestList(GuideUserAction action);
	}

}