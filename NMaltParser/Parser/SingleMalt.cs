using System;
using System.IO;
using System.Text;
using NMaltParser.Core.Config;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Feature;
using NMaltParser.Core.Feature.System;
using NMaltParser.Core.Helper;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Options;
using NMaltParser.Core.Plugin;
using NMaltParser.Core.Propagation;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Parser.Guide;
using NMaltParser.Utilities;

namespace NMaltParser.Parser
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class SingleMalt : DependencyParserConfig
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(DependencyParserConfig)};
		public const int LEARN = 0;
		public const int PARSE = 1;
		protected internal ConfigurationDir configDir;
		protected internal Logger configLogger;
		protected internal int optionContainerIndex;
		protected internal ParsingAlgorithm parsingAlgorithm = null;
		protected internal int mode;
		protected internal SymbolTableHandler symbolTableHandler;
		protected internal DataFormatInstance dataFormatInstance;
		protected internal FeatureModelManager featureModelManager;
		protected internal long startTime;
		protected internal long endTime;
		protected internal int nIterations = 0;
		protected internal PropagationManager propagationManager;
		private Parser parser;
		private Trainer trainer;
		private AbstractParserFactory parserFactory;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(int containerIndex, org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler symbolTableHandler, org.maltparser.core.config.ConfigurationDir configDir, int mode) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initialize(int containerIndex, DataFormatInstance dataFormatInstance, SymbolTableHandler symbolTableHandler, ConfigurationDir configDir, int mode)
		{
			optionContainerIndex = containerIndex;
			this.mode = mode;
			ConfigurationDir = configDir;
			startTime = DateTimeHelper.CurrentUnixTimeMillis();
			configLogger = initConfigLogger(getOptionValue("config", "logfile").ToString(), getOptionValue("config", "logging").ToString());
			this.dataFormatInstance = dataFormatInstance;
			this.symbolTableHandler = symbolTableHandler;
			parserFactory = makeParserFactory();
			if (mode == LEARN)
			{
				checkOptionDependency();
			}
			initPropagation();
			initFeatureSystem();
			initParsingAlgorithm();

			if (configLogger.InfoEnabled)
			{
				URL inputFormatURL = configDir.InputFormatURL;
				URL outputFormatURL = configDir.OutputFormatURL;
				if (inputFormatURL != null)
				{
					if (outputFormatURL == null || outputFormatURL.ToString().Equals(inputFormatURL.ToString()))
					{
						int index = inputFormatURL.ToString().IndexOf('!');
						if (index == -1)
						{
							configLogger.info("  Data Format          : " + inputFormatURL.ToString() + "\n");
						}
						else
						{
							configLogger.info("  Data Format          : " + inputFormatURL.ToString().Substring(index + 1) + "\n");
						}
					}
					else
					{
						int indexIn = inputFormatURL.ToString().IndexOf('!');
						int indexOut = outputFormatURL.ToString().IndexOf('!');
						if (indexIn == -1)
						{
							configLogger.info("  Input Data Format    : " + inputFormatURL.ToString() + "\n");
						}
						else
						{
							configLogger.info("  Input Data Format    : " + inputFormatURL.ToString().Substring(indexIn + 1) + "\n");
						}
						if (indexOut == -1)
						{
							configLogger.info("  Output Data Format   : " + outputFormatURL.ToString() + "\n");
						}
						else
						{
							configLogger.info("  Output Data Format   : " + outputFormatURL.ToString().Substring(indexOut + 1) + "\n");
						}
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initPropagation() throws org.maltparser.core.exception.MaltChainedException
		private void initPropagation()
		{
			string propagationSpecFileName = getOptionValue("singlemalt", "propagation").ToString();
			if (ReferenceEquals(propagationSpecFileName, null) || propagationSpecFileName.Length == 0)
			{
				return;
			}
			propagationManager = new PropagationManager();
			if (mode == LEARN)
			{
				propagationSpecFileName = configDir.copyToConfig(propagationSpecFileName);
				OptionManager.instance().overloadOptionValue(optionContainerIndex, "singlemalt", "propagation", propagationSpecFileName);
			}
			if (LoggerInfoEnabled)
			{
				logInfoMessage("  Propagation          : " + propagationSpecFileName + "\n");
			}
			propagationManager.loadSpecification(findURL(propagationSpecFileName));
			propagationManager.createPropagations(dataFormatInstance, symbolTableHandler);
		}

		/// <summary>
		/// Initialize the parsing algorithm
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initParsingAlgorithm() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void initParsingAlgorithm()
		{
			bool diagnostics = (bool?)getOptionValue("singlemalt", "diagnostics").Value;
			if (mode == LEARN)
			{
				if (!diagnostics)
				{
					parsingAlgorithm = trainer = new BatchTrainer(this, symbolTableHandler);
				}
				else
				{
					parsingAlgorithm = trainer = new BatchTrainerWithDiagnostics(this, symbolTableHandler);
				}
			}
			else if (mode == PARSE)
			{
				if (!diagnostics)
				{
					parsingAlgorithm = parser = new DeterministicParser(this, symbolTableHandler);
				}
				else
				{
					parsingAlgorithm = parser = new DeterministicParserWithDiagnostics(this, symbolTableHandler);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initFeatureSystem() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void initFeatureSystem()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.feature.system.FeatureEngine system = new org.maltparser.core.feature.system.FeatureEngine();
			FeatureEngine system = new FeatureEngine();
			system.load("/appdata/features/ParserFeatureSystem.xml");
			system.load(PluginLoader.instance());
			featureModelManager = new FeatureModelManager(system);
			string featureModelFileName = getOptionValue("guide", "features").ToString().Trim();
			if (featureModelFileName.EndsWith(".par", StringComparison.Ordinal))
			{
				string markingStrategy = getOptionValue("pproj", "marking_strategy").ToString().Trim();
				string coveredRoot = getOptionValue("pproj", "covered_root").ToString().Trim();
				featureModelManager.loadParSpecification(findURL(featureModelFileName), markingStrategy, coveredRoot);
			}
			else
			{
				featureModelManager.loadSpecification(findURL(featureModelFileName));
			}
		}

		/// <summary>
		/// Creates a parser factory specified by the --singlemalt-parsing_algorithm option
		/// </summary>
		/// <returns> a parser factory </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private AbstractParserFactory makeParserFactory() throws org.maltparser.core.exception.MaltChainedException
		private AbstractParserFactory makeParserFactory()
		{
			Type clazz = (Type)getOptionValue("singlemalt", "parsing_algorithm");
			try
			{
				object[] arguments = new object[] {this};
				return (AbstractParserFactory)clazz.GetConstructor(paramTypes).newInstance(arguments);
			}
			catch (NoSuchMethodException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ConfigurationException("The parser factory '" + clazz.FullName + "' cannot be initialized. ", e);
			}
			catch (InstantiationException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ConfigurationException("The parser factory '" + clazz.FullName + "' cannot be initialized. ", e);
			}
			catch (IllegalAccessException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ConfigurationException("The parser factory '" + clazz.FullName + "' cannot be initialized. ", e);
			}
			catch (InvocationTargetException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ConfigurationException("The parser factory '" + clazz.FullName + "' cannot be initialized. ", e);
			}
		}

		public virtual AbstractParserFactory ParserFactory
		{
			get
			{
				return parserFactory;
			}
		}

		public virtual FeatureModelManager FeatureModelManager
		{
			get
			{
				return featureModelManager;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void process(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public virtual void process(object[] arguments)
		{
			if (mode == LEARN)
			{
				if (arguments.Length < 2 || !(arguments[0] is DependencyStructure) || !(arguments[1] is DependencyStructure))
				{
					throw new MaltChainedException("The single malt learn task must be supplied with at least two dependency structures. ");
				}
				DependencyStructure systemGraph = (DependencyStructure)arguments[0];
				DependencyStructure goldGraph = (DependencyStructure)arguments[1];
				if (systemGraph.hasTokens() && Guide != null)
				{
					Guide.finalizeSentence(((Trainer)Algorithm).parse(goldGraph, systemGraph));
				}
			}
			else if (mode == PARSE)
			{
				if (arguments.Length < 1 || !(arguments[0] is DependencyStructure))
				{
					throw new MaltChainedException("The single malt parse task must be supplied with at least one input terminal structure and one output dependency structure. ");
				}
				DependencyStructure processGraph = (DependencyStructure)arguments[0];
				if (processGraph.hasTokens())
				{
					parser.parse(processGraph);
	//				((Parser)getAlgorithm()).parse(processGraph);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parse(org.maltparser.core.syntaxgraph.DependencyStructure graph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parse(DependencyStructure graph)
		{
			if (graph.hasTokens())
			{
	//			((Parser)getAlgorithm()).parse(graph);
				parser.parse(graph);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void oracleParse(org.maltparser.core.syntaxgraph.DependencyStructure goldGraph, org.maltparser.core.syntaxgraph.DependencyStructure oracleGraph) throws org.maltparser.core.exception.MaltChainedException
		public virtual void oracleParse(DependencyStructure goldGraph, DependencyStructure oracleGraph)
		{
			if (oracleGraph.hasTokens())
			{
				if (Guide != null)
				{
					Guide.finalizeSentence(trainer.parse(goldGraph, oracleGraph));
				}
				else
				{
					trainer.parse(goldGraph, oracleGraph);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void train() throws org.maltparser.core.exception.MaltChainedException
		public virtual void train()
		{
			if (Guide == null)
			{
				((Trainer)Algorithm).train();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate(object[] arguments)
		{
	//		if (getAlgorithm() instanceof Trainer) {
	//			((Trainer)getAlgorithm()).terminate();
	//		}
			Algorithm.terminate();
			if (Guide != null)
			{
				Guide.terminate();
			}
			if (mode == LEARN)
			{
				endTime = DateTimeHelper.CurrentUnixTimeMillis();
				long elapsed = endTime - startTime;
				if (configLogger.InfoEnabled)
				{
					configLogger.info("Learning time: " + (new Formatter()).format("%02d:%02d:%02d", elapsed / 3600000, elapsed % 3600000 / 60000, elapsed % 60000 / 1000) + " (" + elapsed + " ms)\n");
				}
			}
			else if (mode == PARSE)
			{
				endTime = DateTimeHelper.CurrentUnixTimeMillis();
				long elapsed = endTime - startTime;
				if (configLogger.InfoEnabled)
				{
					configLogger.info("Parsing time: " + (new Formatter()).format("%02d:%02d:%02d", elapsed / 3600000, elapsed % 3600000 / 60000, elapsed % 60000 / 1000) + " (" + elapsed + " ms)\n");
				}
			}
			if (SystemLogger.logger() != configLogger && configLogger != null)
			{
				configLogger.removeAllAppenders();
			}
		}

		/// <summary>
		/// Initialize the configuration logger
		/// </summary>
		/// <returns> the configuration logger </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.log4j.Logger initConfigLogger(String logfile, String level) throws org.maltparser.core.exception.MaltChainedException
		public virtual Logger initConfigLogger(string logfile, string level)
		{
			if (!ReferenceEquals(logfile, null) && logfile.Length > 0 && !logfile.Equals("stdout", StringComparison.OrdinalIgnoreCase) && configDir != null)
			{
				configLogger = Logger.getLogger(logfile);
				FileAppender fileAppender = null;
				try
				{
					fileAppender = new FileAppender(new PatternLayout("%m"),configDir.WorkingDirectory.Path + File.separator + logfile, true);
				}
				catch (IOException e)
				{
					throw new ConfigurationException("It is not possible to create a configuration log file. ", e);
				}
				fileAppender.Threshold = Level.toLevel(level, Level.INFO);
				configLogger.addAppender(fileAppender);
				configLogger.Level = Level.toLevel(level, Level.INFO);
			}
			else
			{
				configLogger = SystemLogger.logger();
			}

			return configLogger;
		}

		public virtual bool LoggerInfoEnabled
		{
			get
			{
				return configLogger != null && configLogger.InfoEnabled;
			}
		}
		public virtual bool LoggerDebugEnabled
		{
			get
			{
				return configLogger != null && configLogger.DebugEnabled;
			}
		}
		public virtual void logErrorMessage(string message)
		{
			configLogger.error(message);
		}
		public virtual void logInfoMessage(string message)
		{
			configLogger.info(message);
		}
		public virtual void logInfoMessage(char character)
		{
			configLogger.info(character);
		}
		public virtual void logDebugMessage(string message)
		{
			configLogger.debug(message);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeInfoToConfigFile(String message) throws org.maltparser.core.exception.MaltChainedException
		public virtual void writeInfoToConfigFile(string message)
		{
			try
			{
				configDir.InfoFileWriter.Write(message);
				configDir.InfoFileWriter.Flush();
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Could not write to the configuration information file. ", e);

			}
		}

		public virtual Logger ConfigLogger
		{
			get
			{
				return configLogger;
			}
			set
			{
				configLogger = value;
			}
		}


		public virtual ConfigurationDir ConfigurationDir
		{
			get
			{
				return configDir;
			}
			set
			{
				configDir = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStreamWriter getOutputStreamWriter(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamWriter getOutputStreamWriter(string fileName)
		{
			return configDir.getOutputStreamWriter(fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStreamWriter getAppendOutputStreamWriter(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamWriter getAppendOutputStreamWriter(string fileName)
		{
			return configDir.getAppendOutputStreamWriter(fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStreamReader getInputStreamReader(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamReader getInputStreamReader(string fileName)
		{
			return configDir.getInputStreamReader(fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream getInputStreamFromConfigFileEntry(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual Stream getInputStreamFromConfigFileEntry(string fileName)
		{
			return configDir.getInputStreamFromConfigFileEntry(fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getConfigFileEntryURL(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual URL getConfigFileEntryURL(string fileName)
		{
			return configDir.getConfigFileEntryURL(fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.File getFile(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual File getFile(string fileName)
		{
			return configDir.getFile(fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getConfigFileEntryObject(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual object getConfigFileEntryObject(string fileName)
		{
			object @object = null;
			try
			{
				ObjectInputStream input = new ObjectInputStream(getInputStreamFromConfigFileEntry(fileName));
				try
				{
					@object = input.readObject();
				}
				catch (ClassNotFoundException e)
				{
					throw new ConfigurationException("Could not load object '" + fileName + "' from mco-file", e);
				}
				catch (Exception e)
				{
					throw new ConfigurationException("Could not load object '" + fileName + "' from mco-file", e);
				}
				finally
				{
					input.close();
				}
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Could not load object from '" + fileName + "' in mco-file", e);
			}
			return @object;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getConfigFileEntryString(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getConfigFileEntryString(string fileName)
		{
			StringBuilder sb = new StringBuilder();
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader in = new java.io.BufferedReader(new java.io.InputStreamReader(getInputStreamFromConfigFileEntry(fileName), "UTF-8"));
				StreamReader @in = new StreamReader(getInputStreamFromConfigFileEntry(fileName), Encoding.UTF8);
				string line;

				while (!ReferenceEquals((line = @in.ReadLine()), null))
				{
					 sb.Append(line);
					 sb.Append('\n');
				}
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Could not load string from '" + fileName + "' in mco-file", e);
			}
			return sb.ToString();
		}

		public virtual int Mode
		{
			get
			{
				return mode;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOptionValue(String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public virtual object getOptionValue(string optiongroup, string optionname)
		{
			return OptionManager.instance().getOptionValue(optionContainerIndex, optiongroup, optionname);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getOptionValueString(String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getOptionValueString(string optiongroup, string optionname)
		{
			return OptionManager.instance().getOptionValueString(optionContainerIndex, optiongroup, optionname);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.options.OptionManager getOptionManager() throws org.maltparser.core.exception.MaltChainedException
		public virtual OptionManager OptionManager
		{
			get
			{
				return OptionManager.instance();
			}
		}
		/// <summary>
		///****************************** MaltParserConfiguration specific  ******************************* </summary>

		/// <summary>
		/// Returns the list of symbol tables
		/// </summary>
		/// <returns> the list of symbol tables </returns>
		public virtual SymbolTableHandler SymbolTables
		{
			get
			{
				return symbolTableHandler;
			}
		}

		public virtual DataFormatInstance DataFormatInstance
		{
			get
			{
				return dataFormatInstance;
			}
		}

		public virtual PropagationManager PropagationManager
		{
			get
			{
				return propagationManager;
			}
		}

		public virtual ParsingAlgorithm Algorithm
		{
			get
			{
				return parsingAlgorithm;
			}
		}
		/// <summary>
		/// Returns the guide
		/// </summary>
		/// <returns> the guide </returns>
		public virtual ClassifierGuide Guide
		{
			get
			{
				return parsingAlgorithm.Guide;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkOptionDependency() throws org.maltparser.core.exception.MaltChainedException
		public virtual void checkOptionDependency()
		{
			try
			{
				if (configDir.InfoFileWriter != null)
				{
					configDir.InfoFileWriter.Write("\nDEPENDENCIES\n");
				}

				// Copy the feature model file into the configuration directory
				string featureModelFileName = getOptionValue("guide", "features").ToString().Trim();
				if (featureModelFileName.Equals(""))
				{

					// use default feature model depending on the selected parser algorithm
					OptionManager.instance().overloadOptionValue(optionContainerIndex, "guide", "features", getOptionValueString("singlemalt", "parsing_algorithm"));
					featureModelFileName = getOptionValue("guide", "features").ToString().Trim();

					/* START: Temp fix during development of new liblinear and libsvm interface */
					string learner = getOptionValueString("guide", "learner");
					if (!learner.StartsWith("lib", StringComparison.Ordinal))
					{
						learner = "lib" + learner;
					}
					/* END: Temp fix during development of new liblinear and libsvm interface */
					featureModelFileName = featureModelFileName.Replace("{learner}", learner);
					featureModelFileName = featureModelFileName.Replace("{dataformat}", getOptionValue("input", "format").ToString().Trim().Replace(".xml", ""));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
					URLFinder f = new URLFinder();
					featureModelFileName = configDir.copyToConfig(f.findURLinJars(featureModelFileName));
				}
				else
				{
					featureModelFileName = configDir.copyToConfig(featureModelFileName);
				}
				OptionManager.instance().overloadOptionValue(optionContainerIndex, "guide", "features", featureModelFileName);
				if (configDir.InfoFileWriter != null)
				{
					configDir.InfoFileWriter.Write("--guide-features (  -F)                 " + getOptionValue("guide", "features").ToString() + "\n");
				}

				if (getOptionValue("guide", "data_split_column").ToString().Equals("") && !getOptionValue("guide", "data_split_structure").ToString().Equals(""))
				{
					configLogger.warn("Option --guide-data_split_column = '' and --guide-data_split_structure != ''. Option --guide-data_split_structure is overloaded with '', this will cause the parser to induce a single model.\n ");
					OptionManager.instance().overloadOptionValue(optionContainerIndex, "guide", "data_split_structure", "");
					if (configDir.InfoFileWriter != null)
					{
						configDir.InfoFileWriter.Write("--guide-data_split_structure (  -s)\n");
					}
				}
				if (!getOptionValue("guide", "data_split_column").ToString().Equals("") && getOptionValue("guide", "data_split_structure").ToString().Equals(""))
				{
					configLogger.warn("Option --guide-data_split_column != '' and --guide-data_split_structure = ''. Option --guide-data_split_column is overloaded with '', this will cause the parser to induce a single model.\n");
					OptionManager.instance().overloadOptionValue(optionContainerIndex, "guide", "data_split_column", "");
					if (configDir.InfoFileWriter != null)
					{
						configDir.InfoFileWriter.Write("--guide-data_split_column (  -d)\n");
					}
				}

				string decisionSettings = getOptionValue("guide", "decision_settings").ToString().Trim();
				string markingStrategy = getOptionValue("pproj", "marking_strategy").ToString().Trim();
				string coveredRoot = getOptionValue("pproj", "covered_root").ToString().Trim();
				StringBuilder newDecisionSettings = new StringBuilder();

				if (ReferenceEquals(decisionSettings, null) || decisionSettings.Length < 1 || decisionSettings.Equals("default"))
				{
					decisionSettings = "T.TRANS+A.DEPREL";
				}
				else
				{
					decisionSettings = decisionSettings.ToUpper();
				}

				if (markingStrategy.Equals("head", StringComparison.OrdinalIgnoreCase) || markingStrategy.Equals("path", StringComparison.OrdinalIgnoreCase) || markingStrategy.Equals("head+path", StringComparison.OrdinalIgnoreCase))
				{
					if (!Pattern.matches(".*A\\.PPLIFTED.*", decisionSettings))
					{
						newDecisionSettings.Append("+A.PPLIFTED");
					}
				}
				if (markingStrategy.Equals("path", StringComparison.OrdinalIgnoreCase) || markingStrategy.Equals("head+path", StringComparison.OrdinalIgnoreCase))
				{
					if (!Pattern.matches(".*A\\.PPPATH.*", decisionSettings))
					{
						newDecisionSettings.Append("+A.PPPATH");
					}
				}
				if (!coveredRoot.Equals("none", StringComparison.OrdinalIgnoreCase) && !Pattern.matches(".*A\\.PPCOVERED.*", decisionSettings))
				{
					newDecisionSettings.Append("+A.PPCOVERED");
				}
				if (!getOptionValue("guide", "decision_settings").ToString().Equals(decisionSettings) || newDecisionSettings.Length > 0)
				{
					OptionManager.instance().overloadOptionValue(optionContainerIndex, "guide", "decision_settings", decisionSettings + newDecisionSettings.ToString());
					if (configDir.InfoFileWriter != null)
					{
						configDir.InfoFileWriter.Write("--guide-decision_settings (  -gds)                 " + getOptionValue("guide", "decision_settings").ToString() + "\n");
					}
				}
				if (configDir.InfoFileWriter != null)
				{
					configDir.InfoFileWriter.Flush();
				}
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Could not write to the configuration information file. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.net.URL findURL(String propagationSpecFileName) throws org.maltparser.core.exception.MaltChainedException
		private URL findURL(string propagationSpecFileName)
		{
			URL url = null;
			File specFile = configDir.getFile(propagationSpecFileName);
			if (specFile.exists())
			{
				try
				{
					url = new URL("file:///" + specFile.AbsolutePath);
				}
				catch (MalformedURLException e)
				{
					throw new PropagationException("Malformed URL: " + specFile, e);
				}
			}
			else
			{
				url = configDir.getConfigFileEntryURL(propagationSpecFileName);
			}
			return url;
		}
	}

}