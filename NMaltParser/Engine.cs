using System.Collections.Generic;

namespace org.maltparser
{
    using  core.flow;
    using  core.flow.item;
	using  core.helper;
    using  core.options;
	using  core.plugin;


	public class Engine
	{
		private readonly long startTime;
		private readonly FlowChartManager flowChartManager;
		private readonly SortedDictionary<int, FlowChartInstance> flowChartInstances;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Engine() throws org.maltparser.core.exception.MaltChainedException
		public Engine()
		{
			startTime = DateTimeHelper.CurrentUnixTimeMillis();
			flowChartManager = new FlowChartManager();
			flowChartManager.FlowChartSystem.load(GetType().getResource("/appdata/flow/flowchartsystem.xml"));
			flowChartManager.FlowChartSystem.load(PluginLoader.instance());
			flowChartManager.load(GetType().getResource("/appdata/flow/flowcharts.xml"));
			flowChartManager.load(PluginLoader.instance());
			flowChartInstances = new SortedDictionary<int, FlowChartInstance>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.flow.FlowChartInstance initialize(int optionContainerIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual FlowChartInstance initialize(int optionContainerIndex)
		{
			string flowChartName = null;
			if (OptionManager.instance().getOptionValueNoDefault(optionContainerIndex, "config", "flowchart") != null)
			{
				flowChartName = OptionManager.instance().getOptionValue(optionContainerIndex, "config", "flowchart").ToString();
			}
			if (ReferenceEquals(flowChartName, null))
			{
				if (OptionManager.instance().getOptionValueNoDefault(optionContainerIndex, "singlemalt", "mode") != null)
				{
					// This fix maps --singlemalt-mode option to --config-flowchart option because of historical reasons (version 1.0-1.1)
					flowChartName = OptionManager.instance().getOptionValue(optionContainerIndex, "singlemalt", "mode").ToString();
					OptionManager.instance().overloadOptionValue(optionContainerIndex, "config", "flowchart", flowChartName);
				}
				else
				{
					flowChartName = OptionManager.instance().getOptionValue(optionContainerIndex, "config", "flowchart").ToString();
				}
			}
			FlowChartInstance flowChartInstance = flowChartManager.initialize(optionContainerIndex, flowChartName);
			flowChartInstances[optionContainerIndex] = flowChartInstance;
			return flowChartInstance;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void process(int optionContainerIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual void process(int optionContainerIndex)
		{
			FlowChartInstance flowChartInstance = flowChartInstances[optionContainerIndex];
			if (flowChartInstance.hasPreProcessChartItems())
			{
				flowChartInstance.preprocess();
			}
			if (flowChartInstance.hasProcessChartItems())
			{
				int signal = ChartItem.CONTINUE;
				int tic = 0;
				int sentenceCounter = 0;
				int nIteration = 1;
				flowChartInstance.setEngineRegistry("iterations", nIteration);
				System.GC.Collect();
				while (signal != ChartItem.TERMINATE)
				{
					signal = flowChartInstance.process();
					if (signal == ChartItem.CONTINUE)
					{
						sentenceCounter++;
					}
					else if (signal == ChartItem.NEWITERATION)
					{
						SystemLogger.logger().info("\n=== END ITERATION " + nIteration + " ===\n");
						nIteration++;
						flowChartInstance.setEngineRegistry("iterations", nIteration);
					}
					if (sentenceCounter < 101 && sentenceCounter == 1 || sentenceCounter == 10 || sentenceCounter == 100)
					{
						Util.startTicer(SystemLogger.logger(), startTime, 10, sentenceCounter);
					}
					if (sentenceCounter % 100 == 0)
					{
						tic = Util.simpleTicer(SystemLogger.logger(), startTime, 10, tic, sentenceCounter);
					}
				}
				Util.endTicer(SystemLogger.logger(), startTime, 10, tic, sentenceCounter);
			}
			if (flowChartInstance.hasPostProcessChartItems())
			{
				flowChartInstance.postprocess();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate(int optionContainerIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate(int optionContainerIndex)
		{
			flowChartInstances[optionContainerIndex].terminate();
		}
	}

}