using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.parser.guide.instance
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature;
	using  org.maltparser.core.feature;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.syntaxgraph;
	using  org.maltparser.ml;
	using  org.maltparser.ml.lib;
	using  org.maltparser.ml.lib;
	using  org.maltparser.parser.history.action;


	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class AtomicModel : InstanceModel
	{
		public static readonly Type[] argTypes = new Type[] {typeof(org.maltparser.parser.guide.instance.InstanceModel), typeof(Integer)};
		private readonly Model parent;
		private readonly string modelName;
	//	private final FeatureVector featureVector;
		private readonly int index;
		private readonly LearningMethod method;
		private int frequency = 0;


		/// <summary>
		/// Constructs an atomic model.
		/// </summary>
		/// <param name="index"> the index of the atomic model (-1..n), where -1 is special value (used by a single model 
		/// or the master divide model) and n is number of divide models. </param>
		/// <param name="parent"> the parent guide model. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AtomicModel(int index, org.maltparser.parser.guide.Model parent) throws org.maltparser.core.exception.MaltChainedException
		public AtomicModel(int index, Model parent)
		{
			this.parent = parent;
			this.index = index;
			if (index == -1)
			{
				this.modelName = parent.ModelName + ".";
			}
			else
			{
				this.modelName = parent.ModelName + "." + (new Formatter()).format("%03d", index) + ".";
			}
	//		this.featureVector = featureVector;
			this.frequency = 0;
			int? learnerMode = null;
			if (Guide.GuideMode == org.maltparser.parser.guide.ClassifierGuide_GuideMode.CLASSIFY)
			{
				learnerMode = org.maltparser.ml.LearningMethod_Fields.CLASSIFY;
			}
			else if (Guide.GuideMode == org.maltparser.parser.guide.ClassifierGuide_GuideMode.BATCH)
			{
				learnerMode = org.maltparser.ml.LearningMethod_Fields.BATCH;
			}

			// start init learning method
			Type clazz = (Type)Guide.Configuration.getOptionValue("guide", "learner");
			if (clazz == typeof(LibSvm))
			{
				this.method = new LibSvm(this, learnerMode);
			}
			else if (clazz == typeof(LibLinear))
			{
				this.method = new LibLinear(this, learnerMode);
			}
			else
			{
				object[] arguments = new object[] {this, learnerMode};
				try
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: Constructor<?> constructor = clazz.getConstructor(argTypes);
					System.Reflection.ConstructorInfo<object> constructor = clazz.GetConstructor(argTypes);
					this.method = (LearningMethod)constructor.newInstance(arguments);
				}
				catch (NoSuchMethodException e)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new GuideException("The learner class '" + clazz.FullName + "' cannot be initialized. ", e);
				}
				catch (InstantiationException e)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new GuideException("The learner class '" + clazz.FullName + "' cannot be initialized. ", e);
				}
				catch (IllegalAccessException e)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new GuideException("The learner class '" + clazz.FullName + "' cannot be initialized. ", e);
				}
				catch (InvocationTargetException e)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new GuideException("The learner class '" + clazz.FullName + "' cannot be initialized. ", e);
				}
			}
			// end init learning method

			if (learnerMode.Value == org.maltparser.ml.LearningMethod_Fields.BATCH && index == -1 && Guide.Configuration != null)
			{
				Guide.Configuration.writeInfoToConfigFile(method.ToString());
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(FeatureVector featureVector, SingleDecision decision)
		{
			try
			{
				method.addInstance(decision, featureVector);
			}
			catch (System.NullReferenceException e)
			{
				throw new GuideException("The learner cannot be found. ", e);
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances(org.maltparser.core.feature.FeatureModel featureModel) throws org.maltparser.core.exception.MaltChainedException
		public virtual void noMoreInstances(FeatureModel featureModel)
		{
			try
			{
				method.noMoreInstances();
			}
			catch (System.NullReferenceException e)
			{
				throw new GuideException("The learner cannot be found. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void finalizeSentence(DependencyStructure dependencyGraph)
		{
			try
			{
				method.finalizeSentence(dependencyGraph);
			}
			catch (System.NullReferenceException e)
			{
				throw new GuideException("The learner cannot be found. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predict(FeatureVector featureVector, SingleDecision decision)
		{
			try
			{
				return method.predict(featureVector, decision);
			}
			catch (System.NullReferenceException e)
			{
				throw new GuideException("The learner cannot be found. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector predictExtract(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector predictExtract(FeatureVector featureVector, SingleDecision decision)
		{
			try
			{

				if (method.predict(featureVector, decision))
				{
					return featureVector;
				}
				return null;
			}
			catch (System.NullReferenceException e)
			{
				throw new GuideException("The learner cannot be found. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector extract(org.maltparser.core.feature.FeatureVector featureVector) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector extract(FeatureVector featureVector)
		{
			return featureVector;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			if (method != null)
			{
				method.terminate();
			}
		}

		/// <summary>
		/// Moves all instance from this atomic model into the destination atomic model and add the divide feature.
		/// This method is used by the feature divide model to sum up all model below a certain threshold.
		/// </summary>
		/// <param name="model"> the destination atomic model </param>
		/// <param name="divideFeature"> the divide feature </param>
		/// <param name="divideFeatureIndexVector"> the divide feature index vector </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void moveAllInstances(AtomicModel model, org.maltparser.core.feature.function.FeatureFunction divideFeature, java.util.ArrayList<int> divideFeatureIndexVector) throws org.maltparser.core.exception.MaltChainedException
		public virtual void moveAllInstances(AtomicModel model, FeatureFunction divideFeature, List<int> divideFeatureIndexVector)
		{
			if (method == null)
			{
				throw new GuideException("The learner cannot be found. ");
			}
			else if (model == null)
			{
				throw new GuideException("The guide model cannot be found. ");
			}
			else if (divideFeature == null)
			{
				throw new GuideException("The divide feature cannot be found. ");
			}
			else if (divideFeatureIndexVector == null)
			{
				throw new GuideException("The divide feature index vector cannot be found. ");
			}
			((Modifiable)divideFeature).FeatureValue = index;
			method.moveAllInstances(model.Method, divideFeature, divideFeatureIndexVector);
			method.terminate();
		}

		/// <summary>
		/// Invokes the train() of the learning method 
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void train() throws org.maltparser.core.exception.MaltChainedException
		public virtual void train()
		{
			try
			{
				method.train();
				method.terminate();
			}
			catch (System.NullReferenceException e)
			{
				throw new GuideException("The learner cannot be found. ", e);
			}


		}

		/// <summary>
		/// Returns the parent guide model
		/// </summary>
		/// <returns> the parent guide model </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.guide.Model getParent() throws org.maltparser.core.exception.MaltChainedException
		public virtual Model Parent
		{
			get
			{
				if (parent == null)
				{
					throw new GuideException("The atomic model can only be used by a parent model. ");
				}
				return parent;
			}
		}


		public virtual string ModelName
		{
			get
			{
				return modelName;
			}
		}

		/// <summary>
		/// Returns the feature vector used by this atomic model
		/// </summary>
		/// <returns> a feature vector object </returns>
	//	public FeatureVector getFeatures() {
	//		return featureVector;
	//	}

		public virtual ClassifierGuide Guide
		{
			get
			{
				return parent.Guide;
			}
		}

		/// <summary>
		/// Returns the index of the atomic model
		/// </summary>
		/// <returns> the index of the atomic model </returns>
		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		/// <summary>
		/// Returns the frequency (number of instances)
		/// </summary>
		/// <returns> the frequency (number of instances) </returns>
		public virtual int Frequency
		{
			get
			{
				return frequency;
			}
			set
			{
				this.frequency = value;
			}
		}

		/// <summary>
		/// Increase the frequency by 1
		/// </summary>
		public virtual void increaseFrequency()
		{
			if (parent is InstanceModel)
			{
				((InstanceModel)parent).increaseFrequency();
			}
			frequency++;
		}

		public virtual void decreaseFrequency()
		{
			if (parent is InstanceModel)
			{
				((InstanceModel)parent).decreaseFrequency();
			}
			frequency--;
		}

		/// <summary>
		/// Returns a learner object
		/// </summary>
		/// <returns> a learner object </returns>
		public virtual LearningMethod Method
		{
			get
			{
				return method;
			}
		}


		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(method.ToString());
			return sb.ToString();
		}
	}

}