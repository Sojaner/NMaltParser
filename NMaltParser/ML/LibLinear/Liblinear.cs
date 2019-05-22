using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.ml.liblinear
{
    using  core.config;
    using  core.exception;
	using  core.feature;
	using  core.feature.function;
	using  core.feature.value;
    using  core.helper;
	using  core.syntaxgraph;
	using  parser;
	using  parser.guide.instance;
	using  parser.history.action;
	using  parser.history.kbest;


    public class Liblinear : LearningMethod
	{
		public const string LIBLINEAR_VERSION = "1.51";
		public enum Verbostity
		{
			SILENT,
			ERROR,
			ALL
		}
		private LinkedHashMap<string, string> liblinearOptions;

		protected internal InstanceModel owner;
		protected internal int learnerMode;
		protected internal string name;
		protected internal int numberOfInstances;
		protected internal bool saveInstanceFiles;
		protected internal bool excludeNullValues;
		protected internal string pathExternalLiblinearTrain = null;
	//	private int[] cardinalities;
		/// <summary>
		/// Instance output stream writer 
		/// </summary>
		private StreamWriter instanceOutput = null;
		/// <summary>
		/// Liblinear model object, only used during classification.
		/// </summary>
		private Model model = null;

		/// <summary>
		/// Parameter string
		/// </summary>
		private string paramString;

		private List<FeatureNode> xlist = null;

		private Verbostity verbosity;

		private Dictionary<long, int> featureMap;
		private int featureCounter = 1;
		private bool featurePruning = false;
		private SortedSet<XNode> featureSet;

		/// <summary>
		/// Constructs a Liblinear learner.
		/// </summary>
		/// <param name="owner"> the guide model owner </param>
		/// <param name="learnerMode"> the mode of the learner TRAIN or CLASSIFY </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Liblinear(org.maltparser.parser.guide.instance.InstanceModel owner, System.Nullable<int> learnerMode) throws org.maltparser.core.exception.MaltChainedException
		public Liblinear(InstanceModel owner, int? learnerMode)
		{
			Owner = owner;
			LearningMethodName = "liblinear";
			LearnerMode = learnerMode.Value;
			NumberOfInstances = 0;
			verbosity = Verbostity.SILENT;

			liblinearOptions = new LinkedHashMap<string, string>();
			initLiblinearOptions();
			parseParameters(Configuration.getOptionValue("liblinear", "liblinear_options").ToString());
			initSpecialParameters();

			if (learnerMode.Value == LearningMethod_Fields.BATCH)
			{
				if (featurePruning)
				{
					featureMap = new Dictionary<long, int>();
				}
				instanceOutput = new StreamWriter(getInstanceOutputStreamWriter(".ins"));
			}
			if (featurePruning)
			{
				featureSet = new SortedSet<XNode>();
			}
		}

		private int addFeatureMapValue(int featurePosition, int code)
		{
			long key = ((((long)featurePosition) << 48) | (long)code);
			if (featureMap.ContainsKey(key))
			{
				return featureMap[key];
			}
			int value = featureCounter++;
			featureMap[key] = value;
			return value;
		}

		private int getFeatureMapValue(int featurePosition, int code)
		{
			long key = ((((long)featurePosition) << 48) | (long)code);
			if (featureMap.ContainsKey(key))
			{
				return featureMap[key];
			}
			return -1;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void saveFeatureMap(java.io.OutputStream os, java.util.HashMap<long,int> map) throws org.maltparser.core.exception.MaltChainedException
		private void saveFeatureMap(Stream os, Dictionary<long, int> map)
		{
			try
			{
				ObjectOutputStream obj_out_stream = new ObjectOutputStream(os);
				obj_out_stream.writeObject(map);
				obj_out_stream.close();
			}
			catch (IOException e)
			{
				throw new LiblinearException("Save feature map error", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.util.HashMap<long,int> loadFeatureMap(java.io.InputStream is) throws org.maltparser.core.exception.MaltChainedException
		private Dictionary<long, int> loadFeatureMap(Stream @is)
		{
			Dictionary<long, int> map = new Dictionary<long, int>();
			try
			{
				ObjectInputStream obj_in_stream = new ObjectInputStream(@is);
				map = (Dictionary<long, int>)obj_in_stream.readObject();
				obj_in_stream.close();
			}
			catch (ClassNotFoundException e)
			{
				throw new LiblinearException("Load feature map error", e);
			}
			catch (IOException e)
			{
				throw new LiblinearException("Load feature map error", e);
			}
			return map;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.parser.history.action.SingleDecision decision, org.maltparser.core.feature.FeatureVector featureVector) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(SingleDecision decision, FeatureVector featureVector)
		{
			if (featureVector == null)
			{
				throw new LiblinearException("The feature vector cannot be found");
			}
			else if (decision == null)
			{
				throw new LiblinearException("The decision cannot be found");
			}

			StringBuilder sb = new StringBuilder();
			try
			{
				sb.Append(decision.DecisionCode + "\t");
				int n = featureVector.Count;
				for (int i = 0; i < n; i++)
				{
					FeatureValue featureValue = featureVector[i].FeatureValue;
					if (excludeNullValues == true && featureValue.NullValue)
					{
						sb.Append("-1");
					}
					else
					{
						if (featureValue is SingleFeatureValue)
						{
							sb.Append(((SingleFeatureValue)featureValue).IndexCode + "");
						}
						else if (featureValue is MultipleFeatureValue)
						{
							ISet<int> values = ((MultipleFeatureValue)featureValue).Codes;
							int j = 0;
							foreach (int? value in values)
							{
								sb.Append(value.ToString());
								if (j != values.Count - 1)
								{
									sb.Append("|");
								}
								j++;
							}
						}
					}
					sb.Append('\t');
				}
				sb.Append('\n');
				instanceOutput.Write(sb.ToString());
				instanceOutput.Flush();
				increaseNumberOfInstances();
			}
			catch (IOException e)
			{
				throw new LiblinearException("The Liblinear learner cannot write to the instance file. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void finalizeSentence(DependencyStructure dependencyGraph)
		{
		}

		/* (non-Javadoc)
		 * @see org.maltparser.ml.LearningMethod#noMoreInstances()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances() throws org.maltparser.core.exception.MaltChainedException
		public virtual void noMoreInstances()
		{
			closeInstanceWriter();
		}


		/* (non-Javadoc)
		 * @see org.maltparser.ml.LearningMethod#train(org.maltparser.parser.guide.feature.FeatureVector)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void train() throws org.maltparser.core.exception.MaltChainedException
		public virtual void train()
		{
			 if (owner == null)
			 {
				throw new LiblinearException("The parent guide model cannot be found. ");
			 }
	//		cardinalities = getCardinalities(featureVector);
			if (ReferenceEquals(pathExternalLiblinearTrain, null))
			{
				try
				{
					Problem problem = null;
					if (featurePruning)
					{
						problem = readLibLinearProblemWithFeaturePruning(getInstanceInputStreamReader(".ins"));
					}
					else
					{
	//					problem = readLibLinearProblem(getInstanceInputStreamReader(".ins"), cardinalities);
					}
					Configuration config = owner.Guide.Configuration;
					if (config.LoggerInfoEnabled)
					{
						config.logInfoMessage("Creating Liblinear model " + getFile(".mod").Name + "\n");
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.PrintStream out = System.out;
					PrintStream @out = System.out;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.PrintStream err = System.err;
					PrintStream err = System.err;
					System.Out = NoPrintStream.NO_PRINTSTREAM;
					System.Err = NoPrintStream.NO_PRINTSTREAM;
					Linear.saveModel(new File(getFile(".mod").AbsolutePath), Linear.train(problem, LiblinearParameters));
					System.Out = err;
					System.Out = @out;
					if (!saveInstanceFiles)
					{
						getFile(".ins").delete();
					}
				}
				catch (OutOfMemoryException e)
				{
					throw new LiblinearException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
				}
				catch (ArgumentException e)
				{
					throw new LiblinearException("The Liblinear learner was not able to redirect Standard Error stream. ", e);
				}
				catch (SecurityException e)
				{
					throw new LiblinearException("The Liblinear learner cannot remove the instance file. ", e);
				}
				catch (IOException e)
				{
					throw new LiblinearException("The Liblinear learner cannot save the model file '" + getFile(".mod").AbsolutePath + "'. ", e);
				}
			}
			else
			{
				trainExternal();
			}

			if (featurePruning)
			{
				try
				{
					saveFeatureMap(new FileStream(getFile(".map").AbsolutePath, FileMode.Create, FileAccess.Write), featureMap);
				}
				catch (FileNotFoundException e)
				{
					throw new LiblinearException("The Liblinear learner cannot save the feature map file '" + getFile(".map").AbsolutePath + "'. ", e);
				}
			}
			else
			{
	//			saveCardinalities(getInstanceOutputStreamWriter(".car"), cardinalities);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void trainExternal() throws org.maltparser.core.exception.MaltChainedException
		private void trainExternal()
		{
			try
			{
	//			maltSVMFormat2OriginalSVMFormat(getInstanceInputStreamReader(".ins"), getInstanceOutputStreamWriter(".ins.tmp"), cardinalities);
				Configuration config = owner.Guide.Configuration;
				if (config.LoggerInfoEnabled)
				{
					config.logInfoMessage("Creating Liblinear model (external) " + getFile(".mod").Name);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] params = getLibLinearParamStringArray();
				string[] @params = LibLinearParamStringArray;
				string[] arrayCommands = new string[@params.Length + 3];
				int i = 0;
				arrayCommands[i++] = pathExternalLiblinearTrain;
				for (; i <= @params.Length; i++)
				{
					arrayCommands[i] = @params[i - 1];
				}
				arrayCommands[i++] = getFile(".ins.tmp").AbsolutePath;
				arrayCommands[i++] = getFile(".mod").AbsolutePath;

				if (verbosity == Verbostity.ALL)
				{
					config.logInfoMessage('\n');
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Process child = Runtime.getRuntime().exec(arrayCommands);
				Process child = Runtime.Runtime.exec(arrayCommands);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream in = child.getInputStream();
				Stream @in = child.InputStream;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream err = child.getErrorStream();
				Stream err = child.ErrorStream;
				int c;
				while ((c = @in.Read()) != -1)
				{
					if (verbosity == Verbostity.ALL)
					{
						config.logInfoMessage((char)c);
					}
				}
				while ((c = err.Read()) != -1)
				{
					if (verbosity == Verbostity.ALL || verbosity == Verbostity.ERROR)
					{
						config.logInfoMessage((char)c);
					}
				}
				if (child.waitFor() != 0)
				{
					config.logErrorMessage(" FAILED (" + child.exitValue() + ")");
				}
				@in.Close();
				err.Close();
				if (!saveInstanceFiles)
				{
					getFile(".ins").delete();
					getFile(".ins.tmp").delete();
				}
				if (config.LoggerInfoEnabled)
				{
					config.logInfoMessage('\n');
				}
			}
			catch (InterruptedException e)
			{
				 throw new LiblinearException("Liblinear is interrupted. ", e);
			}
			catch (ArgumentException e)
			{
				throw new LiblinearException("The Liblinear learner was not able to redirect Standard Error stream. ", e);
			}
			catch (SecurityException e)
			{
				throw new LiblinearException("The Liblinear learner cannot remove the instance file. ", e);
			}
			catch (IOException e)
			{
				throw new LiblinearException("The Liblinear learner cannot save the model file '" + getFile(".mod").AbsolutePath + "'. ", e);
			}
			catch (OutOfMemoryException e)
			{
				throw new LiblinearException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
			}
		}

	//	private int[] getCardinalities(FeatureVector featureVector) {
	//		int[] cardinalities = new int[featureVector.size()];
	//		int i = 0;
	//		for (FeatureFunction feature : featureVector) {
	//			cardinalities[i++] = feature.getFeatureValue().getCardinality();
	//		}
	//		return cardinalities;
	//	}
	//	
	//	private void saveCardinalities(OutputStreamWriter osw, int[] cardinalities) throws MaltChainedException {
	//		final BufferedWriter out = new BufferedWriter(osw);
	//		try {
	//			for (int i = 0, n = cardinalities.length; i < n; i++) {
	//				out.write(Integer.toString(cardinalities[i]));
	//				if (i < n - 1) {
	//					out.write(',');
	//				}
	//			}
	//			out.write('\n');
	//			out.close();
	//		} catch (IOException e) {
	//			throw new LiblinearException("", e);
	//		}
	//	}

	//	private int[] loadCardinalities(InputStreamReader isr) throws MaltChainedException {
	//		int[] cardinalities = null;
	//		try {
	//			final BufferedReader in = new BufferedReader(isr); 
	//			String line;
	//			if ((line = in.readLine()) != null) {
	//				String[] items = line.split(",");
	//				cardinalities = new int[items.length];
	//				for (int i = 0; i < items.length; i++) {
	//					cardinalities[i] = Integer.parseInt(items[i]);
	//				}
	// 			}
	//			in.close();
	//		} catch (IOException e) {
	//			throw new LiblinearException("", e);
	//		} catch (NumberFormatException e) {
	//			throw new LiblinearException("", e);
	//		}
	//		return cardinalities;
	//	}

		/* (non-Javadoc)
		 * @see org.maltparser.ml.LearningMethod#moveAllInstances(org.maltparser.ml.LearningMethod, org.maltparser.core.feature.function.FeatureFunction, java.util.ArrayList)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void moveAllInstances(org.maltparser.ml.LearningMethod method, org.maltparser.core.feature.function.FeatureFunction divideFeature, java.util.ArrayList<int> divideFeatureIndexVector) throws org.maltparser.core.exception.MaltChainedException
		public virtual void moveAllInstances(LearningMethod method, FeatureFunction divideFeature, List<int> divideFeatureIndexVector)
		{
			if (method == null)
			{
				throw new LiblinearException("The learning method cannot be found. ");
			}
			else if (divideFeature == null)
			{
				throw new LiblinearException("The divide feature cannot be found. ");
			}

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader in = new java.io.BufferedReader(getInstanceInputStreamReader(".ins"));
				StreamReader @in = new StreamReader(getInstanceInputStreamReader(".ins"));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedWriter out = method.getInstanceWriter();
				StreamWriter @out = method.InstanceWriter;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder(6);
				StringBuilder sb = new StringBuilder(6);
				int l = @in.Read();
				char c;
				int j = 0;

				while (true)
				{
					if (l == -1)
					{
						sb.Length = 0;
						break;
					}
					c = (char)l;
					l = @in.Read();
					if (c == '\t')
					{
						if (divideFeatureIndexVector.Contains(j - 1))
						{
							@out.Write(Convert.ToString(((SingleFeatureValue)divideFeature.FeatureValue).IndexCode));
							@out.BaseStream.WriteByte('\t');
						}
						@out.Write(sb.ToString());
						j++;
						@out.BaseStream.WriteByte('\t');
						sb.Length = 0;
					}
					else if (c == '\n')
					{
						@out.Write(sb.ToString());
						if (divideFeatureIndexVector.Contains(j - 1))
						{
							@out.BaseStream.WriteByte('\t');
							@out.Write(Convert.ToString(((SingleFeatureValue)divideFeature.FeatureValue).IndexCode));
						}
						@out.BaseStream.WriteByte('\n');
						sb.Length = 0;
						method.increaseNumberOfInstances();
						decreaseNumberOfInstances();
						j = 0;
					}
					else
					{
						sb.Append(c);
					}
				}
				@in.Close();
				getFile(".ins").delete();
				@out.Flush();
			}
			catch (SecurityException e)
			{
				throw new LiblinearException("The Liblinear learner cannot remove the instance file. ", e);
			}
			catch (NullReferenceException e)
			{
				throw new LiblinearException("The instance file cannot be found. ", e);
			}
			catch (FileNotFoundException e)
			{
				throw new LiblinearException("The instance file cannot be found. ", e);
			}
			catch (IOException e)
			{
				throw new LiblinearException("The Liblinear learner read from the instance file. ", e);
			}

		}

		/* (non-Javadoc)
		 * @see org.maltparser.ml.LearningMethod#predict(org.maltparser.parser.guide.feature.FeatureVector, org.maltparser.ml.KBestList)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predict(FeatureVector featureVector, SingleDecision decision)
		{
			if (model == null)
			{
				try
				{
					model = Linear.loadModel(new StreamReader(getInstanceInputStreamReaderFromConfigFile(".mod")));
				}
				catch (IOException e)
				{
					throw new LiblinearException("The model cannot be loaded. ", e);
				}
			}
			if (model == null)
			{
				throw new LiblinearException("The Liblinear learner cannot predict the next class, because the learning model cannot be found. ");
			}
			else if (featureVector == null)
			{
				throw new LiblinearException("The Liblinear learner cannot predict the next class, because the feature vector cannot be found. ");
			}
			if (featurePruning)
			{
				return predictWithFeaturePruning(featureVector, decision);
			}

	//		if (cardinalities == null) {
	//			if (getConfigFileEntry(".car") != null) {
	//				cardinalities = loadCardinalities(getInstanceInputStreamReaderFromConfigFile(".car"));
	//			} else {
	//				cardinalities = getCardinalities(featureVector);
	//			}
	//		}

			//System.out.println("METHOD PREDICT CARDINALITIES SIZE" + cardinalities.length + " FEATURE VECTOR SIZE " +featureVector.size());
			if (xlist == null)
			{
				xlist = new List<FeatureNode>(featureVector.Count);
			}


	//		int offset = 1;
	//		int i = 0;
	//		for (FeatureFunction feature : featureVector) {
	//			final FeatureValue featureValue = feature.getFeatureValue();
	//			if (!(excludeNullValues == true && featureValue.isNullValue())) {
	//				if (featureValue instanceof SingleFeatureValue) {
	//					if (((SingleFeatureValue)featureValue).getCode() < cardinalities[i]) {
	//						xlist.add(new FeatureNode(((SingleFeatureValue)featureValue).getCode() + offset, 1));
	//					}
	//				} else if (featureValue instanceof MultipleFeatureValue) {
	//					for (Integer value : ((MultipleFeatureValue)featureValue).getCodes()) {
	//						if (value < cardinalities[i]) {
	//							xlist.add(new FeatureNode(value + offset, 1));
	//						}
	//					}
	//				}
	//			}
	//			offset += cardinalities[i];
	//			i++;
	//		}
			FeatureNode[] xarray = new FeatureNode[xlist.Count];
			for (int k = 0; k < xlist.Count; k++)
			{
				xarray[k] = xlist[k];
			}

			if (decision.KBestList.K == 1)
			{
				decision.KBestList.add(Linear.predict(model, xarray));
			}
			else
			{
				liblinear_predict_with_kbestlist(model, xarray, decision.KBestList);
			}

			xlist.Clear();

			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predictWithFeaturePruning(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predictWithFeaturePruning(FeatureVector featureVector, SingleDecision decision)
		{
			if (featureMap == null)
			{
				featureMap = loadFeatureMap(getInputStreamFromConfigFileEntry(".map"));
			}

			for (int i = 0; i < featureVector.Count; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.value.FeatureValue featureValue = featureVector.getFeatureValue(i-1);
				FeatureValue featureValue = featureVector.getFeatureValue(i - 1);
				if (!(excludeNullValues == true && featureValue.NullValue))
				{
					if (featureValue is SingleFeatureValue)
					{
						int v = getFeatureMapValue(i, ((SingleFeatureValue)featureValue).IndexCode);
						if (v != -1)
						{
							featureSet.Add(new XNode(v,1));
						}
					}
					else if (featureValue is MultipleFeatureValue)
					{
						foreach (int? value in ((MultipleFeatureValue)featureValue).Codes)
						{
							int v = getFeatureMapValue(i, value.Value);
							if (v != -1)
							{
								featureSet.Add(new XNode(v,1));
							}
						}
					}
				}
			}
			FeatureNode[] xarray = new FeatureNode[featureSet.Count];
			int k = 0;
			foreach (XNode x in featureSet)
			{
				xarray[k++] = new FeatureNode(x.Index, x.Value);
			}


			if (decision.KBestList.K == 1)
			{
				decision.KBestList.add(Linear.predict(model, xarray));
			}
			else
			{
				liblinear_predict_with_kbestlist(model, xarray, decision.KBestList);
			}
			featureSet.Clear();

			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			closeInstanceWriter();
			model = null;
			xlist = null;
			owner = null;
		}

		public virtual StreamWriter InstanceWriter
		{
			get
			{
				return instanceOutput;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void closeInstanceWriter() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void closeInstanceWriter()
		{
			try
			{
				if (instanceOutput != null)
				{
					instanceOutput.Flush();
					instanceOutput.Close();
					instanceOutput = null;
				}
			}
			catch (IOException e)
			{
				throw new LiblinearException("The Liblinear learner cannot close the instance file. ", e);
			}
		}


		/// <summary>
		/// Returns the parameter string for used for configure Liblinear
		/// </summary>
		/// <returns> the parameter string for used for configure Liblinear </returns>
		public virtual string ParamString
		{
			get
			{
				return paramString;
			}
		}

		public virtual InstanceModel Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
			}
		}


		public virtual int LearnerMode
		{
			get
			{
				return learnerMode;
			}
			set
			{
				learnerMode = value;
			}
		}


		public virtual string LearningMethodName
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		/// <summary>
		/// Returns the current configuration
		/// </summary>
		/// <returns> the current configuration </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.parser.DependencyParserConfig getConfiguration() throws org.maltparser.core.exception.MaltChainedException
		public virtual DependencyParserConfig Configuration
		{
			get
			{
				return owner.Guide.Configuration;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getNumberOfInstances() throws org.maltparser.core.exception.MaltChainedException
		public virtual int NumberOfInstances
		{
			get
			{
				if (numberOfInstances != 0)
				{
					return numberOfInstances;
				}
				else
				{
					//Do a line count of the instance file and return that
    
					StreamReader reader = new StreamReader(getInstanceInputStreamReader(".ins"));
					try
					{
						while (reader.ReadLine() != null)
						{
							numberOfInstances++;
							owner.increaseFrequency();
						}
						reader.Close();
					}
					catch (IOException e)
					{
						throw new MaltChainedException("No instances found in file",e);
					}
					return numberOfInstances;
				}
			}
			set
			{
				numberOfInstances = 0;
			}
		}

		public virtual void increaseNumberOfInstances()
		{
			numberOfInstances++;
			owner.increaseFrequency();
		}

		public virtual void decreaseNumberOfInstances()
		{
			numberOfInstances--;
			owner.decreaseFrequency();
		}



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.OutputStreamWriter getInstanceOutputStreamWriter(String suffix) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual StreamWriter getInstanceOutputStreamWriter(string suffix)
		{
			return Configuration.getAppendOutputStreamWriter(owner.ModelName + LearningMethodName + suffix);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.InputStreamReader getInstanceInputStreamReader(String suffix) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual StreamReader getInstanceInputStreamReader(string suffix)
		{
			return Configuration.getInputStreamReader(owner.ModelName + LearningMethodName + suffix);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.InputStreamReader getInstanceInputStreamReaderFromConfigFile(String suffix) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual StreamReader getInstanceInputStreamReaderFromConfigFile(string suffix)
		{
			try
			{
				return new StreamReader(getInputStreamFromConfigFileEntry(suffix), Encoding.UTF8);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new ConfigurationException("The char set UTF-8 is not supported. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.InputStream getInputStreamFromConfigFileEntry(String suffix) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual Stream getInputStreamFromConfigFileEntry(string suffix)
		{
			return Configuration.getInputStreamFromConfigFileEntry(owner.ModelName + LearningMethodName + suffix);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.File getFile(String suffix) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual File getFile(string suffix)
		{
			return Configuration.getFile(owner.ModelName + LearningMethodName + suffix);
		}

	//	protected JarEntry getConfigFileEntry(String suffix) throws MaltChainedException {
	//		return getConfiguration().getConfigurationDir().getConfigFileEntry(owner.getModelName()+getLearningMethodName()+suffix);
	//	}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public de.bwaldvogel.liblinear.Problem readLibLinearProblemWithFeaturePruning(java.io.InputStreamReader isr) throws org.maltparser.core.exception.MaltChainedException
		public virtual Problem readLibLinearProblemWithFeaturePruning(StreamReader isr)
		{
			Problem problem = new Problem();

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader fp = new java.io.BufferedReader(isr);
				StreamReader fp = new StreamReader(isr);

				problem.bias = -1;
				problem.l = NumberOfInstances;
				problem.x = new FeatureNode[problem.l][];
				problem.y = new int[problem.l];
				int i = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.regex.Pattern tabPattern = java.util.regex.Pattern.compile("\t");
				Pattern tabPattern = Pattern.compile("\t");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.regex.Pattern pipePattern = java.util.regex.Pattern.compile("\\|");
				Pattern pipePattern = Pattern.compile("\\|");
				while (true)
				{
					string line = fp.ReadLine();

					if (ReferenceEquals(line, null))
					{
						break;
					}
					string[] columns = tabPattern.split(line);

					if (columns.Length == 0)
					{
						continue;
					}
					int j = 0;
					try
					{
						problem.y[i] = int.Parse(columns[j]);
						for (j = 1; j < columns.Length; j++)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] items = pipePattern.split(columns[j]);
							string[] items = pipePattern.split(columns[j]);
							for (int k = 0; k < items.Length; k++)
							{
								try
								{
									int colon = items[k].IndexOf(':');
									if (colon == -1)
									{
										if (int.Parse(items[k]) != -1)
										{
											int v = addFeatureMapValue(j, int.Parse(items[k]));
											if (v != -1)
											{
												featureSet.Add(new XNode(v,1));
											}
										}
									}
									else
									{
										int index = addFeatureMapValue(j, int.Parse(items[k].Substring(0,colon)));
										double value;
										if (items[k].Substring(colon + 1).IndexOf('.') != -1)
										{
											value = double.Parse(items[k].Substring(colon + 1));
										}
										else
										{
											value = int.Parse(items[k].Substring(colon + 1));
										}
										featureSet.Add(new XNode(index,value));
									}
								}
								catch (FormatException e)
								{
									throw new LiblinearException("The instance file contain a non-integer value '" + items[k] + "'", e);
								}
							}
						}
						problem.x[i] = new FeatureNode[featureSet.Count];
						int p = 0;
						foreach (XNode x in featureSet)
						{
							problem.x[i][p++] = new FeatureNode(x.Index, x.Value);
						}
						featureSet.Clear();
						i++;
					}
					catch (IndexOutOfRangeException e)
					{
						throw new LiblinearException("Cannot read from the instance file. ", e);
					}
				}
				fp.Close();
				featureSet = null;
				problem.n = featureMap.Count;
				Console.WriteLine("Number of features: " + problem.n);
			}
			catch (IOException e)
			{
				throw new LiblinearException("Cannot read from the instance file. ", e);
			}
			return problem;
		}
		/// <summary>
		/// Reads an instance file into a problem object according to the Malt-SVM format, which is column fixed format (tab-separated).
		/// </summary>
		/// <param name="isr">	the instance stream reader for the instance file </param>
		/// <param name="cardinalities">	a array containing the number of distinct values for a particular column. </param>
		/// <exception cref="LiblinearException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public de.bwaldvogel.liblinear.Problem readLibLinearProblem(java.io.InputStreamReader isr, int[] cardinalities) throws org.maltparser.core.exception.MaltChainedException
		public virtual Problem readLibLinearProblem(StreamReader isr, int[] cardinalities)
		{
			Problem problem = new Problem();

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader fp = new java.io.BufferedReader(isr);
				StreamReader fp = new StreamReader(isr);
				int max_index = 0;
				if (xlist == null)
				{
					xlist = new List<FeatureNode>();
				}
				problem.bias = -1; //getBias();
				problem.l = NumberOfInstances;
				problem.x = new FeatureNode[problem.l][];
				problem.y = new int[problem.l];
				int i = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.regex.Pattern tabPattern = java.util.regex.Pattern.compile("\t");
				Pattern tabPattern = Pattern.compile("\t");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.regex.Pattern pipePattern = java.util.regex.Pattern.compile("\\|");
				Pattern pipePattern = Pattern.compile("\\|");
				while (true)
				{
					string line = fp.ReadLine();

					if (ReferenceEquals(line, null))
					{
						break;
					}
					string[] columns = tabPattern.split(line);

					if (columns.Length == 0)
					{
						continue;
					}

					int offset = 1;
					int j = 0;
					try
					{
						problem.y[i] = int.Parse(columns[j]);
						int p = 0;
						for (j = 1; j < columns.Length; j++)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] items = pipePattern.split(columns[j]);
							string[] items = pipePattern.split(columns[j]);
							for (int k = 0; k < items.Length; k++)
							{
								try
								{
									if (int.Parse(items[k]) != -1)
									{
										xlist.Insert(p, new FeatureNode(int.Parse(items[k]) + offset, 1));
										p++;
									}
								}
								catch (FormatException e)
								{
									throw new LiblinearException("The instance file contain a non-integer value '" + items[k] + "'", e);
								}
							}
							offset += cardinalities[j - 1];
						}
						problem.x[i] = xlist.subList(0, p).toArray(new FeatureNode[0]);
						if (columns.Length > 1)
						{
							max_index = Math.Max(max_index, problem.x[i][p - 1].index);
						}
						i++;
						xlist.Clear();
					}
					catch (IndexOutOfRangeException e)
					{
						throw new LiblinearException("Cannot read from the instance file. ", e);
					}
				}
				fp.Close();
				problem.n = max_index;
				Console.WriteLine("Number of features: " + problem.n);
	//			if ( problem.bias >= 0 ) {
	//				problem.n++;
	//			}
				xlist = null;
			}
			catch (IOException e)
			{
				throw new LiblinearException("Cannot read from the instance file. ", e);
			}
			return problem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initSpecialParameters() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void initSpecialParameters()
		{
			if (Configuration.getOptionValue("singlemalt", "null_value") != null && Configuration.getOptionValue("singlemalt", "null_value").ToString().Equals("none", StringComparison.OrdinalIgnoreCase))
			{
				excludeNullValues = true;
			}
			else
			{
				excludeNullValues = false;
			}
			saveInstanceFiles = ((bool?)Configuration.getOptionValue("liblinear", "save_instance_files")).Value;
	//		featurePruning = ((Boolean)getConfiguration().getOptionValue("liblinear", "feature_pruning")).booleanValue();
			featurePruning = true;
			if (!Configuration.getOptionValue("liblinear", "liblinear_external").ToString().Equals(""))
			{
				try
				{
					if (!Directory.Exists(Configuration.getOptionValue("liblinear", "liblinear_external").ToString()) || File.Exists(Configuration.getOptionValue("liblinear", "liblinear_external").ToString()))
					{
						throw new LiblinearException("The path to the external Liblinear trainer 'svm-train' is wrong.");
					}
					if (Directory.Exists(Configuration.getOptionValue("liblinear", "liblinear_external").ToString()))
					{
						throw new LiblinearException("The option --liblinear-liblinear_external points to a directory, the path should point at the 'train' file or the 'train.exe' file");
					}
					if (!(Configuration.getOptionValue("liblinear", "liblinear_external").ToString().EndsWith("train", StringComparison.Ordinal) || Configuration.getOptionValue("liblinear", "liblinear_external").ToString().EndsWith("train.exe", StringComparison.Ordinal)))
					{
						throw new LiblinearException("The option --liblinear-liblinear_external does not specify the path to 'train' file or the 'train.exe' file. ");
					}
					pathExternalLiblinearTrain = Configuration.getOptionValue("liblinear", "liblinear_external").ToString();
				}
				catch (SecurityException e)
				{
					throw new LiblinearException("Access denied to the file specified by the option --liblinear-liblinear_external. ", e);
				}
			}
			if (Configuration.getOptionValue("liblinear", "verbosity") != null)
			{
				verbosity = Enum.Parse(typeof(Verbostity), Configuration.getOptionValue("liblinear", "verbosity").ToString().ToUpper());
			}
		}

		public virtual string LibLinearOptions
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				foreach (string key in liblinearOptions.Keys)
				{
					sb.Append('-');
					sb.Append(key);
					sb.Append(' ');
					sb.Append(liblinearOptions.get(key));
					sb.Append(' ');
				}
				return sb.ToString();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseParameters(String paramstring) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parseParameters(string paramstring)
		{
			if (ReferenceEquals(paramstring, null))
			{
				return;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] argv;
			string[] argv;
			string allowedFlags = "sceB";
			try
			{
				argv = paramstring.Split("[_\\p{Blank}]", true);
			}
			catch (PatternSyntaxException e)
			{
				throw new LiblinearException("Could not split the liblinear-parameter string '" + paramstring + "'. ", e);
			}
			for (int i = 0; i < argv.Length - 1; i++)
			{
				if (argv[i][0] != '-')
				{
					throw new LiblinearException("The argument flag should start with the following character '-', not with " + argv[i][0]);
				}
				if (++i >= argv.Length)
				{
					throw new LiblinearException("The last argument does not have any value. ");
				}
				try
				{
					int index = allowedFlags.IndexOf(argv[i - 1][1], StringComparison.Ordinal);
					if (index != -1)
					{
						liblinearOptions.put(Convert.ToString(argv[i - 1][1]), argv[i]);
					}
					else
					{
						throw new LiblinearException("Unknown liblinear parameter: '" + argv[i - 1] + "' with value '" + argv[i] + "'. ");
					}
				}
				catch (IndexOutOfRangeException e)
				{
					throw new LiblinearException("The liblinear parameter '" + argv[i - 1] + "' could not convert the string value '" + argv[i] + "' into a correct numeric value. ", e);
				}
				catch (FormatException e)
				{
					throw new LiblinearException("The liblinear parameter '" + argv[i - 1] + "' could not convert the string value '" + argv[i] + "' into a correct numeric value. ", e);
				}
				catch (NullReferenceException e)
				{
					throw new LiblinearException("The liblinear parameter '" + argv[i - 1] + "' could not convert the string value '" + argv[i] + "' into a correct numeric value. ", e);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getBias() throws org.maltparser.core.exception.MaltChainedException
		public virtual double Bias
		{
			get
			{
				try
				{
					return Convert.ToDouble(liblinearOptions.get("B"));
				}
				catch (FormatException e)
				{
					throw new LiblinearException("The liblinear bias value is not numerical value. ", e);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public de.bwaldvogel.liblinear.Parameter getLiblinearParameters() throws org.maltparser.core.exception.MaltChainedException
		public virtual Parameter LiblinearParameters
		{
			get
			{
				Parameter param = new Parameter(SolverType.MCSVM_CS, 0.1, 0.1);
				string type = liblinearOptions.get("s");
    
				if (type.Equals("0"))
				{
					param.SolverType = SolverType.L2R_LR;
				}
				else if (type.Equals("1"))
				{
					param.SolverType = SolverType.L2R_L2LOSS_SVC_DUAL;
				}
				else if (type.Equals("2"))
				{
					param.SolverType = SolverType.L2R_L2LOSS_SVC;
				}
				else if (type.Equals("3"))
				{
					param.SolverType = SolverType.L2R_L1LOSS_SVC_DUAL;
				}
				else if (type.Equals("4"))
				{
					param.SolverType = SolverType.MCSVM_CS;
				}
				else if (type.Equals("5"))
				{
					param.SolverType = SolverType.L1R_L2LOSS_SVC;
				}
				else if (type.Equals("6"))
				{
					param.SolverType = SolverType.L1R_LR;
				}
				else
				{
					throw new LiblinearException("The liblinear type (-s) is not an integer value between 0 and 4. ");
				}
				try
				{
					param.C = Convert.ToDouble(liblinearOptions.get("c"));
				}
				catch (FormatException e)
				{
					throw new LiblinearException("The liblinear cost (-c) value is not numerical value. ", e);
				}
				try
				{
					param.Eps = Convert.ToDouble(liblinearOptions.get("e"));
				}
				catch (FormatException e)
				{
					throw new LiblinearException("The liblinear epsilon (-e) value is not numerical value. ", e);
				}
				return param;
			}
		}

		public virtual void initLiblinearOptions()
		{
			liblinearOptions.put("s", "4"); // type = SolverType.L2LOSS_SVM_DUAL (default)
			liblinearOptions.put("c", "0.1"); // cost = 1 (default)
			liblinearOptions.put("e", "0.1"); // epsilon = 0.1 (default)
			liblinearOptions.put("B", "-1"); // bias = -1 (default)
		}

		public virtual string[] LibLinearParamStringArray
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.ArrayList<String> params = new java.util.ArrayList<String>();
				List<string> @params = new List<string>();
    
				foreach (string key in liblinearOptions.Keys)
				{
					@params.Add("-" + key);
					@params.Add(liblinearOptions.get(key));
				}
				return @params.ToArray();
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void liblinear_predict_with_kbestlist(de.bwaldvogel.liblinear.Model model, FeatureNode[] x, org.maltparser.parser.history.kbest.KBestList kBestList) throws org.maltparser.core.exception.MaltChainedException
		public virtual void liblinear_predict_with_kbestlist(Model model, FeatureNode[] x, KBestList kBestList)
		{
			int i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nr_class = model.getNrClass();
			int nr_class = model.NrClass;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dec_values = new double[nr_class];
			double[] dec_values = new double[nr_class];

			Linear.predictValues(model, x, dec_values);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] labels = model.getLabels();
			int[] labels = model.Labels;
			int[] predictionList = new int[nr_class];
			for (i = 0;i < nr_class;i++)
			{
				predictionList[i] = labels[i];
			}

			double tmpDec;
			int tmpObj;
			int lagest;
			for (i = 0;i < nr_class - 1;i++)
			{
				lagest = i;
				for (int j = i;j < nr_class;j++)
				{
					if (dec_values[j] > dec_values[lagest])
					{
						lagest = j;
					}
				}
				tmpDec = dec_values[lagest];
				dec_values[lagest] = dec_values[i];
				dec_values[i] = tmpDec;
				tmpObj = predictionList[lagest];
				predictionList[lagest] = predictionList[i];
				predictionList[i] = tmpObj;
			}

			int k = nr_class - 1;
			if (kBestList.K != -1)
			{
				k = kBestList.K - 1;
			}

			for (i = 0; i < nr_class && k >= 0; i++, k--)
			{
				if (kBestList is ScoredKBestList)
				{
					((ScoredKBestList)kBestList).add(predictionList[i], (float)dec_values[i]);
				}
				else
				{
					kBestList.add(predictionList[i]);
				}

			}
		}

		/// <summary>
		/// Converts the instance file (Malt's own SVM format) into the Liblinear (SVMLight) format. The input instance file is removed (replaced)
		/// by the instance file in the Liblinear (SVMLight) format. If a column contains -1, the value will be removed in destination file. 
		/// </summary>
		/// <param name="isr"> the input stream reader for the source instance file </param>
		/// <param name="osw">	the output stream writer for the destination instance file </param>
		/// <param name="cardinalities"> a vector containing the number of distinct values for a particular column </param>
		/// <exception cref="LiblinearException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void maltSVMFormat2OriginalSVMFormat(java.io.InputStreamReader isr, java.io.OutputStreamWriter osw, int[] cardinalities) throws org.maltparser.core.exception.MaltChainedException
		public static void maltSVMFormat2OriginalSVMFormat(StreamReader isr, StreamWriter osw, int[] cardinalities)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader in = new java.io.BufferedReader(isr);
				StreamReader @in = new StreamReader(isr);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedWriter out = new java.io.BufferedWriter(osw);
				StreamWriter @out = new StreamWriter(osw);

				int c;
				int j = 0;
				int offset = 1;
				int code = 0;
				while (true)
				{
					c = @in.Read();
					if (c == -1)
					{
						break;
					}

					if (c == '\t' || c == '|')
					{
						if (j == 0)
						{
							@out.Write(Convert.ToString(code));
							j++;
						}
						else
						{
							if (code != -1)
							{
								@out.BaseStream.WriteByte(' ');
								@out.Write(Convert.ToString(code + offset));
								@out.Write(":1");
							}
							if (c == '\t')
							{
								offset += cardinalities[j - 1];
								j++;
							}
						}
						code = 0;
					}
					else if (c == '\n')
					{
						j = 0;
						offset = 1;
						@out.BaseStream.WriteByte('\n');
						code = 0;
					}
					else if (c == '-')
					{
						code = -1;
					}
					else if (code != -1)
					{
						if (c > 47 && c < 58)
						{
							code = code * 10 + (c - 48);
						}
						else
						{
							throw new LiblinearException("The instance file contain a non-integer value, when converting the Malt SVM format into Liblinear format.");
						}
					}
				}
				@in.Close();
				@out.Close();
			}
			catch (IOException e)
			{
				throw new LiblinearException("Cannot read from the instance file, when converting the Malt SVM format into Liblinear format. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws Throwable
		~Liblinear()
		{
			try
			{
				closeInstanceWriter();
			}
			finally
			{
//JAVA TO C# CONVERTER NOTE: The base class finalizer method is automatically called in C#:
//				base.finalize();
			}
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer sb = new StringBuffer();
			StringBuilder sb = new StringBuilder();
			sb.Append("\nLiblinear INTERFACE\n");
			sb.Append("  Liblinear version: " + LIBLINEAR_VERSION + "\n");
			sb.Append("  Liblinear string: " + paramString + "\n");

			sb.Append(LibLinearOptions);
			return sb.ToString();
		}
	}

}