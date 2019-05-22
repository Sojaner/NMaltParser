using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.feature.value
{

	using  function;
	using  helper;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class MultipleFeatureValue : FeatureValue
	{
		protected internal SortedDictionary<int, string> featureValues;

		public MultipleFeatureValue(Function function) : base(function)
		{
			FeatureValues = new SortedDictionary<int, string>();
		}

		public override void reset()
		{
			base.reset();
			featureValues.Clear();
		}

		public virtual void addFeatureValue(int code, string Symbol)
		{
			featureValues[code] = Symbol;
		}

		protected internal virtual SortedDictionary<int, string> FeatureValues
		{
			set
			{
				featureValues = value;
			}
		}

		public virtual ISet<int> Codes
		{
			get
			{
				return (ISet<int>)featureValues.Keys;
			}
		}

		public virtual int FirstCode
		{
			get
			{
				return featureValues.firstKey();
			}
		}

		public virtual ISet<string> Symbols
		{
			get
			{
				return new HashSet<string>(featureValues.Values);
			}
		}

		public virtual string FirstSymbol
		{
			get
			{
				return featureValues[featureValues.firstKey()];
			}
		}

		public override bool Multiple
		{
			get
			{
				return true;
			}
		}

		public override int nFeatureValues()
		{
			return featureValues.Count;
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
			MultipleFeatureValue v = ((MultipleFeatureValue)obj);
			if (!featureValues.Equals(v.featureValues))
			{
				return false;
			}
			return base.Equals(obj);
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append('{');
			foreach (int? code in featureValues.Keys)
			{
				sb.Append('{');
				sb.Append(featureValues[code]);
				sb.Append("->");
				sb.Append(code);
				sb.Append('}');
			}
			sb.Append('}');
			return sb.ToString();
		}
	}

}