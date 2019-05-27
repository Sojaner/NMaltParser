using NMaltParser.Core.Config;
using NMaltParser.Core.Feature;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Propagation;
using NMaltParser.Core.SyntaxGraph;

namespace NMaltParser.Parser
{
    /// <inheritdoc />
    /// <summary>
    /// @author Johan Hall
    /// </summary>
	public interface IDependencyParserConfig : Configuration
	{
		void Parse(IDependencyStructure graph);

		void OracleParse(IDependencyStructure goldGraph, IDependencyStructure oracleGraph);

		DataFormatInstance DataFormatInstance {get;}

		FeatureModelManager FeatureModelManager {get;}

		PropagationManager PropagationManager {get;}

		AbstractParserFactory ParserFactory {get;}
	}

}