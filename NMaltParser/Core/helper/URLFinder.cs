using System;
using System.IO;
using NMaltParser.Core.Exception;
using NMaltParser.Core.Plugin;

namespace NMaltParser.Core.Helper
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class UrlFinder
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
		public virtual Uri FindUrl(string fileString)
		{
			FileInfo specFile = new FileInfo(fileString);

			try
			{
				if (specFile.Exists)
				{
					// found the file in the file system
					return new Uri("file:///" + specFile.FullName);
				}
				else if (fileString.StartsWith("http:", StringComparison.Ordinal) || fileString.StartsWith("file:", StringComparison.Ordinal) || fileString.StartsWith("ftp:", StringComparison.Ordinal) || fileString.StartsWith("jar:", StringComparison.Ordinal))
				{
					// the input string is an URL string starting with http, file, ftp or jar
					return new Uri(fileString);
				}
				else
				{
					return FindUrLinJars(fileString);
				}
			}
			catch (MalformedURLException e)
			{
				throw new MaltChainedException("Malformed URL: " + fileString, e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL findURLinJars(String fileString) throws org.maltparser.core.exception.MaltChainedException
		public virtual Uri FindUrLinJars(string fileString)
		{
			try
			{
				// search in malt.jar and its plugins
				if (GetType().getResource(fileString) != null)
				{
					// found the input string in the malt.jar file
					return GetType().getResource(fileString);
				}
				else
				{
					 foreach (Plugin.Plugin plugin in PluginLoader.instance())
					 {
                         Uri url = !fileString.StartsWith("/", StringComparison.Ordinal) ? new Uri("jar:" + plugin.Url + "!/" + fileString) : new Uri("jar:" + plugin.Url + "!" + fileString);

						try
						{
							Stream stream = url.openStream();

							stream.Close();
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