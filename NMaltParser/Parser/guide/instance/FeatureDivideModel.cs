using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.parser.guide.instance
{


	using  core.exception;
	using  core.feature;
    using  core.feature.value;
	using  core.syntaxgraph;
	using  history.action;

	/// <summary>
	/// The feature divide model is used for divide the training instances into several models according to
	/// a divide feature. Usually this strategy decrease the training and classification time, but can also decrease 
	/// the accuracy of the parser.  
	/// 
	/// @author Johan Hall
	/// </summary>
	public class FeatureDivideModel : InstanceModel
	{
		private readonly Model parent;
		private readonly SortedDictionary<int, AtomicModel> divideModels;
	//	private FeatureVector masterFeatureVector;
		private int frequency = 0;
		private readonly int divideThreshold;
		private AtomicModel masterModel;

		/// <summary>
		/// Constructs a feature divide model.
		/// </summary>
		/// <param name="parent"> the parent guide model. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FeatureDivideModel(org.maltparser.parser.guide.Model parent) throws org.maltparser.core.exception.MaltChainedException
		public FeatureDivideModel(Model parent)
		{
			this.parent = parent;
			Frequency = 0;
	//		this.masterFeatureVector = featureVector;

			string data_split_threshold = Guide.Configuration.getOptionValue("guide", "data_split_threshold").ToString().Trim();
			if (!ReferenceEquals(data_split_threshold, null))
			{
				try
				{
					divideThreshold = int.Parse(data_split_threshold);
				}
				catch (System.FormatException e)
				{
					throw new GuideException("The --guide-data_split_threshold option is not an integer value. ", e);
				}
			}
			else
			{
				divideThreshold = 0;
			}
			divideModels = new SortedDictionary<int, AtomicModel>();
			if (Guide.GuideMode == ClassifierGuide_GuideMode.BATCH)
			{
				masterModel = new AtomicModel(-1, this);
			}
			else if (Guide.GuideMode == ClassifierGuide_GuideMode.CLASSIFY)
			{
				load();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(FeatureVector featureVector, SingleDecision decision)
		{
	//		featureVector.getFeatureModel().getDivideFeatureFunction().update();
			SingleFeatureValue featureValue = (SingleFeatureValue)featureVector.FeatureModel.DivideFeatureFunction.FeatureValue;
			if (!divideModels.ContainsKey(featureValue.IndexCode))
			{
				divideModels[featureValue.IndexCode] = new AtomicModel(featureValue.IndexCode, this);
			}
			FeatureVector divideFeatureVector = featureVector.FeatureModel.getFeatureVector("/" + featureVector.SpecSubModel.SubModelName);
			divideModels[featureValue.IndexCode].addInstance(divideFeatureVector, decision);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances(org.maltparser.core.feature.FeatureModel featureModel) throws org.maltparser.core.exception.MaltChainedException
		public virtual void noMoreInstances(FeatureModel featureModel)
		{
			foreach (int? index in divideModels.Keys)
			{
				divideModels[index].noMoreInstances(featureModel);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.TreeSet<int> removeSet = new java.util.TreeSet<int>();
			SortedSet<int> removeSet = new SortedSet<int>();
			foreach (int? index in divideModels.Keys)
			{
				if (divideModels[index].Frequency <= divideThreshold)
				{
					divideModels[index].moveAllInstances(masterModel, featureModel.DivideFeatureFunction, featureModel.DivideFeatureIndexVector);
					removeSet.Add(index);
				}
			}
			foreach (int? index in removeSet)
			{
				divideModels.Remove(index);
			}
			masterModel.noMoreInstances(featureModel);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void finalizeSentence(DependencyStructure dependencyGraph)
		{
			if (divideModels != null)
			{
				foreach (AtomicModel divideModel in divideModels.Values)
				{
					divideModel.finalizeSentence(dependencyGraph);
				}
			}
			else
			{
				throw new GuideException("The feature divide models cannot be found. ");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predict(FeatureVector featureVector, SingleDecision decision)
		{
			AtomicModel model = getAtomicModel((SingleFeatureValue)featureVector.FeatureModel.DivideFeatureFunction.FeatureValue);
			if (model == null)
			{
				if (Guide.Configuration.LoggerInfoEnabled)
				{
					Guide.Configuration.logInfoMessage("Could not predict the next parser decision because there is " + "no divide or master model that covers the divide value '" + ((SingleFeatureValue)featureVector.FeatureModel.DivideFeatureFunction.FeatureValue).IndexCode + "', as default" + " class code '1' is used. ");
				}
				decision.addDecision(1); // default prediction
				return true;
			}
			return model.predict(getModelFeatureVector(model, featureVector), decision);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector predictExtract(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector predictExtract(FeatureVector featureVector, SingleDecision decision)
		{
			AtomicModel model = getAtomicModel((SingleFeatureValue)featureVector.FeatureModel.DivideFeatureFunction.FeatureValue);
			if (model == null)
			{
				return null;
			}
			return model.predictExtract(getModelFeatureVector(model, featureVector), decision);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.feature.FeatureVector extract(org.maltparser.core.feature.FeatureVector featureVector) throws org.maltparser.core.exception.MaltChainedException
		public virtual FeatureVector extract(FeatureVector featureVector)
		{
			AtomicModel model = getAtomicModel((SingleFeatureValue)featureVector.FeatureModel.DivideFeatureFunction.FeatureValue);
			if (model == null)
			{
				return featureVector;
			}
			return model.extract(getModelFeatureVector(model, featureVector));
		}

		private FeatureVector getModelFeatureVector(AtomicModel model, FeatureVector featureVector)
		{
			if (model.Index == -1)
			{
				return featureVector;
			}
			else
			{
				return featureVector.FeatureModel.getFeatureVector("/" + featureVector.SpecSubModel.SubModelName);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private AtomicModel getAtomicModel(org.maltparser.core.feature.value.SingleFeatureValue featureValue) throws org.maltparser.core.exception.MaltChainedException
		private AtomicModel getAtomicModel(SingleFeatureValue featureValue)
		{
			//((SingleFeatureValue)masterFeatureVector.getFeatureModel().getDivideFeatureFunction().getFeatureValue()).getIndexCode()
			if (divideModels != null && divideModels.ContainsKey(featureValue.IndexCode))
			{
				return divideModels[featureValue.IndexCode];
			}
			else if (masterModel != null && masterModel.Frequency > 0)
			{
				return masterModel;
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			if (divideModels != null)
			{
				foreach (AtomicModel divideModel in divideModels.Values)
				{
					divideModel.terminate();
				}
			}
			if (masterModel != null)
			{
				masterModel.terminate();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void train() throws org.maltparser.core.exception.MaltChainedException
		public virtual void train()
		{
			foreach (AtomicModel divideModel in divideModels.Values)
			{
				divideModel.train();
			}
			masterModel.train();
			save();
			foreach (AtomicModel divideModel in divideModels.Values)
			{
				divideModel.terminate();
			}
			masterModel.terminate();
		}

		/// <summary>
		/// Saves the feature divide model settings .dsm file.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void save() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void save()
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedWriter out = new java.io.BufferedWriter(getGuide().getConfiguration().getOutputStreamWriter(getModelName()+".dsm"));
				StreamWriter @out = new StreamWriter(Guide.Configuration.getOutputStreamWriter(ModelName + ".dsm"));
				@out.Write(masterModel.Index + "\t" + masterModel.Frequency + "\n");

				if (divideModels != null)
				{
					foreach (AtomicModel divideModel in divideModels.Values)
					{
						@out.Write(divideModel.Index + "\t" + divideModel.Frequency + "\n");
					}
				}
				@out.Close();
			}
			catch (IOException e)
			{
				throw new GuideException("Could not write to the guide model settings file '" + ModelName + ".dsm" + "', when " + "saving the guide model settings to file. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void load() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void load()
		{
			string dsmString = Guide.Configuration.getConfigFileEntryString(ModelName + ".dsm");
			string[] lines = dsmString.Split("\n", true);
			Pattern tabPattern = Pattern.compile("\t");
	//		FeatureVector divideFeatureVector = featureVector.getFeatureModel().getFeatureVector("/" + featureVector.getSpecSubModel().getSubModelName());
			for (int i = 0; i < lines.Length; i++)
			{
				string[] cols = tabPattern.split(lines[i]);
				if (cols.Length != 2)
				{
					throw new GuideException("");
				}
				int code = -1;
				int freq = 0;
				try
				{
					code = int.Parse(cols[0]);
					freq = int.Parse(cols[1]);
				}
				catch (System.FormatException e)
				{
					throw new GuideException("Could not convert a string value into an integer value when loading the feature divide model settings (.dsm). ", e);
				}
				if (code == -1)
				{
					masterModel = new AtomicModel(-1, this);
					masterModel.Frequency = freq;
				}
				else if (divideModels != null)
				{
					divideModels[code] = new AtomicModel(code, this);
					divideModels[code].Frequency = freq;
				}
				Frequency = Frequency + freq;
			}
		}

		/// <summary>
		/// Returns the parent model
		/// </summary>
		/// <returns> the parent model </returns>
		public virtual Model Parent
		{
			get
			{
				return parent;
			}
		}

		public virtual ClassifierGuide Guide
		{
			get
			{
				return parent.Guide;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getModelName() throws org.maltparser.core.exception.MaltChainedException
		public virtual string ModelName
		{
			get
			{
				try
				{
					return parent.ModelName;
				}
				catch (System.NullReferenceException e)
				{
					throw new GuideException("The parent guide model cannot be found. ", e);
				}
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
				frequency = value;
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



		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			//TODO
			return sb.ToString();
		}
	}

}