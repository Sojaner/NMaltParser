namespace NMaltParser.Parser.History.Action
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.1
	/// 
	/// </summary>
	public interface MultipleDecision : GuideDecision
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SingleDecision getSingleDecision(int decisionIndex) throws org.maltparser.core.exception.MaltChainedException;
		SingleDecision getSingleDecision(int decisionIndex);
	}

}