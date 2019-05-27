namespace NMaltParser.Parser
{

	public interface AlgoritmInterface
	{
		ParserRegistry ParserRegistry {get;}
		ParserConfiguration CurrentParserConfiguration {get;}
		IDependencyParserConfig Manager {get;}
	}

}