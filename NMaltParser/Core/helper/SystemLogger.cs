namespace org.maltparser.core.helper
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class SystemLogger
	{
		private static SystemLogger uniqueInstance = new SystemLogger();
		private static Logger systemLogger;
		private Level systemVerbosityLevel;
		private ConsoleAppender consoleAppender;

		private SystemLogger()
		{
			systemLogger = Logger.getLogger("System");
			/* System verbosity (Standard Out) */
			consoleAppender = new ConsoleAppender(new PatternLayout("%m"), ConsoleAppender.SYSTEM_ERR);
			consoleAppender.Encoding = "UTF-16";
			systemLogger.addAppender(consoleAppender);
			if (System.getProperty("Malt.verbosity") != null)
			{
				setSystemVerbosityLevel(System.getProperty("Malt.verbosity").ToUpper());
			}
			else
			{
				setSystemVerbosityLevel("INFO");
			}
		}

		/// <summary>
		/// Returns a reference to the single instance.
		/// </summary>
		public static SystemLogger instance()
		{
			return uniqueInstance;
		}

		/// <summary>
		/// Returns a reference to the Logger.
		/// </summary>
		public static Logger logger()
		{
			return systemLogger;
		}

		/// <summary>
		/// Returns the system verbosity level
		/// </summary>
		/// <returns> the system verbosity level </returns>
		public virtual Level getSystemVerbosityLevel()
		{
			return systemVerbosityLevel;
		}

		/// <summary>
		/// Sets the system verbosity level
		/// </summary>
		/// <param name="verbosity">	a system verbosity level </param>
		public virtual void setSystemVerbosityLevel(string verbosity)
		{
			systemVerbosityLevel = Level.toLevel(verbosity, Level.INFO);
			consoleAppender.Threshold = systemVerbosityLevel;
			systemLogger.Level = systemVerbosityLevel;
		}
	}

}