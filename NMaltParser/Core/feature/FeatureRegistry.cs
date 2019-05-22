using System;

namespace org.maltparser.core.feature
{
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;

	public interface FeatureRegistry
	{
		object get(Type key);
		void put(Type key, object value);
		AbstractFeatureFactory getFactory(Type clazz);
		SymbolTableHandler SymbolTableHandler {get;}
		DataFormatInstance DataFormatInstance {get;}
	}

}