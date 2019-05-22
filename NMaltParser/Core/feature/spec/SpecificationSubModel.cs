using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.feature.spec
{

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class SpecificationSubModel : IEnumerable<string>
	{
		private readonly Pattern blanks = Pattern.compile("\\s+");
		private readonly ISet<string> featureSpecSet;
		private readonly string name;


		public SpecificationSubModel() : this("MAIN")
		{
		}

		public SpecificationSubModel(string _name)
		{
			this.name = _name;
			this.featureSpecSet = new SortedSet<string>();
		}

		public virtual void add(string featureSpec)
		{
			if (!string.ReferenceEquals(featureSpec, null) && featureSpec.Trim().Length > 0)
			{
				string strippedFeatureSpec = blanks.matcher(featureSpec).replaceAll("");
				featureSpecSet.Add(strippedFeatureSpec);
			}
		}

		public virtual string SubModelName
		{
			get
			{
				return name;
			}
		}

		public virtual int size()
		{
			return featureSpecSet.Count;
		}

		public virtual IEnumerator<string> GetEnumerator()
		{
			return featureSpecSet.GetEnumerator();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (string str in featureSpecSet)
			{
				sb.Append(str);
				sb.Append('\n');
			}
			return sb.ToString();
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((featureSpecSet == null) ? 0 : featureSpecSet.GetHashCode());
			result = prime * result + ((string.ReferenceEquals(name, null)) ? 0 : name.GetHashCode());
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
			SpecificationSubModel other = (SpecificationSubModel) obj;
			if (featureSpecSet == null)
			{
				if (other.featureSpecSet != null)
				{
					return false;
				}
			}
			else if (!featureSpecSet.SetEquals(other.featureSpecSet))
			{
				return false;
			}
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
			return true;
		}
	}

}