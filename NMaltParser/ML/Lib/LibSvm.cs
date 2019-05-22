using System;
using System.IO;
using NMaltParser.Core.Config;
using NMaltParser.Core.Helper;
using NMaltParser.Parser.Guide.Instance;

namespace NMaltParser.ML.Lib
{
    public class LibSvm : Lib
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LibSvm(org.maltparser.parser.guide.instance.InstanceModel owner, System.Nullable<int> learnerMode) throws org.maltparser.core.exception.MaltChainedException
		public LibSvm(InstanceModel owner, int? learnerMode) : base(owner, learnerMode, "libsvm")
		{
			if (learnerMode.Value == LearningMethod_Fields.CLASSIFY)
			{
				model = (MaltLibModel)getConfigFileEntryObject(".moo");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void trainInternal(java.util.LinkedHashMap<String, String> libOptions) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void trainInternal(LinkedHashMap<string, string> libOptions)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final libsvm.svm_problem prob = readProblem(getInstanceInputStreamReader(".ins"), libOptions);
				svm_problem prob = readProblem(getInstanceInputStreamReader(".ins"), libOptions);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final libsvm.svm_parameter param = getLibSvmParameters(libOptions);
				svm_parameter param = getLibSvmParameters(libOptions);
				if (svm.svm_check_parameter(prob, param) != null)
				{
					throw new LibException(svm.svm_check_parameter(prob, param));
				}
				Configuration config = Configuration;

				if (config.LoggerInfoEnabled)
				{
					config.logInfoMessage("Creating LIBSVM model " + getFile(".moo").Name + "\n");
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.PrintStream out = System.out;
				PrintStream @out = System.out;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.PrintStream err = System.err;
				PrintStream err = System.err;
				System.Out = NoPrintStream.NO_PRINTSTREAM;
				System.Err = NoPrintStream.NO_PRINTSTREAM;
				svm_model model = svm.svm_train(prob, param);
				System.Out = err;
				System.Out = @out;
				ObjectOutputStream output = new ObjectOutputStream(new BufferedOutputStream(new FileStream(getFile(".moo").AbsolutePath, FileMode.Create, FileAccess.Write)));
				try
				{
				  output.writeObject(new MaltLibsvmModel(model, prob));
				}
				finally
				{
				  output.close();
				}
				bool saveInstanceFiles = ((bool?)Configuration.getOptionValue("lib", "save_instance_files")).Value;
				if (!saveInstanceFiles)
				{
					getFile(".ins").delete();
				}
			}
			catch (OutOfMemoryException e)
			{
				throw new LibException("Out of memory. Please increase the Java heap size (-Xmx<size>). ", e);
			}
			catch (ArgumentException e)
			{
				throw new LibException("The LIBSVM learner was not able to redirect Standard Error stream. ", e);
			}
			catch (SecurityException e)
			{
				throw new LibException("The LIBSVM learner cannot remove the instance file. ", e);
			}
			catch (IOException e)
			{
				throw new LibException("The LIBSVM learner cannot save the model file '" + getFile(".mod").AbsolutePath + "'. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void trainExternal(String pathExternalTrain, java.util.LinkedHashMap<String, String> libOptions) throws org.maltparser.core.exception.MaltChainedException
		protected internal override void trainExternal(string pathExternalTrain, LinkedHashMap<string, string> libOptions)
		{
			try
			{
				binariesInstances2SVMFileFormat(getInstanceInputStreamReader(".ins"), getInstanceOutputStreamWriter(".ins.tmp"));
				Configuration config = Configuration;

				if (config.LoggerInfoEnabled)
				{
					config.logInfoMessage("Creating learner model (external) " + getFile(".mod").Name);
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final libsvm.svm_problem prob = readProblem(getInstanceInputStreamReader(".ins"), libOptions);
				svm_problem prob = readProblem(getInstanceInputStreamReader(".ins"), libOptions);
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
				svm_model model = svm.svm_load_model(getFile(".mod").AbsolutePath);
				MaltLibsvmModel xmodel = new MaltLibsvmModel(model, prob);
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
			catch (ArgumentException e)
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
			catch (OutOfMemoryException e)
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
				libOptions.put("s", Convert.ToString(svm_parameter.C_SVC));
				libOptions.put("t", Convert.ToString(svm_parameter.POLY));
				libOptions.put("d", Convert.ToString(2));
				libOptions.put("g", Convert.ToString(0.2));
				libOptions.put("r", Convert.ToString(0));
				libOptions.put("n", Convert.ToString(0.5));
				libOptions.put("m", Convert.ToString(100));
				libOptions.put("c", Convert.ToString(1));
				libOptions.put("e", Convert.ToString(1.0));
				libOptions.put("p", Convert.ToString(0.1));
				libOptions.put("h", Convert.ToString(1));
				libOptions.put("b", Convert.ToString(0));
				return libOptions;
			}
		}

		public override string AllowedLibOptionFlags
		{
			get
			{
				return "stdgrnmcepb";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private libsvm.svm_parameter getLibSvmParameters(java.util.LinkedHashMap<String, String> libOptions) throws org.maltparser.core.exception.MaltChainedException
		private svm_parameter getLibSvmParameters(LinkedHashMap<string, string> libOptions)
		{
			svm_parameter param = new svm_parameter();

			param.svm_type = int.Parse(libOptions.get("s"));
			param.kernel_type = int.Parse(libOptions.get("t"));
			param.degree = int.Parse(libOptions.get("d"));
			param.gamma = Convert.ToDouble(libOptions.get("g"));
			param.coef0 = Convert.ToDouble(libOptions.get("r"));
			param.nu = Convert.ToDouble(libOptions.get("n"));
			param.cache_size = Convert.ToDouble(libOptions.get("m"));
			param.C = Convert.ToDouble(libOptions.get("c"));
			param.eps = Convert.ToDouble(libOptions.get("e"));
			param.p = Convert.ToDouble(libOptions.get("p"));
			param.shrinking = int.Parse(libOptions.get("h"));
			param.probability = int.Parse(libOptions.get("b"));
			param.nr_weight = 0;
			param.weight_label = new int[0];
			param.weight = new double[0];
			return param;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private libsvm.svm_problem readProblem(java.io.InputStreamReader isr, java.util.LinkedHashMap<String, String> libOptions) throws org.maltparser.core.exception.MaltChainedException
		private svm_problem readProblem(StreamReader isr, LinkedHashMap<string, string> libOptions)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final libsvm.svm_problem problem = new libsvm.svm_problem();
			svm_problem problem = new svm_problem();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final libsvm.svm_parameter param = getLibSvmParameters(libOptions);
			svm_parameter param = getLibSvmParameters(libOptions);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.ml.lib.FeatureList featureList = new org.maltparser.ml.lib.FeatureList();
			FeatureList featureList = new FeatureList();
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader fp = new java.io.BufferedReader(isr);
				StreamReader fp = new StreamReader(isr);

				problem.l = NumberOfInstances;
				problem.x = new svm_node[problem.l][];
				problem.y = new double[problem.l];
				int i = 0;

				while (true)
				{
					string line = fp.ReadLine();
					if (ReferenceEquals(line, null))
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
						problem.x[i] = new svm_node[featureList.size()];
						int p = 0;
						for (int k = 0; k < featureList.size(); k++)
						{
							MaltFeatureNode x = featureList.get(k);
							problem.x[i][p] = new svm_node();
							problem.x[i][p].value = x.Value;
							problem.x[i][p].index = x.Index;
							p++;
						}
						i++;
					}
					catch (IndexOutOfRangeException e)
					{
						throw new LibException("Couldn't read libsvm problem from the instance file. ", e);
					}
				}
				fp.Close();
				if (param.gamma == 0)
				{
					param.gamma = 1.0 / featureMap.FeatureCounter;
				}
			}
			catch (IOException e)
			{
				throw new LibException("Couldn't read libsvm problem from the instance file. ", e);
			}
			return problem;
		}
	}

}