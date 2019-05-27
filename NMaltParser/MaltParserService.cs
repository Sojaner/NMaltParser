using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Flow;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Options;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.Symbol.Hash;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Core.SyntaxGraph.Edge;
using NMaltParser.Core.SyntaxGraph.Node;
using NMaltParser.Parser;
using NMaltParser.Utilities;

namespace NMaltParser
{
    /// <summary>
	/// The purpose of MaltParserService is to easily write third-party programs that uses MaltParser. 
	/// 
	///  There are two ways to call the MaltParserService:
	///  1. By running experiments, which allow other programs to train a parser model or parse with a parser model. IO-handling is done by MaltParser.
	///  2. By first initialize a parser model and then call the method parse() with an array of tokens that MaltParser parses. IO-handling of the sentence is
	///  done by the third-party program.
	/// 
	///  How to use MaltParserService, please see the examples provided in the directory 'examples/apiexamples/srcex'
	/// 
	///  Note: This class is not thread-safe and it is not possible parser sentences concurrently with this interface. Please use 
	///  the new interface org.maltparser.concurrent.ConcurrentMaltParserService to parse sentences in a multi threaded environment
	/// 
	/// @author Johan Hall
	/// </summary>
	public class MaltParserService
	{
		private Engine engine;
		private FlowChartInstance flowChartInstance;
		private DataFormatInstance dataFormatInstance;
		private SingleMalt singleMalt;
		private int optionContainer;
		private bool initialized = false;

		/// <summary>
		/// Creates a MaltParserService with the option container 0
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MaltParserService() throws org.maltparser.core.exception.MaltChainedException
		public MaltParserService() : this(0)
		{
		}

		/// <summary>
		/// Creates a MaltParserService with the specified option container. To use different option containers allows the calling program 
		/// to load several parser models or several experiments. The option management in MaltParser uses the singleton design pattern, which means that there can only
		/// be one instance of the option manager. To be able to have several parser models or experiments at same time please use different option containers.
		/// </summary>
		/// <param name="optionContainer"> an integer from 0 to max value of data type Integer </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MaltParserService(int optionContainer) throws org.maltparser.core.exception.MaltChainedException
		public MaltParserService(int optionContainer)
		{
			this.optionContainer = optionContainer;
			initialize();
		}

		/// <summary>
		/// Use this constructor only when you want a MaltParserService without an option manager. Without the option manager MaltParser cannot
		/// load or create a parser model. 
		/// </summary>
		/// <param name="optionFreeInitialization"> true, means that MaltParserService is created without an option manager, false will do the same as MaltParserService(). </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MaltParserService(boolean optionFreeInitialization) throws org.maltparser.core.exception.MaltChainedException
		public MaltParserService(bool optionFreeInitialization)
		{
			if (optionFreeInitialization == false)
			{
				optionContainer = 0;
				initialize();
			}
			else
			{
				optionContainer = -1;
			}
		}

		/// <summary>
		/// Runs a MaltParser experiment. The experiment is controlled by a commandLine string, please see the documentation of MaltParser to see all available options.
		/// </summary>
		/// <param name="commandLine"> a commandLine string that controls the MaltParser. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void runExperiment(String commandLine) throws org.maltparser.core.exception.MaltChainedException
		public virtual void runExperiment(string commandLine)
		{
			OptionManager.instance().parseCommandLine(commandLine, optionContainer);
			engine = new Engine();
			engine.initialize(optionContainer);
			engine.process(optionContainer);
			engine.terminate(optionContainer);
		}

		/// <summary>
		/// Initialize a parser model that later can by used to parse sentences. MaltParser is controlled by a commandLine string, please see the documentation of MaltParser to see all available options.
		/// </summary>
		/// <param name="commandLine"> a commandLine string that controls the MaltParser </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initializeParserModel(String commandLine) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initializeParserModel(string commandLine)
		{
			if (optionContainer == -1)
			{
				throw new MaltChainedException("MaltParserService has been initialized as an option free initialization and therefore no parser model can be initialized.");
			}
			OptionManager.instance().parseCommandLine(commandLine, optionContainer);
			// Creates an engine
			engine = new Engine();
			// Initialize the engine with option container and gets a flow chart instance
			flowChartInstance = engine.initialize(optionContainer);
			// Runs the preprocess chart items of the "parse" flow chart
			if (flowChartInstance.hasPreProcessChartItems())
			{
				flowChartInstance.preprocess();
			}
			singleMalt = (SingleMalt)flowChartInstance.getFlowChartRegistry(typeof(SingleMalt), "singlemalt");
			singleMalt.ConfigurationDir.initDataFormat();
			dataFormatInstance = singleMalt.ConfigurationDir.DataFormatManager.InputDataFormatSpec.createDataFormatInstance(singleMalt.SymbolTables, OptionManager.instance().getOptionValueString(optionContainer, "singlemalt", "null_value"));
			initialized = true;
		}



		/// <summary>
		/// Parses an array of tokens and returns a dependency structure. 
		/// 
		/// Note: To call this method requires that a parser model has been initialized by using the initializeParserModel(). 
		/// </summary>
		/// <param name="tokens"> an array of tokens </param>
		/// <returns> a dependency structure </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.DependencyStructure parse(String[] tokens) throws org.maltparser.core.exception.MaltChainedException
		public virtual IDependencyStructure parse(string[] tokens)
		{
			if (!initialized)
			{
				throw new MaltChainedException("No parser model has been initialized. Please use the method initializeParserModel() before invoking this method.");
			}
			if (tokens == null || tokens.Length == 0)
			{
				throw new MaltChainedException("Nothing to parse. ");
			}

			IDependencyStructure outputGraph = new DependencyGraph(singleMalt.SymbolTables);

			for (int i = 0; i < tokens.Length; i++)
			{
				IEnumerator<ColumnDescription> columns = dataFormatInstance.GetEnumerator();
				DependencyNode node = outputGraph.AddDependencyNode(i + 1);
				string[] items = tokens[i].Split("\t", true);
				for (int j = 0; j < items.Length; j++)
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (columns.hasNext())
					{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						ColumnDescription column = columns.next();
						if (column.Category == ColumnDescription.INPUT && node != null)
						{
							outputGraph.AddLabel(node, column.Name, items[j]);
						}
					}
				}
			}
			outputGraph.SetDefaultRootEdgeLabel(outputGraph.SymbolTables.getSymbolTable("DEPREL"), "ROOT");
			// Invoke parse with the output graph
			singleMalt.Parse(outputGraph);
			return outputGraph;
		}

		/// <summary>
		/// Converts an array of tokens to a dependency structure. 
		/// 
		/// </summary>
		/// <param name="tokens"> an array of tokens </param>
		/// <returns> a dependency structure </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.DependencyStructure toDependencyStructure(String[] tokens) throws org.maltparser.core.exception.MaltChainedException
		public virtual IDependencyStructure toDependencyStructure(string[] tokens)
		{
			if (!initialized)
			{
				throw new MaltChainedException("No parser model has been initialized. Please use the method initializeParserModel() before invoking this method.");
			}
			if (tokens == null || tokens.Length == 0)
			{
				throw new MaltChainedException("Nothing to convert. ");
			}
			IDependencyStructure outputGraph = new DependencyGraph(singleMalt.SymbolTables);

			for (int i = 0; i < tokens.Length; i++)
			{
				IEnumerator<ColumnDescription> columns = dataFormatInstance.GetEnumerator();
				DependencyNode node = outputGraph.AddDependencyNode(i + 1);
				string[] items = tokens[i].Split("\t", true);
				Edge edge = null;
				for (int j = 0; j < items.Length; j++)
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (columns.hasNext())
					{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						ColumnDescription column = columns.next();
						if (column.Category == ColumnDescription.INPUT && node != null)
						{
							outputGraph.AddLabel(node, column.Name, items[j]);
						}
						else if (column.Category == ColumnDescription.HEAD)
						{
							if (column.Category != ColumnDescription.IGNORE && !items[j].Equals("_"))
							{
								edge = ((IDependencyStructure)outputGraph).AddDependencyEdge(int.Parse(items[j]), i + 1);
							}
						}
						else if (column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL && edge != null)
						{
							outputGraph.AddLabel(edge, column.Name, items[j]);
						}
					}
				}
			}
			outputGraph.SetDefaultRootEdgeLabel(outputGraph.SymbolTables.getSymbolTable("DEPREL"), "ROOT");
			return outputGraph;
		}

		/// <summary>
		/// Reads the data format specification file
		/// </summary>
		/// <param name="dataFormatFileName"> the path to the data format specification file </param>
		/// <returns> a data format specification </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.io.dataformat.DataFormatSpecification readDataFormatSpecification(String dataFormatFileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual DataFormatSpecification readDataFormatSpecification(string dataFormatFileName)
		{
			DataFormatSpecification dataFormat = new DataFormatSpecification();
			dataFormat.parseDataFormatXMLfile(dataFormatFileName);
			return dataFormat;
		}

		/// <summary>
		/// Converts an array of tokens to a dependency structure
		/// </summary>
		/// <param name="tokens"> tokens an array of tokens </param>
		/// <param name="dataFormatSpecification"> a data format specification </param>
		/// <returns> a dependency structure </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.DependencyStructure toDependencyStructure(String[] tokens, org.maltparser.core.io.dataformat.DataFormatSpecification dataFormatSpecification) throws org.maltparser.core.exception.MaltChainedException
		public virtual IDependencyStructure toDependencyStructure(string[] tokens, DataFormatSpecification dataFormatSpecification)
		{
			// Creates a symbol table handler
			SymbolTableHandler symbolTables = new HashSymbolTableHandler();

			// Initialize data format instance
			DataFormatInstance dataFormatInstance = dataFormatSpecification.createDataFormatInstance(symbolTables, "none");

			// Creates a dependency graph
			if (tokens == null || tokens.Length == 0)
			{
				throw new MaltChainedException("Nothing to convert. ");
			}
			IDependencyStructure outputGraph = new DependencyGraph(symbolTables);

			for (int i = 0; i < tokens.Length; i++)
			{
				IEnumerator<ColumnDescription> columns = dataFormatInstance.GetEnumerator();
				DependencyNode node = outputGraph.AddDependencyNode(i + 1);
				string[] items = tokens[i].Split("\t", true);
				Edge edge = null;
				for (int j = 0; j < items.Length; j++)
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					if (columns.hasNext())
					{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						ColumnDescription column = columns.next();
						if (column.Category == ColumnDescription.INPUT && node != null)
						{
							outputGraph.AddLabel(node, column.Name, items[j]);
						}
						else if (column.Category == ColumnDescription.HEAD)
						{
							if (column.Category != ColumnDescription.IGNORE && !items[j].Equals("_"))
							{
								edge = ((IDependencyStructure)outputGraph).AddDependencyEdge(int.Parse(items[j]), i + 1);
							}
						}
						else if (column.Category == ColumnDescription.DEPENDENCY_EDGE_LABEL && edge != null)
						{
							outputGraph.AddLabel(edge, column.Name, items[j]);
						}
					}
				}
			}
			outputGraph.SetDefaultRootEdgeLabel(outputGraph.SymbolTables.getSymbolTable("DEPREL"), "ROOT");
			return outputGraph;
		}

		/// <summary>
		/// Converts an array of tokens to a dependency structure
		/// </summary>
		/// <param name="tokens"> an array of tokens </param>
		/// <param name="dataFormatFileName"> the path to the data format file </param>
		/// <returns> a dependency structure </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.DependencyStructure toDependencyStructure(String[] tokens, String dataFormatFileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual IDependencyStructure toDependencyStructure(string[] tokens, string dataFormatFileName)
		{
			return toDependencyStructure(tokens, readDataFormatSpecification(dataFormatFileName));
		}

		/// <summary>
		/// Same as parse(String[] tokens), but instead it returns an array of tokens with a head index and a dependency type at the end of string
		/// </summary>
		/// <param name="tokens"> an array of tokens to parse </param>
		/// <returns> an array of tokens with a head index and a dependency type at the end of string </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String[] parseTokens(String[] tokens) throws org.maltparser.core.exception.MaltChainedException
		public virtual string[] parseTokens(string[] tokens)
		{
			IDependencyStructure outputGraph = parse(tokens);
			StringBuilder sb = new StringBuilder();
			string[] outputTokens = new string[tokens.Length];
			SymbolTable deprelTable = outputGraph.SymbolTables.getSymbolTable("DEPREL");
			foreach (int? index in outputGraph.TokenIndices)
			{
				sb.Length = 0;
				if (index <= tokens.Length)
				{
					DependencyNode node = outputGraph.GetDependencyNode(index.Value);
					sb.Append(tokens[index - 1]);
					sb.Append('\t');
					sb.Append(node.Head.Index);
					sb.Append('\t');
					if (node.HeadEdge.hasLabel(deprelTable))
					{
						sb.Append(node.HeadEdge.getLabelSymbol(deprelTable));
					}
					else
					{
						sb.Append(outputGraph.GetDefaultRootEdgeLabelSymbol(deprelTable));
					}
					outputTokens[index - 1] = sb.ToString();
				}
			}
			return outputTokens;
		}

		/// <summary>
		/// Terminates the parser model.
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminateParserModel() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminateParserModel()
		{
			if (!initialized)
			{
				throw new MaltChainedException("No parser model has been initialized. Please use the method initializeParserModel() before invoking this method.");
			}
			// Runs the postprocess chart items of the "parse" flow chart
			if (flowChartInstance.hasPostProcessChartItems())
			{
				flowChartInstance.postprocess();
			}

			// Terminate the flow chart with an option container
			engine.terminate(optionContainer);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initialize() throws org.maltparser.core.exception.MaltChainedException
		private void initialize()
		{
			if (OptionManager.instance().OptionDescriptions.OptionGroupNameSet.Count > 0)
			{
				return; // OptionManager is already initialized
			}
			OptionManager.instance().loadOptionDescriptionFile();
			OptionManager.instance().generateMaps();
		}


		/// <summary>
		/// Returns the option container index
		/// </summary>
		/// <returns> the option container index </returns>
		public virtual int OptionContainer
		{
			get
			{
				return optionContainer;
			}
		}
	}

}