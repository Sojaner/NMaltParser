using System;

namespace org.maltparser
{

	using  org.maltparser.core.exception;
	using  org.maltparser.core.helper;
	using  org.maltparser.core.helper;
	using  org.maltparser.core.options;
	using  org.maltparser.core.plugin;

	/// <summary>
	/// MaltConsoleEngine controls the MaltParser system using the console version. 
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class MaltConsoleEngine
	{
		public const int OPTION_CONTAINER = 0;

		/// <summary>
		/// Creates a MaltConsoleEngine object
		/// 
		/// </summary>
		public MaltConsoleEngine()
		{
			try
			{
				/* Option and Plug-in management */
				OptionManager.instance().loadOptionDescriptionFile();
	//			if (SystemInfo.getMaltJarPath() != null) {
	//				PluginLoader.instance().loadPlugins(new File(SystemInfo.getMaltJarPath().getParent()+"/plugin"));
	//			}
				OptionManager.instance().generateMaps();
			}
			catch (MaltChainedException e)
			{
				if (SystemLogger.logger().DebugEnabled)
				{
					SystemLogger.logger().debug("",e);
				}
				else
				{
					SystemLogger.logger().error(e.MessageChain);
				}
				Environment.Exit(1);
			}
		}

		/// <summary>
		/// Starts the console engine.
		/// </summary>
		/// <param name="args"> command-line arguments </param>
		public virtual void startEngine(string[] args)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.options.OptionManager om = org.maltparser.core.options.OptionManager.instance();
				OptionManager om = OptionManager.instance();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hasArg = om.parseCommandLine(args,OPTION_CONTAINER);
				bool hasArg = om.parseCommandLine(args,OPTION_CONTAINER);
				/* Update the verbosity level according to the verbosity option */
				string verbosity = null;
				if (hasArg)
				{
					verbosity = (string)OptionManager.instance().getOptionValue(OPTION_CONTAINER,"system", "verbosity");
				}
				else
				{
					verbosity = (string)OptionManager.instance().getOptionDefaultValue("system", "verbosity");
				}
				if (!string.ReferenceEquals(verbosity, null))
				{
					SystemLogger.instance().setSystemVerbosityLevel(verbosity.ToUpper());
				}
				/* Help or reading the option file */
				if (!hasArg || om.getNumberOfOptionValues(OPTION_CONTAINER) == 0)
				{
					SystemLogger.logger().info(SystemInfo.header());
					SystemLogger.logger().info(SystemInfo.shortHelp());
					return;
				}
				else if (om.getOptionValue(OPTION_CONTAINER,"system", "help") != null)
				{
					SystemLogger.logger().info(SystemInfo.header());
					SystemLogger.logger().info(om.OptionDescriptions);
					return;
				}
				else
				{
					if (om.getOptionValue(OPTION_CONTAINER,"system", "option_file") != null && om.getOptionValue(0,"system", "option_file").ToString().Length > 0)
					{
						om.parseOptionInstanceXMLfile((string)om.getOptionValue(OPTION_CONTAINER,"system", "option_file"));
					}
				}
				maltParser();
			}
			catch (MaltChainedException e)
			{
				if (SystemLogger.logger().DebugEnabled)
				{
					SystemLogger.logger().debug("",e);
				}
				else
				{
					SystemLogger.logger().error(e.MessageChain);
				}
				Environment.Exit(1);
			}
		}



		/// <summary>
		/// Creates and executes a MaltParser configuration
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void maltParser() throws org.maltparser.core.exception.MaltChainedException
		private void maltParser()
		{
			if (SystemLogger.logger() != null && SystemLogger.logger().InfoEnabled)
			{
				SystemLogger.logger().info(SystemInfo.header() + "\n");
				SystemLogger.logger().info("Started: " + new DateTime(DateTimeHelper.CurrentUnixTimeMillis()) + "\n");
			}
	//		if (ConcurrentEngine.canUseConcurrentEngine(OPTION_CONTAINER)) {
	//			ConcurrentEngine concurrentEngine = new ConcurrentEngine(OPTION_CONTAINER);
	//			concurrentEngine.loadModel();
	//			concurrentEngine.parse();
	//			concurrentEngine.terminate();
	//		} else {
				Engine engine = new Engine();
				engine.initialize(OPTION_CONTAINER);
				engine.process(OPTION_CONTAINER);
				engine.terminate(OPTION_CONTAINER);
	//		}
			if (SystemLogger.logger().InfoEnabled)
			{
				SystemLogger.logger().info("Finished: " + new DateTime(DateTimeHelper.CurrentUnixTimeMillis()) + "\n");
			}
		}
	}

}