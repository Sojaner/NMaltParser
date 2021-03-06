﻿using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Propagation.Spec;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Edge;

namespace NMaltParser.Core.Propagation
{
    public class PropagationManager
	{
		private readonly PropagationSpecs propagationSpecs;
		private Propagations propagations;

		public PropagationManager()
		{
			propagationSpecs = new PropagationSpecs();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadSpecification(java.net.URL propagationSpecURL) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadSpecification(URL propagationSpecURL)
		{
			PropagationSpecsReader reader = new PropagationSpecsReader();
			reader.load(propagationSpecURL, propagationSpecs);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void createPropagations(org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler tableHandler) throws org.maltparser.core.exception.MaltChainedException
		public virtual void createPropagations(DataFormatInstance dataFormatInstance, SymbolTableHandler tableHandler)
		{
			propagations = new Propagations(propagationSpecs, dataFormatInstance, tableHandler);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void propagate(org.maltparser.core.syntaxgraph.edge.Edge e) throws org.maltparser.core.exception.MaltChainedException
		public virtual void propagate(Edge e)
		{
			if (propagations != null && e != null)
			{
				propagations.propagate(e);
			}
		}

		public virtual PropagationSpecs PropagationSpecs
		{
			get
			{
				return propagationSpecs;
			}
		}

		public virtual Propagations Propagations
		{
			get
			{
				return propagations;
			}
		}
	}

}