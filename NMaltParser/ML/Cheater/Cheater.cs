using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.ml.cheater
{


	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FeatureVector = org.maltparser.core.feature.FeatureVector;
	using FeatureFunction = org.maltparser.core.feature.function.FeatureFunction;
	using FeatureValue = org.maltparser.core.feature.value.FeatureValue;
	using MultipleFeatureValue = org.maltparser.core.feature.value.MultipleFeatureValue;
	using SingleFeatureValue = org.maltparser.core.feature.value.SingleFeatureValue;
	using DependencyStructure = org.maltparser.core.syntaxgraph.DependencyStructure;
	using DependencyParserConfig = org.maltparser.parser.DependencyParserConfig;
	using InstanceModel = org.maltparser.parser.guide.instance.InstanceModel;
	using SingleDecision = org.maltparser.parser.history.action.SingleDecision;

	public class Cheater : LearningMethod
	{
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
		protected internal bool excludeNullValues;
	//	private int[] cardinalities;
		private string cheaterFileName;
		private StreamWriter cheaterWriter = null;
		private bool saveCheatAction;
		private StreamWriter instanceOutput = null;
		private List<int> cheatValues;
		private int cheaterPosition;
		private Verbostity verbosity;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Cheater(org.maltparser.parser.guide.instance.InstanceModel owner, System.Nullable<int> learnerMode) throws org.maltparser.core.exception.MaltChainedException
		public Cheater(InstanceModel owner, int? learnerMode)
		{
			Owner = owner;
			LearningMethodName = "cheater";
			LearnerMode = learnerMode.Value;
			NumberOfInstances = 0;
			verbosity = Verbostity.SILENT;
			initSpecialParameters();

			if (learnerMode.Value == org.maltparser.ml.LearningMethod_Fields.BATCH)
			{
				if (!saveCheatAction)
				{
					instanceOutput = new StreamWriter(getInstanceOutputStreamWriter(".ins"));
				}
				else
				{
					try
					{
						if (!string.ReferenceEquals(cheaterFileName, null) && !cheaterFileName.Equals(""))
						{
							cheaterWriter = new StreamWriter(new FileStream(cheaterFileName, FileMode.Create, FileAccess.Write));
						}
					}
					catch (Exception e)
					{
						throw new CheaterException("", e);
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addInstance(org.maltparser.parser.history.action.SingleDecision decision, org.maltparser.core.feature.FeatureVector featureVector) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addInstance(SingleDecision decision, FeatureVector featureVector)
		{
			if (featureVector == null)
			{
				throw new CheaterException("The feature vector cannot be found");
			}
			else if (decision == null)
			{
				throw new CheaterException("The decision cannot be found");
			}
			if (saveCheatAction && cheaterWriter != null)
			{
				try
				{
					cheaterWriter.Write(decision.DecisionCode + "\n");
				}
				catch (IOException e)
				{
					throw new CheaterException("The cheater learner cannot write to the cheater file. ", e);
				}
			}
			else
			{
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
		//				if (i < n-1) {
							sb.Append('\t');
		//				}
					}
					sb.Append('\n');
					instanceOutput.Write(sb.ToString());
					instanceOutput.Flush();
					increaseNumberOfInstances();
				}
				catch (IOException e)
				{
					throw new CheaterException("The cheater learner cannot write to the instance file. ", e);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void train() throws org.maltparser.core.exception.MaltChainedException
		public virtual void train()
		{
	//		if (featureVector == null) {
	//			throw new CheaterException("The feature vector cannot be found. ");
	//		} else if (owner == null) {
	//			throw new CheaterException("The parent guide model cannot be found. ");
	//		}
	//		if (!saveCheatAction) {
	//			cardinalities = getCardinalities(featureVector);
	//			maltSVMFormat2OriginalSVMFormat(getInstanceInputStreamReader(".ins"), getInstanceOutputStreamWriter(".ins.tmp"), cardinalities);
	//			saveCardinalities(getInstanceOutputStreamWriter(".car"), cardinalities);
	//		}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean predict(org.maltparser.core.feature.FeatureVector featureVector, org.maltparser.parser.history.action.SingleDecision decision) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool predict(FeatureVector featureVector, SingleDecision decision)
		{
	//		if (cardinalities == null) {
	//			if (getConfigFileEntry(".car") != null) {
	//				cardinalities = loadCardinalities(getInstanceInputStreamReaderFromConfigFile(".car"));
	//			} else {
	//				cardinalities = getCardinalities(featureVector);
	//			}
	//		}
			if (cheatValues == null)
			{
				if (string.ReferenceEquals(cheaterFileName, null) || cheaterFileName.Equals(""))
				{
					throw new CheaterException("The cheater file name is assigned. ");
				}
				try
				{
					StreamReader reader = new StreamReader(new FileStream(cheaterFileName, FileMode.Open, FileAccess.Read));
					string line = "";
					cheatValues = new List<int>();
					while (!string.ReferenceEquals((line = reader.ReadLine()), null))
					{
						cheatValues.Add(int.Parse(line));
					}
					cheaterPosition = 0;
					reader.Close();
					cheaterWriter = new StreamWriter(new FileStream(cheaterFileName + ".ins", FileMode.Create, FileAccess.Write));
				}
				catch (Exception e)
				{
					throw new CheaterException("Couldn't find or read from the cheater file '" + cheaterFileName + "'", e);
				}
			}

	//		int offset = 1;
	//		int i = 0;
			int decisionValue = 0;
			StringBuilder csb = new StringBuilder();
			if (cheaterPosition < cheatValues.Count)
			{
				decisionValue = cheatValues[cheaterPosition++];
				csb.Append(decisionValue + " ");
			}
			else
			{
				throw new CheaterException("Not enough cheat values to complete all sentences. ");
			}


	//		for (FeatureFunction feature : featureVector) {
	//			final FeatureValue featureValue = feature.getFeatureValue();
	//			if (!(excludeNullValues == true && featureValue.isNullValue())) {
	//				if (featureValue instanceof SingleFeatureValue) {
	//					if (((SingleFeatureValue)featureValue).getCode() < cardinalities[i]) {
	//						csb.append((((SingleFeatureValue)featureValue).getCode() + offset) + ":" + "1 ");
	//					}
	//				} else if (featureValue instanceof MultipleFeatureValue) {
	//					for (Integer value : ((MultipleFeatureValue)featureValue).getCodes()) {
	//						if (value < cardinalities[i]) {
	//							csb.append((value + offset) + ":" + "1 ");
	//						}
	//					}
	//				}
	//			}
	//			offset += cardinalities[i];
	//			i++;
	//		}
			csb.Length = csb.Length - 1;
			csb.Append('\n');
			try
			{
				cheaterWriter.Write(csb.ToString());
				cheaterWriter.Flush();
			}
			catch (Exception e)
			{
				throw new CheaterException("", e);
			}
			try
			{
				decision.KBestList.add(decisionValue);
			}
			catch (Exception)
			{
				decision.KBestList.add(-1);
			}
			return true;
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
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void noMoreInstances() throws org.maltparser.core.exception.MaltChainedException
		public virtual void noMoreInstances()
		{
			closeInstanceWriter();
			closeCheaterWriter();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			closeInstanceWriter();
			closeCheaterWriter();
			owner = null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void closeCheaterWriter() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void closeCheaterWriter()
		{
			try
			{
				if (cheaterWriter != null)
				{
					cheaterWriter.Flush();
					cheaterWriter.Close();
					cheaterWriter = null;
				}
			}
			catch (IOException e)
			{
				throw new CheaterException("The cheater learner cannot close the cheater file. ", e);
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
				throw new CheaterException("The cheater learner cannot close the instance file. ", e);
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
	//			throw new CheaterException("", e);
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
	//			throw new CheaterException("", e);
	//		} catch (NumberFormatException e) {
	//			throw new CheaterException("", e);
	//		}
	//		return cardinalities;
	//	}

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
			saveCheatAction = ((bool?)Configuration.getOptionValue("cheater", "save_cheat_action")).Value;

			if (!Configuration.getOptionValue("cheater", "cheater_file").ToString().Equals(""))
			{
				cheaterFileName = Configuration.getOptionValue("cheater", "cheater_file").ToString();
			}
			if (Configuration.getOptionValue("liblinear", "verbosity") != null)
			{
				verbosity = Enum.Parse(typeof(Verbostity), Configuration.getOptionValue("cheater", "verbosity").ToString().ToUpper());
			}
		}

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
							throw new CheaterException("The instance file contain a non-integer value, when converting the Malt SVM format into Liblinear format.");
						}
					}
				}
				@in.Close();
				@out.Close();
			}
			catch (IOException e)
			{
				throw new CheaterException("Cannot read from the instance file, when converting the Malt SVM format into Liblinear format. ", e);
			}
		}

		public virtual StreamWriter InstanceWriter
		{
			get
			{
				return instanceOutput;
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
				return numberOfInstances;
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
//ORIGINAL LINE: protected void finalize() throws Throwable
		~Cheater()
		{
			try
			{
				closeInstanceWriter();
				closeCheaterWriter();
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
			sb.Append("\nCheater INTERFACE\n");
	//		sb.append("  Cheater string: "+paramString+"\n");


			return sb.ToString();
		}
	}

}