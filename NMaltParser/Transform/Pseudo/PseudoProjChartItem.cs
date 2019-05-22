﻿using System;
using System.Text;

namespace org.maltparser.transform.pseudo
{

	using  org.maltparser.core.config;
	using  org.maltparser.core.exception;
	using  org.maltparser.core.flow;
	using  org.maltparser.core.flow.item;
	using  org.maltparser.core.flow.spec;
	using  org.maltparser.core.helper;
	using  org.maltparser.core.io.dataformat;
	using  org.maltparser.core.options;
	using  org.maltparser.core.syntaxgraph;
	using  org.maltparser.core.syntaxgraph;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class PseudoProjChartItem : ChartItem
	{
		private string idName;
		private string targetName;
		private string sourceName;
		private string taskName;

		private string marking_strategy;
		private string covered_root;
		private string lifting_order;

		private PseudoProjectivity pproj;
		private bool pprojActive = false;
		private TokenStructure cachedGraph = null;

		public PseudoProjChartItem()
		{
		}

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
				else if (key.Equals("id"))
				{
					idName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("task"))
				{
					taskName = chartItemSpecification.ChartItemAttributes[key];
				}
			}

			if (string.ReferenceEquals(targetName, null))
			{
				targetName = getChartElement("pseudoproj").Attributes.get("target").DefaultValue;
			}
			else if (string.ReferenceEquals(sourceName, null))
			{
				sourceName = getChartElement("pseudoproj").Attributes.get("source").DefaultValue;
			}
			else if (string.ReferenceEquals(idName, null))
			{
				idName = getChartElement("pseudoproj").Attributes.get("id").DefaultValue;
			}
			else if (string.ReferenceEquals(taskName, null))
			{
				taskName = getChartElement("pseudoproj").Attributes.get("task").DefaultValue;
			}

			PseudoProjectivity tmppproj = (PseudoProjectivity)flowChartinstance.getFlowChartRegistry(typeof(org.maltparser.transform.pseudo.PseudoProjectivity), idName);
			if (tmppproj == null)
			{
				pproj = new PseudoProjectivity();
				flowChartinstance.addFlowChartRegistry(typeof(org.maltparser.transform.pseudo.PseudoProjectivity), idName, pproj);
			}
			else
			{
				pproj = tmppproj;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int preprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int preprocess(int signal)
		{
			if (taskName.Equals("init"))
			{
				ConfigurationDir configDir = (ConfigurationDir)flowChartinstance.getFlowChartRegistry(typeof(ConfigurationDir), idName);
	//			SymbolTableHandler symbolTables = configDir.getSymbolTables();
				DataFormatInstance dataFormatInstance = configDir.InputDataFormatInstance;
				marking_strategy = OptionManager.instance().getOptionValue(OptionContainerIndex, "pproj", "marking_strategy").ToString().Trim();
				covered_root = OptionManager.instance().getOptionValue(OptionContainerIndex, "pproj", "covered_root").ToString().Trim();
				lifting_order = OptionManager.instance().getOptionValue(OptionContainerIndex, "pproj", "lifting_order").ToString().Trim();
				if (!marking_strategy.Equals("none", StringComparison.OrdinalIgnoreCase) || !covered_root.Equals("none", StringComparison.OrdinalIgnoreCase))
				{
					pproj.initialize(marking_strategy, covered_root, lifting_order, SystemLogger.logger(), dataFormatInstance, configDir.SymbolTables);
				}
				if (!marking_strategy.Equals("none", StringComparison.OrdinalIgnoreCase) || !covered_root.Equals("none", StringComparison.OrdinalIgnoreCase))
				{
					pprojActive = true;
				}
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int process(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int process(int signal)
		{
			if (cachedGraph == null)
			{
				marking_strategy = OptionManager.instance().getOptionValue(OptionContainerIndex, "pproj", "marking_strategy").ToString().Trim();
				covered_root = OptionManager.instance().getOptionValue(OptionContainerIndex, "pproj", "covered_root").ToString().Trim();
				lifting_order = OptionManager.instance().getOptionValue(OptionContainerIndex, "pproj", "lifting_order").ToString().Trim();

				cachedGraph = (TokenStructure)flowChartinstance.getFlowChartRegistry(typeof(TokenStructure), sourceName);
				if (!marking_strategy.Equals("none", StringComparison.OrdinalIgnoreCase) || !covered_root.Equals("none", StringComparison.OrdinalIgnoreCase))
				{
					pprojActive = true;
				}
			}

			if (pprojActive && cachedGraph is DependencyStructure)
			{
				if (taskName.Equals("proj"))
				{
						pproj.projectivize((DependencyStructure)cachedGraph);
				}
				else if (taskName.Equals("merge"))
				{
						pproj.mergeArclabels((DependencyStructure)cachedGraph);
				}
				else if (taskName.Equals("deproj"))
				{
						pproj.deprojectivize((DependencyStructure)cachedGraph);
	//				marking_strategy = OptionManager.instance().getOptionValue(getOptionContainerIndex(), "pproj", "marking_strategy").toString().trim();
	//				covered_root = OptionManager.instance().getOptionValue(getOptionContainerIndex(), "pproj", "covered_root").toString().trim();
	//				ConfigurationDir configDir = (ConfigurationDir)flowChartinstance.getFlowChartRegistry(org.maltparser.core.config.ConfigurationDir.class, idName);
	//				Deprojectivizer deprojectivizer = new Deprojectivizer(marking_strategy, covered_root, configDir.getInputDataFormatInstance(), configDir.getSymbolTables());
	//				deprojectivizer.deprojectivize((DependencyStructure)cachedGraph);
				}
				else if (taskName.Equals("split"))
				{
						pproj.splitArclabels((DependencyStructure)cachedGraph);
				}
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
			pproj = null;
			pprojActive = false;
			cachedGraph = null;
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
			sb.Append("    pseudoproj ");
			sb.Append("id:");
			sb.Append(idName);
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