using System;
using System.Collections.Generic;
using System.Text;

namespace NMaltParser.Core.Feature.Spec
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class SpecificationModel : IEnumerable<SpecificationSubModel>
	{
		private readonly string specModelName;
		private readonly LinkedHashMap<string, SpecificationSubModel> subModelMap;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SpecificationModel() throws org.maltparser.core.exception.MaltChainedException
		public SpecificationModel() : this(null)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SpecificationModel(String _specModelName) throws org.maltparser.core.exception.MaltChainedException
		public SpecificationModel(string _specModelName)
		{
			specModelName = _specModelName;
			subModelMap = new LinkedHashMap<string, SpecificationSubModel>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(String featureSpec) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(string featureSpec)
		{
			add("MAIN", featureSpec);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void add(String subModelName, String featureSpec) throws org.maltparser.core.exception.MaltChainedException
		public virtual void add(string subModelName, string featureSpec)
		{
			if (ReferenceEquals(subModelName, null) || subModelName.Length < 1 || subModelName.ToUpper().Equals("MAIN"))
			{
				if (!subModelMap.containsKey("MAIN"))
				{
					subModelMap.put("MAIN", new SpecificationSubModel("MAIN"));
				}
				subModelMap.get("MAIN").add(featureSpec);
			}
			else
			{
				if (!subModelMap.containsKey(subModelName.ToUpper()))
				{
					subModelMap.put(subModelName.ToUpper(), new SpecificationSubModel(subModelName.ToUpper()));
				}
				subModelMap.get(subModelName.ToUpper()).add(featureSpec);
			}
		}

		public virtual string SpecModelName
		{
			get
			{
				return specModelName;
			}
		}

		public virtual IEnumerator<SpecificationSubModel> GetEnumerator()
		{
			return subModelMap.values().GetEnumerator();
		}

		public virtual int size()
		{
			return subModelMap.size();
		}

		public virtual SpecificationSubModel getSpecSubModel(string subModelName)
		{
			return subModelMap.get(subModelName);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach (SpecificationSubModel subModel in this)
			{
				if (subModel.Size() > 0)
				{
					if (subModelMap.size() != 1 || subModel.SubModelName.Equals("MAIN", StringComparison.OrdinalIgnoreCase))
					{
						sb.Append(subModel.SubModelName);
						sb.Append('\n');
					}
					sb.Append(subModel.ToString());
				}
			}
			return sb.ToString();
		}
	}

}