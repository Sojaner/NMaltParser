using System;
using System.Collections.Generic;
using System.Text;

namespace NMaltParser.Core.Options
{
    /// <summary>
	/// An option container stores the option values for one instance usage. For example, a
	/// single malt configuration there will only be one option container, but for an ensemble parser there
	/// could be several option containers. 
	/// 
	/// There are four types internal option container:
	/// <ul>
	/// <li>SAVEDOPTION, contains option values load from the saved option file.
	/// <li>DEPENDENCIES_RESOLVED, contains option values that overload option values in COMMANDLINE and OPTIONFILE
	/// due to dependencies with other options.
	/// <li>COMMANDLINE, contains option values that are read from the command-line prompt.
	/// <li>OPTIONFILE, contains option values that are read from the option file.
	/// </ul>
	/// <para>These internal option containers have following priority: SAVEDOPTION, DEPENDENCIES_RESOLVED, COMMANDLINE,
	/// OPTIONFILE. If an option cannot be found in the SAVEDOPTION internal option container it will continue to
	/// look in the DEPENDENCIES_RESOLVED internal option container and and so fourth. If the option value cannot be
	/// found in none of the internal option container, the option manager uses the default option value provided by
	/// the option description.</para>
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class OptionContainer : IComparable<OptionContainer>
	{
		/* Types of internal option container */
		public const int SAVEDOPTION = 0;
		public const int DEPENDENCIES_RESOLVED = 1;
		public const int COMMANDLINE = 2;
		public const int OPTIONFILE = 3;

		private readonly int index;
		private readonly SortedDictionary<Option.Option, object> savedOptionMap;
		private readonly SortedDictionary<Option.Option, object> dependenciesResolvedOptionMap;
		private readonly SortedDictionary<Option.Option, object> commandLineOptionMap;
		private readonly SortedDictionary<Option.Option, object> optionFileOptionMap;

		/// <summary>
		/// Creates an option container
		/// </summary>
		/// <param name="index"> The index of the option container (0..n). </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public OptionContainer(int index) throws OptionException
		public OptionContainer(int index)
		{
			this.index = index;
			savedOptionMap = Collections.synchronizedSortedMap(new SortedDictionary<Option.Option, object>());
			dependenciesResolvedOptionMap = Collections.synchronizedSortedMap(new SortedDictionary<Option.Option, object>());
			commandLineOptionMap = Collections.synchronizedSortedMap(new SortedDictionary<Option.Option, object>());
			optionFileOptionMap = Collections.synchronizedSortedMap(new SortedDictionary<Option.Option, object>());
		}

		/// <summary>
		/// Adds an option value to an option to one of the internal option container specified by the type.
		/// </summary>
		/// <param name="type">	the internal option container </param>
		/// <param name="option">	the option object </param>
		/// <param name="value">		the option value object </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void addOptionValue(int type, org.maltparser.core.options.option.Option option, Object value) throws OptionException
		protected internal virtual void addOptionValue(int type, Option.Option option, object value)
		{
			if (type == SAVEDOPTION)
			{
				savedOptionMap[option] = value;
			}
			else if (type == DEPENDENCIES_RESOLVED)
			{
				dependenciesResolvedOptionMap[option] = value;
			}
			else if (type == COMMANDLINE)
			{
				commandLineOptionMap[option] = value;
			}
			else if (type == OPTIONFILE)
			{
				optionFileOptionMap[option] = value;
			}
			else
			{
				throw new OptionException("Unknown option container type");
			}
		}

		/// <summary>
		/// Returns the option value object for the option. It uses the priority amongst the internal 
		/// option containers.
		/// </summary>
		/// <param name="option"> the option object </param>
		/// <returns> the option value object </returns>
		public virtual object getOptionValue(Option.Option option)
		{
			object value = null;
			for (int i = SAVEDOPTION; i <= OPTIONFILE; i++)
			{
				if (i == SAVEDOPTION)
				{
					value = savedOptionMap[option];
				}
				else if (i == DEPENDENCIES_RESOLVED)
				{
					value = dependenciesResolvedOptionMap[option];
				}
				else if (i == COMMANDLINE)
				{
					value = commandLineOptionMap[option];
				}
				else if (i == OPTIONFILE)
				{
					value = optionFileOptionMap[option];
				}
				if (value != null)
				{
					return value;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a string representation of the option value for the specified option. It uses the priority 
		/// amongst the internal option containers.
		/// </summary>
		/// <param name="option"> the option object </param>
		/// <returns> a string representation of the option value </returns>
		public virtual string getOptionValueString(Option.Option option)
		{
			string value = null;
			for (int i = SAVEDOPTION; i <= OPTIONFILE; i++)
			{
				if (i == SAVEDOPTION)
				{
					value = option.getStringRepresentation(savedOptionMap[option]);
				}
				else if (i == DEPENDENCIES_RESOLVED)
				{
					value = option.getStringRepresentation(dependenciesResolvedOptionMap[option]);
				}
				else if (i == COMMANDLINE)
				{
					value = option.getStringRepresentation(commandLineOptionMap[option]);
				}
				else if (i == OPTIONFILE)
				{
					value = option.getStringRepresentation(optionFileOptionMap[option]);
				}
				if (!ReferenceEquals(value, null))
				{
					return value;
				}
			}
			return null;
		}


		/// <summary>
		/// Returns true if the option is present in the specified internal option container, otherwise false.
		/// </summary>
		/// <param name="type">	the internal option container </param>
		/// <param name="option">	the option object </param>
		/// <returns> true if the option is present in the specified internal option container, otherwise false </returns>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean contains(int type, org.maltparser.core.options.option.Option option) throws OptionException
		public virtual bool contains(int type, Option.Option option)
		{
			if (type == SAVEDOPTION)
			{
				return savedOptionMap.ContainsValue(option);
			}
			else if (type == DEPENDENCIES_RESOLVED)
			{
				return dependenciesResolvedOptionMap.ContainsValue(option);
			}
			else if (type == COMMANDLINE)
			{
				return commandLineOptionMap.ContainsValue(option);
			}
			else if (type == OPTIONFILE)
			{
				return optionFileOptionMap.ContainsValue(option);
			}
			else
			{
				throw new OptionException("Unknown option container type");
			}
		}

		/// <summary>
		/// Returns the number of option values amongst all internal option containers.
		/// </summary>
		/// <returns> the number of option values amongst all internal option containers </returns>
		public virtual int NumberOfOptionValues
		{
			get
			{
				SortedSet<Option.Option> union = new SortedSet<Option.Option>(savedOptionMap.Keys);
				union.addAll(dependenciesResolvedOptionMap.Keys);
				union.addAll(commandLineOptionMap.Keys);
				union.addAll(optionFileOptionMap.Keys);
				return union.Count;
			}
		}

		/// <summary>
		/// Returns the option container index.
		/// </summary>
		/// <returns> the option container index </returns>
		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual int CompareTo(OptionContainer that)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;
			if (this == that)
			{
				return EQUAL;
			}
			if (index < that.index)
			{
				return BEFORE;
			}
			if (index > that.index)
			{
				return AFTER;
			}
			return EQUAL;
		}


		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			SortedSet<Option.Option> union = new SortedSet<Option.Option>(savedOptionMap.Keys);
			union.addAll(dependenciesResolvedOptionMap.Keys);
			union.addAll(commandLineOptionMap.Keys);
			union.addAll(optionFileOptionMap.Keys);
			foreach (Option.Option option in union)
			{
				object value = null;
				for (int i = SAVEDOPTION; i <= OPTIONFILE; i++)
				{
					if (i == SAVEDOPTION)
					{
						value = savedOptionMap[option];
					}
					else if (i == DEPENDENCIES_RESOLVED)
					{
						value = dependenciesResolvedOptionMap[option];
					}
					else if (i == COMMANDLINE)
					{
						value = commandLineOptionMap[option];
					}
					else if (i == OPTIONFILE)
					{
						value = optionFileOptionMap[option];
					}
					if (value != null)
					{
						break;
					}
				}
				sb.Append(option.Group.Name + "\t" + option.Name + "\t" + value + "\n");
			}
			return sb.ToString();
		}

	}

}