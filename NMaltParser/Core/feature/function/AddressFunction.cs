using System;

namespace org.maltparser.core.feature.function
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using AddressValue = org.maltparser.core.feature.value.AddressValue;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public abstract class AddressFunction : Function
	{
		public abstract void update();
		public abstract Type[] ParameterTypes {get;}
		public abstract void initialize(object[] arguments);
		protected internal readonly AddressValue address;
		public AddressFunction()
		{
			this.address = new AddressValue(this);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void update(Object[] arguments) throws org.maltparser.core.exception.MaltChainedException;
		public abstract void update(object[] arguments);

		/// <summary>
		/// Returns the address value of address function
		/// </summary>
		/// <returns> the address value of address function </returns>
		public virtual AddressValue AddressValue
		{
			get
			{
				return address;
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
			if (this.GetType() != obj.GetType())
			{
				return false;
			}

			return address.Equals(((AddressFunction)obj).AddressValue);
		}

		public override string ToString()
		{
			return address.ToString();
		}
	}

}