using System;

namespace org.maltparser.parser
{
	using  org.maltparser.core.feature;
	using  org.maltparser.core.feature;
	using  org.maltparser.core.helper;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.symbol;

	public class ParserRegistry : FeatureRegistry
	{
		private AbstractFeatureFactory abstractParserFactory;
		private AlgoritmInterface algorithm;
		private SymbolTableHandler symbolTableHandler;
		private DataFormatInstance dataFormatInstance;
		private readonly HashMap<Type, object> registry;

		public ParserRegistry()
		{
			this.registry = new HashMap<Type, object>();
		}

		public virtual object get(Type key)
		{
			return registry[key];
		}

		public virtual void put(Type key, object value)
		{
			registry[key] = value;
			if (key == typeof(org.maltparser.parser.AbstractParserFactory))
			{
				abstractParserFactory = (AbstractParserFactory)value;
			}
			else if (key == typeof(org.maltparser.parser.AlgoritmInterface))
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
				this.symbolTableHandler = value;
				this.registry[typeof(SymbolTableHandler)] = value;
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
				this.dataFormatInstance = value;
				this.registry[typeof(DataFormatInstance)] = value;
			}
		}


		public virtual AbstractFeatureFactory getAbstractParserFeatureFactory()
		{
			return abstractParserFactory;
		}

		public virtual void setAbstractParserFeatureFactory(AbstractParserFactory _abstractParserFactory)
		{
			this.registry[typeof(org.maltparser.parser.AbstractParserFactory)] = _abstractParserFactory;
			this.abstractParserFactory = _abstractParserFactory;
		}

		public virtual AlgoritmInterface Algorithm
		{
			get
			{
				return algorithm;
			}
			set
			{
				this.registry[typeof(org.maltparser.parser.AlgoritmInterface)] = value;
				this.algorithm = value;
			}
		}

	}

}