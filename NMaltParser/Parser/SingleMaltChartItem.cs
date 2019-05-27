using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Config;
using NMaltParser.Core.Flow;
using NMaltParser.Core.Flow.Item;
using NMaltParser.Core.Flow.Spec;
using NMaltParser.Core.IO.DataFormat;
using NMaltParser.Core.Options;
using NMaltParser.Core.SyntaxGraph;

namespace NMaltParser.Parser
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class SingleMaltChartItem : ChartItem
	{
		private SingleMalt singleMalt;
		private string idName;
		private string targetName;
		private string sourceName;
		private string modeName;
		private string taskName;
		private IDependencyStructure cachedSourceGraph = null;
		private IDependencyStructure cachedTargetGraph = null;



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(org.maltparser.core.flow.FlowChartInstance flowChartinstance, org.maltparser.core.flow.spec.ChartItemSpecification chartItemSpecification) throws org.maltparser.core.exception.MaltChainedException
		public override void initialize(FlowChartInstance flowChartinstance, ChartItemSpecification chartItemSpecification)
		{
			base.initialize(flowChartinstance, chartItemSpecification);

			foreach (string key in chartItemSpecification.ChartItemAttributes.Keys)
			{
				if (key.Equals("target"))
				{
					targetName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("source"))
				{
					sourceName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("mode"))
				{
					modeName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("task"))
				{
					taskName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("id"))
				{
					idName = chartItemSpecification.ChartItemAttributes[key];
				}
			}
			if (ReferenceEquals(targetName, null))
			{
				targetName = getChartElement("singlemalt").Attributes.get("target").DefaultValue;
			}
			else if (ReferenceEquals(sourceName, null))
			{
				sourceName = getChartElement("singlemalt").Attributes.get("source").DefaultValue;
			}
			else if (ReferenceEquals(modeName, null))
			{
				modeName = getChartElement("singlemalt").Attributes.get("mode").DefaultValue;
			}
			else if (ReferenceEquals(taskName, null))
			{
				taskName = getChartElement("singlemalt").Attributes.get("task").DefaultValue;
			}
			else if (ReferenceEquals(idName, null))
			{
				idName = getChartElement("singlemalt").Attributes.get("id").DefaultValue;
			}

			singleMalt = (SingleMalt)flowChartinstance.getFlowChartRegistry(typeof(SingleMalt), idName);
			if (singleMalt == null)
			{
				singleMalt = new SingleMalt();
				flowChartinstance.addFlowChartRegistry(typeof(SingleMalt), idName, singleMalt);
				flowChartinstance.addFlowChartRegistry(typeof(Configuration), idName, singleMalt);

			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int preprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int preprocess(int signal)
		{
			if (taskName.Equals("init"))
			{
				if (modeName.Equals("learn") || modeName.Equals("parse"))
				{
					OptionManager.instance().overloadOptionValue(OptionContainerIndex, "singlemalt", "mode", modeName);
					ConfigurationDir configDir = (ConfigurationDir)flowChartinstance.getFlowChartRegistry(typeof(ConfigurationDir), idName);
					DataFormatManager dataFormatManager = configDir.DataFormatManager;
					if (modeName.Equals("learn"))
					{
						DataFormatInstance dataFormatInstance = null;
						if (dataFormatManager.InputDataFormatSpec.DataStructure == DataStructure.PHRASE)
						{
							ISet<Dependency> deps = dataFormatManager.InputDataFormatSpec.Dependencies;
							string nullValueStrategy = OptionManager.instance().getOptionValue(OptionContainerIndex, "singlemalt", "null_value").ToString();

							foreach (Dependency dep in dataFormatManager.InputDataFormatSpec.Dependencies)
							{
								dataFormatInstance = dataFormatManager.getDataFormatSpec(dep.DependentOn).createDataFormatInstance(configDir.SymbolTables, nullValueStrategy);
								configDir.addDataFormatInstance(dataFormatManager.OutputDataFormatSpec.DataFormatName, dataFormatInstance);
							}

							string decisionSettings = OptionManager.instance().getOptionValue(OptionContainerIndex,"guide", "decision_settings").ToString().Trim();
							StringBuilder newDecisionSettings = new StringBuilder();
							if (!Pattern.matches(".*A\\.HEADREL.*", decisionSettings))
							{
								newDecisionSettings.Append("+A.HEADREL");
							}
							if (!Pattern.matches(".*A\\.PHRASE.*", decisionSettings))
							{
								newDecisionSettings.Append("+A.PHRASE");
							}
							if (!Pattern.matches(".*A\\.ATTACH.*", decisionSettings))
							{
								newDecisionSettings.Append("+A.ATTACH");
							}
							if (newDecisionSettings.Length > 0)
							{
								OptionManager.instance().overloadOptionValue(OptionContainerIndex, "guide", "decision_settings", decisionSettings + newDecisionSettings.ToString());
							}
						}
						else
						{
							dataFormatInstance = configDir.getDataFormatInstance(dataFormatManager.InputDataFormatSpec.DataFormatName);
						}
						singleMalt.initialize(OptionContainerIndex, dataFormatInstance, configDir.SymbolTables, configDir, SingleMalt.LEARN);
					}
					else if (modeName.Equals("parse"))
					{
						singleMalt.initialize(OptionContainerIndex, configDir.getDataFormatInstance(dataFormatManager.InputDataFormatSpec.DataFormatName), configDir.SymbolTables, configDir, SingleMalt.PARSE);
					}
					else
					{
						return TERMINATE;
					}
				}
				else
				{
					return TERMINATE;
				}
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int process(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int process(int signal)
		{
			if (taskName.Equals("process"))
			{
				if (cachedSourceGraph == null)
				{
					cachedSourceGraph = (IDependencyStructure)flowChartinstance.getFlowChartRegistry(typeof(IDependencyStructure), sourceName);
				}
				if (cachedTargetGraph == null)
				{
					cachedTargetGraph = (IDependencyStructure)flowChartinstance.getFlowChartRegistry(typeof(IDependencyStructure), targetName);
				}
				if (modeName.Equals("learn"))
				{
					singleMalt.OracleParse(cachedSourceGraph, cachedTargetGraph);
				}
				else if (modeName.Equals("parse"))
				{
					singleMalt.Parse(cachedSourceGraph);
	//				if (cachedSourceGraph instanceof MappablePhraseStructureGraph) {
	//					System.out.println("MappablePhraseStructureGraph");
	//					((MappablePhraseStructureGraph)cachedSourceGraph).getMapping().connectUnattachedSpines((MappablePhraseStructureGraph)cachedSourceGraph);
	//				}	
				}
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int postprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int postprocess(int signal)
		{
			if (taskName.Equals("train") && singleMalt.Guide != null)
			{
				singleMalt.Guide.noMoreInstances();
			}
			else if (taskName.Equals("train") && singleMalt.Guide == null)
			{
				singleMalt.train();
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
			if (flowChartinstance.getFlowChartRegistry(typeof(SingleMalt), idName) != null)
			{
				singleMalt.terminate(null);
				flowChartinstance.removeFlowChartRegistry(typeof(SingleMalt), idName);
				flowChartinstance.removeFlowChartRegistry(typeof(Configuration), idName);
				singleMalt = null;
			}
			else
			{
				singleMalt = null;
			}
			cachedSourceGraph = null;
			cachedTargetGraph = null;
		}

		public virtual SingleMalt SingleMalt
		{
			get
			{
				return singleMalt;
			}
			set
			{
				singleMalt = value;
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
				targetName = value;
			}
		}


		public virtual string SourceName
		{
			get
			{
				return sourceName;
			}
			set
			{
				sourceName = value;
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
			sb.Append("    singlemalt ");
			sb.Append("id:");
			sb.Append(idName);
			sb.Append(' ');
			sb.Append("mode:");
			sb.Append(modeName);
			sb.Append(' ');
			sb.Append("task:");
			sb.Append(taskName);
			sb.Append(' ');
			sb.Append("source:");
			sb.Append(sourceName);
			sb.Append(' ');
			sb.Append("target:");
			sb.Append(targetName);
			return sb.ToString();
		}
	}
}