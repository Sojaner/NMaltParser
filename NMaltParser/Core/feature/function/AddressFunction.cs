﻿using System;
using NMaltParser.Core.Feature.Value;

namespace NMaltParser.Core.Feature.Function
{
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
			address = new AddressValue(this);
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
			if (GetType() != obj.GetType())
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