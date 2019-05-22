using System;

namespace org.maltparser.core.feature
{
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.symbol;

	public interface FeatureRegistry
	{
		object get(Type key);
		void put(Type key, object value);
		AbstractFeatureFactory getFactory(Type clazz);
		SymbolTableHandler SymbolTableHandler {get;}
		DataFormatInstance DataFormatInstance {get;}
	}

}