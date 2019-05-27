using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Parser.History.Action;

namespace NMaltParser.Parser.Guide
{
    public interface OracleGuide : Guide
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.history.action.GuideUserAction predict(org.maltparser.core.syntaxgraph.DependencyStructure gold, org.maltparser.parser.ParserConfiguration config) throws org.maltparser.core.exception.MaltChainedException;
		GuideUserAction predict(IDependencyStructure gold, ParserConfiguration config);
	}

}