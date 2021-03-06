﻿using System.Text;

namespace NMaltParser.Core.Feature.Value
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class SingleFeatureValue : FeatureValue
	{
		protected internal int indexCode;
		protected internal string symbol;
		protected internal double value;

		public SingleFeatureValue(Function.Function function) : base(function)
		{
			IndexCode = 0;
			Symbol = null;
			Value = 0;
		}

		public override void reset()
		{
			base.reset();
			IndexCode = 0;
			Symbol = null;
			Value = 0;
		}

		public virtual void update(int indexCode, string symbol, bool nullValue, double value)
		{
			this.indexCode = indexCode;
			this.symbol = symbol;
			this.nullValue = nullValue;
			this.value = value;
		}

		public virtual int IndexCode
		{
			get
			{
				return indexCode;
			}
			set
			{
				indexCode = value;
			}
		}


		public virtual string Symbol
		{
			get
			{
				return symbol;
			}
			set
			{
				symbol = value;
			}
		}


		public virtual double Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}


		public override bool Multiple
		{
			get
			{
				return false;
			}
		}

		public override int nFeatureValues()
		{
			return 1;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			return prime * (prime + indexCode) + ((ReferenceEquals(symbol, null)) ? 0 : symbol.GetHashCode());
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
			SingleFeatureValue other = (SingleFeatureValue) obj;
			if (indexCode != other.indexCode)
			{
				return false;
			}
			if (ReferenceEquals(symbol, null))
			{
				if (!ReferenceEquals(other.symbol, null))
				{
					return false;
				}
			}
			else if (!symbol.Equals(other.symbol))
			{
				return false;
			}
			return base.Equals(obj);
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append('{');
			sb.Append(symbol);
			sb.Append("->");
			sb.Append(indexCode);
			sb.Append('}');
			return sb.ToString();
		}
	}

}