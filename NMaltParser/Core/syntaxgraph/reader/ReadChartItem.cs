using System;
using System.Text;
using System.IO;

namespace org.maltparser.core.syntaxgraph.reader
{

	using ConfigurationDir = org.maltparser.core.config.ConfigurationDir;
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FlowChartInstance = org.maltparser.core.flow.FlowChartInstance;
	using ChartItem = org.maltparser.core.flow.item.ChartItem;
	using ChartItemSpecification = org.maltparser.core.flow.spec.ChartItemSpecification;
	using URLFinder = org.maltparser.core.helper.URLFinder;
	using DataFormatException = org.maltparser.core.io.dataformat.DataFormatException;
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	using DataFormatManager = org.maltparser.core.io.dataformat.DataFormatManager;
	using OptionManager = org.maltparser.core.options.OptionManager;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;

	public class ReadChartItem : ChartItem
	{
		private string idName;
		private string inputFormatName;
		private string inputFileName;
		private string inputCharSet;
		private string readerOptions;
		private int iterations;
		private Type graphReaderClass;

		private string nullValueStrategy;

		private SyntaxGraphReader reader;
		private string targetName;
		private string optiongroupName;
		private DataFormatInstance inputDataFormatInstance;
		private TokenStructure cachedGraph = null;

		public ReadChartItem() : base()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(org.maltparser.core.flow.FlowChartInstance flowChartinstance, org.maltparser.core.flow.spec.ChartItemSpecification chartItemSpecification) throws org.maltparser.core.exception.MaltChainedException
		public override void initialize(FlowChartInstance flowChartinstance, ChartItemSpecification chartItemSpecification)
		{
			base.initialize(flowChartinstance, chartItemSpecification);

			foreach (string key in chartItemSpecification.ChartItemAttributes.Keys)
			{
				if (key.Equals("id"))
				{
					idName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("target"))
				{
					targetName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("optiongroup"))
				{
					optiongroupName = chartItemSpecification.ChartItemAttributes[key];
				}
			}

			if (string.ReferenceEquals(idName, null))
			{
				idName = getChartElement("read").Attributes.get("id").DefaultValue;
			}
			else if (string.ReferenceEquals(targetName, null))
			{
				targetName = getChartElement("read").Attributes.get("target").DefaultValue;
			}
			else if (string.ReferenceEquals(optiongroupName, null))
			{
				optiongroupName = getChartElement("read").Attributes.get("optiongroup").DefaultValue;
			}

			InputFormatName = OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "format").ToString();
			InputFileName = OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "infile").ToString();
			InputCharSet = OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "charset").ToString();
			ReaderOptions = OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "reader_options").ToString();
			if (OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "iterations") != null)
			{
				Iterations = (int?)OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "iterations").Value;
			}
			else
			{
				Iterations = 1;
			}
			SyntaxGraphReaderClass = (Type)OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "reader");

			NullValueStrategy = OptionManager.instance().getOptionValue(OptionContainerIndex, "singlemalt", "null_value").ToString();

			initInput(NullValueStrategy);
			initReader(SyntaxGraphReaderClass, InputFileName, InputCharSet, ReaderOptions, iterations);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int preprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int preprocess(int signal)
		{
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int process(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int process(int signal)
		{
			if (cachedGraph == null)
			{
				cachedGraph = (TokenStructure)flowChartinstance.getFlowChartRegistry(typeof(TokenStructure), targetName);
			}
			int prevIterationCounter = reader.IterationCounter;
			bool moreInput = reader.readSentence(cachedGraph);
			if (!moreInput)
			{
				return ChartItem.TERMINATE;
			}
			else if (prevIterationCounter < reader.IterationCounter)
			{
				return ChartItem.NEWITERATION;
			}
			return ChartItem.CONTINUE;
	//		return continueNextSentence && moreInput;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int postprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int postprocess(int signal)
		{
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
			if (reader != null)
			{
				reader.close();
				reader = null;
			}
			cachedGraph = null;
			inputDataFormatInstance = null;
		}

		public virtual string InputFormatName
		{
			get
			{
				if (string.ReferenceEquals(inputFormatName, null))
				{
					return "/appdata/dataformat/conllx.xml";
				}
				return inputFormatName;
			}
			set
			{
				this.inputFormatName = value;
			}
		}


		public virtual string InputFileName
		{
			get
			{
				if (string.ReferenceEquals(inputFileName, null))
				{
					return "/dev/stdin";
				}
				return inputFileName;
			}
			set
			{
				this.inputFileName = value;
			}
		}


		public virtual string InputCharSet
		{
			get
			{
				if (string.ReferenceEquals(inputCharSet, null))
				{
					return "UTF-8";
				}
				return inputCharSet;
			}
			set
			{
				this.inputCharSet = value;
			}
		}


		public virtual string ReaderOptions
		{
			get
			{
				if (string.ReferenceEquals(readerOptions, null))
				{
					return "";
				}
				return readerOptions;
			}
			set
			{
				this.readerOptions = value;
			}
		}



		public virtual int Iterations
		{
			get
			{
				return iterations;
			}
			set
			{
				this.iterations = value;
			}
		}


		public virtual Type SyntaxGraphReaderClass
		{
			get
			{
				return graphReaderClass;
			}
			set
			{
				try
				{
					if (value != null)
					{
						this.graphReaderClass = value.asSubclass(typeof(org.maltparser.core.syntaxgraph.reader.SyntaxGraphReader));
					}
				}
				catch (System.InvalidCastException e)
				{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new DataFormatException("The class '" + value.FullName + "' is not a subclass of '" + typeof(org.maltparser.core.syntaxgraph.reader.SyntaxGraphReader).FullName + "'. ", e);
				}
			}
		}


		public virtual string NullValueStrategy
		{
			get
			{
				if (string.ReferenceEquals(nullValueStrategy, null))
				{
					return "one";
				}
				return nullValueStrategy;
			}
			set
			{
				this.nullValueStrategy = value;
			}
		}


		public virtual string TargetName
		{
			get
			{
				return targetName;
			}
			set
			{
				this.targetName = value;
			}
		}


		public virtual SyntaxGraphReader Reader
		{
			get
			{
				return reader;
			}
		}

		public virtual DataFormatInstance InputDataFormatInstance
		{
			get
			{
				return inputDataFormatInstance;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initInput(String nullValueStategy) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initInput(string nullValueStategy)
		{
			ConfigurationDir configDir = (ConfigurationDir)flowChartinstance.getFlowChartRegistry(typeof(ConfigurationDir), idName);
			DataFormatManager dataFormatManager = configDir.DataFormatManager;
			SymbolTableHandler symbolTables = configDir.SymbolTables;
			inputDataFormatInstance = dataFormatManager.InputDataFormatSpec.createDataFormatInstance(symbolTables, nullValueStategy);
			configDir.addDataFormatInstance(dataFormatManager.InputDataFormatSpec.DataFormatName, inputDataFormatInstance);

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initReader(Class syntaxGraphReader, String inputFile, String inputCharSet, String readerOptions, int iterations) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initReader(Type syntaxGraphReader, string inputFile, string inputCharSet, string readerOptions, int iterations)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
				URLFinder f = new URLFinder();
				reader = System.Activator.CreateInstance(syntaxGraphReader);
				if (string.ReferenceEquals(inputFile, null) || inputFile.Length == 0 || inputFile.Equals("/dev/stdin"))
				{
					reader.open(System.in, inputCharSet);
				}
				else if (Directory.Exists(inputFile) || File.Exists(inputFile))
				{
					reader.NIterations = iterations;
					reader.open(inputFile, inputCharSet);
				}
				else
				{
					reader.NIterations = iterations;
					reader.open(f.findURL(inputFile), inputCharSet);
				}
				reader.DataFormatInstance = inputDataFormatInstance;
				reader.Options = readerOptions;
			}
			catch (InstantiationException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DataFormatException("The data reader '" + syntaxGraphReader.FullName + "' cannot be initialized. ", e);
			}
			catch (IllegalAccessException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DataFormatException("The data reader '" + syntaxGraphReader.FullName + "' cannot be initialized. ", e);
			}
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			return obj.ToString().Equals(this.ToString());
		}

		public override int GetHashCode()
		{
			return 217 + (null == ToString() ? 0 : ToString().GetHashCode());
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("    read ");
			sb.Append("id:");
			sb.Append(idName);
			sb.Append(' ');
			sb.Append("target:");
			sb.Append(targetName);
			sb.Append(' ');
			sb.Append("optiongroup:");
			sb.Append(optiongroupName);
			return sb.ToString();
		}
	}

}