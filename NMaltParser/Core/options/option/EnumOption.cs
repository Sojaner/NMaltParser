using System.Collections.Generic;
using System.Text;

namespace NMaltParser.Core.Options.Option
{
    /// <summary>
	/// An enumerate option is an option that can only contain string value, which is in the legal value set.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class EnumOption : Option
	{
		private string defaultValue;
		private readonly SortedSet<string> legalValues;
		private readonly IDictionary<string, string> legalValueDesc;

		/// <summary>
		/// Creates an enumerate option description
		/// </summary>
		/// <param name="group">	a reference to the option group. </param>
		/// <param name="name">	the name of the option. </param>
		/// <param name="shortDescription">	a short description of the option. </param>
		/// <param name="flag">	a short string that can be used in the command line. </param>
		/// <param name="usage">	a string that explains the usage of the option. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EnumOption(org.maltparser.core.options.OptionGroup group, String name, String shortDescription, String flag, String usage) throws org.maltparser.core.exception.MaltChainedException
		public EnumOption(OptionGroup group, string name, string shortDescription, string flag, string usage) : base(group, name, shortDescription, flag, usage)
		{
			legalValues = Collections.synchronizedSortedSet(new SortedSet<string>());
			legalValueDesc = Collections.synchronizedMap(new Dictionary<string, string>());
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getValueObject(java.lang.String)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getValueObject(String value) throws org.maltparser.core.exception.MaltChainedException
		public override object getValueObject(string value)
		{
			if (ReferenceEquals(value, null))
			{
				return null;
			}
			else if (legalValues.Contains(value))
			{
				return value;
			}
			else
			{
				throw new OptionException("'" + value + "' is not a legal value for the '" + Name + "' option. ");
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
				return defaultValue;
			}
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getDefaultValueString()
		 */
		public override string DefaultValueString
		{
			get
			{
				return defaultValue.ToString();
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
				if (ReferenceEquals(value, null))
				{
					if (legalValues.Count == 0)
					{
						throw new OptionException("The default value of the '" + Name + "' option is null and the legal value set is empty.");
					}
					else
					{
						defaultValue = legalValues.Min;
					}
				}
				else if (legalValues.Contains(value.ToLower()))
				{
					defaultValue = value.ToLower();
				}
				else
				{
					throw new OptionException("The default value '" + value + "' for the '" + Name + "' option is not a legal value. ");
				}
			}
		}

		/// <summary>
		/// Adds a legal value
		/// </summary>
		/// <param name="value">	a legal value name </param>
		/// <param name="desc">	a short description of the legal value </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLegalValue(String value, String desc) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addLegalValue(string value, string desc)
		{
			if (ReferenceEquals(value, null) || value.Equals(""))
			{
				throw new OptionException("The legal value is missing for the '" + Name + "' option. ");
			}
			else if (legalValues.Contains(value.ToLower()))
			{
				throw new OptionException("The legal value '" + value + "' already exists for the '" + Name + "' option. ");
			}
			else
			{
				legalValues.Add(value.ToLower());
				if (ReferenceEquals(desc, null) || desc.Equals(""))
				{
					legalValueDesc[value.ToLower()] = "Description is missing. ";
				}
				else
				{
					legalValueDesc[value.ToLower()] = desc;
				}
			}
		}

		/// <summary>
		/// Adds a legal value without a description
		/// </summary>
		/// <param name="value">	a legal value name </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLegalValue(String value) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addLegalValue(string value)
		{
			addLegalValue(value, null);
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getStringRepresentation(java.lang.Object)
		 */
		public override string getStringRepresentation(object value)
		{
			if (value is string && legalValues.Contains(value))
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
			Formatter formatter = new Formatter(sb);
			foreach (string value in legalValues)
			{
				formatter.format("%2s%-10s - %-20s\n", "", value, legalValueDesc[value]);
			}
			sb.Append("-----------------------------------------------------------------------------\n");
			return sb.ToString();
		}
	}

}