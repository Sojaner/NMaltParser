using System.Text;
using NMaltParser.Core.Config;
using NMaltParser.Core.Flow;
using NMaltParser.Core.Flow.Item;
using NMaltParser.Core.Flow.Spec;
using NMaltParser.Core.Helper;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Options;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Ds2PS;

namespace NMaltParser.Core.SyntaxGraph
{
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
		private ITokenStructure graph;

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
			if (ReferenceEquals(idName, null))
			{
				idName = getChartElement("graph").Attributes.get("id").DefaultValue;
			}
			else if (ReferenceEquals(structureName, null))
			{
				structureName = getChartElement("graph").Attributes.get("structure").DefaultValue;
			}
			else if (ReferenceEquals(taskName, null))
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
					flowChartinstance.addFlowChartRegistry(typeof(IDependencyStructure), structureName, graph);
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
					flowChartinstance.addFlowChartRegistry(typeof(IDependencyStructure), structureName, graph);
					flowChartinstance.addFlowChartRegistry(typeof(PhraseStructure), structureName, graph);
				}
				else if (dependency == false && phrase == true)
				{
					graph = new PhraseStructureGraph(symbolTables);
					flowChartinstance.addFlowChartRegistry(typeof(PhraseStructure), structureName, graph);
				}
				else
				{
					graph = new Sentence(symbolTables);
				}

				if (dataFormatInstance != null)
				{
					((IDependencyStructure)graph).SetDefaultRootEdgeLabels(OptionManager.instance().getOptionValue(OptionContainerIndex, "graph", "root_label").ToString(), dataFormatInstance.getDependencyEdgeLabelSymbolTables(symbolTables));
				}
				flowChartinstance.addFlowChartRegistry(typeof(ITokenStructure), structureName, graph);
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
				graph.Clear();
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
			if (GetType() != obj.GetType())
			{
				return false;
			}
			return obj.ToString().Equals(ToString());
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