using System;
using System.Text;

namespace org.maltparser.ml.lib
{

	public class MaltFeatureNode : IComparable<MaltFeatureNode>
	{
		internal int index;
		internal double value;

		public MaltFeatureNode()
		{
			index = -1;
			value = 0;
		}

		public MaltFeatureNode(int index, double value)
		{
			Index = index;
			Value = value;
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
			set
			{
				this.index = value;
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


		public virtual void clear()
		{
			index = -1;
			value = 0;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long temp = Double.doubleToLongBits(value);
			long temp = System.BitConverter.DoubleToInt64Bits(value);
			return prime * (prime + index) + (int)(temp ^ ((long)((ulong)temp >> 32)));
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
			MaltFeatureNode other = (MaltFeatureNode) obj;
			if (index != other.index)
			{
				return false;
			}
			if (System.BitConverter.DoubleToInt64Bits(value) != Double.doubleToLongBits(other.value))
			{
				return false;
			}
			return true;
		}

		public virtual int CompareTo(MaltFeatureNode aThat)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;

			if (this == aThat)
			{
				return EQUAL;
			}

			if (this.index < aThat.index)
			{
				return BEFORE;
			}
			if (this.index > aThat.index)
			{
				return AFTER;
			}

			if (this.value < aThat.value)
			{
				return BEFORE;
			}
			if (this.value > aThat.value)
			{
				return AFTER;
			}

			return EQUAL;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("MaltFeatureNode [index=");
			sb.Append(index);
			sb.Append(", value=");
			sb.Append(value);
			sb.Append("]");
			return sb.ToString();
		}
	}

}