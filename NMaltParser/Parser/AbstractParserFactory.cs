namespace org.maltparser.parser
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using AbstractFeatureFactory = org.maltparser.core.feature.AbstractFeatureFactory;
	using OracleGuide = org.maltparser.parser.guide.OracleGuide;
	using GuideUserHistory = org.maltparser.parser.history.GuideUserHistory;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public interface AbstractParserFactory : AbstractFeatureFactory
	{
		/// <summary>
		/// Creates a parser configuration
		/// </summary>
		/// <returns> a parser configuration </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParserConfiguration makeParserConfiguration() throws org.maltparser.core.exception.MaltChainedException;
		ParserConfiguration makeParserConfiguration();
		/// <summary>
		/// Creates a transition system
		/// </summary>
		/// <returns> a transition system </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TransitionSystem makeTransitionSystem() throws org.maltparser.core.exception.MaltChainedException;
		TransitionSystem makeTransitionSystem();
		/// <summary>
		/// Creates an oracle guide
		/// </summary>
		/// <param name="history"> a reference to the history </param>
		/// <returns>  an oracle guide </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.guide.OracleGuide makeOracleGuide(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException;
		OracleGuide makeOracleGuide(GuideUserHistory history);
	}

}