using System;

namespace org.maltparser.core.feature
{
	using  io.dataformat;
	using  symbol;

	public interface FeatureRegistry
	{
		object get(Type key);
		void put(Type key, object value);
		AbstractFeatureFactory getFactory(Type clazz);
		SymbolTableHandler SymbolTableHandler {get;}
		DataFormatInstance DataFormatInstance {get;}
	}

}