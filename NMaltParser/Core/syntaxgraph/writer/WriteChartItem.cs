using System;
using System.Text;

namespace org.maltparser.core.syntaxgraph.writer
{
	using  org.maltparser.core.config;
	using  org.maltparser.core.exception;
	using  org.maltparser.core.flow;
	using  org.maltparser.core.flow.item;
	using  org.maltparser.core.flow.spec;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.options;
	using  org.maltparser.core.symbol;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class WriteChartItem : ChartItem
	{
		private string idName;
		private string outputFormatName;
		private string outputFileName;
		private string outputCharSet;
		private string writerOptions;
		private Type graphWriterClass;

		private string nullValueStrategy;

		private SyntaxGraphWriter writer;
		private string sourceName;
		private string optiongroupName;
		private DataFormatInstance outputDataFormatInstance;
		private TokenStructure cachedGraph = null;

		public WriteChartItem() : base()
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
				else if (key.Equals("source"))
				{
					sourceName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("optiongroup"))
				{
					optiongroupName = chartItemSpecification.ChartItemAttributes[key];
				}
			}

			if (string.ReferenceEquals(idName, null))
			{
				idName = getChartElement("write").Attributes.get("id").DefaultValue;
			}
			else if (string.ReferenceEquals(sourceName, null))
			{
				sourceName = getChartElement("write").Attributes.get("source").DefaultValue;
			}
			else if (string.ReferenceEquals(optiongroupName, null))
			{
				optiongroupName = getChartElement("write").Attributes.get("optiongroup").DefaultValue;
			}

			OutputFormatName = OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "format").ToString();
			OutputFileName = OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "outfile").ToString();
			OutputCharSet = OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "charset").ToString();
			WriterOptions = OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "writer_options").ToString();
			SyntaxGraphWriterClass = (Type)OptionManager.instance().getOptionValue(OptionContainerIndex, optiongroupName, "writer");

			NullValueStrategy = OptionManager.instance().getOptionValue(OptionContainerIndex, "singlemalt", "null_value").ToString();

			initOutput(NullValueStrategy);
			initWriter(SyntaxGraphWriterClass, OutputFileName, OutputCharSet, WriterOptions);
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
				cachedGraph = (TokenStructure)flowChartinstance.getFlowChartRegistry(typeof(TokenStructure), sourceName);
				writer.writeProlog();
			}
			writer.writeSentence(cachedGraph);
			if (signal == ChartItem.TERMINATE)
			{
				writer.writeEpilog();
			}
			return signal;
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
			if (writer != null)
			{
				writer.close();
				writer = null;
			}
			outputDataFormatInstance = null;
			cachedGraph = null;
		}

		public virtual string OutputFormatName
		{
			get
			{
				if (string.ReferenceEquals(outputFormatName, null))
				{
					return "/appdata/dataformat/conllx.xml";
				}
				return outputFormatName;
			}
			set
			{
				this.outputFormatName = value;
			}
		}


		public virtual string OutputFileName
		{
			get
			{
				if (string.ReferenceEquals(outputFileName, null))
				{
					return "/dev/stdout";
				}
				return outputFileName;
			}
			set
			{
				this.outputFileName = value;
			}
		}


		public virtual string OutputCharSet
		{
			get
			{
				if (string.ReferenceEquals(outputCharSet, null))
				{
					return "UTF-8";
				}
				return outputCharSet;
			}
			set
			{
				this.outputCharSet = value;
			}
		}


		public virtual string WriterOptions
		{
			get
			{
				if (string.ReferenceEquals(writerOptions, null))
				{
					return "";
				}
				return writerOptions;
			}
			set
			{
				this.writerOptions = value;
			}
		}


		public virtual Type SyntaxGraphWriterClass
		{
			get
			{
				return graphWriterClass;
			}
			set
			{
				try
				{
					if (value != null)
					{
						this.graphWriterClass = value.asSubclass(typeof(org.maltparser.core.syntaxgraph.writer.SyntaxGraphWriter));
					}
				}
				catch (System.InvalidCastException e)
				{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new DataFormatException("The class '" + value.FullName + "' is not a subclass of '" + typeof(org.maltparser.core.syntaxgraph.writer.SyntaxGraphWriter).FullName + "'. ", e);
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


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initOutput(String nullValueStategy) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initOutput(string nullValueStategy)
		{
			ConfigurationDir configDir = (ConfigurationDir)flowChartinstance.getFlowChartRegistry(typeof(ConfigurationDir), idName);
			DataFormatManager dataFormatManager = configDir.DataFormatManager;
			SymbolTableHandler symbolTables = configDir.SymbolTables;

			if (configDir.sizeDataFormatInstance() == 0 || dataFormatManager.InputDataFormatSpec != dataFormatManager.OutputDataFormatSpec)
			{
				outputDataFormatInstance = dataFormatManager.OutputDataFormatSpec.createDataFormatInstance(symbolTables, nullValueStategy);
				configDir.addDataFormatInstance(dataFormatManager.InputDataFormatSpec.DataFormatName, outputDataFormatInstance);
			}
			else
			{
				outputDataFormatInstance = configDir.getDataFormatInstance(dataFormatManager.InputDataFormatSpec.DataFormatName); //dataFormatInstances.get(dataFormatManager.getInputDataFormatSpec().getDataFormatName());
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initWriter(Class syntaxGraphWriterClass, String outputFile, String outputCharSet, String writerOption) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initWriter(Type syntaxGraphWriterClass, string outputFile, string outputCharSet, string writerOption)
		{
			try
			{
				writer = System.Activator.CreateInstance(syntaxGraphWriterClass);
				if (string.ReferenceEquals(outputFile, null) || outputFile.Length == 0 || outputFile.Equals("/dev/stdout"))
				{
					writer.open(System.out, outputCharSet);
				}
				else
				{
					writer.open(outputFile, outputCharSet);
				}
				writer.DataFormatInstance = outputDataFormatInstance;
				writer.Options = writerOption;
			}
			catch (InstantiationException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DataFormatException("The data writer '" + syntaxGraphWriterClass.FullName + "' cannot be initialized. ", e);
			}
			catch (IllegalAccessException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new DataFormatException("The data writer '" + syntaxGraphWriterClass.FullName + "' cannot be initialized. ", e);
			}
		}

		public virtual Type GraphWriterClass
		{
			get
			{
				return graphWriterClass;
			}
		}

		public virtual SyntaxGraphWriter Writer
		{
			get
			{
				return writer;
			}
		}

		public virtual string SourceName
		{
			get
			{
				return sourceName;
			}
		}

		public virtual DataFormatInstance OutputDataFormatInstance
		{
			get
			{
				return outputDataFormatInstance;
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
			StringBuilder sb = new StringBuilder();
			sb.Append("    write ");
			sb.Append("id:");
			sb.Append(idName);
			sb.Append(' ');
			sb.Append("source:");
			sb.Append(sourceName);
			sb.Append(' ');
			sb.Append("optiongroup:");
			sb.Append(optiongroupName);
			return sb.ToString();
		}
	}

}