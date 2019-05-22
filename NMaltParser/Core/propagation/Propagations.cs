using System.Collections.Generic;

namespace org.maltparser.core.propagation
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	using PropagationSpec = org.maltparser.core.propagation.spec.PropagationSpec;
	using PropagationSpecs = org.maltparser.core.propagation.spec.PropagationSpecs;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using Edge = org.maltparser.core.syntaxgraph.edge.Edge;

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