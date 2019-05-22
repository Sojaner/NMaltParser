using System.Collections.Generic;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Propagation.Spec;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Edge;

namespace NMaltParser.Core.Propagation
{
    public class Propagations
	{
		private readonly List<Propagation> propagations;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Propagations(org.maltparser.core.propagation.spec.PropagationSpecs specs,org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public Propagations(PropagationSpecs specs, DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler)
		{
			propagations = new List<Propagation>(specs.Count);
			foreach (PropagationSpec spec in specs)
			{
				propagations.Add(new Propagation(spec, dataFormatInstance, tableHandler));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void propagate(org.maltparser.core.syntaxgraph.edge.Edge e) throws org.maltparser.core.exception.MaltChainedException
		public virtual void propagate(Edge e)
		{
			foreach (Propagation propagation in propagations)
			{
				propagation.propagate(e);
			}
		}

		public virtual List<Propagation> getPropagations()
		{
			return propagations;
		}

		public override string ToString()
		{
			return "Propagations [propagations=" + propagations + "]";
		}
	}

}