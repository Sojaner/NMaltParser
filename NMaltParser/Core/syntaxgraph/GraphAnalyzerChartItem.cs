﻿using System;
using System.IO;
using NMaltParser.Core.Config;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Flow;
using NMaltParser.Core.Flow.Item;
using NMaltParser.Core.Flow.Spec;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Options;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph
{
    public class GraphAnalyzerChartItem : ChartItem
	{
		private string idName;
		private string sourceName;
		private string task;
		private ConfigurationDir configDir;
		private IDependencyStructure cachedSource = null;
		private StreamWriter writer;
		private bool closeStream = true;
		private int graphCounter = 1;

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
			}
			if (ReferenceEquals(idName, null))
			{
				idName = getChartElement("analyzer").Attributes.get("id").DefaultValue;
			}
			else if (ReferenceEquals(sourceName, null))
			{
				sourceName = getChartElement("analyzer").Attributes.get("source").DefaultValue;
			}
			task = OptionManager.instance().getOptionValue(OptionContainerIndex, "analyzer", "task").ToString();
			configDir = (ConfigurationDir)flowChartinstance.getFlowChartRegistry(typeof(ConfigurationDir), idName);
			open(task + ".dat",OptionManager.instance().getOptionValue(OptionContainerIndex, "input", "charset").ToString());
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
			if (task.Equals("projectivity"))
			{
				if (cachedSource == null)
				{
					cachedSource = (IDependencyStructure)flowChartinstance.getFlowChartRegistry(typeof(IDependencyStructure), sourceName);
				}
				try
				{
					writer.append("graph # ");
					writer.append(Convert.ToString(graphCounter));
					writer.append('\n');
					foreach (int index in cachedSource.TokenIndices)
					{
						DependencyNode node = cachedSource.GetDependencyNode(index);

						writer.append(Convert.ToString(node.Index));
						writer.append('\t');
						writer.append(Convert.ToString(node.Head.Index));
						writer.append('\t');
						writer.append('#');
						writer.append('\t');
						if (node.Projective)
						{
							writer.append("@P");
						}
						else
						{
							writer.append("@N");
						}
						writer.append('\n');
					}
					writer.append('\n');
				}
				catch (IOException e)
				{
					throw new MaltChainedException("", e);
				}
				graphCounter++;
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
			cachedSource = null;
			close();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void open(String fileName, String charsetName) throws org.maltparser.core.exception.MaltChainedException
		private void open(string fileName, string charsetName)
		{
			try
			{
				open(new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write), charsetName));
			}
			catch (FileNotFoundException e)
			{
				throw new DataFormatException("The output file '" + fileName + "' cannot be found.", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new DataFormatException("The character encoding set '" + charsetName + "' isn't supported.", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void open(java.io.OutputStreamWriter osw) throws org.maltparser.core.exception.MaltChainedException
		private void open(StreamWriter osw)
		{
			Writer = new StreamWriter(osw);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void setWriter(java.io.BufferedWriter writer) throws org.maltparser.core.exception.MaltChainedException
		private StreamWriter Writer
		{
			set
			{
				close();
				writer = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void close() throws org.maltparser.core.exception.MaltChainedException
		private void close()
		{
			try
			{
				if (writer != null)
				{
					writer.Flush();
					if (closeStream)
					{
						writer.Close();
					}
					writer = null;
				}
			}
			catch (IOException e)
			{
				throw new DataFormatException("Could not close the output file. ", e);
			}

		}
	}

}