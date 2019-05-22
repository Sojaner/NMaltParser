using System;
using System.IO;

namespace org.maltparser.core.helper
{


	using  org.maltparser.core.exception;
	using  org.maltparser.core.plugin;
	using  org.maltparser.core.plugin;

	/// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class URLFinder
	{
		/// <summary>
		/// Search for a file according the following priority:
		/// <ol>
		/// <li>The local file system
		/// <li>Specified as an URL (starting with http:, file:, ftp: or jar:
		/// <li>MaltParser distribution file (malt.jar)
		/// <li>MaltParser plugins
		/// </ol>
		/// 
		/// If the file string is found, an URL object is returned, otherwise <b>null</b>
		/// </summary>
		/// <param name="fileString">	the file string to convert into an URL. </param>
		/// <returns> an URL object, if the file string is found, otherwise <b>null</b> </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL findURL(String fileString) throws org.maltparser.core.exception.MaltChainedException
		public virtual URL findURL(string fileString)
		{
			File specFile = new File(fileString);

			try
			{
				if (specFile.exists())
				{
					// found the file in the file system
					return new URL("file:///" + specFile.AbsolutePath);
				}
				else if (fileString.StartsWith("http:", StringComparison.Ordinal) || fileString.StartsWith("file:", StringComparison.Ordinal) || fileString.StartsWith("ftp:", StringComparison.Ordinal) || fileString.StartsWith("jar:", StringComparison.Ordinal))
				{
					// the input string is an URL string starting with http, file, ftp or jar
					return new URL(fileString);
				}
				else
				{
					return findURLinJars(fileString);
				}
			}
			catch (MalformedURLException e)
			{
				throw new MaltChainedException("Malformed URL: " + fileString, e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL findURLinJars(String fileString) throws org.maltparser.core.exception.MaltChainedException
		public virtual URL findURLinJars(string fileString)
		{
			try
			{
				// search in malt.jar and its plugins
				if (this.GetType().getResource(fileString) != null)
				{
					// found the input string in the malt.jar file
					return this.GetType().getResource(fileString);
				}
				else
				{
					 foreach (Plugin plugin in PluginLoader.instance())
					 {
						URL url = null;
						if (!fileString.StartsWith("/", StringComparison.Ordinal))
						{
							url = new URL("jar:" + plugin.Url + "!/" + fileString);
						}
						else
						{
							url = new URL("jar:" + plugin.Url + "!" + fileString);
						}

						try
						{
							Stream @is = url.openStream();
							@is.Close();
						}
						catch (IOException)
						{
							continue;
						}
						// found the input string in one of the plugins
						return url;
					 }
					// could not convert the input string into an URL
					return null;
				}
			}
			catch (MalformedURLException e)
			{
				throw new MaltChainedException("Malformed URL: " + fileString, e);
			}
		}
	}

}