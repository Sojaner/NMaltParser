using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.options
{

	using  org.maltparser.core.options.option;


	/// <summary>
	/// An option group categories a group of options.
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class OptionGroup
	{
		private readonly string name;
		private readonly Dictionary<string, Option> options;

		public static int toStringSetting = 0;
		public const int WITHGROUPNAME = 0;
		public const int NOGROUPNAME = 1;

		/// <summary>
		/// Creates an option group with an option group name.
		/// </summary>
		/// <param name="name">	The name of the option group </param>
		public OptionGroup(string name)
		{
			this.name = name;
			options = new Dictionary<string, Option>();
		}

		/// <summary>
		/// Returns the name of the option group
		/// 
		/// @return	the name of the option group
		/// </summary>
		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Sets the name of the option group
		/// </summary>
		/// <param name="name">	the name of the option group </param>
	//	public void setName(String name) {
	//		this.name = name.toLowerCase();
	//	}

		/// <summary>
		/// Adds an option to the option group.
		/// </summary>
		/// <param name="option">	an option </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addOption(org.maltparser.core.options.option.Option option) throws OptionException
		public virtual void addOption(Option option)
		{
			if (string.ReferenceEquals(option.Name, null) || option.Name.Equals(""))
			{
				throw new OptionException("The option name is null or contains the empty string. ");
			}
			else if (options.ContainsKey(option.Name.ToLower()))
			{
				throw new OptionException("The option name already exists for that option group. ");
			}
			else
			{
				options[option.Name.ToLower()] = option;
			}
		}

		/// <summary>
		/// Returns the option according to the option name.
		/// </summary>
		/// <param name="optionname">	an option name
		/// @return	an option, <i>null</i> if the option name can't be found </param>
		public virtual Option getOption(string optionname)
		{
			return options[optionname];
		}

		/// <summary>
		/// Returns all options for this option group.
		/// 
		/// @return	a list of options
		/// </summary>
		public virtual ICollection<Option> OptionList
		{
			get
			{
				return options.Values;
			}
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
	//		for (String value : new TreeSet<String>(options.keySet())) {
	//			sb.append("super.put(\""); 
	//			sb.append("--");
	//			sb.append(name);
	//			sb.append("-");
	//			sb.append(options.get(value).getName());
	//			sb.append("\", \"");
	//			sb.append(options.get(value).getDefaultValueString());
	//			sb.append("\");\n");
	//		}

			if (OptionGroup.toStringSetting == OptionGroup.WITHGROUPNAME)
			{
				sb.Append("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");
				sb.Append("+ " + name + "\n");
				sb.Append("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");
			}

			foreach (string value in new SortedSet<string>(options.Keys))
			{
				sb.Append(options[value].ToString());
			}
			return sb.ToString();
		}
	}

}