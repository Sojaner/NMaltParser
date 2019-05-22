using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.flow.spec
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.flow.item;
	using  org.w3c.dom;
	using  org.w3c.dom;
	using  org.w3c.dom;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class ChartItemSpecification
	{
		private string chartItemName;
		private Type chartItemClass;
		private Dictionary<string, string> attributes;

		public ChartItemSpecification() : this(null,null)
		{
		}

		public ChartItemSpecification(string chartItemName, Type chartItemClass)
		{
			ChartItemName = chartItemName;
			ChartItemClass = chartItemClass;
			attributes = new Dictionary<string, string>(3);
		}

		public virtual string ChartItemName
		{
			get
			{
				return chartItemName;
			}
			set
			{
				this.chartItemName = value;
			}
		}


		public virtual Type ChartItemClass
		{
			get
			{
				return chartItemClass;
			}
			set
			{
				this.chartItemClass = value;
			}
		}


		public virtual Dictionary<string, string> ChartItemAttributes
		{
			get
			{
				return attributes;
			}
		}

		public virtual string getChartItemAttribute(string key)
		{
			return attributes[key];
		}

		public virtual void addChartItemAttribute(string key, string value)
		{
			attributes[key] = value;
		}

		public virtual void removeChartItemAttribute(string key)
		{
			attributes.Remove(key);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void read(org.w3c.dom.Element chartItemSpec, org.maltparser.core.flow.FlowChartManager flowCharts) throws org.maltparser.core.exception.MaltChainedException
		public virtual void read(Element chartItemSpec, FlowChartManager flowCharts)
		{
			chartItemName = chartItemSpec.getAttribute("item");
			chartItemClass = flowCharts.FlowChartSystem.getChartElement(chartItemName).ChartItemClass;

			NamedNodeMap attrs = chartItemSpec.Attributes;
			for (int i = 0 ; i < attrs.Length ; i++)
			{
				Attr attribute = (Attr)attrs.item(i);
				addChartItemAttribute(attribute.Name,attribute.Value);
			}
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((string.ReferenceEquals(chartItemName, null)) ? 0 : chartItemName.GetHashCode());
			result = prime * result + ((attributes == null) ? 0 : attributes.GetHashCode());
			result = prime * result + ((chartItemClass == null) ? 0 : chartItemClass.GetHashCode());
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
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			ChartItemSpecification other = (ChartItemSpecification) obj;
			if (string.ReferenceEquals(chartItemName, null))
			{
				if (!string.ReferenceEquals(other.chartItemName, null))
				{
					return false;
				}
			}
			else if (!chartItemName.Equals(other.chartItemName))
			{
				return false;
			}

			if (attributes == null)
			{
				if (other.attributes != null)
				{
					return false;
				}
			}
			else if (!attributes.Equals(other.attributes))
			{
				return false;
			}

			if (chartItemClass == null)
			{
				if (other.chartItemClass != null)
				{
					return false;
				}
			}
			else if (!chartItemClass.Equals(other.chartItemClass))
			{
				return false;
			}

			return true;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(chartItemName);
			sb.Append(' ');
			foreach (string key in attributes.Keys)
			{
				sb.Append(key);
				sb.Append('=');
				sb.Append(attributes[key]);
				sb.Append(' ');
			}
			return sb.ToString();
		}

	}

}