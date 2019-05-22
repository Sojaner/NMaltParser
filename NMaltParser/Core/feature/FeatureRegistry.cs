using System;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Symbol;

namespace NMaltParser.Core.Feature
{
    public interface FeatureRegistry
	{
		object get(Type key);
		void put(Type key, object value);
		AbstractFeatureFactory getFactory(Type clazz);
		SymbolTableHandler SymbolTableHandler {get;}
		DataFormatInstance DataFormatInstance {get;}
	}

}