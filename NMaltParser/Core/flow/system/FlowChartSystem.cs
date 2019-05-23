using System.Collections.Generic;
using System.IO;
using System.Text;
using NMaltParser.Core.Feature;
using NMaltParser.Core.Flow.System.Elem;
using NMaltParser.Core.Helper;
using NMaltParser.Core.Plugin;

namespace NMaltParser.Core.Flow.System
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class FlowChartSystem
	{
		private Dictionary<string, ChartElement> chartElements;

		public FlowChartSystem()
		{
			chartElements = new Dictionary<string, ChartElement>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(String urlstring) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(string urlstring)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
			UrlFinder f = new UrlFinder();
			load(f.FindUrl(urlstring));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(org.maltparser.core.plugin.PluginLoader plugins) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(PluginLoader plugins)
		{
			 foreach (Plugin.Plugin plugin in plugins)
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
//ORIGINAL LINE: public void load(java.net.URL specModelURL) throws org.maltparser.core.exception.MaltChainedException
		public virtual void load(URL specModelURL)
		{
			try
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();
				Element root = null;

				root = db.parse(specModelURL.openStream()).DocumentElement;

				if (root == null)
				{
					throw new FlowException("The flow chart system file '" + specModelURL.File + "' cannot be found. ");
				}

				readChartElements(root);
			}
			catch (IOException e)
			{
				throw new FlowException("The flow chart system file '" + specModelURL.File + "' cannot be found. ", e);
			}
			catch (ParserConfigurationException e)
			{
				throw new FlowException("Problem parsing the file " + specModelURL.File + ". ", e);
			}
			catch (SAXException e)
			{
				throw new FlowException("Problem parsing the file " + specModelURL.File + ". ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readChartElements(org.w3c.dom.Element root) throws org.maltparser.core.exception.MaltChainedException
		public virtual void readChartElements(Element root)
		{
			NodeList chartElem = root.getElementsByTagName("chartelement");
			for (int i = 0; i < chartElem.Length; i++)
			{
				ChartElement chartElement = new ChartElement();
				chartElement.read((Element)chartElem.item(i), this);
				chartElements[((Element)chartElem.item(i)).getAttribute("item")] = chartElement;
			}
		}

		public virtual ChartElement getChartElement(string name)
		{
			return chartElements[name];
		}


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("CHART ELEMENTS:\n");
			foreach (string key in chartElements.Keys)
			{
				sb.Append(chartElements[key]);
				sb.Append('\n');
			}
			return sb.ToString();
		}
	}

}