using System;
using System.IO;

namespace org.maltparser.ml.lib
{

	using  de.bwaldvogel.liblinear;
	using  de.bwaldvogel.liblinear;
	using  de.bwaldvogel.liblinear;
	using  de.bwaldvogel.liblinear;
	using  de.bwaldvogel.liblinear;
	using  de.bwaldvogel.liblinear;

	using  org.maltparser.core.config;
	using  org.maltparser.core.exception;
	using  org.maltparser.core.helper;
	using  org.maltparser.core.helper;
	using  org.maltparser.parser.guide.instance;

	public class LibLinear : Lib
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LibLinear(org.maltparser.parser.guide.instance.InstanceModel owner, System.Nullable<int> learnerMode) throws org.maltparser.core.exception.MaltChainedException
		public LibLinear(InstanceModel owner, int? learnerMode) : base(owner, learnerMode, "liblinear")
		{
			if (learnerMode.Value == org.maltparser.ml.LearningMethod_Fields.CLASSIFY)
			{
				model = (MaltLibModel)getConfigFileEntryObject(".moo");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void trainInternal(java.util.LinkedHashMap<String, String> libOptions) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void trainInternal(LinkedHashMap<string, string> libOptions)
		{
			Configuration config = Configuration;

			if (config.LoggerInfoEnabled)
			{
				config.logInfoMessage("Creating Liblinear model " + getFile(".moo").Name + "\n");
			}
			double[] wmodel = null;
			int[] labels = null;
			int nr_class = 0;
			int nr_feature = 0;
			Parameter parameter = getLiblinearParameters(libOptions);
			try
			{
				Problem problem = readProblem(getInstanceInputStreamReader(".ins"));
				bool res = checkProblem(problem);
				if (res == false)
				{
					throw new LibException("Abort (The number of training instances * the number of classes) > " + int.MaxValue + " and this is not supported by LibLinear. ");
				}
				if (config.LoggerInfoEnabled)
				{
					config.logInfoMessage("- Train a parser model using LibLinear.\n");
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.PrintStream out = System.out;
				PrintStream @out = System.out;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.PrintStream err = System.err;
				PrintStream err = System.err;
				System.Out = NoPrintStream.NO_PRINTSTREAM;
				System.Err = NoPrintStream.NO_PRINTSTREAM;
				Model model = Linear.train(problem, parameter);
				System.Out = err;
				System.Out = @out;
				problem = null;
				wmodel = model.FeatureWeights;
				labels = model.Labels;
				nr_class = model.NrClass;
				nr_feature = model.NrFeature;
				bool saveInstanceFiles = ((bool?)Configuration.getOptionValue("lib", "save_instance_files")).Value;
				if (!saveInstanceFiles)
				{
					getFile(".ins").delete();
				}
			}
			catch (System.OutOfMemoryException e)
			{
				throw new LibException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
			}
			catch (System.ArgumentException e)
			{
				throw new LibException("The Liblinear learner was not able to redirect Standard Error stream. ", e);
			}
			catch (SecurityException e)
			{
				throw new LibException("The Liblinear learner cannot remove the instance file. ", e);
			}
			catch (NegativeArraySizeException e)
			{
				throw new LibException("(The number of training instances * the number of classes) > " + int.MaxValue + " and this is not supported by LibLinear.", e);
			}

			if (config.LoggerInfoEnabled)
			{
				config.logInfoMessage("- Optimize the memory usage\n");
			}
			MaltLiblinearModel xmodel = null;
			try
			{
	//			System.out.println("Nr Features:" +  nr_feature);
	//			System.out.println("nr_class:" + nr_class);
	//			System.out.println("wmodel.length:" + wmodel.length);		
				double[][] wmatrix = convert2(wmodel, nr_class, nr_feature);
				xmodel = new MaltLiblinearModel(labels, nr_class, wmatrix.Length, wmatrix, parameter.SolverType);
				if (config.LoggerInfoEnabled)
				{
					config.logInfoMessage("- Save the Liblinear model " + getFile(".moo").Name + "\n");
				}
			}
			catch (System.OutOfMemoryException e)
			{
				throw new LibException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
			}
			try
			{
				if (xmodel != null)
				{
					ObjectOutputStream output = new ObjectOutputStream(new BufferedOutputStream(new FileStream(getFile(".moo").AbsolutePath, FileMode.Create, FileAccess.Write)));
					try
					{
					  output.writeObject(xmodel);
					}
					finally
					{
					  output.close();
					}
				}
			}
			catch (System.OutOfMemoryException e)
			{
				throw new LibException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
			}
			catch (System.ArgumentException e)
			{
				throw new LibException("The Liblinear learner was not able to redirect Standard Error stream. ", e);
			}
			catch (SecurityException e)
			{
				throw new LibException("The Liblinear learner cannot remove the instance file. ", e);
			}
			catch (IOException e)
			{
				throw new LibException("The Liblinear learner cannot save the model file '" + getFile(".mod").AbsolutePath + "'. ", e);
			}
		}

		private double[][] convert2(double[] w, int nr_class, int nr_feature)
		{
			int[] wlength = new int[nr_feature];
			int nr_nfeature = 0;
	//        int ne = 0;
	//        int nr = 0;
	//        int no = 0;
	//        int n = 0;

			// Identify length of new weight array for each feature
			for (int i = 0; i < nr_feature; i++)
			{
				int k = nr_class;
				for (int t = i * nr_class; (t + (k - 1)) >= t; k--)
				{
					if (w[t + k - 1] != 0.0)
					{
						break;
					}
				}
				int b = k;
				if (b != 0)
				{
					for (int t = i * nr_class; (t + (b - 1)) >= t; b--)
					{
						if (b != k)
						{
							if (w[t + b - 1] != w[t + b])
							{
								break;
							}
						}
					}
				}
				if (k == 0 || b == 0)
				{
					wlength[i] = 0;
				}
				else
				{
					wlength[i] = k;
					nr_nfeature++;
				}
			}
			// Allocate the weight matrix with the new number of features and
			// an array wsignature that efficient compare if weight vector can be reused by another feature. 
			double[][] wmatrix = new double[nr_nfeature][];
			double[] wsignature = new double[nr_nfeature];
			long?[] reverseMap = featureMap.reverseMap();
			int @in = 0;
			for (int i = 0; i < nr_feature; i++)
			{
				if (wlength[i] == 0)
				{
					// if the length of the weight vector is zero than eliminate the feature from the feature map.
	//            	ne++;
					featureMap.removeIndex(reverseMap[i + 1].Value);
					reverseMap[i + 1] = null;
				}
				else
				{
					bool reuse = false;
					double[] copy = new double[wlength[i]];
					Array.Copy(w, i * nr_class, copy, 0, wlength[i]);
					featureMap.setIndex(reverseMap[i + 1].Value, @in + 1);
					for (int j = 0; j < copy.Length; j++)
					{
						wsignature[@in] += copy[j];
					}
					for (int j = 0; j < @in; j++)
					{
						if (wsignature[j] == wsignature[@in])
						{
							// if the signatures is equal then do more narrow comparison  
							if (Util.Equals(copy, wmatrix[j]))
							{
								// if equal then reuse the weight vector
								wmatrix[@in] = wmatrix[j];
								reuse = true;
	//		            		nr++;
								break;
							}
						}
					}
					if (reuse == false)
					{
						// if no reuse has done use the new weight vector in the weight matrix 
	//	            	no++;
						wmatrix[@in] = copy;
					}
					@in++;
				}
	//            n++;
			}
			featureMap.FeatureCounter = nr_nfeature;
	//        System.out.println("NE:"+ne);
	//        System.out.println("NR:"+nr);
	//        System.out.println("NO:"+no);
	//        System.out.println("N :"+n);
			return wmatrix;
		}

		public static bool eliminate(double[] a)
		{
			if (a.Length == 0)
			{
				return true;
			}
			for (int i = 1; i < a.Length; i++)
			{
				if (a[i] != a[i - 1])
				{
					return false;
				}
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void trainExternal(String pathExternalTrain, java.util.LinkedHashMap<String, String> libOptions) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void trainExternal(string pathExternalTrain, LinkedHashMap<string, string> libOptions)
		{
			try
			{
				Configuration config = Configuration;
				if (config.LoggerInfoEnabled)
				{
					config.logInfoMessage("Creating liblinear model (external) " + getFile(".mod").Name);
				}
				binariesInstances2SVMFileFormat(getInstanceInputStreamReader(".ins"), getInstanceOutputStreamWriter(".ins.tmp"));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] params = getLibParamStringArray(libOptions);
				string[] @params = getLibParamStringArray(libOptions);
				string[] arrayCommands = new string[@params.Length + 3];
				int i = 0;
				arrayCommands[i++] = pathExternalTrain;
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
				if (config.LoggerInfoEnabled)
				{
					config.logInfoMessage("\nSaving Liblinear model " + getFile(".moo").Name + "\n");
				}
				MaltLiblinearModel xmodel = new MaltLiblinearModel(getFile(".mod"));
				ObjectOutputStream output = new ObjectOutputStream(new BufferedOutputStream(new FileStream(getFile(".moo").AbsolutePath, FileMode.Create, FileAccess.Write)));
				try
				{
				  output.writeObject(xmodel);
				}
				finally
				{
				  output.close();
				}
				bool saveInstanceFiles = ((bool?)Configuration.getOptionValue("lib", "save_instance_files")).Value;
				if (!saveInstanceFiles)
				{
					getFile(".ins").delete();
					getFile(".mod").delete();
					getFile(".ins.tmp").delete();
				}
				if (config.LoggerInfoEnabled)
				{
					config.logInfoMessage('\n');
				}
			}
			catch (InterruptedException e)
			{
				 throw new LibException("Learner is interrupted. ", e);
			}
			catch (System.ArgumentException e)
			{
				throw new LibException("The learner was not able to redirect Standard Error stream. ", e);
			}
			catch (SecurityException e)
			{
				throw new LibException("The learner cannot remove the instance file. ", e);
			}
			catch (IOException e)
			{
				throw new LibException("The learner cannot save the model file '" + getFile(".mod").AbsolutePath + "'. ", e);
			}
			catch (System.OutOfMemoryException e)
			{
				throw new LibException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
			base.terminate();
		}

		public override LinkedHashMap<string, string> DefaultLibOptions
		{
			get
			{
				LinkedHashMap<string, string> libOptions = new LinkedHashMap<string, string>();
				libOptions.put("s", "4"); // type = SolverType.MCSVM_CS (default)
				libOptions.put("c", "0.1"); // cost = 1 (default)
				libOptions.put("e", "0.1"); // epsilon = 0.1 (default)
				libOptions.put("B", "-1"); // bias = -1 (default)
				return libOptions;
			}
		}

		public override string AllowedLibOptionFlags
		{
			get
			{
				return "sceB";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private de.bwaldvogel.liblinear.Problem readProblem(java.io.InputStreamReader isr) throws org.maltparser.core.exception.MaltChainedException
		private Problem readProblem(StreamReader isr)
		{
			Problem problem = new Problem();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.ml.lib.FeatureList featureList = new org.maltparser.ml.lib.FeatureList();
			FeatureList featureList = new FeatureList();
			if (Configuration.LoggerInfoEnabled)
			{
				Configuration.logInfoMessage("- Read all training instances.\n");
			}
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

				while (true)
				{
					string line = fp.ReadLine();
					if (string.ReferenceEquals(line, null))
					{
						break;
					}
					int y = binariesInstance(line, featureList);
					if (y == -1)
					{
						continue;
					}
					try
					{
						problem.y[i] = y;
						problem.x[i] = new FeatureNode[featureList.size()];
						int p = 0;
						for (int k = 0; k < featureList.size(); k++)
						{
							MaltFeatureNode x = featureList.get(k);
							problem.x[i][p++] = new FeatureNode(x.Index, x.Value);
						}
						i++;
					}
					catch (System.IndexOutOfRangeException e)
					{
						throw new LibException("Couldn't read liblinear problem from the instance file. ", e);
					}

				}
				fp.Close();
				problem.n = featureMap.size();
			}
			catch (IOException e)
			{
				throw new LibException("Cannot read from the instance file. ", e);
			}

			return problem;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean checkProblem(de.bwaldvogel.liblinear.Problem problem) throws org.maltparser.core.exception.MaltChainedException
		private bool checkProblem(Problem problem)
		{
			int max_y = problem.y[0];
			for (int i = 1; i < problem.y.length; i++)
			{
				if (problem.y[i] > max_y)
				{
					max_y = problem.y[i];
				}
			}
			if (max_y * problem.l < 0)
			{ // max_y * problem.l > Integer.MAX_VALUE
				if (Configuration.LoggerInfoEnabled)
				{
					Configuration.logInfoMessage("*** Abort (The number of training instances * the number of classes) > Max array size: (" + problem.l + " * " + max_y + ") > " + int.MaxValue + " and this is not supported by LibLinear.\n");
				}
				return false;
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private de.bwaldvogel.liblinear.Parameter getLiblinearParameters(java.util.LinkedHashMap<String, String> libOptions) throws org.maltparser.core.exception.MaltChainedException
		private Parameter getLiblinearParameters(LinkedHashMap<string, string> libOptions)
		{
			Parameter param = new Parameter(SolverType.MCSVM_CS, 0.1, 0.1);
			string type = libOptions.get("s");

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
			else if (type.Equals("7"))
			{
				param.SolverType = SolverType.L2R_LR_DUAL;
			}
			else
			{
				throw new LibException("The liblinear type (-s) is not an integer value between 0 and 4. ");
			}
			try
			{
				param.C = Convert.ToDouble(libOptions.get("c"));
			}
			catch (System.FormatException e)
			{
				throw new LibException("The liblinear cost (-c) value is not numerical value. ", e);
			}
			try
			{
				param.Eps = Convert.ToDouble(libOptions.get("e"));
			}
			catch (System.FormatException e)
			{
				throw new LibException("The liblinear epsilon (-e) value is not numerical value. ", e);
			}
			return param;
		}
	}

}