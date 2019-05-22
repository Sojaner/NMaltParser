using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.ml.libsvm
{


	using Configuration = org.maltparser.core.config.Configuration;
	using ConfigurationException = org.maltparser.core.config.ConfigurationException;
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureVector = org.maltparser.core.feature.FeatureVector;
	using FeatureFunction = org.maltparser.core.feature.function.FeatureFunction;
	using FeatureValue = org.maltparser.core.feature.value.FeatureValue;
	using MultipleFeatureValue = org.maltparser.core.feature.value.MultipleFeatureValue;
	using SingleFeatureValue = org.maltparser.core.feature.value.SingleFeatureValue;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using LiblinearException = org.maltparser.ml.liblinear.LiblinearException;
	using DependencyParserConfig = org.maltparser.parser.DependencyParserConfig;
	using InstanceModel = org.maltparser.parser.guide.instance.InstanceModel;
	using SingleDecision = org.maltparser.parser.history.action.SingleDecision;
	using KBestList = org.maltparser.parser.history.kbest.KBestList;
	using ScoredKBestList = org.maltparser.parser.history.kbest.ScoredKBestList;

	/// <summary>
	/// Implements an interface to the LIBSVM learner (currently the LIBSVM 2.91 is used). More information
	/// about LIBSVM can be found at 
	/// <a href="http://www.csie.ntu.edu.tw/~cjlin/libsvm/" target="_blank">LIBSVM -- A Library for Support Vector Machines</a>.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// </summary>
	public class Libsvm : LearningMethod
	{
		public const string LIBSVM_VERSION = "2.91";
		public enum Verbostity
		{
			SILENT,
			ERROR,
			ALL
		}
		protected internal InstanceModel owner;
		protected internal int learnerMode;
		protected internal string name;
		protected internal int numberOfInstances;
		protected internal bool saveInstanceFiles;
		protected internal bool excludeNullValues;
		protected internal string pathExternalSVMTrain = null;
	//	private int[] cardinalities;

		/// <summary>
		/// Instance output stream writer 
		/// </summary>
		private StreamWriter instanceOutput = null;
		/// <summary>
		/// LIBSVM svm_model object, only used during classification.
		/// </summary>
		private svm_model model = null;

		/// <summary>
		/// LIBSVM svm_parameter object
		/// </summary>
		private svm_parameter svmParam;
		/// <summary>
		/// Parameter string
		/// </summary>
		private string paramString;
		/// <summary>
		/// An array of LIBSVM svm_node objects, only used during classification.
		/// </summary>
		private List<svm_node> xlist = null;

		private Verbostity verbosity;
		/// <summary>
		/// Constructs a LIBSVM learner.
		/// </summary>
		/// <param name="owner"> the guide model owner </param>
		/// <param name="learnerMode"> the mode of the learner TRAIN or CLASSIFY </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Libsvm(org.maltparser.parser.guide.instance.InstanceModel owner, System.Nullable<int> learnerMode) throws org.maltparser.core.exception.MaltChainedException
		public Libsvm(InstanceModel owner, int? learnerMode)
		{
			Owner = owner;
			LearningMethodName = "libsvm";
			LearnerMode = learnerMode.Value;
			NumberOfInstances = 0;
			verbosity = Verbostity.SILENT;
			initSvmParam(Configuration.getOptionValue("libsvm", "libsvm_options").ToString());
			initSpecialParameters();
			if (learnerMode.Value == org.maltparser.ml.LearningMethod_Fields.BATCH)
			{
	//			if (owner.getGuide().getConfiguration().getConfigLogger().isInfoEnabled()) {
	//				if (pathExternalSVMTrain != null) {
	//					owner.getGuide().getConfiguration().getConfigLogger().info("  Learner              : LIBSVM external "+ getParamString() + "\n");
	//				} else {
	//					owner.getGuide().getConfiguration().getConfigLogger().info("  Learner              : LIBSVM "+LIBSVM_VERSION+" "+ getParamString() + "\n");
	//				}
	//			}
				instanceOutput = new StreamWriter(getInstanceOutputStreamWriter(".ins"));
			}
	//		else {
	//			if (owner.getGuide().getConfiguration().getConfigLogger().isInfoEnabled()) {
	//				owner.getGuide().getConfiguration().getConfigLogger().info("  Classifier           : LIBSVM "+LIBSVM_VERSION+" "+ getParamString()+ "\n");
	//			}
	//		}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.parser.history.action.SingleDecision decision, org.maltparser.core.feature.FeatureVector featureVector) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(SingleDecision decision, FeatureVector featureVector)
		{
			if (featureVector == null)
			{
				throw new LibsvmException("The feature vector cannot be found");
			}
			else if (decision == null)
			{
				throw new LibsvmException("The decision cannot be found");
			}
			try
			{
				instanceOutput.Write(decision.DecisionCode + "\t");
				for (int i = 0; i < featureVector.Count; i++)
				{
					FeatureValue featureValue = featureVector[i].FeatureValue;
					if (excludeNullValues == true && featureValue.NullValue)
					{
						instanceOutput.Write("-1");
					}
					else
					{
						if (featureValue is SingleFeatureValue)
						{
							instanceOutput.Write(((SingleFeatureValue)featureValue).IndexCode + "");
						}
						else if (featureValue is MultipleFeatureValue)
						{
							ISet<int> values = ((MultipleFeatureValue)featureValue).Codes;
							int j = 0;
							foreach (int? value in values)
							{
								instanceOutput.Write(value.ToString());
								if (j != values.Count - 1)
								{
									instanceOutput.Write("|");
								}
								j++;
							}
						}
					}
					if (i != featureVector.Count)
					{
						instanceOutput.BaseStream.WriteByte('\t');
					}
				}

				instanceOutput.BaseStream.WriteByte('\n');
				instanceOutput.Flush();
				increaseNumberOfInstances();
			}
			catch (IOException e)
			{
				throw new LibsvmException("The LIBSVM learner cannot write to the instance file. ", e);
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
				throw new LibsvmException("The parent guide model cannot be found. ");
			}
	//		cardinalities = getCardinalities(featureVector);
	//		if (pathExternalSVMTrain == null) {
	//			try {
	//				final svm_problem prob = readProblemMaltSVMFormat(getInstanceInputStreamReader(".ins"), cardinalities, svmParam);
	//				if(svm.svm_check_parameter(prob, svmParam) != null) {
	//					throw new LibsvmException(svm.svm_check_parameter(prob, svmParam));
	//				}
	//				owner.getGuide().getConfiguration().getConfigLogger().info("Creating LIBSVM model "+getFile(".mod").getName()+"\n");
	//				final PrintStream out = System.out;
	//				final PrintStream err = System.err;
	//				System.setOut(NoPrintStream.NO_PRINTSTREAM);
	//				System.setErr(NoPrintStream.NO_PRINTSTREAM);
	//				
	//				svm.svm_save_model(getFile(".mod").getAbsolutePath(), svm.svm_train(prob, svmParam));
	//				System.setOut(err);
	//				System.setOut(out);
	//				if (!saveInstanceFiles) {
	//					getFile(".ins").delete();
	//				}
	//			} catch (OutOfMemoryError e) {
	//				throw new LibsvmException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
	//			} catch (IllegalArgumentException e) {
	//				throw new LibsvmException("The LIBSVM learner was not able to redirect Standard Error stream. ", e);
	//			} catch (SecurityException e) {
	//				throw new LibsvmException("The LIBSVM learner cannot remove the instance file. ", e);
	//			} catch (IOException e) {
	//				throw new LibsvmException("The LIBSVM learner cannot save the model file '"+getFile(".mod").getAbsolutePath()+"'. ", e);
	//			}
	//		} else {
	//			trainExternal(featureVector);
	//		}
	//		saveCardinalities(getInstanceOutputStreamWriter(".car"), cardinalities);
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
					config.logInfoMessage("Creating LIBSVM model (svm-train) " + getFile(".mod").Name);
				}


//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<String> commands = new java.util.ArrayList<String>();
				List<string> commands = new List<string>();
				commands.Add(pathExternalSVMTrain);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] params = getSVMParamStringArray(svmParam);
				string[] @params = getSVMParamStringArray(svmParam);
				for (int i = 0; i < @params.Length; i++)
				{
					commands.Add(@params[i]);
				}
				commands.Add(getFile(".ins.tmp").AbsolutePath);
				commands.Add(getFile(".mod").AbsolutePath);
				string[] arrayCommands = commands.ToArray();

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
				 throw new LibsvmException("SVM-trainer is interrupted. ", e);
			}
			catch (System.ArgumentException e)
			{
				throw new LibsvmException("The LIBSVM learner was not able to redirect Standard Error stream. ", e);
			}
			catch (SecurityException e)
			{
				throw new LibsvmException("The LIBSVM learner cannot remove the instance file. ", e);
			}
			catch (IOException e)
			{
				throw new LibsvmException("The LIBSVM learner cannot save the model file '" + getFile(".mod").AbsolutePath + "'. ", e);
			}
			catch (System.OutOfMemoryException e)
			{
				throw new LibsvmException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
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
	//			throw new LibsvmException("Couldn't save the cardinalities to file. ", e);
	//		}
	//	}
	//	
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
	//			throw new LibsvmException("The cardinalities cannot be read because wrongly formatted. ", e);
	//		} catch (NumberFormatException e) {
	//			throw new LibsvmException("Couldn't load the cardinalities from file. ", e);
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
				throw new LibsvmException("The learning method cannot be found. ");
			}
			else if (divideFeature == null)
			{
				throw new LibsvmException("The divide feature cannot be found. ");
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
						if (sb.Length > 0)
						{
							@out.Write(sb.ToString());
						}
						if (divideFeatureIndexVector.Contains(j - 1))
						{
							if (sb.Length > 0)
							{
								@out.BaseStream.WriteByte('\t');
							}
							@out.Write(Convert.ToString(((SingleFeatureValue)divideFeature.FeatureValue).IndexCode));
						}
						@out.BaseStream.WriteByte('\n');
						sb.Length = 0;
						method.increaseNumberOfInstances();
						this.decreaseNumberOfInstances();
						j = 0;
					}
					else
					{
						sb.Append(c);
					}
				}
				@in.Close();
				getFile(".ins").delete();
			}
			catch (SecurityException e)
			{
				throw new LibsvmException("The LIBSVM learner cannot remove the instance file. ", e);
			}
			catch (System.NullReferenceException e)
			{
				throw new LibsvmException("The instance file cannot be found. ", e);
			}
			catch (FileNotFoundException e)
			{
				throw new LibsvmException("The instance file cannot be found. ", e);
			}
			catch (IOException e)
			{
				throw new LibsvmException("The LIBSVM learner read from the instance file. ", e);
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
					model = svm.svm_load_model(new StreamReader(getInstanceInputStreamReaderFromConfigFile(".mod")));
				}
				catch (IOException e)
				{
					throw new LibsvmException("The model cannot be loaded. ", e);
				}
			}
	//		if (cardinalities == null) {
	//			if (getConfigFileEntry(".car") != null) {
	//				cardinalities = loadCardinalities(getInstanceInputStreamReaderFromConfigFile(".car"));
	//			} else {
	//				cardinalities = getCardinalities(featureVector);
	//			}
	//		}
			if (xlist == null)
			{
				xlist = new List<svm_node>(featureVector.Count);
			}
			if (model == null)
			{
				throw new LibsvmException("The LIBSVM learner cannot predict the next class, because the learning model cannot be found. ");
			}
			else if (featureVector == null)
			{
				throw new LibsvmException("The LIBSVM learner cannot predict the next class, because the feature vector cannot be found. ");
			}
	//		int j = 0;
	//		int offset = 0;
	//		int i = 0;
	//		for (FeatureFunction feature : featureVector) {
	//			final FeatureValue featureValue = feature.getFeatureValue();
	//			if (!(excludeNullValues == true && featureValue.isNullValue())) {
	//				if (featureValue instanceof SingleFeatureValue) {
	//					if (((SingleFeatureValue)featureValue).getCode() < cardinalities[i]) {
	//						if (j >= xlist.size()) {
	//							svm_node x =  new svm_node();
	//							x.value = 1;
	//							xlist.add(j,x);
	//						}
	//						xlist.get(j++).index = ((SingleFeatureValue)featureValue).getCode() + offset;
	//					}
	//				} else if (featureValue instanceof MultipleFeatureValue) {
	//					for (Integer value : ((MultipleFeatureValue)featureValue).getCodes()) {
	//						if (value < cardinalities[i]) {
	////						if (((MultipleFeatureValue)featureValue).isKnown(value)) {
	//							if (j >= xlist.size()) {
	//								svm_node x =  new svm_node();
	//								x.value = 1;
	//								xlist.add(j,x);
	//							}
	//							xlist.get(j++).index = value + offset;
	//						}
	//					}
	//				}
	//			}
	//			offset += cardinalities[i];
	//			i++;
	//		}
	//
	//		svm_node[] xarray = new svm_node[j];
	//		for (int k = 0; k < j; k++) {
	//			xarray[k] = xlist.get(k);
	//		}
	//		try {
	//			if (decision.getKBestList().getK() == 1 || svm.svm_get_svm_type(model) == svm_parameter.ONE_CLASS ||
	//					svm.svm_get_svm_type(model) == svm_parameter.EPSILON_SVR ||
	//					svm.svm_get_svm_type(model) == svm_parameter.NU_SVR) {
	//				decision.getKBestList().add((int)svm.svm_predict(model, xarray));
	//			} else {
	//				svm_predict_with_kbestlist(model, xarray, decision.getKBestList());
	//			}
	//
	//		} catch (OutOfMemoryError e) {
	//				throw new LibsvmException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
	//		}

			return true;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			closeInstanceWriter();
			model = null;
			svmParam = null;
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
				throw new LibsvmException("The LIBSVM learner cannot close the instance file. ", e);
			}
		}

		/// <summary>
		/// Initialize the LIBSVM according to the parameter string
		/// </summary>
		/// <param name="paramString"> the parameter string to configure the LIBSVM learner. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initSvmParam(String paramString) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void initSvmParam(string paramString)
		{
			this.paramString = paramString;
			svmParam = new svm_parameter();
			initParameters(svmParam);
			parseParameters(paramString, svmParam);
		}

		/// <summary>
		/// Returns the parameter string for used for configure LIBSVM
		/// </summary>
		/// <returns> the parameter string for used for configure LIBSVM </returns>
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
				this.owner = value;
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
				this.learnerMode = value;
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
				this.name = value;
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
				this.numberOfInstances = 0;
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
	//		return getConfiguration().getInputStreamReaderFromConfigFile(owner.getModelName()+getLearningMethodName()+suffix);
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

		/// <summary>
		/// Reads an instance file into a svm_problem object according to the Malt-SVM format, which is column fixed format (tab-separated).
		/// </summary>
		/// <param name="isr">	the instance stream reader for the instance file </param>
		/// <param name="cardinalities">	a array containing the number of distinct values for a particular column. </param>
		/// <param name="param">	a svm_parameter object </param>
		/// <exception cref="LibsvmException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final libsvm.svm_problem readProblemMaltSVMFormat(java.io.InputStreamReader isr, int[] cardinalities, libsvm.svm_parameter param) throws org.maltparser.core.exception.MaltChainedException
		public svm_problem readProblemMaltSVMFormat(StreamReader isr, int[] cardinalities, svm_parameter param)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final libsvm.svm_problem prob = new libsvm.svm_problem();
			svm_problem prob = new svm_problem();
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader fp = new java.io.BufferedReader(isr);
				StreamReader fp = new StreamReader(isr);
				int max_index = 0;
				if (xlist == null)
				{
					xlist = new List<svm_node>();
				}
				prob.l = NumberOfInstances;
				prob.x = new svm_node[prob.l][];
				prob.y = new double[prob.l];
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
					if (string.ReferenceEquals(line, null))
					{
						break;
					}
					string[] columns = tabPattern.split(line);

					if (columns.Length == 0)
					{
						continue;
					}

					int offset = 0;
					int j = 0;
					try
					{
						prob.y[i] = (double)int.Parse(columns[j]);
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
										xlist.Insert(p, new svm_node());
										xlist[p].value = 1;
										xlist[p].index = int.Parse(items[k]) + offset;
										p++;
									}
								}
								catch (System.FormatException e)
								{
									throw new LibsvmException("The instance file contain a non-integer value '" + items[k] + "'", e);
								}
							}
							offset += cardinalities[j - 1];
						}
						prob.x[i] = xlist.subList(0, p).toArray(new svm_node[0]);
						if (columns.Length > 1)
						{
							max_index = Math.Max(max_index, xlist[p - 1].index);
						}
						i++;
						xlist.Clear();
					}
					catch (System.IndexOutOfRangeException e)
					{
						throw new LibsvmException("Cannot read from the instance file. ", e);
					}
				}
				fp.Close();
				if (param.gamma == 0)
				{
					param.gamma = 1.0 / max_index;
				}
				xlist = null;
			}
			catch (IOException e)
			{
				throw new LibsvmException("Cannot read from the instance file. ", e);
			}
			return prob;
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
			saveInstanceFiles = ((bool?)Configuration.getOptionValue("libsvm", "save_instance_files")).Value;

			if (!Configuration.getOptionValue("libsvm", "libsvm_external").ToString().Equals(""))
			{
				try
				{
					if (!Directory.Exists(Configuration.getOptionValue("libsvm", "libsvm_external").ToString()) || File.Exists(Configuration.getOptionValue("libsvm", "libsvm_external").ToString()))
					{
						throw new LibsvmException("The path to the external LIBSVM trainer 'svm-train' is wrong.");
					}
					if (Directory.Exists(Configuration.getOptionValue("libsvm", "libsvm_external").ToString()))
					{
						throw new LibsvmException("The option --libsvm-libsvm_external points to a directory, the path should point at the 'svm-train' file or the 'svm-train.exe' file");
					}
					if (!(Configuration.getOptionValue("libsvm", "libsvm_external").ToString().EndsWith("svm-train", StringComparison.Ordinal) || Configuration.getOptionValue("libsvm", "libsvm_external").ToString().EndsWith("svm-train.exe", StringComparison.Ordinal)))
					{
						throw new LibsvmException("The option --libsvm-libsvm_external does not specify the path to 'svm-train' file or the 'svm-train.exe' file. ");
					}
					pathExternalSVMTrain = Configuration.getOptionValue("libsvm", "libsvm_external").ToString();
				}
				catch (SecurityException e)
				{
					throw new LibsvmException("Access denied to the file specified by the option --libsvm-libsvm_external. ", e);
				}
			}
			if (Configuration.getOptionValue("libsvm", "verbosity") != null)
			{
				verbosity = Enum.Parse(typeof(Verbostity), Configuration.getOptionValue("libsvm", "verbosity").ToString().ToUpper());
			}
		}

		/// <summary>
		/// Assign a default value to all svm parameters
		/// </summary>
		/// <param name="param">	a svm_parameter object </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initParameters(libsvm.svm_parameter param) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void initParameters(svm_parameter param)
		{
			if (param == null)
			{
				throw new LibsvmException("Svm-parameters cannot be found. ");
			}
			param.svm_type = svm_parameter.C_SVC;
			param.kernel_type = svm_parameter.POLY;
			param.degree = 2;
			param.gamma = 0.2; // 1/k
			param.coef0 = 0;
			param.nu = 0.5;
			param.cache_size = 100;
			param.C = 1;
			param.eps = 1.0;
			param.p = 0.1;
			param.shrinking = 1;
			param.probability = 0;
			param.nr_weight = 0;
			param.weight_label = new int[0];
			param.weight = new double[0];
		}

		/// <summary>
		/// Returns a string containing all svm-parameters of interest
		/// </summary>
		/// <param name="param"> a svm_parameter object </param>
		/// <returns> a string containing all svm-parameters of interest </returns>
		public virtual string toStringParameters(svm_parameter param)
		{
			if (param == null)
			{
				throw new System.ArgumentException("Svm-parameters cannot be found. ");
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer sb = new StringBuffer();
			StringBuilder sb = new StringBuilder();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] svmtypes = {"C_SVC", "NU_SVC","ONE_CLASS","EPSILON_SVR","NU_SVR"};
			string[] svmtypes = new string[] {"C_SVC", "NU_SVC", "ONE_CLASS", "EPSILON_SVR", "NU_SVR"};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] kerneltypes = {"LINEAR", "POLY","RBF","SIGMOID","PRECOMPUTED"};
			string[] kerneltypes = new string[] {"LINEAR", "POLY", "RBF", "SIGMOID", "PRECOMPUTED"};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.DecimalFormat dform = new java.text.DecimalFormat("#0.0#");
			DecimalFormat dform = new DecimalFormat("#0.0#");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.DecimalFormatSymbols sym = new java.text.DecimalFormatSymbols();
			DecimalFormatSymbols sym = new DecimalFormatSymbols();
			sym.DecimalSeparator = '.';
			dform.DecimalFormatSymbols = sym;
			sb.Append("LIBSVM SETTINGS\n");
			sb.Append("  SVM type      : " + svmtypes[param.svm_type] + " (" + param.svm_type + ")\n");
			sb.Append("  Kernel        : " + kerneltypes[param.kernel_type] + " (" + param.kernel_type + ")\n");
			if (param.kernel_type == svm_parameter.POLY)
			{
				sb.Append("  Degree        : " + param.degree + "\n");
			}
			if (param.kernel_type == svm_parameter.POLY || param.kernel_type == svm_parameter.RBF || param.kernel_type == svm_parameter.SIGMOID)
			{
				sb.Append("  Gamma         : " + dform.format(param.gamma) + "\n");
				if (param.kernel_type == svm_parameter.POLY || param.kernel_type == svm_parameter.SIGMOID)
				{
					sb.Append("  Coef0         : " + dform.format(param.coef0) + "\n");
				}
			}
			if (param.svm_type == svm_parameter.NU_SVC || param.svm_type == svm_parameter.NU_SVR || param.svm_type == svm_parameter.ONE_CLASS)
			{
				sb.Append("  Nu            : " + dform.format(param.nu) + "\n");
			}
			sb.Append("  Cache Size    : " + dform.format(param.cache_size) + " MB\n");
			if (param.svm_type == svm_parameter.C_SVC || param.svm_type == svm_parameter.NU_SVR || param.svm_type == svm_parameter.EPSILON_SVR)
			{
				sb.Append("  C             : " + dform.format(param.C) + "\n");
			}
			sb.Append("  Eps           : " + dform.format(param.eps) + "\n");
			if (param.svm_type == svm_parameter.EPSILON_SVR)
			{
				sb.Append("  P             : " + dform.format(param.p) + "\n");
			}
			sb.Append("  Shrinking     : " + param.shrinking + "\n");
			sb.Append("  Probability   : " + param.probability + "\n");
			if (param.svm_type == svm_parameter.C_SVC)
			{
				sb.Append("  #Weight       : " + param.nr_weight + "\n");
				if (param.nr_weight > 0)
				{
					sb.Append("  Weight labels : ");
					for (int i = 0; i < param.nr_weight; i++)
					{
						sb.Append(param.weight_label[i]);
						if (i != param.nr_weight - 1)
						{
							sb.Append(", ");
						}
					}
					sb.Append("\n");
					for (int i = 0; i < param.nr_weight; i++)
					{
						sb.Append(dform.format(param.weight));
						if (i != param.nr_weight - 1)
						{
							sb.Append(", ");
						}
					}
					sb.Append("\n");
				}
			}
			return sb.ToString();
		}

		public virtual string[] getSVMParamStringArray(svm_parameter param)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<String> params = new java.util.ArrayList<String>();
			List<string> @params = new List<string>();

			if (param.svm_type != 0)
			{
				@params.Add("-s");
				@params.Add((new int?(param.svm_type)).ToString());
			}
			if (param.kernel_type != 2)
			{
				@params.Add("-t");
				@params.Add((new int?(param.kernel_type)).ToString());
			}
			if (param.degree != 3)
			{
				@params.Add("-d");
				@params.Add((new int?(param.degree)).ToString());
			}
			@params.Add("-g");
			@params.Add((new double?(param.gamma)).ToString());
			if (param.coef0 != 0)
			{
				@params.Add("-r");
				@params.Add((new double?(param.coef0)).ToString());
			}
			if (param.nu != 0.5)
			{
				@params.Add("-n");
				@params.Add((new double?(param.nu)).ToString());
			}
			if (param.cache_size != 100)
			{
				@params.Add("-m");
				@params.Add((new double?(param.cache_size)).ToString());
			}
			if (param.C != 1)
			{
				@params.Add("-c");
				@params.Add((new double?(param.C)).ToString());
			}
			if (param.eps != 0.001)
			{
				@params.Add("-e");
				@params.Add((new double?(param.eps)).ToString());
			}
			if (param.p != 0.1)
			{
				@params.Add("-p");
				@params.Add((new double?(param.p)).ToString());
			}
			if (param.shrinking != 1)
			{
				@params.Add("-h");
				@params.Add((new int?(param.shrinking)).ToString());
			}
			if (param.probability != 0)
			{
				@params.Add("-b");
				@params.Add((new int?(param.probability)).ToString());
			}

			return @params.ToArray();
		}

		/// <summary>
		/// Parses the parameter string. The parameter string must contain parameter and value pairs, which are separated by a blank 
		/// or a underscore. The parameter begins with a character '-' followed by a one-character flag and the value must comply with
		/// the parameters data type. Some examples:
		/// 
		/// -s 0 -t 1 -d 2 -g 0.4 -e 0.1
		/// -s_0_-t_1_-d_2_-g_0.4_-e_0.1
		/// </summary>
		/// <param name="paramstring">	the parameter string </param>
		/// <param name="param">	a svm_parameter object </param>
		/// <exception cref="LibsvmException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseParameters(String paramstring, libsvm.svm_parameter param) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parseParameters(string paramstring, svm_parameter param)
		{
			if (param == null)
			{
				throw new LibsvmException("Svm-parameters cannot be found. ");
			}
			if (string.ReferenceEquals(paramstring, null))
			{
				return;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] argv;
			string[] argv;
			try
			{
				argv = paramstring.Split("[_\\p{Blank}]", true);
			}
			catch (PatternSyntaxException e)
			{
				throw new LibsvmException("Could not split the svm-parameter string '" + paramstring + "'. ", e);
			}
			for (int i = 0; i < argv.Length - 1; i++)
			{
				if (argv[i][0] != '-')
				{
					throw new LibsvmException("The argument flag should start with the following character '-', not with " + argv[i][0]);
				}
				if (++i >= argv.Length)
				{
					throw new LibsvmException("The last argument does not have any value. ");
				}
				try
				{
					switch (argv[i - 1][1])
					{
					case 's':
						param.svm_type = int.Parse(argv[i]);
						break;
					case 't':
						param.kernel_type = int.Parse(argv[i]);
						break;
					case 'd':
						param.degree = int.Parse(argv[i]);
						break;
					case 'g':
						param.gamma = Convert.ToDouble(argv[i]);
						break;
					case 'r':
						param.coef0 = Convert.ToDouble(argv[i]);
						break;
					case 'n':
						param.nu = Convert.ToDouble(argv[i]);
						break;
					case 'm':
						param.cache_size = Convert.ToDouble(argv[i]);
						break;
					case 'c':
						param.C = Convert.ToDouble(argv[i]);
						break;
					case 'e':
						param.eps = Convert.ToDouble(argv[i]);
						break;
					case 'p':
						param.p = Convert.ToDouble(argv[i]);
						break;
					case 'h':
						param.shrinking = int.Parse(argv[i]);
						break;
					case 'b':
						param.probability = int.Parse(argv[i]);
						break;
					case 'w':
						++param.nr_weight;
						{
							int[] old = param.weight_label;
							param.weight_label = new int[param.nr_weight];
							Array.Copy(old,0,param.weight_label,0,param.nr_weight - 1);
						}

						{
							double[] old = param.weight;
							param.weight = new double[param.nr_weight];
							Array.Copy(old,0,param.weight,0,param.nr_weight - 1);
						}

						param.weight_label[param.nr_weight - 1] = int.Parse(argv[i].Substring(2));
						param.weight[param.nr_weight - 1] = Convert.ToDouble(argv[i]);
						break;
					case 'Y':
					case 'V':
					case 'S':
					case 'F':
					case 'T':
					case 'M':
					case 'N':
						break;
					default:
						throw new LibsvmException("Unknown svm parameter: '" + argv[i - 1] + "' with value '" + argv[i] + "'. ");
					}
				}
				catch (System.IndexOutOfRangeException e)
				{
					throw new LibsvmException("The svm-parameter '" + argv[i - 1] + "' could not convert the string value '" + argv[i] + "' into a correct numeric value. ", e);
				}
				catch (System.FormatException e)
				{
					throw new LibsvmException("The svm-parameter '" + argv[i - 1] + "' could not convert the string value '" + argv[i] + "' into a correct numeric value. ", e);
				}
				catch (System.NullReferenceException e)
				{
					throw new LibsvmException("The svm-parameter '" + argv[i - 1] + "' could not convert the string value '" + argv[i] + "' into a correct numeric value. ", e);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void svm_predict_with_kbestlist(libsvm.svm_model model, svm_node[] x, org.maltparser.parser.history.kbest.KBestList kBestList) throws org.maltparser.core.exception.MaltChainedException
		public virtual void svm_predict_with_kbestlist(svm_model model, svm_node[] x, KBestList kBestList)
		{
			int i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nr_class = libsvm.svm.svm_get_nr_class(model);
			int nr_class = svm.svm_get_nr_class(model);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dec_values = new double[nr_class*(nr_class-1)/2];
			double[] dec_values = new double[nr_class * (nr_class - 1) / 2];
			svm.svm_predict_values(model, x, dec_values);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] vote = new int[nr_class];
			int[] vote = new int[nr_class];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] score = new double[nr_class];
			double[] score = new double[nr_class];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] voteindex = new int[nr_class];
			int[] voteindex = new int[nr_class];
			for (i = 0;i < nr_class;i++)
			{
				vote[i] = 0;
				score[i] = 0.0;
				voteindex[i] = i;
			}
			int pos = 0;
			for (i = 0;i < nr_class;i++)
			{
				for (int j = i + 1;j < nr_class;j++)
				{
					if (dec_values[pos] > 0)
					{
						vote[i]++;
					}
					else
					{
						vote[j]++;
					}
					score[i] += dec_values[pos];
					score[j] += dec_values[pos];
					pos++;
				}
			}
			for (i = 0;i < nr_class;i++)
			{
				score[i] = score[i] / nr_class;
			}
			int lagest, tmpint;
			double tmpdouble;
			for (i = 0;i < nr_class - 1;i++)
			{
				lagest = i;
				for (int j = i;j < nr_class;j++)
				{
					if (vote[j] > vote[lagest])
					{
						lagest = j;
					}
				}
				tmpint = vote[lagest];
				vote[lagest] = vote[i];
				vote[i] = tmpint;
				tmpdouble = score[lagest];
				score[lagest] = score[i];
				score[i] = tmpdouble;
				tmpint = voteindex[lagest];
				voteindex[lagest] = voteindex[i];
				voteindex[i] = tmpint;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] labels = new int[nr_class];
			int[] labels = new int[nr_class];
			svm.svm_get_labels(model, labels);
			int k = nr_class - 1;
			if (kBestList.K != -1)
			{
				k = kBestList.K - 1;
			}

			for (i = 0; i < nr_class && k >= 0; i++, k--)
			{
				if (vote[i] > 0 || i == 0)
				{
					if (kBestList is ScoredKBestList)
					{
						((ScoredKBestList)kBestList).add(labels[voteindex[i]], (float)vote[i] / (float)(nr_class * (nr_class - 1) / 2));
					}
					else
					{
						kBestList.add(labels[voteindex[i]]);
					}
				}
			}
		}
		/// <summary>
		/// Converts the instance file (Malt's own SVM format) into the LIBSVM (SVMLight) format. The input instance file is removed (replaced)
		/// by the instance file in the LIBSVM (SVMLight) format. If a column contains -1, the value will be removed in destination file. 
		/// </summary>
		/// <param name="isr"> the input stream reader for the source instance file </param>
		/// <param name="osw">	the output stream writer for the destination instance file </param>
		/// <param name="cardinalities"> a vector containing the number of distinct values for a particular column </param>
		/// <exception cref="LibsvmException"> </exception>
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
				int offset = 0;
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
						offset = 0;
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
							throw new LibsvmException("The instance file contain a non-integer value, when converting the Malt SVM format into LIBSVM format.");
						}
					}
				}
				@in.Close();
				@out.Close();
			}
			catch (IOException e)
			{
				throw new LibsvmException("Cannot read from the instance file, when converting the Malt SVM format into LIBSVM format. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws Throwable
		~Libsvm()
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
			sb.Append("\nLIBSVM INTERFACE\n");
			sb.Append("  LIBSVM version: " + LIBSVM_VERSION + "\n");
			sb.Append("  SVM-param string: " + paramString + "\n");

			sb.Append(toStringParameters(svmParam));
			return sb.ToString();
		}
	}

}