namespace org.maltparser.parser.history
{
    using  action;

	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public abstract class HistoryStructure
	{
		public HistoryStructure()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract HistoryNode getNewHistoryNode(HistoryNode previousNode, org.maltparser.parser.history.action.GuideUserAction action) throws org.maltparser.core.exception.MaltChainedException;
		public abstract HistoryNode getNewHistoryNode(HistoryNode previousNode, GuideUserAction action);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void clear() throws org.maltparser.core.exception.MaltChainedException;
		public abstract void clear();
	}

}