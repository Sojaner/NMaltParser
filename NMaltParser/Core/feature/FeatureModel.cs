using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.feature
{
    using  function;
    using  spec;
    using  system;


    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	[Serializable]
	public class FeatureModel : Dictionary<string, FeatureVector>
	{
		public const long serialVersionUID = 3256444702936019250L;
		private static readonly Pattern splitPattern = Pattern.compile("\\(|\\)|\\[|\\]|,");
		private readonly SpecificationModel specModel;
		private readonly List<AddressFunction> addressFunctionCache;
		private readonly List<FeatureFunction> featureFunctionCache;
		private readonly FeatureFunction divideFeatureFunction;
		private readonly FeatureRegistry registry;
		private readonly FeatureEngine featureEngine;
		private readonly FeatureVector mainFeatureVector;
		private readonly List<int> divideFeatureIndexVector;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FeatureModel(org.maltparser.core.feature.spec.SpecificationModel _specModel, FeatureRegistry _registry, org.maltparser.core.feature.system.FeatureEngine _engine, String dataSplitColumn, String dataSplitStructure) throws org.maltparser.core.exception.MaltChainedException
		public FeatureModel(SpecificationModel _specModel, FeatureRegistry _registry, FeatureEngine _engine, string dataSplitColumn, string dataSplitStructure)
		{
			specModel = _specModel;
			registry = _registry;
			featureEngine = _engine;
			addressFunctionCache = new List<AddressFunction>();
			featureFunctionCache = new List<FeatureFunction>();
			FeatureVector tmpMainFeatureVector = null;
			foreach (SpecificationSubModel subModel in specModel)
			{
				FeatureVector fv = new FeatureVector(this, subModel);
				if (tmpMainFeatureVector == null)
				{
					if (subModel.SubModelName.Equals("MAIN"))
					{
						tmpMainFeatureVector = fv;
					}
					else
					{
						tmpMainFeatureVector = fv;
						put(subModel.SubModelName, fv);
					}
				}
				else
				{
					put(subModel.SubModelName, fv);
				}
			}
			mainFeatureVector = tmpMainFeatureVector;
			if (!ReferenceEquals(dataSplitColumn, null) && dataSplitColumn.Length > 0 && !ReferenceEquals(dataSplitStructure, null) && dataSplitStructure.Length > 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
				StringBuilder sb = new StringBuilder();
				sb.Append("InputColumn(");
				sb.Append(dataSplitColumn);
				sb.Append(", ");
				sb.Append(dataSplitStructure);
				sb.Append(')');
				divideFeatureFunction = identifyFeature(sb.ToString());
	//			this.divideFeatureIndexVectorMap = new HashMap<String,ArrayList<Integer>>();
				divideFeatureIndexVector = new List<int>();

				for (int i = 0; i < mainFeatureVector.Count; i++)
				{
					if (mainFeatureVector[i].Equals(divideFeatureFunction))
					{
						divideFeatureIndexVector.Add(i);
					}
				}
				foreach (SpecificationSubModel subModel in specModel)
				{
					FeatureVector featureVector = get(subModel.SubModelName);
					if (featureVector == null)
					{
						featureVector = mainFeatureVector;
					}
					string divideKeyName = "/" + subModel.SubModelName;
	//				divideFeatureIndexVectorMap.put(divideKeyName, divideFeatureIndexVector);

					FeatureVector divideFeatureVector = (FeatureVector)featureVector.clone();
					foreach (int? i in divideFeatureIndexVector)
					{
						divideFeatureVector.Remove(divideFeatureVector[i]);
					}
					put(divideKeyName,divideFeatureVector);
				}
			}
			else
			{
				divideFeatureFunction = null;
	//			this.divideFeatureIndexVectorMap = null;
				divideFeatureIndexVector = null;
			}
		}

		public virtual SpecificationModel SpecModel
		{
			get
			{
				return specModel;
			}
		}

		public virtual FeatureRegistry Registry
		{
			get
			{
				return registry;
			}
		}

		public virtual FeatureEngine FeatureEngine
		{
			get
			{
				return featureEngine;
			}
		}

		public virtual FeatureVector MainFeatureVector
		{
			get
			{
				return mainFeatureVector;
			}
		}

		public virtual FeatureVector getFeatureVector(string subModelName)
		{
			return get(subModelName);
		}

		public virtual FeatureVector getFeatureVector(string decisionSymbol, string subModelName)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			if (decisionSymbol.Length > 0)
			{
				sb.Append(decisionSymbol);
				sb.Append('.');
			}
			sb.Append(subModelName);
			if (containsKey(sb.ToString()))
			{
				return get(sb.ToString());
			}
			else if (containsKey(subModelName))
			{
				return get(subModelName);
			}
			return mainFeatureVector;
		}

		public virtual FeatureFunction DivideFeatureFunction
		{
			get
			{
				return divideFeatureFunction;
			}
		}

		public virtual bool hasDivideFeatureFunction()
		{
			return divideFeatureFunction != null;
		}

	//	public ArrayList<Integer> getDivideFeatureIndexVectorMap(String divideSubModelName) {
	//		return divideFeatureIndexVectorMap.get(divideSubModelName);
	//	}
	//
	//	public boolean hasDivideFeatureIndexVectorMap() {
	//		return divideFeatureIndexVectorMap != null;
	//	}

		public virtual List<int> DivideFeatureIndexVector
		{
			get
			{
				return divideFeatureIndexVector;
			}
		}

		public virtual bool hasDivideFeatureIndexVector()
		{
			return divideFeatureIndexVector != null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update() throws org.maltparser.core.exception.MaltChainedException
		public virtual void update()
		{
			for (int i = 0, n = addressFunctionCache.Count; i < n; i++)
			{
				addressFunctionCache[i].update();
			}

			for (int i = 0, n = featureFunctionCache.Count; i < n; i++)
			{
				featureFunctionCache[i].update();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void update(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public virtual void update(object[] arguments)
		{
			for (int i = 0, n = addressFunctionCache.Count; i < n; i++)
			{
				addressFunctionCache[i].update(arguments);
			}

			for (int i = 0, n = featureFunctionCache.Count; i < n; i++)
			{
				featureFunctionCache[i].update();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.function.FeatureFunction identifyFeature(String spec) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureFunction identifyFeature(string spec)
		{
			string[] items = splitPattern.split(spec);
			Stack<object> objects = new Stack<object>();
			for (int i = items.Length - 1; i >= 0; i--)
			{
				if (items[i].Trim().Length != 0)
				{
					objects.Push(items[i].Trim());
				}
			}
			identifyFeatureFunction(objects);
			if (objects.Count != 1 || !(objects.Peek() is FeatureFunction) || (objects.Peek() is AddressFunction))
			{
				throw new FeatureException("The feature specification '" + spec + "' were not recognized properly. ");
			}
			return (FeatureFunction)objects.Pop();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void identifyFeatureFunction(java.util.Stack<Object> objects) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void identifyFeatureFunction(Stack<object> objects)
		{
			Function function = featureEngine.newFunction(objects.Peek().ToString(), registry);
			if (function != null)
			{
				objects.Pop();
				if (objects.Count > 0)
				{
					identifyFeatureFunction(objects);
				}
				initializeFunction(function, objects);
			}
			else
			{
				if (objects.Count > 0)
				{
					object o = objects.Pop();
					if (objects.Count > 0)
					{
						identifyFeatureFunction(objects);
					}
					objects.Push(o);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initializeFunction(org.maltparser.core.feature.function.Function function, java.util.Stack<Object> objects) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void initializeFunction(Function function, Stack<object> objects)
		{
			Type[] paramTypes = function.ParameterTypes;
			object[] arguments = new object[paramTypes.Length];
			for (int i = 0; i < paramTypes.Length; i++)
			{
				if (paramTypes[i] == typeof(Integer))
				{
					if (objects.Peek() is string)
					{
						string @object = (string)objects.Pop();
						try
						{
							objects.Push(int.Parse(@object));
						}
						catch (FormatException e)
						{
							throw new FeatureException("The function '" + function.GetType() + "' cannot be initialized with argument '" + @object + "'" + ", expect an integer value. ", e);
						}
					}
					else
					{
						throw new FeatureException("The function '" + function.GetType() + "' cannot be initialized with argument '" + objects.Peek() + "'" + ", expect an integer value. ");
					}
				}
				else if (paramTypes[i] == typeof(Double))
				{
					if (objects.Peek() is string)
					{
						string @object = (string)objects.Pop();
						try
						{
							objects.Push(double.Parse(@object));
						}
						catch (FormatException e)
						{
							throw new FeatureException("The function '" + function.GetType() + "' cannot be initialized with argument '" + @object + "'" + ", expect a numeric value. ", e);
						}
					}
					else
					{
						throw new FeatureException("The function '" + function.GetType() + "' cannot be initialized with argument '" + objects.Peek() + "'" + ", expect a numeric value. ");
					}
				}
				else if (paramTypes[i] == typeof(Boolean))
				{
					if (objects.Peek() is string)
					{
						objects.Push(bool.Parse(((string)objects.Pop())));
					}
					else
					{
						throw new FeatureException("The function '" + function.GetType() + "' cannot be initialized with argument '" + objects.Peek() + "'" + ", expect a boolean value. ");

					}
				}
				if (!paramTypes[i].IsInstanceOfType(objects.Peek()))
				{
					throw new FeatureException("The function '" + function.GetType() + "' cannot be initialized with argument '" + objects.Peek() + "'");
				}
				arguments[i] = objects.Pop();
			}
			function.initialize(arguments);
			if (function is AddressFunction)
			{
				int index = addressFunctionCache.IndexOf(function);
				if (index != -1)
				{
					function = addressFunctionCache[index];
				}
				else
				{
					addressFunctionCache.Add((AddressFunction)function);
				}
			}
			else if (function is FeatureFunction)
			{
				int index = featureFunctionCache.IndexOf(function);
				if (index != -1)
				{
					function = featureFunctionCache[index];
				}
				else
				{
					featureFunctionCache.Add((FeatureFunction)function);
				}
			}
			objects.Push(function);
		}

		public override string ToString()
		{
			return specModel.ToString();
		}
	}

}