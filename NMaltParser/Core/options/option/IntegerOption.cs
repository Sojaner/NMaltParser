using System;
using System.Text;

namespace org.maltparser.core.options.option
{
	using  org.maltparser.core.exception;

	/// <summary>
	/// An integer option is an option that can only contain an integer value.  
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class IntegerOption : Option
	{
		private int defaultValue = 0;

		/// <summary>
		/// Creates an integer option description
		/// </summary>
		/// <param name="group">	a reference to the option group. </param>
		/// <param name="name">	the name of the option. </param>
		/// <param name="shortDescription">	a short description of the option. </param>
		/// <param name="flag">	a short string that can be used in the command line. </param>
		/// <param name="usage">	a string that explains the usage of the option. </param>
		/// <param name="defaultValue">	a default value string (must be an integer value). </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public IntegerOption(org.maltparser.core.options.OptionGroup group, String name, String shortDescription, String flag, String usage, String defaultValue) throws org.maltparser.core.exception.MaltChainedException
		public IntegerOption(OptionGroup group, string name, string shortDescription, string flag, string usage, string defaultValue) : base(group, name, shortDescription, flag, usage)
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
			try
			{
				return new int?(int.Parse(value));
			}
			catch (System.FormatException e)
			{
				throw new OptionException("Illegal integer value '" + value + "' for the '" + Name + "' option. ", e);
			}
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getDefaultValueObject()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getDefaultValueObject() throws org.maltparser.core.exception.MaltChainedException
		public override object DefaultValueObject
		{
			get
			{
				return new int?(defaultValue);
			}
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#setDefaultValue(java.lang.String)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setDefaultValue(String defaultValue) throws org.maltparser.core.exception.MaltChainedException
		public override string DefaultValue
		{
			set
			{
				try
				{
					this.defaultValue = int.Parse(value);
				}
				catch (System.FormatException e)
				{
					throw new OptionException("Illegal integer default value '" + value + "' for the '" + Name + "' option. ", e);
				}
			}
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getDefaultValueString()
		 */
		public override string DefaultValueString
		{
			get
			{
				return Convert.ToString(defaultValue);
			}
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getStringRepresentation(java.lang.Object)
		 */
		public override string getStringRepresentation(object value)
		{
			if (value is int?)
			{
				return value.ToString();
			}
			return null;
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#toString()
		 */
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