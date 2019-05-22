using System;

namespace org.maltparser.ml.liblinear
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


		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + index;
			long temp;
			temp = System.BitConverter.DoubleToInt64Bits(value);
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
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			XNode other = (XNode) obj;
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

		public virtual int CompareTo(XNode aThat)
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
			return "XNode [index=" + index + ", value=" + value + "]";
		}
	}

}