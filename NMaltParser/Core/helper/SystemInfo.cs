using System.Text;
using NMaltParser.Core.Options;

namespace NMaltParser.Core.Helper
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class SystemInfo
	{
		private static SystemInfo uniqueInstance = new SystemInfo();
		private static string version;
		private static string buildDate;

		private SystemInfo()
		{
			URL url = GetType().getResource("/appdata/release.properties");
			if (url != null)
			{
				Properties properties = new Properties();
				try
				{
				   properties.load(url.openStream());
				}
				catch (IOException)
				{

				}
				version = properties.getProperty("version", "undef");
				buildDate = properties.getProperty("builddate", "undef");
			}
			else
			{
				version = "undef";
				buildDate = "undef";
			}
		}

		/// <summary>
		/// Returns a reference to the single instance.
		/// </summary>
		public static SystemInfo instance()
		{
			return uniqueInstance;
		}

		/// <summary>
		/// Returns the application header
		/// </summary>
		/// <returns> the application header </returns>
		public static string header()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("-----------------------------------------------------------------------------\n" + "                          MaltParser " + version + "                             \n" + "-----------------------------------------------------------------------------\n" + "         MALT (Models and Algorithms for Language Technology) Group          \n" + "             Vaxjo University and Uppsala University                         \n" + "                             Sweden                                          \n" + "-----------------------------------------------------------------------------\n");
			return sb.ToString();
		}

		/// <summary>
		/// Returns a short version of the help
		/// </summary>
		/// <returns> a short version of the help </returns>
		public static string shortHelp()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("\n" + "Usage: \n" + "   java -jar maltparser-" + version + ".jar -f <path to option file> <options>\n" + "   java -jar maltparser-" + version + ".jar -h for more help and options\n\n" + OptionManager.instance().OptionDescriptions.toStringOptionGroup("system") + "Documentation: docs/index.html\n");
			return sb.ToString();
		}


		/// <summary>
		/// Returns the version number as string
		/// </summary>
		/// <returns> the version number as string </returns>
		public static string Version
		{
			get
			{
				return version;
			}
		}

		/// <summary>
		/// Returns the build date
		/// </summary>
		/// <returns> the build date </returns>
		public static string BuildDate
		{
			get
			{
				return buildDate;
			}
		}
	}

}