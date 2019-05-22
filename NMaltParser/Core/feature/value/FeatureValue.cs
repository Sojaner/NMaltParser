using System.Text;

namespace NMaltParser.Core.Feature.Value
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public abstract class FeatureValue : FunctionValue
	{
		protected internal bool nullValue;

		public FeatureValue(Function.Function function) : base(function)
		{
			NullValue = true;
		}

		public override void reset()
		{
			NullValue = true;
		}

		public virtual bool NullValue
		{
			get
			{
				return nullValue;
			}
			set
			{
				nullValue = value;
			}
		}


		public abstract bool Multiple {get;}

		public abstract int nFeatureValues();

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
			return base.Equals(obj);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("[null=");
			sb.Append(nullValue);
			sb.Append("]");
			return sb.ToString();
		}
	}
}