using System.Collections.Generic;

namespace org.maltparser.core.syntaxgraph.node
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using HeadRules = org.maltparser.core.syntaxgraph.headrules.HeadRules;

	public interface NonTerminalNode : PhraseStructureNode
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TokenNode identifyHead(org.maltparser.core.syntaxgraph.headrules.HeadRules headRules) throws org.maltparser.core.exception.MaltChainedException;
		TokenNode identifyHead(HeadRules headRules);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TokenNode getLexicalHead(org.maltparser.core.syntaxgraph.headrules.HeadRules headRules) throws org.maltparser.core.exception.MaltChainedException;
		TokenNode getLexicalHead(HeadRules headRules);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TokenNode getLexicalHead() throws org.maltparser.core.exception.MaltChainedException;
		TokenNode LexicalHead {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PhraseStructureNode getHeadChild(org.maltparser.core.syntaxgraph.headrules.HeadRules headRules) throws org.maltparser.core.exception.MaltChainedException;
		PhraseStructureNode getHeadChild(HeadRules headRules);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PhraseStructureNode getHeadChild() throws org.maltparser.core.exception.MaltChainedException;
		PhraseStructureNode HeadChild {get;}
		SortedSet<PhraseStructureNode> Children {get;}
		PhraseStructureNode getChild(int index);
		PhraseStructureNode LeftChild {get;}
		PhraseStructureNode RightChild {get;}
		int nChildren();
		bool hasNonTerminalChildren();
		bool hasTerminalChildren();
		int Height {get;}
		bool Continuous {get;}
		bool ContinuousExcludeTerminalsAttachToRoot {get;}
		//public void reArrangeChildrenAccordingToLeftAndRightProperDesendant() throws MaltChainedException;
	}

}