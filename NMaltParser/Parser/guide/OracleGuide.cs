﻿namespace org.maltparser.parser.guide
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using GuideUserAction = org.maltparser.parser.history.action.GuideUserAction;

	public interface OracleGuide : Guide
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException;
		GuideUserAction predict(DependencyStructure gold, ParserConfiguration config);
	}

}