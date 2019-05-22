using System.Text;

namespace org.maltparser.core.syntaxgraph
{
	using ConfigurationDir = org.maltparser.core.config.ConfigurationDir;
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FlowChartInstance = org.maltparser.core.flow.FlowChartInstance;
	using FlowException = org.maltparser.core.flow.FlowException;
	using ChartItem = org.maltparser.core.flow.item.ChartItem;
	using ChartItemSpecification = org.maltparser.core.flow.spec.ChartItemSpecification;
	using HashSet = org.maltparser.core.helper.HashSet;
	using DataFormatInstance = org.maltparser.core.io.dataformat.DataFormatInstance;
	using DataFormatManager = org.maltparser.core.io.dataformat.DataFormatManager;
	using DataStructure = org.maltparser.core.io.dataformat.DataFormatSpecification.DataStructure;
	using Dependency = org.maltparser.core.io.dataformat.DataFormatSpecification.Dependency;
	using OptionManager = org.maltparser.core.options.OptionManager;
	using SymbolTableHandler = org.maltparser.core.symbol.SymbolTableHandler;
	using LosslessMapping = org.maltparser.core.syntaxgraph.ds2ps.LosslessMapping;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class SyntaxGraphChartItem : ChartItem
	{
		private string idName;
		private string structureName;
		private string taskName;
		private TokenStructure graph;

		public SyntaxGraphChartItem() : base()
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
				else if (key.Equals("structure"))
				{
					structureName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("task"))
				{
					taskName = chartItemSpecification.ChartItemAttributes[key];
				}
			}
			if (string.ReferenceEquals(idName, null))
			{
				idName = getChartElement("graph").Attributes.get("id").DefaultValue;
			}
			else if (string.ReferenceEquals(structureName, null))
			{
				structureName = getChartElement("graph").Attributes.get("structure").DefaultValue;
			}
			else if (string.ReferenceEquals(taskName, null))
			{
				taskName = getChartElement("graph").Attributes.get("task").DefaultValue;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int preprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int preprocess(int signal)
		{
			if (taskName.Equals("create"))
			{
				bool phrase = false;
				bool dependency = false;
				ConfigurationDir configDir = (ConfigurationDir)flowChartinstance.getFlowChartRegistry(typeof(ConfigurationDir), idName);
				DataFormatInstance dataFormatInstance = null;
				DataFormatManager dataFormatManager = configDir.DataFormatManager;
				SymbolTableHandler symbolTables = configDir.SymbolTables;



				foreach (string key in configDir.DataFormatInstanceKeys)
				{
					DataFormatInstance dfi = configDir.getDataFormatInstance(key);
					if (dfi.DataFormarSpec.DataStructure == DataStructure.PHRASE)
					{
						phrase = true;
					}
					if (dfi.DataFormarSpec.DataStructure == DataStructure.DEPENDENCY)
					{
						dependency = true;
						dataFormatInstance = dfi;
					}
				}

				if (dependency == false && OptionManager.instance().getOptionValue(OptionContainerIndex, "config", "flowchart").ToString().Equals("learn"))
				{
					dependency = true;
					HashSet<Dependency> deps = dataFormatManager.InputDataFormatSpec.Dependencies;
					string nullValueStategy = OptionManager.instance().getOptionValue(OptionContainerIndex, "singlemalt", "null_value").ToString();
					foreach (Dependency dep in deps)
					{
						dataFormatInstance = dataFormatManager.getDataFormatSpec(dep.DependentOn).createDataFormatInstance(symbolTables, nullValueStategy);
						configDir.addDataFormatInstance(dataFormatManager.OutputDataFormatSpec.DataFormatName, dataFormatInstance);
					}
				}

				if (dependency == true && phrase == false)
				{
					graph = new DependencyGraph(symbolTables);
					flowChartinstance.addFlowChartRegistry(typeof(org.maltparser.core.syntaxgraph.DependencyStructure), structureName, graph);
				}
				else if (dependency == true && phrase == true)
				{
					graph = new MappablePhraseStructureGraph(symbolTables);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.io.dataformat.DataFormatInstance inFormat = configDir.getDataFormatInstance(dataFormatManager.getInputDataFormatSpec().getDataFormatName());
					DataFormatInstance inFormat = configDir.getDataFormatInstance(dataFormatManager.InputDataFormatSpec.DataFormatName);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.io.dataformat.DataFormatInstance outFormat = configDir.getDataFormatInstance(dataFormatManager.getOutputDataFormatSpec().getDataFormatName());
					DataFormatInstance outFormat = configDir.getDataFormatInstance(dataFormatManager.OutputDataFormatSpec.DataFormatName);

					if (inFormat != null && outFormat != null)
					{
						LosslessMapping mapping = null;
						if (inFormat.DataFormarSpec.DataStructure == DataStructure.DEPENDENCY)
						{
							mapping = new LosslessMapping(inFormat, outFormat, symbolTables);
						}
						else
						{
							mapping = new LosslessMapping(outFormat, inFormat, symbolTables);
						}
						if (inFormat.DataFormarSpec.DataStructure == DataStructure.PHRASE)
						{
							mapping.setHeadRules(OptionManager.instance().getOptionValue(OptionContainerIndex, "graph", "head_rules").ToString());
						}
						((MappablePhraseStructureGraph)graph).Mapping = mapping;
					}
					else
					{
						throw new FlowException("Couldn't determine the input and output data format. ");
					}
					flowChartinstance.addFlowChartRegistry(typeof(org.maltparser.core.syntaxgraph.DependencyStructure), structureName, graph);
					flowChartinstance.addFlowChartRegistry(typeof(org.maltparser.core.syntaxgraph.PhraseStructure), structureName, graph);
				}
				else if (dependency == false && phrase == true)
				{
					graph = new PhraseStructureGraph(symbolTables);
					flowChartinstance.addFlowChartRegistry(typeof(org.maltparser.core.syntaxgraph.PhraseStructure), structureName, graph);
				}
				else
				{
					graph = new Sentence(symbolTables);
				}

				if (dataFormatInstance != null)
				{
					((DependencyStructure)graph).setDefaultRootEdgeLabels(OptionManager.instance().getOptionValue(OptionContainerIndex, "graph", "root_label").ToString(), dataFormatInstance.getDependencyEdgeLabelSymbolTables(symbolTables));
				}
				flowChartinstance.addFlowChartRegistry(typeof(org.maltparser.core.syntaxgraph.TokenStructure), structureName, graph);
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int process(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int process(int signal)
		{
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
			if (graph != null)
			{
				graph.clear();
				graph = null;
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
			sb.Append("    graph ");
			sb.Append("id:");
			sb.Append(idName);
			sb.Append(' ');
			sb.Append("task:");
			sb.Append(taskName);
			sb.Append(' ');
			sb.Append("structure:");
			sb.Append(structureName);
			return sb.ToString();
		}
	}

}