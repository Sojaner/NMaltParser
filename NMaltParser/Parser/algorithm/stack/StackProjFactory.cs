namespace org.maltparser.parser.algorithm.stack
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using OracleGuide = org.maltparser.parser.guide.OracleGuide;
	using GuideUserHistory = org.maltparser.parser.history.GuideUserHistory;
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class StackProjFactory : StackFactory
	{
		public StackProjFactory(DependencyParserConfig _manager) : base(_manager)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.TransitionSystem makeTransitionSystem() throws org.maltparser.core.exception.MaltChainedException
		public override TransitionSystem makeTransitionSystem()
		{
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Transition system    : Projective\n");
			}
			return new Projective(manager.PropagationManager);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.guide.OracleGuide makeOracleGuide(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public override OracleGuide makeOracleGuide(GuideUserHistory history)
		{
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Oracle               : Projective\n");
			}
			return new ProjectiveOracle(manager, history);
		}
	}

}