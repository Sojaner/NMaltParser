using System;
using System.Text;

namespace org.maltparser.core.flow.system.elem
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.flow.item;
	using  org.maltparser.core.plugin;
	using  org.w3c.dom;
	using  org.w3c.dom;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class ChartElement
	{
		private string item;
		private Type chartItemClass;
		private LinkedHashMap<string, ChartAttribute> attributes;

		public ChartElement()
		{
			attributes = new LinkedHashMap<string, ChartAttribute>();
		}

		public virtual string Item
		{
			get
			{
				return item;
			}
			set
			{
				this.item = value;
			}
		}


		public virtual void addAttribute(string name, ChartAttribute attribute)
		{
			attributes.put(name, attribute);
		}

		public virtual ChartAttribute getAttribute(string name)
		{
			return attributes.get(name);
		}

		public virtual Type ChartItemClass
		{
			get
			{
				return chartItemClass;
			}
		}


		public virtual LinkedHashMap<string, ChartAttribute> Attributes
		{
			get
			{
				return attributes;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void read(org.w3c.dom.Element chartElem, org.maltparser.core.flow.system.FlowChartSystem flowChartSystem) throws org.maltparser.core.exception.MaltChainedException
		public virtual void read(Element chartElem, FlowChartSystem flowChartSystem)
		{
			Item = chartElem.getAttribute("item");
			string chartItemClassName = chartElem.getAttribute("class");
			Type clazz = null;
			try
			{
				if (PluginLoader.instance() != null)
				{
					clazz = PluginLoader.instance().getClass(chartItemClassName);
				}
				if (clazz == null)
				{
					clazz = Type.GetType(chartItemClassName);
				}
				this.chartItemClass = clazz.asSubclass(typeof(ChartItem));
			}
			catch (System.InvalidCastException e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new FlowException("The class '" + clazz.FullName + "' is not a subclass of '" + typeof(ChartItem).FullName + "'. ", e);
			}
			catch (ClassNotFoundException e)
			{
				throw new FlowException("The class " + chartItemClassName + "  could not be found. ", e);
			}
			NodeList attrElements = chartElem.getElementsByTagName("attribute");
			for (int i = 0; i < attrElements.Length; i++)
			{
				ChartAttribute attribute = new ChartAttribute();
				attribute.read((Element)attrElements.item(i),flowChartSystem);
				attributes.put(((Element)attrElements.item(i)).getAttribute("name"), attribute);
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("    ");
			sb.Append(item);
			sb.Append(' ');
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			sb.Append(chartItemClass.FullName);
			sb.Append('\n');
			foreach (string key in attributes.Keys)
			{
				sb.Append("       ");
				sb.Append(attributes.get(key));
				sb.Append('\n');
			}
			return sb.ToString();
		}
	}

}