using System;
using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Flow.Item;
using NMaltParser.Core.Flow.Spec;

namespace NMaltParser.Core.Flow
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class FlowChartInstance
	{
		private FlowChartManager flowChartManager;
		private int optionContainerIndex;
		private string name;
		private ChartSpecification chartSpecification;
		private readonly LinkedHashSet<ChartItem> preProcessChartItems;
		private readonly LinkedHashSet<ChartItem> processChartItems;
		private readonly LinkedHashSet<ChartItem> postProcessChartItems;

	//	private SymbolTableHandler symbolTables;
	//	private DataFormatManager dataFormatManager;
	//	private final HashMap<String,DataFormatInstance> dataFormatInstances;
		private readonly Dictionary<string, object> flowChartRegistry;
		private readonly Dictionary<string, object> engineRegistry;
		private readonly StringBuilder flowChartRegistryKey;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FlowChartInstance(int optionContainerIndex, org.maltparser.core.flow.spec.ChartSpecification chartSpecification, FlowChartManager flowChartManager) throws org.maltparser.core.exception.MaltChainedException
		public FlowChartInstance(int optionContainerIndex, ChartSpecification chartSpecification, FlowChartManager flowChartManager)
		{
			FlowChartManager = flowChartManager;
			OptionContainerIndex = optionContainerIndex;
			ChartSpecification = chartSpecification;

			flowChartRegistry = new Dictionary<string, object>();
			engineRegistry = new Dictionary<string, object>();
			flowChartRegistryKey = new StringBuilder();
	//		dataFormatInstances = new HashMap<String, DataFormatInstance>(3);
	//		
	//		String inputFormatName = OptionManager.instance().getOptionValue(0, "input", "format").toString();
	//		String outputFormatName = OptionManager.instance().getOptionValue(0, "output", "format").toString();
	//		SystemLogger.logger().info(inputFormatName + "\n");
	//		SystemLogger.logger().info(outputFormatName + "\n");
	////		featureModelFileName = configDir.copyToConfig(Util.findURLinJars(featureModelFileName));
	//		dataFormatManager = new DataFormatManager(inputFormatName, outputFormatName);
	//		symbolTables = new TrieSymbolTableHandler();

			preProcessChartItems = new LinkedHashSet<ChartItem>();
			foreach (ChartItemSpecification chartItemSpecification in chartSpecification.PreProcessChartItemSpecifications)
			{
				preProcessChartItems.add(initChartItem(chartItemSpecification));
			}
			processChartItems = new LinkedHashSet<ChartItem>();
			foreach (ChartItemSpecification chartItemSpecification in chartSpecification.ProcessChartItemSpecifications)
			{
				processChartItems.add(initChartItem(chartItemSpecification));
			}

			postProcessChartItems = new LinkedHashSet<ChartItem>();
			foreach (ChartItemSpecification chartItemSpecification in chartSpecification.PostProcessChartItemSpecifications)
			{
				postProcessChartItems.add(initChartItem(chartItemSpecification));
			}


		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.core.flow.item.ChartItem initChartItem(org.maltparser.core.flow.spec.ChartItemSpecification chartItemSpecification) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual ChartItem initChartItem(ChartItemSpecification chartItemSpecification)
		{
			ChartItem chartItem = null;
			try
			{
				chartItem = Activator.CreateInstance(chartItemSpecification.ChartItemClass);
				chartItem.initialize(this, chartItemSpecification);
			}
			catch (InstantiationException e)
			{
				throw new FlowException("The chart item '" + chartItemSpecification.ChartItemName + "' could not be created. ", e);
			}
			catch (IllegalAccessException e)
			{
				throw new FlowException("The chart item '" + chartItemSpecification.ChartItemName + "' could not be created. ", e);
			}
			return chartItem;
		}

	//	public SymbolTableHandler getSymbolTables() {
	//		return symbolTables;
	//	}
	//
	//	public void setSymbolTables(SymbolTableHandler symbolTables) {
	//		this.symbolTables = symbolTables;
	//	}
	//
	//	public DataFormatManager getDataFormatManager() {
	//		return dataFormatManager;
	//	}
	//
	//	public void setDataFormatManager(DataFormatManager dataFormatManager) {
	//		this.dataFormatManager = dataFormatManager;
	//	}

		private void setFlowChartRegistryKey(Type entryClass, string identifier)
		{
			flowChartRegistryKey.Length = 0;
			flowChartRegistryKey.Append(identifier.ToString());
			flowChartRegistryKey.Append(entryClass.ToString());
		}

		public virtual void addFlowChartRegistry(Type entryClass, string identifier, object entry)
		{
			setFlowChartRegistryKey(entryClass, identifier);
			flowChartRegistry[flowChartRegistryKey.ToString()] = entry;
		}

		public virtual void removeFlowChartRegistry(Type entryClass, string identifier)
		{
			setFlowChartRegistryKey(entryClass, identifier);
			flowChartRegistry.Remove(flowChartRegistryKey.ToString());
		}

		public virtual object getFlowChartRegistry(Type entryClass, string identifier)
		{
			setFlowChartRegistryKey(entryClass, identifier);
			return flowChartRegistry[flowChartRegistryKey.ToString()];
		}

		public virtual void setEngineRegistry(string key, object value)
		{
			engineRegistry[key] = value;
		}

		public virtual object getEngineRegistry(string key)
		{
			return engineRegistry[key];
		}

	//	public HashMap<String, DataFormatInstance> getDataFormatInstances() {
	//		return dataFormatInstances;
	//	}

		public virtual FlowChartManager FlowChartManager
		{
			get
			{
				return flowChartManager;
			}
			set
			{
				flowChartManager = value;
			}
		}


		public virtual int OptionContainerIndex
		{
			get
			{
				return optionContainerIndex;
			}
			set
			{
				optionContainerIndex = value;
			}
		}


		public virtual ChartSpecification ChartSpecification
		{
			get
			{
				return chartSpecification;
			}
			set
			{
				chartSpecification = value;
			}
		}


		public virtual LinkedHashSet<ChartItem> PreProcessChartItems
		{
			get
			{
				return preProcessChartItems;
			}
		}

		public virtual LinkedHashSet<ChartItem> ProcessChartItems
		{
			get
			{
				return processChartItems;
			}
		}

		public virtual LinkedHashSet<ChartItem> PostProcessChartItems
		{
			get
			{
				return postProcessChartItems;
			}
		}

		public virtual bool hasPreProcessChartItems()
		{
			return !(preProcessChartItems.size() == 0);
		}

		public virtual bool hasProcessChartItems()
		{
			return !(processChartItems.size() == 0);
		}

		public virtual bool hasPostProcessChartItems()
		{
			return !(postProcessChartItems.size() == 0);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int preprocess() throws org.maltparser.core.exception.MaltChainedException
		public virtual int preprocess()
		{
			LinkedHashSet<ChartItem> chartItems = PreProcessChartItems;
			if (chartItems.size() == 0)
			{
				return ChartItem.TERMINATE;
			}
			int signal = ChartItem.CONTINUE;
			foreach (ChartItem chartItem in chartItems)
			{
				signal = chartItem.preprocess(signal);
				if (signal == ChartItem.TERMINATE)
				{
					return signal;
				}
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int process() throws org.maltparser.core.exception.MaltChainedException
		public virtual int process()
		{
			LinkedHashSet<ChartItem> chartItems = ProcessChartItems;
			if (chartItems.size() == 0)
			{
				return ChartItem.TERMINATE;
			}
			int signal = ChartItem.CONTINUE;
			foreach (ChartItem chartItem in chartItems)
			{
				signal = chartItem.process(signal);
	//			if (!more) {
	//				return false;
	//			}
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int postprocess() throws org.maltparser.core.exception.MaltChainedException
		public virtual int postprocess()
		{
			LinkedHashSet<ChartItem> chartItems = PostProcessChartItems;
			if (chartItems.size() == 0)
			{
				return ChartItem.TERMINATE;
			}
			int signal = ChartItem.CONTINUE;
			foreach (ChartItem chartItem in chartItems)
			{
				signal = chartItem.postprocess(signal);
				if (signal == ChartItem.TERMINATE)
				{
					return signal;
				}
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			LinkedHashSet<ChartItem> chartItems = PreProcessChartItems;
			foreach (ChartItem chartItem in chartItems)
			{
				chartItem.terminate();
			}
			chartItems = ProcessChartItems;
			foreach (ChartItem chartItem in chartItems)
			{
				chartItem.terminate();
			}
			chartItems = PostProcessChartItems;
			foreach (ChartItem chartItem in chartItems)
			{
				chartItem.terminate();
			}
			flowChartRegistry.Clear();
			engineRegistry.Clear();
			flowChartRegistryKey.Length = 0;
	//		symbolTables = null;

		}

		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}


		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + optionContainerIndex;
			result = prime * result + ((ReferenceEquals(name, null)) ? 0 : name.GetHashCode());
			result = prime * result + ((chartSpecification == null) ? 0 : chartSpecification.GetHashCode());
	//		result = prime * result + ((dataFormatInstances == null) ? 0 : dataFormatInstances.hashCode());
	//		result = prime * result + ((dataFormatManager == null) ? 0 : dataFormatManager.hashCode());
			result = prime * result + ((flowChartRegistry == null) ? 0 : flowChartRegistry.GetHashCode());
			result = prime * result + ((postProcessChartItems == null) ? 0 : postProcessChartItems.GetHashCode());
			result = prime * result + ((preProcessChartItems == null) ? 0 : preProcessChartItems.GetHashCode());
			result = prime * result + ((processChartItems == null) ? 0 : processChartItems.GetHashCode());
	//		result = prime * result + ((symbolTables == null) ? 0 : symbolTables.hashCode());
			return result;
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
			FlowChartInstance other = (FlowChartInstance) obj;
			if (optionContainerIndex != other.optionContainerIndex)
			{
				return false;
			}
			if (ReferenceEquals(name, null))
			{
				if (!ReferenceEquals(other.name, null))
				{
					return false;
				}
			}
			else if (!name.Equals(other.name))
			{
				return false;
			}
			if (chartSpecification == null)
			{
				if (other.chartSpecification != null)
				{
					return false;
				}
			}
			else if (!chartSpecification.Equals(other.chartSpecification))
			{
				return false;
			}
	//		if (dataFormatInstances == null) {
	//			if (other.dataFormatInstances != null)
	//				return false;
	//		} else if (!dataFormatInstances.equals(other.dataFormatInstances))
	//			return false;
	//		if (dataFormatManager == null) {
	//			if (other.dataFormatManager != null)
	//				return false;
	//		} else if (!dataFormatManager.equals(other.dataFormatManager))
	//			return false;
			if (flowChartRegistry == null)
			{
				if (other.flowChartRegistry != null)
				{
					return false;
				}
			}
			else if (!flowChartRegistry.Equals(other.flowChartRegistry))
			{
				return false;
			}
			if (postProcessChartItems == null)
			{
				if (other.postProcessChartItems != null)
				{
					return false;
				}
			}
			else if (!postProcessChartItems.Equals(other.postProcessChartItems))
			{
				return false;
			}
			if (preProcessChartItems == null)
			{
				if (other.preProcessChartItems != null)
				{
					return false;
				}
			}
			else if (!preProcessChartItems.Equals(other.preProcessChartItems))
			{
				return false;
			}
			if (processChartItems == null)
			{
				if (other.processChartItems != null)
				{
					return false;
				}
			}
			else if (!processChartItems.Equals(other.processChartItems))
			{
				return false;
			}
	//		if (symbolTables == null) {
	//			if (other.symbolTables != null)
	//				return false;
	//		} else if (!symbolTables.equals(other.symbolTables))
	//			return false;
			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(name);
			sb.Append('\n');
			if (preProcessChartItems.size() > 0)
			{
				sb.Append("  preprocess:");
				sb.Append('\n');
				foreach (ChartItem key in preProcessChartItems)
				{
					sb.Append(key);
					sb.Append('\n');
				}
			}
			if (processChartItems.size() > 0)
			{
				sb.Append("  process:");
				sb.Append('\n');
				foreach (ChartItem key in processChartItems)
				{
					sb.Append(key);
					sb.Append('\n');
				}
			}
			if (postProcessChartItems.size() > 0)
			{
				sb.Append("  postprocess:");
				sb.Append('\n');
				foreach (ChartItem key in postProcessChartItems)
				{
					sb.Append(key);
					sb.Append('\n');
				}
			}

			return sb.ToString();
		}
	}

}