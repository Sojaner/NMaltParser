﻿namespace org.maltparser.parser.algorithm.twoplanar
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	using OracleGuide = org.maltparser.parser.guide.OracleGuide;
	using GuideUserHistory = org.maltparser.parser.history.GuideUserHistory;
	/// <summary>
	/// @author Carlos Gomez Rodriguez
	/// 
	/// </summary>
	public class TwoPlanarArcEagerFactory : TwoPlanarFactory
	{
		public TwoPlanarArcEagerFactory(DependencyParserConfig _manager) : base(_manager)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.TransitionSystem makeTransitionSystem() throws org.maltparser.core.exception.MaltChainedException
		public override TransitionSystem makeTransitionSystem()
		{
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Transition system    : 2-Planar Arc-Eager\n");
			}
			return new TwoPlanar(manager.PropagationManager);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.guide.OracleGuide makeOracleGuide(org.maltparser.parser.history.GuideUserHistory history) throws org.maltparser.core.exception.MaltChainedException
		public override OracleGuide makeOracleGuide(GuideUserHistory history)
		{
			if (manager.LoggerInfoEnabled)
			{
				manager.logInfoMessage("  Oracle               : 2-Planar Arc-Eager\n");
			}
			return new TwoPlanarArcEagerOracle(manager, history);
		}
	}

}