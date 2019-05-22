using System;
using System.Text;

namespace NMaltParser.Core.Feature.Value
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class AddressValue : FunctionValue
	{
		private object address;

		public AddressValue(Function.Function function) : base(function)
		{
			Address = null;
		}

		public override void reset()
		{
			Address = null;
		}

		public virtual Type AddressClass
		{
			get
			{
				if (address != null)
				{
					return address.GetType();
				}
				return null;
			}
		}

		public virtual object Address
		{
			get
			{
				return address;
			}
			set
			{
				address = value;
			}
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
			AddressValue other = (AddressValue) obj;
			if (address == null)
			{
				if (other.address != null)
				{
					return false;
				}
			}
			else if (!address.Equals(other.address))
			{
				return false;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return 31 + ((address == null) ? 0 : address.GetHashCode());
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append(address.ToString());
			return sb.ToString();
		}
	}

}