using System;
using System.Text;

namespace org.maltparser.core.options.option
{
	using  org.maltparser.core.exception;

	/// <summary>
	/// A boolean option is an option that can only contain a boolean value (true or false). 
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class BoolOption : Option
	{
		private bool? defaultValue;

		/// <summary>
		/// Creates a boolean option description
		/// </summary>
		/// <param name="group">	a reference to the option group. </param>
		/// <param name="name">	the name of the option. </param>
		/// <param name="shortDescription">	a short description of the option. </param>
		/// <param name="flag">	a short string that can be used in the command line. </param>
		/// <param name="usage">	a string that explains the usage of the option. </param>
		/// <param name="defaultValue">	a default value string (true or false). </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BoolOption(org.maltparser.core.options.OptionGroup group, String name, String shortDescription, String flag, String usage, String defaultValue) throws org.maltparser.core.exception.MaltChainedException
		public BoolOption(OptionGroup group, string name, string shortDescription, string flag, string usage, string defaultValue) : base(group, name, shortDescription, flag, usage)
		{
			DefaultValue = defaultValue;
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getValueObject(java.lang.String)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getValueObject(String value) throws org.maltparser.core.exception.MaltChainedException
		public override object getValueObject(string value)
		{
			if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				return new bool?(true);
			}
			else if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
			{
				return new bool?(false);
			}
			else
			{
				throw new OptionException("Illegal boolean value '" + value + "' for the '" + Name + "' option. ");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getDefaultValueObject() throws org.maltparser.core.exception.MaltChainedException
		public override object DefaultValueObject
		{
			get
			{
				return new bool?(defaultValue);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDefaultValue(String defaultValue) throws org.maltparser.core.exception.MaltChainedException
		public override string DefaultValue
		{
			set
			{
				if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					this.defaultValue = true;
				}
				else if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
				{
					this.defaultValue = false;
				}
				else
				{
					throw new OptionException("Illegal boolean default value '" + value + "' for the '" + Name + "' option. ");
				}
			}
		}

		public override string DefaultValueString
		{
			get
			{
				return defaultValue.ToString();
			}
		}

		public override string getStringRepresentation(object value)
		{
			if (value is bool?)
			{
				return value.ToString();
			}
			return null;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("-----------------------------------------------------------------------------\n");
			return sb.ToString();
		}
	}


}