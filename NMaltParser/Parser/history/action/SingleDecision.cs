namespace org.maltparser.parser.history.action
{
	using  org.maltparser.core.exception;
	using  org.maltparser.parser.history.container;
	using  org.maltparser.parser.history.container.TableContainer;
	using  org.maltparser.parser.history.kbest;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.1
	/// 
	/// </summary>
	public interface SingleDecision : GuideDecision
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addDecision(int code) throws org.maltparser.core.exception.MaltChainedException;
		void addDecision(int code);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addDecision(String symbol) throws org.maltparser.core.exception.MaltChainedException;
		void addDecision(string symbol);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDecisionCode() throws org.maltparser.core.exception.MaltChainedException;
		int DecisionCode {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getDecisionSymbol() throws org.maltparser.core.exception.MaltChainedException;
		string DecisionSymbol {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getDecisionCode(String symbol) throws org.maltparser.core.exception.MaltChainedException;
		int getDecisionCode(string symbol);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.kbest.KBestList getKBestList() throws org.maltparser.core.exception.MaltChainedException;
		KBestList KBestList {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean updateFromKBestList() throws org.maltparser.core.exception.MaltChainedException;
		bool updateFromKBestList();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean continueWithNextDecision() throws org.maltparser.core.exception.MaltChainedException;
		bool continueWithNextDecision();
		TableContainer TableContainer {get;}
		TableContainer.RelationToNextDecision RelationToNextDecision {get;}
	}

}