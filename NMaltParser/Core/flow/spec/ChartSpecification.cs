using System.Text;

namespace org.maltparser.core.flow.spec
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using Element = org.w3c.dom.Element;
	using NodeList = org.w3c.dom.NodeList;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class ChartSpecification
	{
		private string name;
		private LinkedHashSet<ChartItemSpecification> preProcessChartItemSpecifications;
		private LinkedHashSet<ChartItemSpecification> processChartItemSpecifications;
		private LinkedHashSet<ChartItemSpecification> postProcessChartItemSpecifications;

		public ChartSpecification()
		{
			preProcessChartItemSpecifications = new LinkedHashSet<ChartItemSpecification>(7);
			processChartItemSpecifications = new LinkedHashSet<ChartItemSpecification>(7);
			postProcessChartItemSpecifications = new LinkedHashSet<ChartItemSpecification>(7);
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
			}
		}

		public virtual LinkedHashSet<ChartItemSpecification> PreProcessChartItemSpecifications
		{
			get
			{
				return preProcessChartItemSpecifications;
			}
		}

		public virtual void addPreProcessChartItemSpecifications(ChartItemSpecification chartItemSpecification)
		{
			preProcessChartItemSpecifications.add(chartItemSpecification);
		}

		public virtual void removePreProcessChartItemSpecifications(ChartItemSpecification chartItemSpecification)
		{
			preProcessChartItemSpecifications.remove(chartItemSpecification);
		}

		public virtual LinkedHashSet<ChartItemSpecification> ProcessChartItemSpecifications
		{
			get
			{
				return processChartItemSpecifications;
			}
		}

		public virtual void addProcessChartItemSpecifications(ChartItemSpecification chartItemSpecification)
		{
			processChartItemSpecifications.add(chartItemSpecification);
		}

		public virtual void removeProcessChartItemSpecifications(ChartItemSpecification chartItemSpecification)
		{
			processChartItemSpecifications.remove(chartItemSpecification);
		}

		public virtual LinkedHashSet<ChartItemSpecification> PostProcessChartItemSpecifications
		{
			get
			{
				return postProcessChartItemSpecifications;
			}
		}

		public virtual void addPostProcessChartItemSpecifications(ChartItemSpecification chartItemSpecification)
		{
			postProcessChartItemSpecifications.add(chartItemSpecification);
		}

		public virtual void removePostProcessChartItemSpecifications(ChartItemSpecification chartItemSpecification)
		{
			postProcessChartItemSpecifications.remove(chartItemSpecification);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void read(org.w3c.dom.Element chartElem, org.maltparser.core.flow.FlowChartManager flowCharts) throws org.maltparser.core.exception.MaltChainedException
		public virtual void read(Element chartElem, FlowChartManager flowCharts)
		{
			Name = chartElem.getAttribute("name");
			NodeList flowChartProcessList = chartElem.getElementsByTagName("preprocess");
			if (flowChartProcessList.Length == 1)
			{
				readChartItems((Element)flowChartProcessList.item(0), flowCharts, preProcessChartItemSpecifications);
			}
			else if (flowChartProcessList.Length > 1)
			{
				throw new FlowException("The flow chart '" + Name + "' has more than one preprocess elements. ");
			}

			flowChartProcessList = chartElem.getElementsByTagName("process");
			if (flowChartProcessList.Length == 1)
			{
				readChartItems((Element)flowChartProcessList.item(0), flowCharts, processChartItemSpecifications);
			}
			else if (flowChartProcessList.Length > 1)
			{
				throw new FlowException("The flow chart '" + Name + "' has more than one process elements. ");
			}

			flowChartProcessList = chartElem.getElementsByTagName("postprocess");
			if (flowChartProcessList.Length == 1)
			{
				readChartItems((Element)flowChartProcessList.item(0), flowCharts, postProcessChartItemSpecifications);
			}
			else if (flowChartProcessList.Length > 1)
			{
				throw new FlowException("The flow chart '" + Name + "' has more than one postprocess elements. ");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readChartItems(org.w3c.dom.Element chartElem, org.maltparser.core.flow.FlowChartManager flowCharts, java.util.LinkedHashSet<ChartItemSpecification> chartItemSpecifications) throws org.maltparser.core.exception.MaltChainedException
		private void readChartItems(Element chartElem, FlowChartManager flowCharts, LinkedHashSet<ChartItemSpecification> chartItemSpecifications)
		{
			NodeList flowChartItemList = chartElem.getElementsByTagName("chartitem");
			for (int i = 0; i < flowChartItemList.Length; i++)
			{
				ChartItemSpecification chartItemSpecification = new ChartItemSpecification();
				chartItemSpecification.read((Element)flowChartItemList.item(i), flowCharts);
				chartItemSpecifications.add(chartItemSpecification);
			}
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((string.ReferenceEquals(name, null)) ? 0 : name.GetHashCode());
			result = prime * result + ((postProcessChartItemSpecifications == null) ? 0 : postProcessChartItemSpecifications.GetHashCode());
			result = prime * result + ((preProcessChartItemSpecifications == null) ? 0 : preProcessChartItemSpecifications.GetHashCode());
			result = prime * result + ((processChartItemSpecifications == null) ? 0 : processChartItemSpecifications.GetHashCode());
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
			ChartSpecification other = (ChartSpecification) obj;
			if (string.ReferenceEquals(name, null))
			{
				if (!string.ReferenceEquals(other.name, null))
				{
					return false;
				}
			}
			else if (!name.Equals(other.name))
			{
				return false;
			}
			if (postProcessChartItemSpecifications == null)
			{
				if (other.postProcessChartItemSpecifications != null)
				{
					return false;
				}
			}
			else if (!postProcessChartItemSpecifications.Equals(other.postProcessChartItemSpecifications))
			{
				return false;
			}
			if (preProcessChartItemSpecifications == null)
			{
				if (other.preProcessChartItemSpecifications != null)
				{
					return false;
				}
			}
			else if (!preProcessChartItemSpecifications.Equals(other.preProcessChartItemSpecifications))
			{
				return false;
			}
			if (processChartItemSpecifications == null)
			{
				if (other.processChartItemSpecifications != null)
				{
					return false;
				}
			}
			else if (!processChartItemSpecifications.Equals(other.processChartItemSpecifications))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(name);
			sb.Append('\n');
			if (preProcessChartItemSpecifications.size() > 0)
			{
				sb.Append("  preprocess:");
				sb.Append('\n');
				foreach (ChartItemSpecification key in preProcessChartItemSpecifications)
				{
					sb.Append(key);
					sb.Append('\n');
				}
			}
			if (processChartItemSpecifications.size() > 0)
			{
				sb.Append("  process:");
				sb.Append('\n');
				foreach (ChartItemSpecification key in processChartItemSpecifications)
				{
					sb.Append(key);
					sb.Append('\n');
				}
			}
			if (postProcessChartItemSpecifications.size() > 0)
			{
				sb.Append("  postprocess:");
				sb.Append('\n');
				foreach (ChartItemSpecification key in postProcessChartItemSpecifications)
				{
					sb.Append(key);
					sb.Append('\n');
				}
			}
			return sb.ToString();
		}
	}

}