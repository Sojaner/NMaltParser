using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.ml.lib
{




	using  org.maltparser.core.exception;
	using  org.maltparser.core.feature;
	using  org.maltparser.core.feature.function;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.feature.value;
	using  org.maltparser.core.syntaxgraph;
	using  org.maltparser.parser;
	using  org.maltparser.parser.guide.instance;
	using  org.maltparser.parser.history.action;

	public abstract class Lib : LearningMethod
	{
		public enum Verbostity
		{
			SILENT,
			ERROR,
			ALL
		}
		protected internal readonly Verbostity verbosity;
		private readonly InstanceModel owner;
		private readonly int learnerMode;
		private readonly string name;
		protected internal readonly FeatureMap featureMap;
		private readonly bool excludeNullValues;
		private StreamWriter instanceOutput = null;
		protected internal MaltLibModel model = null;

		private int numberOfInstances;

		/// <summary>
		/// Constructs a Lib learner.
		/// </summary>
		/// <param name="owner"> the guide model owner </param>
		/// <param name="learnerMode"> the mode of the learner BATCH or CLASSIFY </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Lib(org.maltparser.parser.guide.instance.InstanceModel owner, System.Nullable<int> learnerMode, String learningMethodName) throws org.maltparser.core.exception.MaltChainedException
		public Lib(InstanceModel owner, int? learnerMode, string learningMethodName)
		{
			this.owner = owner;
			this.learnerMode = learnerMode.Value;
			this.name = learningMethodName;
			if (Configuration.getOptionValue("lib", "verbosity") != null)
			{
				this.verbosity = Enum.Parse(typeof(Verbostity), Configuration.getOptionValue("lib", "verbosity").ToString().ToUpper());
			}
			else
			{
				this.verbosity = Verbostity.SILENT;
			}
			NumberOfInstances = 0;
			if (Configuration.getOptionValue("singlemalt", "null_value") != null && Configuration.getOptionValue("singlemalt", "null_value").ToString().Equals("none", StringComparison.OrdinalIgnoreCase))
			{
				excludeNullValues = true;
			}
			else
			{
				excludeNullValues = false;
			}

			if (learnerMode.Value == org.maltparser.ml.LearningMethod_Fields.BATCH)
			{
				featureMap = new FeatureMap();
				instanceOutput = new StreamWriter(getInstanceOutputStreamWriter(".ins"));
			}
			else if (learnerMode.Value == org.maltparser.ml.LearningMethod_Fields.CLASSIFY)
			{
				featureMap = (FeatureMap)getConfigFileEntryObject(".map");
			}
			else
			{
				featureMap = null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.parser.history.action.SingleDecision decision, org.maltparser.core.feature.FeatureVector featureVector) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(SingleDecision decision, FeatureVector featureVector)
		{
			if (featureVector == null)
			{
				throw new LibException("The feature vector cannot be found");
			}
			else if (decision == null)
			{
				throw new LibException("The decision cannot be found");
			}

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
				StringBuilder sb = new StringBuilder();
				sb.Append(decision.DecisionCode + "\t");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = featureVector.size();
				int n = featureVector.Count;
				for (int i = 0; i < n; i++)
				{
					FeatureValue featureValue = featureVector.getFeatureValue(i);
					if (featureValue == null || (excludeNullValues == true && featureValue.NullValue))
					{
						sb.Append("-1");
					}
					else
					{
						if (!featureValue.Multiple)
						{
							SingleFeatureValue singleFeatureValue = (SingleFeatureValue)featureValue;
							if (singleFeatureValue.Value == 1)
							{
								sb.Append(singleFeatureValue.IndexCode);
							}
							else if (singleFeatureValue.Value == 0)
							{
								sb.Append("-1");
							}
							else
							{
								sb.Append(singleFeatureValue.IndexCode);
								sb.Append(":");
								sb.Append(singleFeatureValue.Value);
							}
						}
						else
						{ //if (featureValue instanceof MultipleFeatureValue) {
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
	//					else {
	//						throw new LibException("Don't recognize the type of feature value: "+featureValue.getClass());
	//					}
					}
					sb.Append('\t');
				}
				sb.Append('\n');
				instanceOutput.Write(sb.ToString());
				instanceOutput.Flush();
				increaseNumberOfInstances();
	//			sb.setLength(0);
			}
			catch (IOException e)
			{
				throw new LibException("The learner cannot write to the instance file. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void finalizeSentence(org.maltparser.core.syntaxgraph.DependencyStructure dependencyGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void finalizeSentence(DependencyStructure dependencyGraph)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void moveAllInstances(org.maltparser.ml.LearningMethod method, org.maltparser.core.feature.function.FeatureFunction divideFeature, java.util.ArrayList<int> divideFeatureIndexVector) throws org.maltparser.core.exception.MaltChainedException
		public virtual void moveAllInstances(LearningMethod method, FeatureFunction divideFeature, List<int> divideFeatureIndexVector)
		{
			if (method == null)
			{
				throw new LibException("The learning method cannot be found. ");
			}
			else if (divideFeature == null)
			{
				throw new LibException("The divide feature cannot be found. ");
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
				@out.Flush();
			}
			catch (SecurityException e)
			{
				throw new LibException("The learner cannot remove the instance file. ", e);
			}
			catch (System.NullReferenceException e)
			{
				throw new LibException("The instance file cannot be found. ", e);
			}
			catch (FileNotFoundException e)
			{
				throw new LibException("The instance file cannot be found. ", e);
			}
			catch (IOException e)
			{
				throw new LibException("The learner read from the instance file. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances() throws org.maltparser.core.exception.MaltChainedException
		public virtual void noMoreInstances()
		{
			closeInstanceWriter();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predict(FeatureVector featureVector, SingleDecision decision)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.ml.lib.FeatureList featureList = new org.maltparser.ml.lib.FeatureList();
			FeatureList featureList = new FeatureList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = featureVector.size();
			int size = featureVector.Count;
			for (int i = 1; i <= size; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.value.FeatureValue featureValue = featureVector.getFeatureValue(i-1);
				FeatureValue featureValue = featureVector.getFeatureValue(i - 1);
				if (featureValue != null && !(excludeNullValues == true && featureValue.NullValue))
				{
					if (!featureValue.Multiple)
					{
						SingleFeatureValue singleFeatureValue = (SingleFeatureValue)featureValue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = featureMap.getIndex(i, singleFeatureValue.getIndexCode());
						int index = featureMap.getIndex(i, singleFeatureValue.IndexCode);
						if (index != -1 && singleFeatureValue.Value != 0)
						{
							featureList.add(index,singleFeatureValue.Value);
						}
					}
					else
					{
						foreach (int? value in ((MultipleFeatureValue)featureValue).Codes)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int v = featureMap.getIndex(i, value);
							int v = featureMap.getIndex(i, value.Value);
							if (v != -1)
							{
								featureList.add(v,1);
							}
						}
					}
				}
			}
			try
			{
				decision.KBestList.addList(model.predict(featureList.toArray()));
			}
			catch (System.OutOfMemoryException e)
			{
				throw new LibException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
			}
			return true;
		}

	//	protected abstract int[] prediction(FeatureList featureList) throws MaltChainedException;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void train() throws org.maltparser.core.exception.MaltChainedException
		public virtual void train()
		{
			if (owner == null)
			{
				throw new LibException("The parent guide model cannot be found. ");
			}
			string pathExternalTrain = null;
			if (!Configuration.getOptionValue("lib", "external").ToString().Equals(""))
			{
				string path = Configuration.getOptionValue("lib", "external").ToString();
				try
				{
					if (!Directory.Exists(path) || File.Exists(path))
					{
						throw new LibException("The path to the external  trainer 'svm-train' is wrong.");
					}
					if (Directory.Exists(path))
					{
						throw new LibException("The option --lib-external points to a directory, the path should point at the 'train' file or the 'train.exe' file in the libsvm or the liblinear package");
					}
					if (!(path.EndsWith("train", StringComparison.Ordinal) || path.EndsWith("train.exe", StringComparison.Ordinal)))
					{
						throw new LibException("The option --lib-external does not specify the path to 'train' file or the 'train.exe' file in the libsvm or the liblinear package. ");
					}
					pathExternalTrain = path;
				}
				catch (SecurityException e)
				{
					throw new LibException("Access denied to the file specified by the option --lib-external. ", e);
				}
			}
			LinkedHashMap<string, string> libOptions = DefaultLibOptions;
			parseParameters(Configuration.getOptionValue("lib", "options").ToString(), libOptions, AllowedLibOptionFlags);

	//		long startTime = System.currentTimeMillis();

	//		if (configLogger.isInfoEnabled()) {
	//			configLogger.info("\nStart training\n");
	//		}
			if (!string.ReferenceEquals(pathExternalTrain, null))
			{
				trainExternal(pathExternalTrain, libOptions);
			}
			else
			{
				trainInternal(libOptions);
			}
	//		long elapsed = System.currentTimeMillis() - startTime;
	//		if (configLogger.isInfoEnabled()) {
	//			configLogger.info("Time 1: " +new Formatter().format("%02d:%02d:%02d", elapsed/3600000, elapsed%3600000/60000, elapsed%60000/1000)+" ("+elapsed+" ms)\n");
	//		}
			try
			{
	//			if (configLogger.isInfoEnabled()) {
	//				configLogger.info("\nSaving feature map "+getFile(".map").getName()+"\n");
	//			}
				saveFeatureMap(new BufferedOutputStream(new FileStream(getFile(".map").AbsolutePath, FileMode.Create, FileAccess.Write)), featureMap);
			}
			catch (FileNotFoundException e)
			{
				throw new LibException("The learner cannot save the feature map file '" + getFile(".map").AbsolutePath + "'. ", e);
			}
	//		elapsed = System.currentTimeMillis() - startTime;
	//		if (configLogger.isInfoEnabled()) {
	//			configLogger.info("Time 2: " +new Formatter().format("%02d:%02d:%02d", elapsed/3600000, elapsed%3600000/60000, elapsed%60000/1000)+" ("+elapsed+" ms)\n");
	//		}
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void trainExternal(String pathExternalTrain, java.util.LinkedHashMap<String, String> libOptions) throws org.maltparser.core.exception.MaltChainedException;
		protected internal abstract void trainExternal(string pathExternalTrain, LinkedHashMap<string, string> libOptions);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void trainInternal(java.util.LinkedHashMap<String, String> libOptions) throws org.maltparser.core.exception.MaltChainedException;
		protected internal abstract void trainInternal(LinkedHashMap<string, string> libOptions);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			closeInstanceWriter();
	//		owner = null;
	//		model = null;
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
				throw new LibException("The learner cannot close the instance file. ", e);
			}
		}

		public virtual InstanceModel Owner
		{
			get
			{
				return owner;
			}
		}

		public virtual int LearnerMode
		{
			get
			{
				return learnerMode;
			}
		}

		public virtual string LearningMethodName
		{
			get
			{
				return name;
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object getConfigFileEntryObject(String suffix) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual object getConfigFileEntryObject(string suffix)
		{
			return Configuration.getConfigFileEntryObject(owner.ModelName + LearningMethodName + suffix);
		}

		public virtual string[] getLibParamStringArray(LinkedHashMap<string, string> libOptions)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<String> params = new java.util.ArrayList<String>();
			List<string> @params = new List<string>();

			foreach (string key in libOptions.Keys)
			{
				@params.Add("-" + key);
				@params.Add(libOptions.get(key));
			}
			return @params.ToArray();
		}

		public abstract LinkedHashMap<string, string> DefaultLibOptions {get;}
		public abstract string AllowedLibOptionFlags {get;}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseParameters(String paramstring, java.util.LinkedHashMap<String, String> libOptions, String allowedLibOptionFlags) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parseParameters(string paramstring, LinkedHashMap<string, string> libOptions, string allowedLibOptionFlags)
		{
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
				throw new LibException("Could not split the parameter string '" + paramstring + "'. ", e);
			}
			for (int i = 0; i < argv.Length - 1; i++)
			{
				if (argv[i][0] != '-')
				{
					throw new LibException("The argument flag should start with the following character '-', not with " + argv[i][0]);
				}
				if (++i >= argv.Length)
				{
					throw new LibException("The last argument does not have any value. ");
				}
				try
				{
					int index = allowedLibOptionFlags.IndexOf(argv[i - 1][1]);
					if (index != -1)
					{
						libOptions.put(Convert.ToString(argv[i - 1][1]), argv[i]);
					}
					else
					{
						throw new LibException("Unknown learner parameter: '" + argv[i - 1] + "' with value '" + argv[i] + "'. ");
					}
				}
				catch (System.IndexOutOfRangeException e)
				{
					throw new LibException("The learner parameter '" + argv[i - 1] + "' could not convert the string value '" + argv[i] + "' into a correct numeric value. ", e);
				}
				catch (System.FormatException e)
				{
					throw new LibException("The learner parameter '" + argv[i - 1] + "' could not convert the string value '" + argv[i] + "' into a correct numeric value. ", e);
				}
				catch (System.NullReferenceException e)
				{
					throw new LibException("The learner parameter '" + argv[i - 1] + "' could not convert the string value '" + argv[i] + "' into a correct numeric value. ", e);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws Throwable
		~Lib()
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

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer sb = new StringBuffer();
			StringBuilder sb = new StringBuilder();
			sb.Append('\n');
			sb.Append(LearningMethodName);
			sb.Append(" INTERFACE\n");
			try
			{
				sb.Append(Configuration.getOptionValue("lib", "options").ToString());
			}
			catch (MaltChainedException)
			{
			}
			return sb.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int binariesInstance(String line, org.maltparser.ml.lib.FeatureList featureList) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual int binariesInstance(string line, FeatureList featureList)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.regex.Pattern tabPattern = java.util.regex.Pattern.compile("\t");
			Pattern tabPattern = Pattern.compile("\t");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.regex.Pattern pipePattern = java.util.regex.Pattern.compile("\\|");
			Pattern pipePattern = Pattern.compile("\\|");
			int y = -1;
			featureList.clear();
			try
			{
				string[] columns = tabPattern.split(line);

				if (columns.Length == 0)
				{
					return -1;
				}
				try
				{
					y = int.Parse(columns[0]);
				}
				catch (System.FormatException e)
				{
					throw new LibException("The instance file contain a non-integer value '" + columns[0] + "'", e);
				}
				for (int j = 1; j < columns.Length; j++)
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
									int v = featureMap.addIndex(j, int.Parse(items[k]));
									if (v != -1)
									{
										featureList.add(v,1);
									}
								}
							}
							else
							{
								int index = featureMap.addIndex(j, int.Parse(items[k].Substring(0,colon)));
								double value;
								if (items[k].Substring(colon + 1).IndexOf('.') != -1)
								{
									value = double.Parse(items[k].Substring(colon + 1));
								}
								else
								{
									value = int.Parse(items[k].Substring(colon + 1));
								}
								featureList.add(index,value);
							}
						}
						catch (System.FormatException e)
						{
							throw new LibException("The instance file contain a non-numeric value '" + items[k] + "'", e);
						}
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new LibException("Couln't read from the instance file. ", e);
			}
			return y;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void binariesInstances2SVMFileFormat(java.io.InputStreamReader isr, java.io.OutputStreamWriter osw) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void binariesInstances2SVMFileFormat(StreamReader isr, StreamWriter osw)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader in = new java.io.BufferedReader(isr);
				StreamReader @in = new StreamReader(isr);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedWriter out = new java.io.BufferedWriter(osw);
				StreamWriter @out = new StreamWriter(osw);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.ml.lib.FeatureList featureSet = new org.maltparser.ml.lib.FeatureList();
				FeatureList featureSet = new FeatureList();
				while (true)
				{
					string line = @in.ReadLine();
					if (string.ReferenceEquals(line, null))
					{
						break;
					}
					int y = binariesInstance(line, featureSet);
					if (y == -1)
					{
						continue;
					}
					@out.Write(Convert.ToString(y));

					for (int k = 0; k < featureSet.size(); k++)
					{
						MaltFeatureNode x = featureSet.get(k);
						@out.BaseStream.WriteByte(' ');
						@out.Write(Convert.ToString(x.Index));
						@out.BaseStream.WriteByte(':');
						@out.Write(Convert.ToString(x.Value));
					}
					@out.BaseStream.WriteByte('\n');
				}
				@in.Close();
				@out.Close();
			}
			catch (System.FormatException e)
			{
				throw new LibException("The instance file contain a non-numeric value", e);
			}
			catch (IOException e)
			{
				throw new LibException("Couldn't read from the instance file, when converting the Malt instances into LIBSVM/LIBLINEAR format. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void saveFeatureMap(java.io.OutputStream os, org.maltparser.ml.lib.FeatureMap map) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void saveFeatureMap(Stream os, FeatureMap map)
		{
			try
			{
				ObjectOutputStream output = new ObjectOutputStream(os);
				try
				{
				  output.writeObject(map);
				}
				finally
				{
				  output.close();
				}
			}
			catch (IOException e)
			{
				throw new LibException("Save feature map error", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.ml.lib.FeatureMap loadFeatureMap(java.io.InputStream is) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual FeatureMap loadFeatureMap(Stream @is)
		{
			FeatureMap map = new FeatureMap();
			try
			{
				ObjectInputStream input = new ObjectInputStream(@is);
				try
				{
					map = (FeatureMap)input.readObject();
				}
				finally
				{
					input.close();
				}
			}
			catch (ClassNotFoundException e)
			{
				throw new LibException("Load feature map error", e);
			}
			catch (IOException e)
			{
				throw new LibException("Load feature map error", e);
			}
			return map;
		}
	}

}