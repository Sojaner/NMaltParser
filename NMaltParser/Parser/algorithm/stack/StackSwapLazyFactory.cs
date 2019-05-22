namespace org.maltparser.parser.algorithm.stack
{
	using  org.maltparser.core.exception;
	using  org.maltparser.parser.guide;
	using  org.maltparser.parser.history;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class StackSwapLazyFactory : StackFactory
	{
		public StackSwapLazyFactory(DependencyParserConfig _manager) : base(_manager)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.TransitionSystem makeTransitionSystem() throws org.maltparser.core.exception.MaltChainedException
		public override TransitionSystem makeTransitionSystem()
		{
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Transition system    : Non-Projective\n");
			}
			return new NonProjective(manager.PropagationManager);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.guide.OracleGuide makeOracleGuide(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public override OracleGuide makeOracleGuide(GuideUserHistory history)
		{
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Oracle               : Swap-Lazy\n");
			}
			return new SwapLazyOracle(manager, history);
		}
	}

}