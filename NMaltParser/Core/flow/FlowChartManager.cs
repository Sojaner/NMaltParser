using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.flow
{
    using  feature;
	using  spec;
	using  system;
	using  helper;
	using  plugin;

    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class FlowChartManager
	{
		private static FlowChartManager uniqueInstance = new FlowChartManager();
		private readonly FlowChartSystem flowChartSystem;
		private readonly Dictionary<string, ChartSpecification> chartSpecifications;

		public FlowChartManager()
		{
			flowChartSystem = new FlowChartSystem();
			chartSpecifications = new Dictionary<string, ChartSpecification>();
		}

		/// <summary>
		/// Returns a reference to the single instance.
		/// </summary>
		public static FlowChartManager instance()
		{
			return uniqueInstance;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(String urlstring) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(string urlstring)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
			URLFinder f = new URLFinder();
			load(f.findURL(urlstring));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(org.maltparser.core.plugin.PluginLoader plugins) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(PluginLoader plugins)
		{
			 foreach (Plugin plugin in plugins)
			 {
				URL url = null;
				try
				{
					url = new URL("jar:" + plugin.Url + "!/appdata/plugin.xml");
				}
				catch (MalformedURLException e)
				{
					throw new FeatureException("Malformed URL: 'jar:" + plugin.Url + "!plugin.xml'", e);
				}
				try
				{
					Stream @is = url.openStream();
					@is.Close();
				}
				catch (IOException)
				{
					continue;
				}

				load(url);
			 }
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.net.URL url) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(URL url)
		{
			try
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();
				Element root = null;

				root = db.parse(url.openStream()).DocumentElement;
				if (root == null)
				{
					throw new FlowException("The flow chart specification file '" + url.File + "' cannot be found. ");
				}
				readFlowCharts(root);
			}
			catch (IOException e)
			{
				throw new FlowException("The flow chart specification file '" + url.File + "' cannot be found. ", e);
			}
			catch (ParserConfigurationException e)
			{
				throw new FlowException("Problem parsing the flow chart file " + url.File + ". ", e);
			}
			catch (SAXException e)
			{
				throw new FlowException("Problem parsing the flow chart file " + url.File + ". ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readFlowCharts(org.w3c.dom.Element flowcharts) throws org.maltparser.core.exception.MaltChainedException
		private void readFlowCharts(Element flowcharts)
		{
			NodeList flowChartList = flowcharts.getElementsByTagName("flowchart");
			for (int i = 0; i < flowChartList.Length; i++)
			{
				string flowChartName = ((Element)flowChartList.item(i)).getAttribute("name");
				if (!chartSpecifications.ContainsKey(flowChartName))
				{
					ChartSpecification chart = new ChartSpecification();
					chartSpecifications[flowChartName] = chart;
					chart.read((Element)flowChartList.item(i), this);
				}
				else
				{
					throw new FlowException("Problem parsing the flow chart file. The flow chart with the name " + flowChartName + " already exists. ");
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FlowChartInstance initialize(int optionContainerIndex, String flowChartName) throws org.maltparser.core.exception.MaltChainedException
		public virtual FlowChartInstance initialize(int optionContainerIndex, string flowChartName)
		{
			return new FlowChartInstance(optionContainerIndex, chartSpecifications[flowChartName], this);
		}

		public virtual FlowChartSystem FlowChartSystem
		{
			get
			{
				return flowChartSystem;
			}
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("FLOW CHART SYSTEM\n");
			sb.Append(flowChartSystem);
			sb.Append('\n');
			sb.Append("FLOW CHARTS:\n");
			foreach (string key in chartSpecifications.Keys)
			{
				sb.Append(chartSpecifications[key]);
				sb.Append('\n');
			}
			return sb.ToString();
		}

	}

}