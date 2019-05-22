using System.Collections.Generic;

namespace org.maltparser.parser.history
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using GuideUserAction = org.maltparser.parser.history.action.GuideUserAction;
	using ActionContainer = org.maltparser.parser.history.container.ActionContainer;
	using TableContainer = org.maltparser.parser.history.container.TableContainer;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.1
	/// 
	/// </summary>
	public interface GuideUserHistory
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction getEmptyGuideUserAction() throws org.maltparser.core.exception.MaltChainedException;
		GuideUserAction EmptyGuideUserAction {get;}
		List<ActionContainer> ActionContainers {get;}
		ActionContainer[] ActionContainerArray {get;}
		int NumberOfDecisions {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException;
		void clear();
	//	public void setKBestListClass(Class<?> kBestListClass) throws MaltChainedException;
	//	public Class<?> getKBestListClass();
		int KBestSize {get;}
	//	public void setKBestSize(int kBestSize);
	//	public void setSeparator(String separator) throws MaltChainedException;
		List<TableContainer> DecisionTables {get;}
		List<TableContainer> ActionTables {get;}
	}

}