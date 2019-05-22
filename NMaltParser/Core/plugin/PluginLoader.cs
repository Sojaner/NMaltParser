using System;
using System.Collections.Generic;
using System.Text;

namespace org.maltparser.core.plugin
{
	using  org.maltparser.core.exception;

	/// <summary>
	/// Loads MaltParser plug-ins and makes new instances of classes within these plug-ins. 
	/// 
	/// @author Johan Hall
	/// 
	/// @since 1.0
	/// </summary>
	public class PluginLoader : IEnumerable<Plugin>
	{
		private Dictionary<string, Plugin> plugins;
		private SortedSet<string> pluginNames;
		private File[] directories;
		private JarLoader jarLoader;
		private static PluginLoader uniqueInstance = new PluginLoader();

		/// <summary>
		/// Creates a PluginLoader
		/// </summary>
		/// <exception cref="PluginException"> </exception>
		private PluginLoader()
		{
			pluginNames = new SortedSet<string>();
			plugins = new Dictionary<string, Plugin>();
			jarLoader = null;
		}

		/// <summary>
		/// Returns a reference to the single instance.
		/// </summary>
		public static PluginLoader instance()
		{
			return uniqueInstance;
		}

		/// <summary>
		/// Loads plug-ins from one directory
		/// </summary>
		/// <param name="pluginDirectory"> The directory that contains all plug-ins </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadPlugins(java.io.File pluginDirectory) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadPlugins(File pluginDirectory)
		{
			this.loadPlugins(new File[] {pluginDirectory});
		}

		/// <summary>
		/// Loads plug-ins from one or more directories
		/// </summary>
		/// <param name="pluginDirectories"> An array of directories that contains all plug-ins </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadPlugins(java.io.File[] pluginDirectories) throws org.maltparser.core.exception.MaltChainedException
		public virtual void loadPlugins(File[] pluginDirectories)
		{
			directories = new File[pluginDirectories.Length];
			for (int i = 0; i < directories.Length; i++)
			{
				directories[i] = pluginDirectories[i];
			}

			try
			{
				Type self = Type.GetType("org.maltparser.core.plugin.PluginLoader");
				jarLoader = new JarLoader(self.ClassLoader);
			}
			catch (ClassNotFoundException e)
			{
				throw new PluginException("The class 'org.maltparser.core.plugin.PluginLoader' not found. ", e);
			}
			traverseDirectories();
		}

		/// <summary>
		/// Traverse all the plug-in directories
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void traverseDirectories() throws org.maltparser.core.exception.MaltChainedException
		private void traverseDirectories()
		{
			for (int i = 0; i < directories.Length; i++)
			{
				traverseDirectory(directories[i]);
			}
		}

		/// <summary>
		/// Traverse all plug-ins and sub-directories within one plug-in directory.
		/// </summary>
		/// <param name="directory"> The directory that contains plug-ins </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void traverseDirectory(java.io.File directory) throws org.maltparser.core.exception.MaltChainedException
		private void traverseDirectory(File directory)
		{
			if (!directory.Directory && directory.Name.EndsWith(".jar"))
			{
				pluginNames.Add(directory.AbsolutePath);
				Plugin plugin = new Plugin(directory);
				plugins[directory.AbsolutePath] = plugin;
				if (jarLoader.readJarFile(plugin.Url) == false)
				{
					plugins.Remove(directory.AbsolutePath);
				}
			}

			if (directory.Directory)
			{
				string[] children = directory.list();
				for (int i = 0; i < children.Length; i++)
				{
					traverseDirectory(new File(directory, children[i]));
				}
			}
		}

		/// <summary>
		/// Returns the Class object for the class with the specified name.
		/// </summary>
		/// <param name="classname"> the fully qualified name of the desired class </param>
		/// <returns> the Class object for the class with the specified name. </returns>
		public virtual Type getClass(string classname)
		{
			if (jarLoader != null)
			{
				return jarLoader.getClass(classname);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Creates a new instance of a class within one of the plug-ins
		/// </summary>
		/// <param name="classname"> The fully qualified name of the desired class </param>
		/// <param name="argTypes"> An array of classes (fully qualified name) that specify the arguments to the constructor </param>
		/// <param name="args"> An array of objects that will be the actual parameters to the constructor (the type should corresponds to the argTypes). </param>
		/// <returns> a reference to the created instance. </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object newInstance(String classname, Class[] argTypes, Object[] args) throws org.maltparser.core.exception.MaltChainedException
		public virtual object newInstance(string classname, Type[] argTypes, object[] args)
		{
			try
			{
				if (jarLoader == null)
				{
					return null;
				}
				Type clazz = jarLoader.getClass(classname);
				object o = null;
				if (clazz == null)
				{
					return null;
				}
				if (argTypes != null)
				{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: Constructor<?> constructor = clazz.getConstructor(argTypes);
					System.Reflection.ConstructorInfo<object> constructor = clazz.GetConstructor(argTypes);
					o = constructor.newInstance(args);
				}
				else
				{
					o = System.Activator.CreateInstance(clazz);
				}
				return o;
			}
			catch (NoSuchMethodException e)
			{
				throw new PluginException("The plugin loader was not able to create an instance of the class '" + classname + "'. ", e);
			}
			catch (InstantiationException e)
			{
				throw new PluginException("The plugin loader was not able to create an instance of the class '" + classname + "'. ", e);
			}
			catch (IllegalAccessException e)
			{
				throw new PluginException("The plugin loader was not able to create an instance of the class '" + classname + "'. ", e);
			}
			catch (InvocationTargetException e)
			{
				throw new PluginException("The plugin loader was not able to create an instance of the class '" + classname + "'. ", e);
			}
		}

		public virtual IEnumerator<Plugin> GetEnumerator()
		{
			return plugins.Values.GetEnumerator();
		}


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (Plugin plugin in this)
			{
				sb.Append(plugin.ToString() + "\n");
			}
			return sb.ToString();
		}
	}

}