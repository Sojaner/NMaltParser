using System;
using System.Collections.Generic;
using System.IO;

namespace org.maltparser.core.lw.parser
{

	using  org.maltparser.concurrent.graph.dataformat;
	using  config;
    using  feature;
	using  io.dataformat;
	using  graph;
    using  options;
	using  propagation;
	using  symbol;
	using  symbol.parse;
	using  syntaxgraph;
	using  org.maltparser.parser;

    /// <summary>
	///  A lightweight version of org.maltparser.parser.SingleMalt. This class can only perform parsing and is used by 
	///  the concurrent MaltParser model. 
	/// 
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class LWSingleMalt : DependencyParserConfig
	{
		public static readonly Type[] paramTypes = new Type[] {typeof(DependencyParserConfig)};
		private readonly McoModel mcoModel;
		private readonly int optionContainerIndex;
		private readonly DataFormatInstance dataFormatInstance;
		private readonly PropagationManager propagationManager;
		private readonly FeatureModelManager featureModelManager;
		private readonly AbstractParserFactory parserFactory;
		private readonly string decisionSettings;
		private readonly int kBestSize;
		private readonly string classitem_separator;
		private readonly URL featureModelURL;
		private readonly string dataSplitColumn;
		private readonly string dataSplitStructure;
		private readonly bool excludeNullValues;
		private readonly LWDecisionModel decisionModel;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LWSingleMalt(int containerIndex, org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, McoModel _mcoModel, org.maltparser.core.propagation.PropagationManager _propagationManager, org.maltparser.core.feature.FeatureModelManager _featureModelManager) throws org.maltparser.core.exception.MaltChainedException
		public LWSingleMalt(int containerIndex, DataFormatInstance dataFormatInstance, McoModel _mcoModel, PropagationManager _propagationManager, FeatureModelManager _featureModelManager)
		{
			optionContainerIndex = containerIndex;
			mcoModel = _mcoModel;
			this.dataFormatInstance = dataFormatInstance;
			propagationManager = _propagationManager;
			featureModelManager = _featureModelManager;
			parserFactory = makeParserFactory();
			decisionSettings = getOptionValue("guide", "decision_settings").ToString().Trim();
			kBestSize = ((int?)getOptionValue("guide", "kbest")).Value;
			classitem_separator = getOptionValue("guide", "classitem_separator").ToString().Trim();
			featureModelURL = getConfigFileEntryURL(getOptionValue("guide", "features").ToString().Trim());
			dataSplitColumn = getOptionValue("guide", "data_split_column").ToString().Trim();
			dataSplitStructure = getOptionValue("guide", "data_split_structure").ToString().Trim();
			excludeNullValues = getOptionValue("singlemalt", "null_value").ToString().Equals("none", StringComparison.OrdinalIgnoreCase);
			decisionModel = new LWDecisionModel(mcoModel, excludeNullValues, getOptionValueString("guide","learner"));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.maltparser.parser.AbstractParserFactory makeParserFactory() throws org.maltparser.core.exception.MaltChainedException
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
		public FeatureModelManager FeatureModelManager
		{
			get
			{
				return featureModelManager;
			}
		}

		public AbstractParserFactory ParserFactory
		{
			get
			{
				return parserFactory;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parse(org.maltparser.core.syntaxgraph.DependencyStructure graph) throws org.maltparser.core.exception.MaltChainedException
		public void parse(DependencyStructure graph)
		{
			if (graph.hasTokens())
			{
				LWDeterministicParser parser = new LWDeterministicParser(this, graph.SymbolTables);
				parser.parse(graph);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.List<String[]> parseSentences(java.util.List<String[]> inputSentences, String defaultRootLabel, int markingStrategy, boolean coveredRoot, org.maltparser.core.symbol.SymbolTableHandler parentSymbolTableHandler, org.maltparser.concurrent.graph.dataformat.DataFormat concurrentDataFormat) throws org.maltparser.core.exception.MaltChainedException
		public IList<string[]> parseSentences(IList<string[]> inputSentences, string defaultRootLabel, int markingStrategy, bool coveredRoot, SymbolTableHandler parentSymbolTableHandler, DataFormat concurrentDataFormat)
		{
			IList<string[]> outputSentences = Collections.synchronizedList(new List<string[]>());
			SymbolTableHandler parseSymbolTableHandler = new ParseSymbolTableHandler(parentSymbolTableHandler);
			LWDependencyGraph parseGraph = new LWDependencyGraph(concurrentDataFormat, parseSymbolTableHandler);
			LWDeterministicParser parser = new LWDeterministicParser(this, parseSymbolTableHandler);

			for (int i = 0; i < inputSentences.Count; i++)
			{
				string[] tokens = inputSentences[i];
				// TODO nothing to parse
				parseGraph.resetTokens(tokens, defaultRootLabel, false);
				parser.parse(parseGraph);
				if (markingStrategy != 0 || coveredRoot)
				{
					(new LWDeprojectivizer()).deprojectivize(parseGraph, markingStrategy);
				}
				string[] outputTokens = new string[tokens.Length];
				for (int j = 0; j < outputTokens.Length; j++)
				{
					outputTokens[j] = parseGraph.getDependencyNode(j + 1).ToString();
				}
				outputSentences.Add(outputTokens);
			}
			return outputSentences;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void oracleParse(org.maltparser.core.syntaxgraph.DependencyStructure goldGraph, org.maltparser.core.syntaxgraph.DependencyStructure oracleGraph) throws org.maltparser.core.exception.MaltChainedException
		public void oracleParse(DependencyStructure goldGraph, DependencyStructure oracleGraph)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException
		public void terminate(object[] arguments)
		{
		}

		public bool LoggerInfoEnabled
		{
			get
			{
				return false;
			}
		}
		public bool LoggerDebugEnabled
		{
			get
			{
				return false;
			}
		}
		public void logErrorMessage(string message)
		{
		}
		public void logInfoMessage(string message)
		{
		}
		public void logInfoMessage(char character)
		{
		}
		public void logDebugMessage(string message)
		{
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeInfoToConfigFile(String message) throws org.maltparser.core.exception.MaltChainedException
		public void writeInfoToConfigFile(string message)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStreamWriter getOutputStreamWriter(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public StreamWriter getOutputStreamWriter(string fileName)
		{
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStreamWriter getAppendOutputStreamWriter(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public StreamWriter getAppendOutputStreamWriter(string fileName)
		{
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStreamReader getInputStreamReader(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public StreamReader getInputStreamReader(string fileName)
		{
			try
			{
				return mcoModel.getInputStreamReader(fileName, "UTF-8");
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Couldn't read file " + fileName + " from mco-file ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream getInputStreamFromConfigFileEntry(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public Stream getInputStreamFromConfigFileEntry(string fileName)
		{
			try
			{
				return mcoModel.getInputStream(fileName);
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Couldn't read file " + fileName + " from mco-file ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getConfigFileEntryURL(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public URL getConfigFileEntryURL(string fileName)
		{
			try
			{
				return mcoModel.getMcoEntryURL(fileName);
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Couldn't read file " + fileName + " from mco-file ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getConfigFileEntryObject(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public object getConfigFileEntryObject(string fileName)
		{
			return mcoModel.getMcoEntryObject(fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getConfigFileEntryString(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public string getConfigFileEntryString(string fileName)
		{
			return mcoModel.getMcoEntryString(fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.File getFile(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public File getFile(string fileName)
		{
			return new File(System.getProperty("user.dir") + File.separator + fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOptionValue(String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public object getOptionValue(string optiongroup, string optionname)
		{
			return OptionManager.instance().getOptionValue(optionContainerIndex, optiongroup, optionname);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getOptionValueString(String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public string getOptionValueString(string optiongroup, string optionname)
		{
			return OptionManager.instance().getOptionValueString(optionContainerIndex, optiongroup, optionname);
		}

		public SymbolTableHandler SymbolTables
		{
			get
			{
				return null;
			}
		}

		public DataFormatInstance DataFormatInstance
		{
			get
			{
				return dataFormatInstance;
			}
		}

		public PropagationManager PropagationManager
		{
			get
			{
				return propagationManager;
			}
		}

		public string DecisionSettings
		{
			get
			{
				return decisionSettings;
			}
		}

		public int getkBestSize()
		{
			return kBestSize;
		}

		public string Classitem_separator
		{
			get
			{
				return classitem_separator;
			}
		}

		public URL FeatureModelURL
		{
			get
			{
				return featureModelURL;
			}
		}

		public string DataSplitColumn
		{
			get
			{
				return dataSplitColumn;
			}
		}

		public string DataSplitStructure
		{
			get
			{
				return dataSplitStructure;
			}
		}

		public bool ExcludeNullValues
		{
			get
			{
				return excludeNullValues;
			}
		}

		public LWDecisionModel DecisionModel
		{
			get
			{
				return decisionModel;
			}
		}
	}

}