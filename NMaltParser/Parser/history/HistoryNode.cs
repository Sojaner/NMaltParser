﻿namespace org.maltparser.parser.history
{
	using  org.maltparser.core.exception;
	using  org.maltparser.parser.history.action;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public interface HistoryNode
	{
		HistoryNode PreviousNode {get;set;}
		GuideUserAction Action {get;set;}
		int Position {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException;
		void clear();
	}

}