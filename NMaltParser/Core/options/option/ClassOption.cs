using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.options.option
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.plugin;

	/// <summary>
	/// A class type option is an option that can only contain string value that corresponds to a class. 
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class ClassOption : Option
	{
		private Type defaultValue;
		private readonly SortedSet<string> legalValues;
		private readonly IDictionary<string, string> legalValueDesc;
		private readonly IDictionary<string, Type> legalValueClass;
		private readonly IDictionary<Type, string> classLegalValues;

		/// <summary>
		/// Creates a class type option description
		/// </summary>
		/// <param name="group">	a reference to the option group. </param>
		/// <param name="name">	the name of the option. </param>
		/// <param name="shortDescription">	a short description of the option. </param>
		/// <param name="flag">	a short string that can be used in the command line. </param>
		/// <param name="usage">	a string that explains the usage of the option. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ClassOption(org.maltparser.core.options.OptionGroup group, String name, String shortDescription, String flag, String usage) throws org.maltparser.core.exception.MaltChainedException
		public ClassOption(OptionGroup group, string name, string shortDescription, string flag, string usage) : base(group, name, shortDescription, flag, usage)
		{
			legalValues = Collections.synchronizedSortedSet(new SortedSet<string>());
			legalValueDesc = Collections.synchronizedMap(new Dictionary<string, string>());
			legalValueClass = Collections.synchronizedMap(new Dictionary<string, Type>());
			classLegalValues = Collections.synchronizedMap(new Dictionary<Type, string>());
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getValueObject(java.lang.String)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getValueObject(String value) throws org.maltparser.core.exception.MaltChainedException
		public override object getValueObject(string value)
		{
			if (string.ReferenceEquals(value, null))
			{
				return null;
			}
			else if (legalValues.Contains(value))
			{
				return legalValueClass[value];
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
//ORIGINAL LINE: public Object getDefaultValueObject() throws org.maltparser.core.options.OptionException
		public override object DefaultValueObject
		{
			get
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Returns a string representation of particular class
		/// </summary>
		/// <param name="clazz">	an class object
		/// @return	a string representation of particular class, if not present null is returned. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getLegalValueString(Class clazz) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getLegalValueString(Type clazz)
		{
			return classLegalValues[clazz];
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
				if (string.ReferenceEquals(value, null))
				{
					if (legalValues.Count == 0)
					{
						throw new OptionException("The default value is null and the legal value set is empty for the '" + Name + "' option. ");
					}
					else
					{
						this.defaultValue = legalValueClass[((SortedSet<string>)legalValueClass.Keys).Min];
					}
				}
				else if (legalValues.Contains(value.ToLower()))
				{
					this.defaultValue = legalValueClass[value.ToLower()];
				}
				else
				{
					throw new OptionException("The default value '" + value + "' is not a legal value for the '" + Name + "' option. ");
				}
			}
		}

		/// <summary>
		/// Returns the class that corresponds to the enumerate string value.
		/// </summary>
		/// <param name="value">	an enumerate string value </param>
		/// <returns> the class that corresponds to the enumerate string value. </returns>
		public virtual Type getClazz(string value)
		{
			return legalValueClass[value];
		}

		/// <summary>
		/// Adds a legal value that corresponds to a class
		/// </summary>
		/// <param name="value">	a legal value name </param>
		/// <param name="desc">	a short description of the legal value </param>
		/// <param name="classname">	the fully qualified name of the class </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLegalValue(String value, String desc, String classname) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addLegalValue(string value, string desc, string classname)
		{
			if (string.ReferenceEquals(value, null) || value.Equals(""))
			{
				throw new OptionException("The legal value is missing for the '" + Name + "' option. ");
			}
			else if (legalValues.Contains(value.ToLower()))
			{
				throw new OptionException("The legal value for the '" + Name + "' option already exists. ");
			}
			else
			{
				legalValues.Add(value.ToLower());
				if (string.ReferenceEquals(desc, null) || desc.Equals(""))
				{
					legalValueDesc[value.ToLower()] = "Description is missing. ";
				}
				else
				{
					legalValueDesc[value.ToLower()] = desc;
				}
				if (string.ReferenceEquals(classname, null) || classname.Equals(""))
				{
					throw new OptionException("The class name used by the '" + Name + "' option is missing. ");
				}
				else
				{
					try
					{
						Type clazz = null;
						if (PluginLoader.instance() != null)
						{
							clazz = PluginLoader.instance().getClass(classname);
						}
						if (clazz == null)
						{
							clazz = Type.GetType(classname);
						}
						legalValueClass[value] = clazz;
						classLegalValues[clazz] = value;
					}
					catch (ClassNotFoundException e)
					{
						throw new OptionException("The class " + classname + " for the '" + Name + "' option could not be found. ", e);
					}
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
				return classLegalValues[defaultValue];
			}
		}

		/* (non-Javadoc)
		 * @see org.maltparser.core.options.option.Option#getStringRepresentation(java.lang.Object)
		 */
		public override string getStringRepresentation(object value)
		{
			if (value is Type && classLegalValues.ContainsKey(value))
			{
				return classLegalValues[value];
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