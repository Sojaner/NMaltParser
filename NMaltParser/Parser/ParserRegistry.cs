using System;

namespace org.maltparser.parser
{
	using  core.feature;
    using  core.helper;
	using  core.io.dataformat;
	using  core.symbol;

	public class ParserRegistry : FeatureRegistry
	{
		private AbstractFeatureFactory abstractParserFactory;
		private AlgoritmInterface algorithm;
		private SymbolTableHandler symbolTableHandler;
		private DataFormatInstance dataFormatInstance;
		private readonly HashMap<Type, object> registry;

		public ParserRegistry()
		{
			registry = new HashMap<Type, object>();
		}

		public virtual object get(Type key)
		{
			return registry[key];
		}

		public virtual void put(Type key, object value)
		{
			registry[key] = value;
			if (key == typeof(AbstractParserFactory))
			{
				abstractParserFactory = (AbstractParserFactory)value;
			}
			else if (key == typeof(AlgoritmInterface))
			{
				algorithm = (AlgoritmInterface)value;
			}
		}

		public virtual AbstractFeatureFactory getFactory(Type clazz)
		{
			return abstractParserFactory;
		}

		public virtual SymbolTableHandler SymbolTableHandler
		{
			get
			{
				return symbolTableHandler;
			}
			set
			{
				symbolTableHandler = value;
				registry[typeof(SymbolTableHandler)] = value;
			}
		}


		public virtual DataFormatInstance DataFormatInstance
		{
			get
			{
				return dataFormatInstance;
			}
			set
			{
				dataFormatInstance = value;
				registry[typeof(DataFormatInstance)] = value;
			}
		}


		public virtual AbstractFeatureFactory getAbstractParserFeatureFactory()
		{
			return abstractParserFactory;
		}

		public virtual void setAbstractParserFeatureFactory(AbstractParserFactory _abstractParserFactory)
		{
			registry[typeof(AbstractParserFactory)] = _abstractParserFactory;
			abstractParserFactory = _abstractParserFactory;
		}

		public virtual AlgoritmInterface Algorithm
		{
			get
			{
				return algorithm;
			}
			set
			{
				registry[typeof(AlgoritmInterface)] = value;
				algorithm = value;
			}
		}

	}

}