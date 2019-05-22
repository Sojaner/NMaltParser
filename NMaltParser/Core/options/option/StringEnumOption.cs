using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.options.option
{

	using  exception;

	/// <summary>
	/// A string enum type option is an option that can only contain string value that corresponds to another string.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class StringEnumOption : Option
	{
		private string defaultValue;
		private readonly SortedSet<string> legalValues;
		private readonly IDictionary<string, string> legalValueDesc;
		private readonly IDictionary<string, string> valueMapto;
		private readonly IDictionary<string, string> maptoValue;

		/// <summary>
		/// Creates a stringenum type option description
		/// </summary>
		/// <param name="group">	a reference to the option group. </param>
		/// <param name="name">	the name of the option. </param>
		/// <param name="shortDescription">	a short description of the option. </param>
		/// <param name="flag">	a short string that can be used in the command line. </param>
		/// <param name="usage">	a string that explains the usage of the option. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public StringEnumOption(org.maltparser.core.options.OptionGroup group, String name, String shortDescription, String flag, String usage) throws org.maltparser.core.exception.MaltChainedException
		public StringEnumOption(OptionGroup group, string name, string shortDescription, string flag, string usage) : base(group, name, shortDescription, flag, usage)
		{
			legalValues = Collections.synchronizedSortedSet(new SortedSet<string>());
			legalValueDesc = Collections.synchronizedMap(new Dictionary<string, string>());
			valueMapto = Collections.synchronizedMap(new Dictionary<string, string>());
			maptoValue = Collections.synchronizedMap(new Dictionary<string, string>());
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
				return valueMapto[value];
			}
			else
			{
				return value;
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

		/// <summary>
		/// Returns the legal value identifier name (an enumerate string value)
		/// </summary>
		/// <param name="value"> the mapped legal value </param>
		/// <returns> the legal value identifier name, null if it could not be found </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getLegalValueString(String value) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getLegalValueString(string value)
		{
			return maptoValue[value];
		}

		/// <summary>
		/// Returns the mapped legal value
		/// </summary>
		/// <param name="value"> an enumerate string value </param>
		/// <returns> the mapped legal value, null if it could not be found </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getLegalValueMapToString(String value) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getLegalValueMapToString(string value)
		{
			return valueMapto[value];
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
						throw new OptionException("The default value is null and the legal value set is empty for the '" + Name + "' option. ");
					}
					else
					{
						defaultValue = valueMapto[((SortedSet<string>)valueMapto.Keys).Min];
					}
				}
				else if (legalValues.Contains(value.ToLower()))
				{
					defaultValue = valueMapto[value.ToLower()];
				}
				else if (value.Equals(""))
				{
					defaultValue = value;
				}
				else
				{
					throw new OptionException("The default value '" + value + "' for the '" + Name + "' option is not a legal value. ");
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
				return defaultValue.ToString();
			}
		}

		/// <summary>
		/// Returns the mapped legal value that corresponds to the enumerate string value.
		/// </summary>
		/// <param name="value">	an enumerate string value </param>
		/// <returns> the mapped legal value that corresponds to the enumerate string value. </returns>
		public virtual string getMapto(string value)
		{
			return valueMapto[value];
		}

		/// <summary>
		/// Adds a legal value that corresponds to another string
		/// </summary>
		/// <param name="value">	a legal value name </param>
		/// <param name="desc">	a short description of the legal value </param>
		/// <param name="mapto">	a mapto string value </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLegalValue(String value, String desc, String mapto) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addLegalValue(string value, string desc, string mapto)
		{
			if (ReferenceEquals(value, null) || value.Equals(""))
			{
				throw new OptionException("The legal value is missing for the option " + Name + ".");
			}
			else if (legalValues.Contains(value.ToLower()))
			{
				throw new OptionException("The legal value " + value + " already exists for the option " + Name + ". ");
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
				if (ReferenceEquals(mapto, null) || mapto.Equals(""))
				{
					throw new OptionException("A mapto value is missing for the option " + Name + ". ");
				}
				else
				{
					valueMapto[value] = mapto;
					maptoValue[mapto] = value;
				}
			}
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getStringRepresentation(java.lang.Object)
		 */
		public override string getStringRepresentation(object value)
		{
			if (value is string)
			{
				if (legalValues.Contains(value))
				{
					return valueMapto[value];
				}
				else
				{
					return value.ToString();
				}
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
			return sb.ToString();
		}
	}
}