using System;
using System.Text;

namespace org.maltparser.core.options.option
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;

	/// <summary>
	/// Abstract class that contains description of an option that are the same over all option types.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public abstract class Option : IComparable<Option>
	{
		public const int NONE = 0;
		/// <summary>
		/// The option is only relevant during learning 
		/// </summary>
		public const int TRAIN = 1;
		/// <summary>
		/// The option is only relevant during processing (parsing)
		/// </summary>
		public const int PROCESS = 2;
		/// <summary>
		/// The option is relevant both during learning and processing (parsing)
		/// </summary>
		public const int BOTH = 3;
		/// <summary>
		/// The option is saved during learning and cannot be overloaded during processing (parsing)
		/// </summary>
		public const int SAVE = 4;

		private readonly OptionGroup group;
		private readonly string name;
		private readonly string shortDescription;
		private string flag;
		private int usage;
		private bool ambiguous;

		/// <summary>
		/// Creates an option description
		/// </summary>
		/// <param name="group">	a reference to the option group. </param>
		/// <param name="name">	the name of the option. </param>
		/// <param name="shortDescription">	a short description of the option. </param>
		/// <param name="flag">	a flag that can be used in the command line. </param>
		/// <param name="usage">	a string that explains the usage of the option. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Option(org.maltparser.core.options.OptionGroup group, String name, String shortDescription, String flag, String usage) throws org.maltparser.core.exception.MaltChainedException
		public Option(OptionGroup group, string name, string shortDescription, string flag, string usage)
		{
			this.group = group;
			if (string.ReferenceEquals(name, null) || name.Length == 0)
			{
				throw new OptionException("The option name has no value. ");
			}
			this.name = name.ToLower();
			this.shortDescription = shortDescription;
			Flag = flag;
			setUsage(usage);
			Ambiguous = false;
		}

		/// <summary>
		/// Returns the corresponding object for the option value (specified as a string value).
		/// </summary>
		/// <param name="value">	the string option value </param>
		/// <returns> the corresponding object for the option value (specified as a string value). </returns>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Object getValueObject(String value) throws org.maltparser.core.exception.MaltChainedException;
		public abstract object getValueObject(string value);

		/// <summary>
		/// Returns the object for the default value for option.
		/// 
		/// @return	the object for the default value for option. </summary>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract Object getDefaultValueObject() throws org.maltparser.core.exception.MaltChainedException;
		public abstract object DefaultValueObject {get;}

		/// <summary>
		/// Returns a string representation of the default value.
		/// </summary>
		/// <returns> a string representation of the default value </returns>
		public abstract string DefaultValueString {get;}

		/// <summary>
		/// Sets the default value for the option.
		/// </summary>
		/// <param name="defaultValue">	the string default value </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void setDefaultValue(String defaultValue) throws org.maltparser.core.exception.MaltChainedException;
		public abstract string DefaultValue {set;}


		/// <summary>
		/// Returns a string representation of the option value.
		/// </summary>
		/// <param name="value">	an option value object </param>
		/// <returns> a string representation of the option value, if the option value could not be found null is returned. </returns>
		public abstract string getStringRepresentation(object value);

		/// <summary>
		/// Returns a reference to a option group.
		/// 
		/// @return	a reference to a option group.
		/// </summary>
		public virtual OptionGroup Group
		{
			get
			{
				return group;
			}
		}

		/// <summary>
		/// Returns the name of the option.
		/// 
		/// @return	the name of the option.
		/// </summary>
		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Returns a short description of the option
		/// 
		/// @return	a short description of the option
		/// </summary>
		public virtual string ShortDescription
		{
			get
			{
				return shortDescription;
			}
		}

		/// <summary>
		/// Returns a character that is used as a flag for the command line input
		/// 
		/// @return	a character that is used as a flag for the command line input
		/// </summary>
		public virtual string Flag
		{
			get
			{
				return flag;
			}
			set
			{
				if (string.ReferenceEquals(value, null))
				{
					this.flag = null;
				}
				else
				{
					this.flag = value;
				}
			}
		}
		/// <summary>
		/// Returns the usage of the option.
		/// 
		/// @return	the usage of the option.
		/// </summary>
		public virtual int getUsage()
		{
			return usage;
		}
		/// <summary>
		/// Sets the usage of the option.
		/// </summary>
		/// <param name="usage">	the usage of the option. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUsage(String usage) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setUsage(string usage)
		{
			if (string.ReferenceEquals(usage, null) || usage.Equals("") || usage.ToLower().Equals("none"))
			{
				this.usage = Option.NONE;
			}
			else if (usage.ToLower().Equals("train"))
			{
				this.usage = Option.TRAIN;
			}
			else if (usage.ToLower().Equals("process"))
			{
				this.usage = Option.PROCESS;
			}
			else if (usage.ToLower().Equals("both"))
			{
				this.usage = Option.BOTH;
			}
			else if (usage.ToLower().Equals("save"))
			{
				this.usage = Option.SAVE;
			}
			else
			{
				throw new OptionException("Illegal use of the usage attibute value: " + usage + " for the '" + Name + "' option. ");
			}
		}
		/// <summary>
		/// Sets the usage of the option.
		/// </summary>
		/// <param name="usage">	the usage of the option. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUsage(int usage) throws org.maltparser.core.exception.MaltChainedException
		public virtual void setUsage(int usage)
		{
			if (usage >= 0 && usage <= 4)
			{
				this.usage = usage;
			}
			else
			{
				throw new OptionException("Illegal use of the usage attibute value: " + usage + " for the '" + Name + "' option. ");
			}
		}

		/// <summary>
		/// Returns true if the option name is ambiguous over all option groups, otherwise false.
		/// 
		/// @return	true if the option name is ambiguous over all option groups, otherwise false.
		/// </summary>
		public virtual bool Ambiguous
		{
			get
			{
				return ambiguous;
			}
			set
			{
				this.ambiguous = value;
			}
		}


		public virtual int CompareTo(Option o)
		{
			if (group.Name.Equals(o.group.Name))
			{
				return name.CompareTo(o.Name);
			}
			return group.Name.CompareTo(o.group.Name);
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			int splitsize = 45;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			Formatter formatter = new Formatter(sb);
			formatter.format("%-20s ", Name);
			if (Ambiguous)
			{
				formatter.format("*");
			}
			else
			{
				sb.Append(" ");
			}
			if (!string.ReferenceEquals(Flag, null))
			{
				formatter.format("(%4s) : ", "-" + Flag);
			}
			else
			{
				sb.Append("       : ");
			}
			int r = shortDescription.Length / splitsize;
			for (int i = 0; i <= r; i++)
			{
				if (shortDescription.Substring(splitsize * i).Length <= splitsize)
				{
					formatter.format(((i == 0)?"%s":"%28s") + "%-45s\n", "", shortDescription.Substring(splitsize * i));
				}
				else
				{
					formatter.format(((i == 0)?"%s":"%28s") + "%-45s\n", "", shortDescription.Substring(splitsize * i, splitsize));
				}
			}
			return sb.ToString();
		}
	}

}