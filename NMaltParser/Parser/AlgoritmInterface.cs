namespace org.maltparser.parser
{

	public interface AlgoritmInterface
	{
		ParserRegistry ParserRegistry {get;}
		ParserConfiguration CurrentParserConfiguration {get;}
		DependencyParserConfig Manager {get;}
	}

}