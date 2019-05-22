using System.Collections.Generic;

namespace org.maltparser.parser.history.action
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using ActionContainer = org.maltparser.parser.history.container.ActionContainer;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public interface GuideUserAction
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAction(java.util.ArrayList<org.maltparser.parser.history.container.ActionContainer> actionContainers) throws org.maltparser.core.exception.MaltChainedException;
		void addAction(List<ActionContainer> actionContainers);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAction(org.maltparser.parser.history.container.ActionContainer[] actionContainers) throws org.maltparser.core.exception.MaltChainedException;
		void addAction(ActionContainer[] actionContainers);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getAction(java.util.ArrayList<org.maltparser.parser.history.container.ActionContainer> actionContainers) throws org.maltparser.core.exception.MaltChainedException;
		void getAction(List<ActionContainer> actionContainers);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void getAction(org.maltparser.parser.history.container.ActionContainer[] actionContainers) throws org.maltparser.core.exception.MaltChainedException;
		void getAction(ActionContainer[] actionContainers);
		int numberOfActions();
	}

}