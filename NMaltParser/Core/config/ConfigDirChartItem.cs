using System;
using System.Text;

namespace org.maltparser.core.config
{

	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using FlowChartInstance = org.maltparser.core.flow.FlowChartInstance;
	using ChartItem = org.maltparser.core.flow.item.ChartItem;
	using ChartItemSpecification = org.maltparser.core.flow.spec.ChartItemSpecification;
	using SystemInfo = org.maltparser.core.helper.SystemInfo;
	using SystemLogger = org.maltparser.core.helper.SystemLogger;
	using OptionManager = org.maltparser.core.options.OptionManager;
	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class ConfigDirChartItem : ChartItem
	{
		private string idName;
		private string taskName;
		private string optionFileName;
		private URL configDirURL;
		private string configDirName;
		private ConfigurationDir configDir;
		private string outCharSet;
		private string inCharSet;

		public ConfigDirChartItem()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initialize(org.maltparser.core.flow.FlowChartInstance flowChartinstance, org.maltparser.core.flow.spec.ChartItemSpecification chartItemSpecification) throws org.maltparser.core.exception.MaltChainedException
		public override void initialize(FlowChartInstance flowChartinstance, ChartItemSpecification chartItemSpecification)
		{
			base.initialize(flowChartinstance, chartItemSpecification);

			foreach (string key in chartItemSpecification.ChartItemAttributes.Keys)
			{
				if (key.Equals("id"))
				{
					idName = chartItemSpecification.ChartItemAttributes[key];
				}
				else if (key.Equals("task"))
				{
					taskName = chartItemSpecification.ChartItemAttributes[key];
				}
			}
			if (string.ReferenceEquals(idName, null))
			{
				idName = getChartElement("configdir").Attributes.get("id").DefaultValue;
			}
			else if (string.ReferenceEquals(taskName, null))
			{
				taskName = getChartElement("configdir").Attributes.get("task").DefaultValue;
			}

			if (OptionManager.instance().getOptionValue(OptionContainerIndex, "config", "url") != null && OptionManager.instance().getOptionValue(OptionContainerIndex,"config", "url").ToString().Length > 0)
			{
				try
				{
					configDirURL = new URL(OptionManager.instance().getOptionValue(OptionContainerIndex, "config", "url").ToString());
				}
				catch (MalformedURLException e)
				{
					throw new ConfigurationException("The URL '" + OptionManager.instance().getOptionValue(OptionContainerIndex, "config", "url").ToString() + "' is malformed. ", e);
				}
			}
			if (OptionManager.instance().getOptionValue(OptionContainerIndex, "config", "name").ToString().EndsWith(".mco", StringComparison.Ordinal))
			{
				int index = OptionManager.instance().getOptionValue(OptionContainerIndex, "config", "name").ToString().LastIndexOf('.');
				configDirName = OptionManager.instance().getOptionValue(OptionContainerIndex, "config", "name").ToString().Substring(0,index);
			}
			else
			{
				configDirName = OptionManager.instance().getOptionValue(OptionContainerIndex, "config", "name").ToString();
			}

			try
			{
				if (OptionManager.instance().getOptionValue(OptionContainerIndex, "system", "option_file") != null)
				{
					optionFileName = OptionManager.instance().getOptionValue(OptionContainerIndex, "system", "option_file").ToString();
				}
			}
			catch (ConfigurationException e)
			{
				throw new ConfigurationException("The option file '" + optionFileName + "' could not be copied. ",e);
			}
			if (OptionManager.instance().getOptionValue(OptionContainerIndex, "output", "charset") != null)
			{
				outCharSet = OptionManager.instance().getOptionValue(OptionContainerIndex, "output", "charset").ToString();
			}
			else
			{
				outCharSet = "UTF-8";
			}

			if (OptionManager.instance().getOptionValue(OptionContainerIndex, "input", "charset") != null)
			{
				inCharSet = OptionManager.instance().getOptionValue(OptionContainerIndex, "input", "charset").ToString();
			}
			else
			{
				inCharSet = "UTF-8";
			}

			configDir = (ConfigurationDir)flowChartinstance.getFlowChartRegistry(typeof(org.maltparser.core.config.ConfigurationDir), idName);
			if (configDir == null)
			{
				if (configDirURL != null)
				{
					configDir = new ConfigurationDir(configDirURL);
				}
				else
				{
					configDir = new ConfigurationDir(configDirName, idName, OptionContainerIndex);
				}

				flowChartinstance.addFlowChartRegistry(typeof(org.maltparser.core.config.ConfigurationDir), idName, configDir);
			}
			if (taskName.Equals("versioning"))
			{
				configDir.versioning();
			}
			else if (taskName.Equals("loadsavedoptions"))
			{
				configDir.initCreatedByMaltParserVersionFromInfoFile();
				if (string.ReferenceEquals(configDir.CreatedByMaltParserVersion, null))
				{
					SystemLogger.logger().warn("Couln't determine which version of MaltParser that created the parser model: " + configDirName + ".mco\n MaltParser will terminate\n");
					Environment.Exit(1);
				}
				else if (!configDir.CreatedByMaltParserVersion.Substring(0,3).Equals(SystemInfo.Version.Substring(0,3)))
				{
					if (!((configDir.CreatedByMaltParserVersion.Substring(0,3).Equals("1.7") || configDir.CreatedByMaltParserVersion.Substring(0,3).Equals("1.8") || configDir.CreatedByMaltParserVersion.Substring(0,3).Equals("1.9")) && SystemInfo.Version.Substring(0,3).Equals("1.9")))
					{
						SystemLogger.logger().error("The parser model '" + configDirName + ".mco' is created by MaltParser " + configDir.CreatedByMaltParserVersion + ".\n");
						SystemLogger.logger().error("You have to re-train the parser model to be able to parse with current version of MaltParser.\n");
						Environment.Exit(1);
					}
				}
				OptionManager.instance().loadOptions(OptionContainerIndex, configDir.getInputStreamReaderFromConfigFile("savedoptions.sop"));
				configDir.initDataFormat();
			}
			else if (taskName.Equals("createdir"))
			{
				configDir.CreatedByMaltParserVersion = SystemInfo.Version;
				configDir.createConfigDirectory();
				if (!string.ReferenceEquals(optionFileName, null) && optionFileName.Length > 0)
				{
					configDir.copyToConfig(new File(optionFileName));
				}
				configDir.initDataFormat();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int preprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int preprocess(int signal)
		{
			if (taskName.Equals("unpack"))
			{
				SystemLogger.logger().info("Unpacking the parser model '" + configDirName + ".mco' ...\n");
				configDir.unpackConfigFile();
			}
			else if (taskName.Equals("info"))
			{
				configDir.echoInfoFile();
			}
			else if (taskName.Equals("loadsymboltables"))
			{
				configDir.SymbolTables.load(configDir.getInputStreamReaderFromConfigFileEntry("symboltables.sym",inCharSet));
			}
			return signal;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int process(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int process(int signal)
		{
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int postprocess(int signal) throws org.maltparser.core.exception.MaltChainedException
		public override int postprocess(int signal)
		{
			if (taskName.Equals("createfile"))
			{
				OptionManager.instance().saveOptions(OptionContainerIndex, configDir.getOutputStreamWriter("savedoptions.sop"));
				configDir.createConfigFile();
			}
			else if (taskName.Equals("deletedir"))
			{
				configDir.terminate();
				configDir.deleteConfigDirectory();
			}
			else if (taskName.Equals("savesymboltables"))
			{
				configDir.SymbolTables.save(configDir.getOutputStreamWriter("symboltables.sym", outCharSet));
			}
			return signal;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public override void terminate()
		{
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			return obj.ToString().Equals(this.ToString());
		}

		public override int GetHashCode()
		{
			return 217 + (null == ToString() ? 0 : ToString().GetHashCode());
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("    configdir ");
			sb.Append("id:");
			sb.Append(idName);
			sb.Append(' ');
			sb.Append("task:");
			sb.Append(taskName);

			return sb.ToString();
		}

	}

}