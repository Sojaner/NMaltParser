using System;
using System.Text;

namespace org.maltparser.core.feature.system
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature.function;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class FunctionDescription
	{
		private readonly string name;
		private readonly Type functionClass;
		private readonly bool hasSubfunctions;
		private readonly bool hasFactory;

		public FunctionDescription(string _name, Type _functionClass, bool _hasSubfunctions, bool _hasFactory)
		{
			this.name = _name;
			this.functionClass = _functionClass;
			this.hasSubfunctions = _hasSubfunctions;
			this.hasFactory = _hasFactory;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.function.Function newFunction(org.maltparser.core.feature.FeatureRegistry registry) throws org.maltparser.core.exception.MaltChainedException
		public virtual Function newFunction(FeatureRegistry registry)
		{
			if (hasFactory)
			{
	//			for (Class<?> c : registry.keySet()) {
	//				try {
	//					c.asSubclass(functionClass);
	//				} catch (ClassCastException e) {
	//					continue;
	//				}
	//				return ((AbstractFeatureFactory)registry.get(c)).makeFunction(name);
	//			}
	//			return null;
				return registry.getFactory(functionClass).makeFunction(name, registry);
			}
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: Constructor<?>[] constructors = functionClass.getConstructors();
			System.Reflection.ConstructorInfo<object>[] constructors = functionClass.GetConstructors();
			if (constructors.Length == 0)
			{
				try
				{
					return (Function)System.Activator.CreateInstance(functionClass);
				}
				catch (InstantiationException e)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new FeatureException("The function '" + functionClass.FullName + "' cannot be initialized. ", e);
				}
				catch (IllegalAccessException e)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new FeatureException("The function '" + functionClass.FullName + "' cannot be initialized. ", e);
				}
			}
			Type[] @params = constructors[0].ParameterTypes;
			if (@params.Length == 0)
			{
				try
				{
					return (Function)System.Activator.CreateInstance(functionClass);
				}
				catch (InstantiationException e)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new FeatureException("The function '" + functionClass.FullName + "' cannot be initialized. ", e);
				}
				catch (IllegalAccessException e)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new FeatureException("The function '" + functionClass.FullName + "' cannot be initialized. ", e);
				}
			}
			object[] arguments = new object[@params.Length];
			for (int i = 0; i < @params.Length; i++)
			{
				if (hasSubfunctions && @params[i] == typeof(string))
				{
					arguments[i] = name;
				}
				else
				{
					arguments[i] = registry.get(@params[i]);
					if (arguments[i] == null)
					{
						return null;
					}
				}
			}
			try
			{
				return (Function)constructors[0].newInstance(arguments);
			}
			catch (InstantiationException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new FeatureException("The function '" + functionClass.FullName + "' cannot be initialized. ", e);
			}
			catch (IllegalAccessException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new FeatureException("The function '" + functionClass.FullName + "' cannot be initialized. ", e);
			}
			catch (InvocationTargetException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new FeatureException("The function '" + functionClass.FullName + "' cannot be initialized. ", e);
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual Type FunctionClass
		{
			get
			{
				return functionClass;
			}
		}

		public virtual bool HasSubfunctions
		{
			get
			{
				return hasSubfunctions;
			}
		}

		public virtual bool HasFactory
		{
			get
			{
				return hasFactory;
			}
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			if (!(name.Equals(((FunctionDescription)obj).Name, StringComparison.OrdinalIgnoreCase)))
			{
				return false;
			}
			else if (!(functionClass.Equals(((FunctionDescription)obj).FunctionClass)))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(name);
			sb.Append("->");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			sb.Append(functionClass.FullName);
			return sb.ToString();
		}
	}

}