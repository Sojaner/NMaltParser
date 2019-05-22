using NMaltParser.Core.Symbol;

namespace NMaltParser.Parser.History.Container
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public interface DecisionPropertyTable
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean continueWithNextDecision(int code) throws org.maltparser.core.exception.MaltChainedException;
		bool continueWithNextDecision(int code);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean continueWithNextDecision(String symbol) throws org.maltparser.core.exception.MaltChainedException;
		bool continueWithNextDecision(string symbol);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.Table getTableForNextDecision(int code) throws org.maltparser.core.exception.MaltChainedException;
		Table getTableForNextDecision(int code);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.symbol.Table getTableForNextDecision(String symbol) throws org.maltparser.core.exception.MaltChainedException;
		Table getTableForNextDecision(string symbol);
	}

}