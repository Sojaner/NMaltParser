using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.options
{


	using  exception;
	using  helper;
    using  option;

    /// <summary>
	/// Organizes all the option descriptions. Option descriptions can be loaded from the application data <code>/appdata/options.xml</code>, but also 
	/// from a plugin option description file (always with the name <code>plugin.xml</code>).
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class OptionDescriptions
	{
		private readonly Dictionary<string, OptionGroup> optionGroups;
		private readonly SortedSet<string> ambiguous;
		private readonly Dictionary<string, Option> unambiguousOptionMap;
		private readonly Dictionary<string, Option> ambiguousOptionMap;
		private readonly Dictionary<string, Option> flagOptionMap;

		/// <summary>
		/// Creates the Option Descriptions
		/// </summary>
		public OptionDescriptions()
		{
			optionGroups = new Dictionary<string, OptionGroup>();
			ambiguous = new SortedSet<string>();
			unambiguousOptionMap = new Dictionary<string, Option>();
			ambiguousOptionMap = new Dictionary<string, Option>();
			flagOptionMap = new Dictionary<string, Option>();
		}


		/// <summary>
		/// Returns an option based on the option name and/or the option group name 
		/// </summary>
		/// <param name="optiongroup">	the name of the option group </param>
		/// <param name="optionname">	the option name </param>
		/// <returns> an option based on the option name and/or the option group name </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.options.option.Option getOption(String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public virtual Option getOption(string optiongroup, string optionname)
		{
			if (ReferenceEquals(optionname, null) || optionname.Length <= 0)
			{
				throw new OptionException("The option name '" + optionname + "' cannot be found");
			}
			Option option;
			if (ambiguous.Contains(optionname.ToLower()))
			{
				if (ReferenceEquals(optiongroup, null) || optiongroup.Length <= 0)
				{
					throw new OptionException("The option name '" + optionname + "' is ambiguous use option group name to distinguish the option. ");
				}
				else
				{
					option = ambiguousOptionMap[optiongroup.ToLower() + "-" + optionname.ToLower()];
					if (option == null)
					{
						throw new OptionException("The option '--" + optiongroup.ToLower() + "-" + optionname.ToLower() + " does not exist. ");
					}
				}
			}
			else
			{
				option = unambiguousOptionMap[optionname.ToLower()];
				if (option == null)
				{
					throw new OptionException("The option '--" + optionname.ToLower() + " doesn't exist. ");
				}
			}
			return option;
		}

		/// <summary>
		/// Returns an option based on the option flag
		/// </summary>
		/// <param name="optionflag"> the option flag </param>
		/// <returns> an option based on the option flag </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.options.option.Option getOption(String optionflag) throws org.maltparser.core.exception.MaltChainedException
		public virtual Option getOption(string optionflag)
		{
			Option option = flagOptionMap[optionflag];
			if (option == null)
			{
				throw new OptionException("The option flag -" + optionflag + " could not be found. ");
			}
			return option;
		}

		/// <summary>
		/// Returns a set of option that are marked as SAVEOPTION
		/// </summary>
		/// <returns> a set of option that are marked as SAVEOPTION </returns>
		public virtual ISet<Option> SaveOptionSet
		{
			get
			{
				ISet<Option> optionToSave = new HashSet<Option>();
    
				foreach (string optionname in unambiguousOptionMap.Keys)
				{
					if (unambiguousOptionMap[optionname].getUsage() == Option.SAVE)
					{
						optionToSave.Add(unambiguousOptionMap[optionname]);
					}
				}
				foreach (string optionname in ambiguousOptionMap.Keys)
				{
					if (ambiguousOptionMap[optionname].getUsage() == Option.SAVE)
					{
						optionToSave.Add(ambiguousOptionMap[optionname]);
					}
				}
				return optionToSave;
			}
		}

		/// <summary>
		/// Return a sorted set of option group names
		/// </summary>
		/// <returns> a sorted set of option group names </returns>
		public virtual SortedSet<string> OptionGroupNameSet
		{
			get
			{
				return new SortedSet<string>(optionGroups.Keys);
			}
		}

		/// <summary>
		/// Returns a collection of option that are member of an option group 
		/// </summary>
		/// <param name="groupname"> the name of the option group </param>
		/// <returns> a collection of option that are member of an option group  </returns>
		protected internal virtual ICollection<Option> getOptionGroupList(string groupname)
		{
			return optionGroups[groupname].OptionList;
		}

		/// <summary>
		/// Parse a XML file that contains the options used for controlling the application. The method
		/// calls the parseOptionGroups to parse the set of option groups in the DOM tree. 
		/// </summary>
		/// <param name="url">	The path to a XML file that explains the options used in the application. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseOptionDescriptionXMLfile(java.net.URL url) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parseOptionDescriptionXMLfile(URL url)
		{
			if (url == null)
			{
				throw new OptionException("The URL to the default option file is null. ");
			}

			try
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();

				Element root = db.parse(url.openStream()).DocumentElement;
				NodeList groups = root.getElementsByTagName("optiongroup");
				Element group;
				for (int i = 0; i < groups.Length; i++)
				{
					group = (Element)groups.item(i);
					string groupname = group.getAttribute("groupname").ToLower();
					OptionGroup og = null;
					if (optionGroups.ContainsKey(groupname))
					{
						og = optionGroups[groupname];
					}
					else
					{
						optionGroups[groupname] = new OptionGroup(groupname);
						og = optionGroups[groupname];
					}
					parseOptionsDescription(group, og);
				}
			}
			catch (java.io.IOException e)
			{
				throw new OptionException("Can't find the file " + url.ToString() + ".", e);
			}
			catch (OptionException e)
			{
				throw new OptionException("Problem parsing the file " + url.ToString() + ". ", e);
			}
			catch (ParserConfigurationException e)
			{
				throw new OptionException("Problem parsing the file " + url.ToString() + ". ", e);
			}
			catch (SAXException e)
			{
				throw new OptionException("Problem parsing the file " + url.ToString() + ". ", e);
			}
		}


		/// <summary>
		/// Parse a set of options within an option group to collect all information of individual options. 
		/// </summary>
		/// <param name="group"> a reference to an individual option group in the DOM tree. </param>
		/// <param name="og"> a reference to the corresponding option group in the HashMap. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseOptionsDescription(org.w3c.dom.Element group, OptionGroup og) throws org.maltparser.core.exception.MaltChainedException
		private void parseOptionsDescription(Element group, OptionGroup og)
		{
			NodeList options = group.getElementsByTagName("option");
			Element option;
			for (int i = 0; i < options.Length; i++)
			{
				option = (Element)options.item(i);
				string optionname = option.getAttribute("name").ToLower();
				string optiontype = option.getAttribute("type").ToLower();
				string defaultValue = option.getAttribute("default");
				string usage = option.getAttribute("usage").ToLower();
				string flag = option.getAttribute("flag");

				NodeList shortdescs = option.getElementsByTagName("shortdesc");
				Element shortdesc;
				string shortdesctext = "";
				if (shortdescs.Length == 1)
				{
					shortdesc = (Element)shortdescs.item(0);
					shortdesctext = shortdesc.TextContent;
				}

				if (optiontype.Equals("string") || optiontype.Equals("bool") || optiontype.Equals("integer") || optiontype.Equals("unary"))
				{
					Option op = og.getOption(optionname);
					if (op != null)
					{
						throw new OptionException("The option name '" + optionname + "' for option group '" + og.Name + "' already exists. It is only allowed to override the class and enum option type to add legal value. ");
					}
				}
				else if (optiontype.Equals("class") || optiontype.Equals("enum") || optiontype.Equals("stringenum"))
				{
					Option op = og.getOption(optionname);
					if (op != null)
					{
						if (op is EnumOption && !optiontype.Equals("enum"))
						{
							throw new OptionException("The option name '" + optionname + "' for option group '" + og.Name + "' already exists. The existing option is of enum type, but the new option is of '" + optiontype + "' type. ");
						}
						if (op is ClassOption && !optiontype.Equals("class"))
						{
							throw new OptionException("The option name '" + optionname + "' for option group '" + og.Name + "' already exists. The existing option is of class type, but the new option is of '" + optiontype + "' type. ");
						}
						if (op is StringEnumOption && !optiontype.Equals("stringenum"))
						{
							throw new OptionException("The option name '" + optionname + "' for option group '" + og.Name + "' already exists. The existing option is of urlenum type, but the new option is of '" + optiontype + "' type. ");
						}
					}
				}
				if (optiontype.Equals("string"))
				{
					og.addOption(new StringOption(og, optionname, shortdesctext, flag, usage, defaultValue));
				}
				else if (optiontype.Equals("bool"))
				{
					og.addOption(new BoolOption(og, optionname, shortdesctext, flag, usage, defaultValue));
				}
				else if (optiontype.Equals("integer"))
				{
					og.addOption(new IntegerOption(og, optionname, shortdesctext, flag, usage, defaultValue));
				}
				else if (optiontype.Equals("unary"))
				{
					og.addOption(new UnaryOption(og, optionname, shortdesctext, flag, usage));
				}
				else if (optiontype.Equals("enum"))
				{
					Option op = og.getOption(optionname);
					EnumOption eop = null;
					if (op == null)
					{
						eop = new EnumOption(og, optionname, shortdesctext, flag, usage);
					}
					else
					{
						if (op is EnumOption)
						{
							eop = (EnumOption)op;
						}
					}

					NodeList legalvalues = option.getElementsByTagName("legalvalue");
					Element legalvalue;
					for (int j = 0; j < legalvalues.Length; j++)
					{
						legalvalue = (Element)legalvalues.item(j);
						string legalvaluename = legalvalue.getAttribute("name");
						string legalvaluetext = legalvalue.TextContent;
						eop.addLegalValue(legalvaluename, legalvaluetext);
					}
					if (op == null)
					{
						eop.DefaultValue = defaultValue;
						og.addOption(eop);
					}

				}
				else if (optiontype.Equals("class"))
				{
					Option op = og.getOption(optionname);
					ClassOption cop = null;
					if (op == null)
					{
						cop = new ClassOption(og, optionname, shortdesctext, flag, usage);
					}
					else
					{
						if (op is ClassOption)
						{
							cop = (ClassOption)op;
						}
					}

					NodeList legalvalues = option.getElementsByTagName("legalvalue");
					Element legalvalue;
					for (int j = 0; j < legalvalues.Length; j++)
					{
						legalvalue = (Element)legalvalues.item(j);
						string legalvaluename = legalvalue.getAttribute("name").ToLower();
						string classname = legalvalue.getAttribute("class");
						string legalvaluetext = legalvalue.TextContent;
						cop.addLegalValue(legalvaluename, legalvaluetext, classname);
					}
					if (op == null)
					{
						cop.DefaultValue = defaultValue;
						og.addOption(cop);
					}
				}
				else if (optiontype.Equals("stringenum"))
				{
					Option op = og.getOption(optionname);
					StringEnumOption ueop = null;
					if (op == null)
					{
						ueop = new StringEnumOption(og, optionname, shortdesctext, flag, usage);
					}
					else
					{
						if (op is StringEnumOption)
						{
							ueop = (StringEnumOption)op;
						}
					}

					NodeList legalvalues = option.getElementsByTagName("legalvalue");
					Element legalvalue;
					for (int j = 0; j < legalvalues.Length; j++)
					{
						legalvalue = (Element)legalvalues.item(j);
						string legalvaluename = legalvalue.getAttribute("name").ToLower();
						string url = legalvalue.getAttribute("mapto");
						string legalvaluetext = legalvalue.TextContent;
						ueop.addLegalValue(legalvaluename, legalvaluetext, url);
					}
					if (op == null)
					{
						ueop.DefaultValue = defaultValue;
						og.addOption(ueop);
					}
				}
				else
				{
					throw new OptionException("Illegal option type found in the setting file. ");
				}
			}
		}

		public virtual bool hasOptions()
		{
			return optionGroups.Count > 0;
		}

		/// <summary>
		/// Creates several option maps for fast access to individual options.  
		/// </summary>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void generateMaps() throws org.maltparser.core.exception.MaltChainedException
		public virtual void generateMaps()
		{
			foreach (string groupname in optionGroups.Keys)
			{
				OptionGroup og = optionGroups[groupname];
				ICollection<Option> options = og.OptionList;

				foreach (Option option in options)
				{
					if (ambiguous.Contains(option.Name))
					{
						option.Ambiguous = true;
						ambiguousOptionMap[option.Group.Name + "-" + option.Name] = option;
					}
					else
					{
						if (!unambiguousOptionMap.ContainsKey(option.Name))
						{
							unambiguousOptionMap[option.Name] = option;
						}
						else
						{
							Option ambig = unambiguousOptionMap[option.Name];
							unambiguousOptionMap.Remove(ambig);
							ambig.Ambiguous = true;
							option.Ambiguous = true;
							ambiguous.Add(option.Name);
							ambiguousOptionMap[ambig.Group.Name + "-" + ambig.Name] = ambig;
							ambiguousOptionMap[option.Group.Name + "-" + option.Name] = option;
						}
					}
					if (!ReferenceEquals(option.Flag, null))
					{
						Option co = flagOptionMap[option.Flag];
						if (co != null)
						{
							flagOptionMap.Remove(co);
							co.Flag = null;
							option.Flag = null;
							if (SystemLogger.logger().DebugEnabled)
							{
								SystemLogger.logger().debug("Ambiguous use of an option flag -> the option flag is removed for all ambiguous options\n");
							}
						}
						else
						{
							flagOptionMap[option.Flag] = option;
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns a string representation that contains printable information of several options maps
		/// </summary>
		/// <returns> a string representation that contains printable information of several options maps </returns>
		public virtual string toStringMaps()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("UnambiguousOptionMap\n");
			foreach (string optionname in new SortedSet<string>(unambiguousOptionMap.Keys))
			{
				sb.Append("   " + optionname + "\n");
			}
			sb.Append("AmbiguousSet\n");
			foreach (string optionname in ambiguous)
			{
				sb.Append("   " + optionname + "\n");
			}
			sb.Append("AmbiguousOptionMap\n");
			foreach (string optionname in new SortedSet<string>(ambiguousOptionMap.Keys))
			{
				sb.Append("   " + optionname + "\n");
			}
			sb.Append("CharacterOptionMap\n");
			foreach (string flag in new SortedSet<string>(flagOptionMap.Keys))
			{
				sb.Append("   -" + flag + " -> " + flagOptionMap[flag].Name + "\n");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Returns a string representation of a option group without the option group name in the string. 
		/// </summary>
		/// <param name="groupname">	The option group name </param>
		/// <returns> a string representation of a option group </returns>
		public virtual string toStringOptionGroup(string groupname)
		{
			OptionGroup.toStringSetting = OptionGroup.NOGROUPNAME;
			return optionGroups[groupname].ToString() + "\n";
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			OptionGroup.toStringSetting = OptionGroup.WITHGROUPNAME;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (string groupname in new SortedSet<string>(optionGroups.Keys))
			{
				sb.Append(optionGroups[groupname].ToString() + "\n");
			}
			return sb.ToString();
		}
	}

}