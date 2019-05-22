using System;

namespace NMaltParser.ML.LibLinear
{
	public class XNode : IComparable<XNode>
	{
		private int index;
		private double value;

		public XNode(int index, double value)
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
				index = value;
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


		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + index;
			long temp;
			temp = BitConverter.DoubleToInt64Bits(value);
			result = prime * result + (int)(temp ^ ((long)((ulong)temp >> 32)));
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
			if (GetType() != obj.GetType())
			{
				return false;
			}
			XNode other = (XNode) obj;
			if (index != other.index)
			{
				return false;
			}
			if (BitConverter.DoubleToInt64Bits(value) != Double.doubleToLongBits(other.value))
			{
				return false;
			}
			return true;
		}

		public virtual int CompareTo(XNode aThat)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;

			if (this == aThat)
			{
				return EQUAL;
			}

			if (index < aThat.index)
			{
				return BEFORE;
			}
			if (index > aThat.index)
			{
				return AFTER;
			}

			if (value < aThat.value)
			{
				return BEFORE;
			}
			if (value > aThat.value)
			{
				return AFTER;
			}

			return EQUAL;
		}

		public override string ToString()
		{
			return "XNode [index=" + index + ", value=" + value + "]";
		}
	}

}