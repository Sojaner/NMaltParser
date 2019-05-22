using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.plugin
{
    using  helper;
	using  options;


	/// <summary>
	/// The jar class loader loads the content of a jar file that complies with a MaltParser Plugin.
	/// 
	/// @author Johan Hall
	/// </summary>
	public class JarLoader : SecureClassLoader
	{
		private Dictionary<string, sbyte[]> classByteArrays;
		private Dictionary<string, Type> classes;

		/// <summary>
		/// Creates a new class loader that is specialized for loading jar files.
		/// </summary>
		/// <param name="parent"> The parent class loader </param>
		public JarLoader(ClassLoader parent) : base(parent)
		{
			classByteArrays = new Dictionary<string, sbyte[]>();
			classes = new Dictionary<string, Type>();
		}

		/* (non-Javadoc)
		 * @see java.lang.ClassLoader#findClass(java.lang.String)
		 */
		protected internal virtual Type findClass(string name)
		{
			string urlName = name.Replace('.', '/');
			sbyte[] buf;

			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				int i = name.LastIndexOf('.');
				if (i >= 0)
				{
					sm.checkPackageDefinition(name.Substring(0, i));
				}
			}

			buf = (sbyte[]) classByteArrays[urlName];
			if (buf != null)
			{
				return defineClass(null, buf, 0, buf.Length);
			}
			return null;
		}

		/// <summary>
		/// Loads the content of a jar file that comply with a MaltParser Plugin  
		/// </summary>
		/// <param name="jarUrl"> The URL to the jar file </param>
		/// <exception cref="PluginException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean readJarFile(java.net.URL jarUrl) throws org.maltparser.core.exception.MaltChainedException
		public virtual bool readJarFile(URL jarUrl)
		{
			JarInputStream jis;
			JarEntry je;
			ISet<URL> pluginXMLs = new HashSet<URL>();

			/*if (logger.isDebugEnabled()) {
				logger.debug("Loading jar " + jarUrl+"\n");
			}*/
			JarFile jarFile;
			try
			{
				jarFile = new JarFile(jarUrl.File);
			}
			catch (IOException e)
			{
				throw new PluginException("Could not open jar file " + jarUrl + ". ", e);
			}
			try
			{
				Manifest manifest = jarFile.Manifest;
				if (manifest != null)
				{
					Attributes manifestAttributes = manifest.MainAttributes;
					if (!(manifestAttributes.getValue("MaltParser-Plugin") != null && manifestAttributes.getValue("MaltParser-Plugin").Equals("true")))
					{
						return false;
					}
					if (manifestAttributes.getValue("Class-Path") != null)
					{
						string[] classPathItems = manifestAttributes.getValue("Class-Path").Split(" ");
						for (int i = 0; i < classPathItems.Length; i++)
						{
							URL u;
							try
							{
								u = new URL(jarUrl.Protocol + ":" + (new File(jarFile.Name)).ParentFile.Path + "/" + classPathItems[i]);
							}
							catch (MalformedURLException e)
							{
								throw new PluginException("The URL to the plugin jar-class-path '" + jarUrl.Protocol + ":" + (new File(jarFile.Name)).ParentFile.Path + "/" + classPathItems[i] + "' is wrong. ", e);
							}
							URLClassLoader sysloader = (URLClassLoader)ClassLoader.SystemClassLoader;
							Type sysclass = typeof(URLClassLoader);
							System.Reflection.MethodInfo method = sysclass.getDeclaredMethod("addURL",new Type[]{typeof(URL)});
							method.Accessible = true;
							method.invoke(sysloader,new object[]{u});
						}
					}
				}
			}
			catch (PatternSyntaxException e)
			{
				throw new PluginException("Could not split jar-class-path entries in the jar-file '" + jarFile.Name + "'. ", e);
			}
			catch (IOException e)
			{
				throw new PluginException("Could not read the manifest file in the jar-file '" + jarFile.Name + "'. ", e);
			}
			catch (NoSuchMethodException e)
			{
				throw new PluginException("", e);
			}
			catch (IllegalAccessException e)
			{
				throw new PluginException("", e);
			}
			catch (InvocationTargetException e)
			{
				throw new PluginException("", e);
			}

			try
			{
				jis = new JarInputStream(jarUrl.openConnection().InputStream);

				while ((je = jis.NextJarEntry) != null)
				{
					string jarName = je.Name;
					if (jarName.EndsWith(".class", StringComparison.Ordinal))
					{
						/* if (logger.isDebugEnabled()) {
							logger.debug("  Loading class: " + jarName+"\n");
						}*/
						loadClassBytes(jis, jarName);
						Type clazz = findClass(jarName.Substring(0, jarName.Length - 6));
						classes[jarName.Substring(0, jarName.Length - 6).Replace('/','.')] = clazz;
						loadClass(jarName.Substring(0, jarName.Length - 6).Replace('/', '.'));
					}
					if (jarName.EndsWith("plugin.xml", StringComparison.Ordinal))
					{
						pluginXMLs.Add(new URL("jar:" + jarUrl.Protocol + ":" + jarUrl.Path + "!/" + jarName));
					}
					jis.closeEntry();
				}
				foreach (URL url in pluginXMLs)
				{
					/* if (logger.isDebugEnabled()) {
						logger.debug("  Loading "+url+"\n");
					}*/
					OptionManager.instance().loadOptionDescriptionFile(url);
				}
			}
			catch (MalformedURLException e)
			{
				throw new PluginException("The URL to the plugin.xml is wrong. ", e);
			}
			catch (IOException e)
			{
				throw new PluginException("cannot open jar file " + jarUrl + ". ", e);
			}
			catch (ClassNotFoundException e)
			{
				throw new PluginException("The class " + e.Message + " can't be found. ", e);
			}
			return true;
		}

		/// <summary>
		/// Returns the Class object for the class with the specified name.
		/// </summary>
		/// <param name="classname"> the fully qualified name of the desired class </param>
		/// <returns> the Class object for the class with the specified name. </returns>
		public virtual Type getClass(string classname)
		{
			return (Type)classes[classname];
		}

		/// <summary>
		/// Reads a jar file entry into a byte array.
		/// </summary>
		/// <param name="jis"> The jar input stream </param>
		/// <param name="jarName"> The name of a jar file entry </param>
		/// <exception cref="PluginException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void loadClassBytes(java.util.jar.JarInputStream jis, String jarName) throws org.maltparser.core.exception.MaltChainedException
		private void loadClassBytes(JarInputStream jis, string jarName)
		{
			BufferedInputStream jarBuf = new BufferedInputStream(jis);
			MemoryStream jarOut = new MemoryStream();
			int b;
			try
			{
				while ((b = jarBuf.read()) != -1)
				{
					jarOut.WriteByte(b);
				}
				classByteArrays[jarName.Substring(0, jarName.Length - 6)] = jarOut.toByteArray();
			}
			catch (IOException e)
			{
				throw new PluginException("Error reading entry " + jarName + ". ", e);
			}
		}

		/// <summary>
		/// Checks package access
		/// </summary>
		/// <param name="name">	the package name </param>
		protected internal virtual void checkPackageAccess(string name)
		{
			SecurityManager sm = System.SecurityManager;
			if (sm != null)
			{
				sm.checkPackageAccess(name);
			}
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("The MaltParser Plugin Loader (JarLoader)\n");
			sb.Append("---------------------------------------------------------------------\n");
			foreach (string entry in new SortedSet<string>(classes.Keys))
			{
				sb.Append("   " + entry + "\n");
			}
			return sb.ToString();
		}
	}

}