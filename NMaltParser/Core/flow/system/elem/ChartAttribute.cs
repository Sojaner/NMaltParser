using System.Text;

namespace org.maltparser.core.flow.system.elem
{
	using  org.maltparser.core.exception;
	using  org.w3c.dom;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class ChartAttribute
	{
		private string name;
		private string defaultValue;


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


		public virtual string DefaultValue
		{
			get
			{
				return defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void read(org.w3c.dom.Element attrElem, org.maltparser.core.flow.system.FlowChartSystem flowChartSystem) throws org.maltparser.core.exception.MaltChainedException
		public virtual void read(Element attrElem, FlowChartSystem flowChartSystem)
		{
			Name = attrElem.getAttribute("name");
			DefaultValue = attrElem.getAttribute("default");
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(name);
			sb.Append(' ');
			sb.Append(defaultValue);
			return sb.ToString();
		}
	}

}