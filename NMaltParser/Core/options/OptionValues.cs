using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.options
{

	using  org.maltparser.core.options.option;

	/// <summary>
	/// OptionValues contain a number of option containers, which contains the option values (the instance of 
	/// options).
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class OptionValues
	{
		private readonly SortedDictionary<int, OptionContainer> optionContainers;

		/// <summary>
		/// Creates OptionValues.
		/// </summary>
		public OptionValues() : base()
		{
			optionContainers = Collections.synchronizedSortedMap(new SortedDictionary<int, OptionContainer>());
		}

		/// <summary>
		/// Returns the option value for an option that is in a specific option container.
		/// </summary>
		/// <param name="containerIndex">	the index of the option container. </param>
		/// <param name="option">	the option object
		/// @return	an object that contains the value of the option, <i>null</i> if the option value could not be found. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOptionValue(int containerIndex, org.maltparser.core.options.option.Option option) throws OptionException
		public virtual object getOptionValue(int containerIndex, Option option)
		{
			OptionContainer oc = optionContainers[containerIndex];
			if (oc == null)
			{
				throw new OptionException("The option container '" + containerIndex + "' cannot be found. ");
			}
			return oc.getOptionValue(option);
		}

		/// <summary>
		/// Returns a string representation of the option value for an option that is in a specific option container.
		/// </summary>
		/// <param name="containerIndex">	the index of the option container. </param>
		/// <param name="option">	an option object </param>
		/// <returns> a string representation of the option value for an option that is in a specific option container. </returns>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getOptionValueString(int containerIndex, org.maltparser.core.options.option.Option option) throws OptionException
		public virtual string getOptionValueString(int containerIndex, Option option)
		{
			OptionContainer oc = optionContainers[containerIndex];
			if (oc == null)
			{
				throw new OptionException("The option container '" + containerIndex + "' cannot be found. ");
			}
			return oc.getOptionValueString(option);
		}

		/// <summary>
		/// Returns the option value for an option.
		/// </summary>
		/// <param name="option">	an option object
		/// @return	 the option value for an option, <i>null</i> if the option value could not be found. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOptionValue(org.maltparser.core.options.option.Option option) throws OptionException
		public virtual object getOptionValue(Option option)
		{
			if (optionContainers.Count == 0)
			{
				return null;
			}
			OptionContainer oc = optionContainers[optionContainers.firstKey()];
			return oc.getOptionValue(option);
		}

		/// <summary>
		/// Returns the number of option values for a particular option container.
		/// </summary>
		/// <param name="containerIndex">	The index of the option container.
		/// @return	 the number of option values for a particular option container. </param>
		public virtual int getNumberOfOptionValues(int containerIndex)
		{
			if (!optionContainers.ContainsKey(containerIndex))
			{
				return 0;
			}
			return optionContainers[containerIndex].NumberOfOptionValues;
		}

		/// <summary>
		/// Returns a sorted set of container names.
		/// 
		/// @return	a sorted set of container names.
		/// </summary>
		public virtual ISet<int> OptionContainerIndices
		{
			get
			{
				return optionContainers.Keys;
			}
		}


		/// <summary>
		/// Adds an option value to an option to one of the internal option container specified by the type.
		/// </summary>
		/// <param name="containerType">		the type of the option container. </param>
		/// <param name="containerIndex">	the index of the option container. </param>
		/// <param name="option">	an option to add </param>
		/// <param name="value">	an option value to add
		/// @return	true if the value is added, false if the value already is in use. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean addOptionValue(int containerType, int containerIndex, org.maltparser.core.options.option.Option option, Object value) throws OptionException
		protected internal virtual bool addOptionValue(int containerType, int containerIndex, Option option, object value)
		{
			if (option == null)
			{
				throw new OptionException("The option cannot be found. ");
			}
			if (value == null)
			{
				throw new OptionException("The option value cannot be found. ");
			}

			if (!optionContainers.ContainsKey(containerIndex))
			{
				optionContainers[containerIndex] = new OptionContainer(containerIndex);
			}
			OptionContainer oc = optionContainers[containerIndex];
			if (oc == null)
			{
				throw new OptionException("The option container index " + containerIndex + " is unknown");
			}
			if (!oc.contains(containerType, option))
			{
				oc.addOptionValue(containerType, option, value);
				return true;
			}
			return false;
		}


		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			if (optionContainers.Count == 0)
			{
				sb.Append("No option values.");
			}
			else if (optionContainers.Count == 1)
			{
				sb.Append(optionContainers[optionContainers.firstKey()]);
			}
			else
			{
				foreach (int? index in optionContainers.Keys)
				{
					sb.Append("Option container : " + index + "\n");
					sb.Append(optionContainers[index] + "\n");
				}
			}
			return sb.ToString();
		}
	}

}