using System.Text;

namespace org.maltparser.core.plugin
{

	using  org.maltparser.core.exception;

	/// <summary>
	/// The class Plugin contains information about a plug-in that comply to the the MaltParser Plugin Standard.
	/// 
	/// 
	/// @author Johan Hall
	/// 
	/// @since 1.0
	/// </summary>
	public class Plugin
	{
		private JarFile archive;
		private URL url;
		private string pluginName;


		/// <summary>
		/// Creates a plug-in container.
		/// </summary>
		/// <param name="filename">	The file name that contains the plugin </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Plugin(String filename) throws org.maltparser.core.exception.MaltChainedException
		public Plugin(string filename) : this(new File(filename))
		{
		}

		/// <summary>
		/// Creates a plug-in container.
		/// </summary>
		/// <param name="file">	The jar file that contains the plugin </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Plugin(java.io.File file) throws org.maltparser.core.exception.MaltChainedException
		public Plugin(File file)
		{
			try
			{
				Archive = new JarFile(file);
				Url = new URL("file", null, file.AbsolutePath);
				register();
			}
			catch (FileNotFoundException e)
			{
				throw new PluginException("The file '" + file.Path + File.separator + file.Name + "' cannot be found. ", e);
			}
			catch (MalformedURLException e)
			{
				throw new PluginException("Malformed URL to the jar file '" + archive.Name + "'. ", e);
			}
			catch (IOException e)
			{
				throw new PluginException("The jar file '" + file.Path + File.separator + file.Name + "' cannot be initialized. ", e);
			}
		}

		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void register() throws org.maltparser.core.exception.MaltChainedException
		private void register()
		{
			try
			{
				Attributes atts = archive.Manifest.MainAttributes;
				pluginName = atts.getValue("Plugin-Name");
				if (string.ReferenceEquals(pluginName, null))
				{
					pluginName = archive.Name;
				}
			}
			catch (IOException e)
			{
				throw new PluginException("Could not get the 'Plugin-Name' in the manifest for the plugin (jar-file). ", e);
			}
		}

		/// <summary>
		/// Returns the archive.
		/// </summary>
		/// <returns> the jar archive. </returns>
		public virtual JarFile Archive
		{
			get
			{
				return archive;
			}
			set
			{
				this.archive = value;
			}
		}
		/// <summary>
		/// Returns the plug-in name.
		/// </summary>
		/// <returns> the plug-in name. </returns>
		public virtual string PluginName
		{
			get
			{
				return pluginName;
			}
			set
			{
				this.pluginName = value;
			}
		}

		/// <summary>
		/// Returns the URL
		/// </summary>
		/// <returns> the URL </returns>
		public virtual URL Url
		{
			get
			{
				return url;
			}
			set
			{
				this.url = value;
			}
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(pluginName + " : " + url.ToString());
			return sb.ToString();
		}
	}
}