using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.options
{



	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using EnumOption = org.maltparser.core.options.option.EnumOption;
	using ClassOption = org.maltparser.core.options.option.ClassOption;
	using StringEnumOption = org.maltparser.core.options.option.StringEnumOption;
	using Option = org.maltparser.core.options.option.Option;
	using UnaryOption = org.maltparser.core.options.option.UnaryOption;
	using PluginLoader = org.maltparser.core.plugin.PluginLoader;
	using Element = org.w3c.dom.Element;
	using NodeList = org.w3c.dom.NodeList;
	using SAXException = org.xml.sax.SAXException;


	/// <summary>
	///  Option Manager is the management class for all option handling. All queries and manipulations of an option or an option value
	///   should go through this class. 
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class OptionManager
	{
		public const int DEFAULTVALUE = -1;
		private readonly OptionDescriptions optionDescriptions;
		private readonly OptionValues optionValues;
		private static OptionManager uniqueInstance = new OptionManager();

		/// <summary>
		/// Creates the Option Manager
		/// </summary>
		private OptionManager()
		{
			optionValues = new OptionValues();
			optionDescriptions = new OptionDescriptions();
		}

		/// <summary>
		/// Returns a reference to the single instance.
		/// </summary>
		public static OptionManager instance()
		{
			return uniqueInstance;
		}

		/// <summary>
		/// Loads the option description file <code>/appdata/options.xml</code>
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadOptionDescriptionFile() throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadOptionDescriptionFile()
		{
			optionDescriptions.parseOptionDescriptionXMLfile(this.GetType().getResource("/appdata/options.xml"));
		}


		/// <summary>
		/// Loads the option description file
		/// </summary>
		/// <param name="url">	URL of the option description file </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadOptionDescriptionFile(java.net.URL url) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadOptionDescriptionFile(URL url)
		{
			optionDescriptions.parseOptionDescriptionXMLfile(url);
		}

		/// <summary>
		/// Returns the option description 
		/// </summary>
		/// <returns> the option description  </returns>
		public virtual OptionDescriptions OptionDescriptions
		{
			get
			{
				return optionDescriptions;
			}
		}

		public virtual bool hasOptions()
		{
			return optionDescriptions.hasOptions();
		}

		/// <summary>
		/// Returns the option value for an option that is specified by the option group name and option name. The
		/// container name points out the specific option container. 
		/// 
		/// </summary>
		/// <param name="containerIndex">	The index of the option container (0..n and -1 is default values). </param>
		/// <param name="optiongroup">	The name of the option group. </param>
		/// <param name="optionname">	The name of the option. </param>
		/// <returns> an object that contains the value of the option, <i>null</i> if the option value could not be found. </returns>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOptionValue(int containerIndex, String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public virtual object getOptionValue(int containerIndex, string optiongroup, string optionname)
		{
			Option option = optionDescriptions.getOption(optiongroup, optionname);

			if (containerIndex == OptionManager.DEFAULTVALUE)
			{
				return option.DefaultValueObject;
			}
			object value = optionValues.getOptionValue(containerIndex, option);
			if (value == null)
			{
				value = option.DefaultValueObject;
			}
			return value;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOptionDefaultValue(String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public virtual object getOptionDefaultValue(string optiongroup, string optionname)
		{
			Option option = optionDescriptions.getOption(optiongroup, optionname);
			return option.DefaultValueObject;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getOptionValueNoDefault(int containerIndex, String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public virtual object getOptionValueNoDefault(int containerIndex, string optiongroup, string optionname)
		{
			Option option = optionDescriptions.getOption(optiongroup, optionname);

			if (containerIndex == OptionManager.DEFAULTVALUE)
			{
				return option.DefaultValueObject;
			}
			return optionValues.getOptionValue(containerIndex, option);
		}

		/// <summary>
		/// Returns a string representation of the option value for an option that is specified by the option group name and the option name. The
		/// container name points out the specific option container. 
		/// </summary>
		/// <param name="containerIndex">	The index of the option container (0..n and -1 is default values). </param>
		/// <param name="optiongroup">	The name of the option group. </param>
		/// <param name="optionname">	The name of the option. </param>
		/// <returns> a string representation of the option value </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getOptionValueString(int containerIndex, String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getOptionValueString(int containerIndex, string optiongroup, string optionname)
		{
			Option option = optionDescriptions.getOption(optiongroup, optionname);
			string value = optionValues.getOptionValueString(containerIndex, option);
			if (string.ReferenceEquals(value, null))
			{
				value = option.DefaultValueString;
			}
			return value;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getOptionValueStringNoDefault(int containerIndex, String optiongroup, String optionname) throws org.maltparser.core.exception.MaltChainedException
		public virtual string getOptionValueStringNoDefault(int containerIndex, string optiongroup, string optionname)
		{
			return optionValues.getOptionValueString(containerIndex, optionDescriptions.getOption(optiongroup, optionname));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addLegalValue(String optiongroup, String optionname, String value, String desc, String target) throws org.maltparser.core.exception.MaltChainedException
		public virtual void addLegalValue(string optiongroup, string optionname, string value, string desc, string target)
		{
			Option option = optionDescriptions.getOption(optiongroup, optionname);
			if (option != null)
			{
				if (option is EnumOption)
				{
					((EnumOption)option).addLegalValue(value, desc);
				}
				else if (option is ClassOption)
				{
					((ClassOption)option).addLegalValue(value, desc, target);
				}
				else if (option is StringEnumOption)
				{
					((StringEnumOption)option).addLegalValue(value, desc, target);
				}
			}
		}

		/// <summary>
		/// Overloads the option value specified by the container index, the option group name, the option name.
		/// This method is used to override option that have specific dependencies. 
		/// </summary>
		/// <param name="containerIndex">	the index of the option container (0..n and -1 is default values). </param>
		/// <param name="optiongroup">	the name of the option group. </param>
		/// <param name="optionname">	the name of the option. </param>
		/// <param name="value"> the option value that should replace the current option value. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void overloadOptionValue(int containerIndex, String optiongroup, String optionname, String value) throws org.maltparser.core.exception.MaltChainedException
		public virtual void overloadOptionValue(int containerIndex, string optiongroup, string optionname, string value)
		{
	//		Option option = optionDescriptions.getOption(optiongroup, optionname);
	//		if (value == null) {
	//    		throw new OptionException("The option value is missing. ");
	//    	}
	//    	Object ovalue = option.getValueObject(value);
	//    	optionValues.addOptionValue(OptionContainer.DEPENDENCIES_RESOLVED, containerIndex, option, ovalue);
			overloadOptionValue(containerIndex, OptionContainer.DEPENDENCIES_RESOLVED, optiongroup, optionname, value);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void overloadOptionValue(int containerIndex, int containerType, String optiongroup, String optionname, String value) throws org.maltparser.core.exception.MaltChainedException
		public virtual void overloadOptionValue(int containerIndex, int containerType, string optiongroup, string optionname, string value)
		{
			Option option = optionDescriptions.getOption(optiongroup, optionname);
			if (string.ReferenceEquals(value, null))
			{
				throw new OptionException("The option value is missing. ");
			}
			object ovalue = option.getValueObject(value);
			optionValues.addOptionValue(containerType, containerIndex, option, ovalue);
		}

		/// <summary>
		/// Returns the number of option values for a particular option container.
		/// </summary>
		/// <param name="containerIndex">	The index of the option container (0..n). </param>
		/// <returns> the number of option values for a particular option container. </returns>
		public virtual int getNumberOfOptionValues(int containerIndex)
		{
			return optionValues.getNumberOfOptionValues(containerIndex);
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
				return optionValues.OptionContainerIndices;
			}
		}
		/// <summary>
		/// Loads the saved options (options that are marked with <code>usage=save</code>). 
		/// </summary>
		/// <param name="fileName">	The path to the file where to load the saved options. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadOptions(int containerIndex, String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadOptions(int containerIndex, string fileName)
		{
			try
			{
				loadOptions(containerIndex, new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read), Encoding.UTF8));
			}
			catch (FileNotFoundException e)
			{
				throw new OptionException("The saved option file '" + fileName + "' cannot be found. ", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new OptionException("The charset is unsupported. ", e);
			}
		}


		/// <summary>
		/// Loads the saved options (options that are marked with <code>usage=Option.SAVE</code>). 
		/// </summary>
		/// <param name="isr">	the input stream reader of the saved options file. </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadOptions(int containerIndex, java.io.InputStreamReader isr) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadOptions(int containerIndex, StreamReader isr)
		{
			try
			{
				StreamReader br = new StreamReader(isr);
				string line = null;
				Option option = null;
				Pattern tabPattern = Pattern.compile("\t");
				while (!string.ReferenceEquals((line = br.ReadLine()), null))
				{
					string[] items = tabPattern.split(line);
					if (items.Length < 3 || items.Length > 4)
					{
						throw new OptionException("Could not load the saved option. ");
					}
					option = optionDescriptions.getOption(items[1], items[2]);
					object ovalue;
					if (items.Length == 3)
					{
						ovalue = "";
					}
					else
					{
						if (option is ClassOption)
						{
							if (items[3].StartsWith("class ", StringComparison.Ordinal))
							{
								Type clazz = null;
								if (PluginLoader.instance() != null)
								{
									clazz = PluginLoader.instance().getClass(items[3].Substring(6));
								}
								if (clazz == null)
								{
									clazz = Type.GetType(items[3].Substring(6));
								}
								ovalue = option.getValueObject(((ClassOption)option).getLegalValueString(clazz));
							}
							else
							{
								ovalue = option.getValueObject(items[3]);
							}
						}
						else
						{
							ovalue = option.getValueObject(items[3]);
						}
					}
					optionValues.addOptionValue(OptionContainer.SAVEDOPTION, containerIndex, option, ovalue);
				}

				br.Close();
			}
			catch (ClassNotFoundException e)
			{
				throw new OptionException("The class cannot be found. ", e);
			}
			catch (System.FormatException e)
			{
				throw new OptionException("Option container index isn't an integer value. ", e);
			}
			catch (IOException e)
			{
				throw new OptionException("Error when reading the saved options. ", e);
			}
		}

		/// <summary>
		/// Saves all options that are marked as <code>usage=Option.SAVE</code>
		/// </summary>
		/// <param name="fileName">	The path to the file where the saveOption should by saved. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void saveOptions(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void saveOptions(string fileName)
		{
			try
			{
				saveOptions(new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write), Encoding.UTF8));
			}
			catch (FileNotFoundException e)
			{
				throw new OptionException("The file '" + fileName + "' cannot be created. ", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new OptionException("The charset 'UTF-8' is unsupported. ", e);
			}

		}

		/// <summary>
		/// Saves all options that are marked as <code>usage=Option.SAVE</code>
		/// </summary>
		/// <param name="osw">	the output stream writer of the saved option file </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void saveOptions(java.io.OutputStreamWriter osw) throws org.maltparser.core.exception.MaltChainedException
		public virtual void saveOptions(StreamWriter osw)
		{
			try
			{
				StreamWriter bw = new StreamWriter(osw);
				ISet<Option> optionToSave = optionDescriptions.SaveOptionSet;

				object value = null;
				foreach (int? index in optionValues.OptionContainerIndices)
				{
					foreach (Option option in optionToSave)
					{
						value = optionValues.getOptionValue(index.Value, option);
						if (value == null)
						{
							value = option.DefaultValueObject;
						}
						bw.append(index + "\t" + option.Group.Name + "\t" + option.Name + "\t" + value + "\n");
					}
				}
				bw.Flush();
				bw.Close();
			}
			catch (IOException e)
			{
				throw new OptionException("Error when saving the saved options. ", e);
			}
		}

		/// <summary>
		/// Saves all options that are marked as usage=Option.SAVE for a particular option container.
		/// </summary>
		/// <param name="containerIndex">	The index of the option container (0..n). </param>
		/// <param name="fileName">	The path to the file where the saveOption should by saved. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void saveOptions(int containerIndex, String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void saveOptions(int containerIndex, string fileName)
		{
			try
			{
				saveOptions(containerIndex, new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write), Encoding.UTF8));
			}
			catch (FileNotFoundException e)
			{
				throw new OptionException("The file '" + fileName + "' cannot be found.", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new OptionException("The charset 'UTF-8' is unsupported. ", e);
			}
		}

		/// <summary>
		/// Saves all options that are marked as usage=Option.SAVE for a particular option container.
		/// </summary>
		/// <param name="containerIndex"> The index of the option container (0..n). </param>
		/// <param name="osw"> 	the output stream writer of the saved option file </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void saveOptions(int containerIndex, java.io.OutputStreamWriter osw) throws org.maltparser.core.exception.MaltChainedException
		public virtual void saveOptions(int containerIndex, StreamWriter osw)
		{
			try
			{
				StreamWriter bw = new StreamWriter(osw);
				ISet<Option> optionToSave = optionDescriptions.SaveOptionSet;

				object value = null;
				foreach (Option option in optionToSave)
				{
					value = optionValues.getOptionValue(containerIndex, option);
					if (value == null)
					{
						value = option.DefaultValueObject;
					}
					bw.append(containerIndex + "\t" + option.Group.Name + "\t" + option.Name + "\t" + value + "\n");
				}

				bw.Flush();
				bw.Close();
			}
			catch (IOException e)
			{
				throw new OptionException("Error when saving the saved options.", e);
			}
		}

		/// <summary>
		/// Creates several option maps for fast access to individual options.  
		/// </summary>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void generateMaps() throws org.maltparser.core.exception.MaltChainedException
		public virtual void generateMaps()
		{
			optionDescriptions.generateMaps();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean parseCommandLine(String argString, int containerIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool parseCommandLine(string argString, int containerIndex)
		{
			return parseCommandLine(argString.Split(" ", true), containerIndex);
		}

		/// <summary>
		/// Parses the command line arguments.
		/// </summary>
		/// <param name="args"> An array of arguments that are supplied when starting the application. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean parseCommandLine(String[] args, int containerIndex) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool parseCommandLine(string[] args, int containerIndex)
		{
			if (args == null || args.Length == 0)
			{
				return false;
			}
			int i = 0;
			Dictionary<string, string> oldFlags = new Dictionary<string, string>();
			oldFlags["llo"] = "lo";
			oldFlags["lso"] = "lo";
			oldFlags["lli"] = "li";
			oldFlags["lsi"] = "li";
			oldFlags["llx"] = "lx";
			oldFlags["lsx"] = "lx";
			oldFlags["llv"] = "lv";
			oldFlags["lsv"] = "lv";
			while (i < args.Length)
			{
				Option option = null;
				string value = null;
				/* Recognizes
				 * --optiongroup-optionname=value
				 * --optionname=value
				 * --optiongroup-optionname (unary option)
				 * --optionname (unary option)
				 */ 
				if (args[i].StartsWith("--", StringComparison.Ordinal))
				{
					if (args[i].Length == 2)
					{
						throw new OptionException("The argument contains only '--', please check the user guide to see the correct format. ");
					}
					string optionstring;
					string optiongroup;
					string optionname;
					int indexEqualSign = args[i].IndexOf('=');
					if (indexEqualSign != -1)
					{
						value = args[i].Substring(indexEqualSign + 1);
						optionstring = args[i].Substring(2, indexEqualSign - 2);
					}
					else
					{
						value = null;
						optionstring = args[i].Substring(2);
					}
					int indexMinusSign = optionstring.IndexOf('-');
					if (indexMinusSign != -1)
					{
						optionname = optionstring.Substring(indexMinusSign + 1);
						optiongroup = optionstring.Substring(0, indexMinusSign);
					}
					else
					{
						optiongroup = null;
						optionname = optionstring;
					}

					option = optionDescriptions.getOption(optiongroup, optionname);
					if (option is UnaryOption)
					{
						value = "used";
					}
					i++;
				}
				/* Recognizes
				 * -optionflag value
				 * -optionflag (unary option)
				 */
				else if (args[i].StartsWith("-", StringComparison.Ordinal))
				{
					if (args[i].Length < 2)
					{
						throw new OptionException("Wrong use of option flag '" + args[i] + "', please check the user guide to see the correct format. ");
					}
					string flag = "";
					if (oldFlags.ContainsKey(args[i].Substring(1)))
					{
						flag = oldFlags[args[i].Substring(1)];
					}
					else
					{
						flag = args[i].Substring(1);
					}

					// Error message if the old flag '-r' (root handling) is used 
					if (args[i].Substring(1).Equals("r"))
					{
						throw new OptionException("The flag -r (root_handling) is replaced with two flags -nr (allow_root) and -ne (allow_reduce) since MaltParser 1.7. Read more about these changes in the user guide.");
					}

					option = optionDescriptions.getOption(flag);

					if (option is UnaryOption)
					{
						value = "used";
					}
					else
					{
						i++;
						if (args.Length > i)
						{
							value = args[i];
						}
						else
						{
							throw new OptionException("Could not find the corresponding value for -" + option.Flag + ". ");
						}
					}
					i++;
				}
				else
				{
					throw new OptionException("The option should starts with a minus sign (-), error at argument '" + args[i] + "'");
				}
				object optionvalue = option.getValueObject(value);
				optionValues.addOptionValue(OptionContainer.COMMANDLINE, containerIndex, option, optionvalue);
			}
			return true;
		}


		/// <summary>
		/// Parses the option file for option values. 
		/// </summary>
		/// <param name="fileName"> The option file name (must be a xml file). </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseOptionInstanceXMLfile(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parseOptionInstanceXMLfile(string fileName)
		{
			File file = new File(fileName);

			try
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();

				Element root = db.parse(file).DocumentElement;
				NodeList containers = root.getElementsByTagName("optioncontainer");
				Element container;
				for (int i = 0; i < containers.Length; i++)
				{
					container = (Element)containers.item(i);
					parseOptionValues(container, i);
				}
			}
			catch (IOException e)
			{
				throw new OptionException("Can't find the file " + fileName + ". ", e);
			}
			catch (OptionException e)
			{
				throw new OptionException("Problem parsing the file " + fileName + ". ", e);
			}
			catch (ParserConfigurationException e)
			{
				throw new OptionException("Problem parsing the file " + fileName + ". ", e);
			}
			catch (SAXException e)
			{
				throw new OptionException("Problem parsing the file " + fileName + ". ", e);
			}
		}

		/// <summary>
		/// Parses an option container for option values.
		/// </summary>
		/// <param name="container">	a reference to an individual option container in the DOM tree. </param>
		/// <param name="containerName">	the name of this container. </param>
		/// <exception cref="OptionException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseOptionValues(org.w3c.dom.Element container, int containerIndex) throws org.maltparser.core.exception.MaltChainedException
		private void parseOptionValues(Element container, int containerIndex)
		{
			NodeList optiongroups = container.getElementsByTagName("optiongroup");
			Element optiongroup;
			for (int i = 0; i < optiongroups.Length; i++)
			{
				optiongroup = (Element)optiongroups.item(i);
				string groupname = optiongroup.getAttribute("groupname").ToLower();
				if (string.ReferenceEquals(groupname, null))
				{
					throw new OptionException("The option group name is missing. ");
				}
				NodeList optionvalues = optiongroup.getElementsByTagName("option");
				Element optionvalue;

				for (int j = 0; j < optionvalues.Length; j++)
				{
					optionvalue = (Element)optionvalues.item(j);
					string optionname = optionvalue.getAttribute("name").ToLower();
					string value = optionvalue.getAttribute("value");

					if (string.ReferenceEquals(optionname, null))
					{
						throw new OptionException("The option name is missing. ");
					}

					Option option = optionDescriptions.getOption(groupname, optionname);

					if (option is UnaryOption)
					{
						value = "used";
					}
					if (string.ReferenceEquals(value, null))
					{
						throw new OptionException("The option value is missing. ");
					}
					object ovalue = option.getValueObject(value);
					optionValues.addOptionValue(OptionContainer.OPTIONFILE, containerIndex, option, ovalue);
				}
			}
		}

		/// <summary>
		/// Returns a string representation of all option value, except the options in a option group specified
		/// by the excludeGroup argument.
		/// </summary>
		/// <param name="containerIndex"> The index of the option container (0..n and -1 is default values). </param>
		/// <param name="excludeGroups"> a set of option group names that should by excluded in the string representation </param>
		/// <returns> a string representation of all option value </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String toStringPrettyValues(int containerIndex, java.util.Set<String> excludeGroups) throws org.maltparser.core.exception.MaltChainedException
		public virtual string toStringPrettyValues(int containerIndex, ISet<string> excludeGroups)
		{
			int reservedSpaceForOptionName = 30;
			OptionGroup.toStringSetting = OptionGroup.WITHGROUPNAME;
			StringBuilder sb = new StringBuilder();
			if (containerIndex == OptionManager.DEFAULTVALUE)
			{
				foreach (string groupname in optionDescriptions.OptionGroupNameSet)
				{
					if (excludeGroups.Contains(groupname))
					{
						continue;
					}
					sb.Append(groupname + "\n");
					foreach (Option option in optionDescriptions.getOptionGroupList(groupname))
					{
						int nSpaces = reservedSpaceForOptionName - option.Name.Length;
						if (nSpaces <= 1)
						{
							nSpaces = 1;
						}
						sb.Append((new Formatter()).format("  %s (%4s)%" + nSpaces + "s %s\n", option.Name, "-" + option.Flag," ", option.DefaultValueString));
					}
				}
			}
			else
			{
				foreach (string groupname in optionDescriptions.OptionGroupNameSet)
				{
					if (excludeGroups.Contains(groupname))
					{
						continue;
					}
					sb.Append(groupname + "\n");
					foreach (Option option in optionDescriptions.getOptionGroupList(groupname))
					{
						string value = optionValues.getOptionValueString(containerIndex, option);
						int nSpaces = reservedSpaceForOptionName - option.Name.Length;
						if (nSpaces <= 1)
						{
							nSpaces = 1;
						}

						if (string.ReferenceEquals(value, null))
						{
							sb.Append((new Formatter()).format("  %s (%4s)%" + nSpaces + "s %s\n", option.Name, "-" + option.Flag, " ", option.DefaultValueString));
						}
						else
						{
							sb.Append((new Formatter()).format("  %s (%4s)%" + nSpaces + "s %s\n", option.Name, "-" + option.Flag, " ", value));
						}
					}
				}
			}
			return sb.ToString();
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(optionDescriptions + "\n");
			sb.Append(optionValues + "\n");
			return sb.ToString();
		}
	}

}