using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.config
{

	using  version;
	using  exception;
	using  helper;
    using  io.dataformat;
    using  options;
	using  symbol;
	using  symbol.hash;
	using  symbol.parse;

	/// <summary>
	/// This class contains methods for handle the configuration directory.
	/// 
	/// @author Johan Hall
	/// </summary>
	public class ConfigurationDir
	{
		protected internal const int BUFFER = 4096;
		protected internal File configDirectory;
		protected internal string name;
		protected internal string type;
		protected internal File workingDirectory;
		protected internal URL url;
		protected internal int containerIndex;
		protected internal StreamWriter infoFile = null;
		protected internal string createdByMaltParserVersion;

		private SymbolTableHandler symbolTables;
		private DataFormatManager dataFormatManager;
		private Dictionary<string, DataFormatInstance> dataFormatInstances;
		private URL inputFormatURL;
		private URL outputFormatURL;

		/// <summary>
		/// Creates a configuration directory from a mco-file specified by an URL.
		/// </summary>
		/// <param name="url">	an URL to a mco-file </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ConfigurationDir(java.net.URL url) throws org.maltparser.core.exception.MaltChainedException
		public ConfigurationDir(URL url)
		{
			initWorkingDirectory();
			Url = url;
			initNameNTypeFromInfoFile(url);
	//		initData();
		}

		/// <summary>
		/// Creates a new configuration directory or a configuration directory from a mco-file
		/// </summary>
		/// <param name="name">	the name of the configuration </param>
		/// <param name="type">	the type of configuration </param>
		/// <param name="containerIndex">	the container index </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ConfigurationDir(String name, String type, int containerIndex) throws org.maltparser.core.exception.MaltChainedException
		public ConfigurationDir(string name, string type, int containerIndex)
		{
			ContainerIndex = containerIndex;

			initWorkingDirectory();
			if (!ReferenceEquals(name, null) && name.Length > 0 && !ReferenceEquals(type, null) && type.Length > 0)
			{
				Name = name;
				Type = type;
			}
			else
			{
				throw new ConfigurationException("The configuration name is not specified. ");
			}

			ConfigDirectory = new File(workingDirectory.Path + File.separator + Name);

			string mode = OptionManager.instance().getOptionValue(containerIndex, "config", "flowchart").ToString().Trim();
			if (mode.Equals("parse"))
			{
				// During parsing also search for the MaltParser configuration file in the class path
				File mcoPath = new File(workingDirectory.Path + File.separator + Name + ".mco");
				if (!mcoPath.exists())
				{
					string classpath = System.getProperty("java.class.path");
					string[] items = classpath.Split(System.getProperty("path.separator"), true);
					bool found = false;
					foreach (string item in items)
					{
						File candidateDir = new File(item);
						if (candidateDir.exists() && candidateDir.Directory)
						{
							File candidateConfigFile = new File(candidateDir.Path + File.separator + Name + ".mco");
							if (candidateConfigFile.exists())
							{
								initWorkingDirectory(candidateDir.Path);
								ConfigDirectory = new File(workingDirectory.Path + File.separator + Name);
								found = true;
								break;
							}
						}
					}
					if (found == false)
					{
						throw new ConfigurationException("Couldn't find the MaltParser configuration file: " + Name + ".mco");
					}
				}
				try
				{
					url = mcoPath.toURI().toURL();
				}
				catch (MalformedURLException)
				{
					// should never happen
					throw new ConfigurationException("File path could not be represented as a URL.");
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initDataFormat() throws org.maltparser.core.exception.MaltChainedException
		public virtual void initDataFormat()
		{
			string inputFormatName = OptionManager.instance().getOptionValue(containerIndex, "input", "format").ToString().Trim();
			string outputFormatName = OptionManager.instance().getOptionValue(containerIndex, "output", "format").ToString().Trim();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
			URLFinder f = new URLFinder();

			if (configDirectory != null && configDirectory.exists())
			{
				if (outputFormatName.Length == 0 || inputFormatName.Equals(outputFormatName))
				{
					URL inputFormatURL = f.findURLinJars(inputFormatName);
					if (inputFormatURL != null)
					{
						outputFormatName = inputFormatName = copyToConfig(inputFormatURL);
					}
					else
					{
						outputFormatName = inputFormatName = copyToConfig(inputFormatName);
					}
				}
				else
				{
					URL inputFormatURL = f.findURLinJars(inputFormatName);
					if (inputFormatURL != null)
					{
						inputFormatName = copyToConfig(inputFormatURL);
					}
					else
					{
						inputFormatName = copyToConfig(inputFormatName);
					}
					URL outputFormatURL = f.findURLinJars(outputFormatName);
					if (inputFormatURL != null)
					{
						outputFormatName = copyToConfig(outputFormatURL);
					}
					else
					{
						outputFormatName = copyToConfig(outputFormatName);
					}
				}
				OptionManager.instance().overloadOptionValue(containerIndex, "input", "format", inputFormatName);
			}
			else
			{
				if (outputFormatName.Length == 0)
				{
					outputFormatName = inputFormatName;
				}
			}
			dataFormatInstances = new Dictionary<string, DataFormatInstance>(3);
			inputFormatURL = findURL(inputFormatName);
			outputFormatURL = findURL(outputFormatName);
			if (outputFormatURL != null)
			{
				try
				{
					Stream @is = outputFormatURL.openStream();
				}
				catch (FileNotFoundException)
				{
					outputFormatURL = f.findURL(outputFormatName);
				}
				catch (IOException)
				{
					outputFormatURL = f.findURL(outputFormatName);
				}
			}
			else
			{
				outputFormatURL = f.findURL(outputFormatName);
			}
			dataFormatManager = new DataFormatManager(inputFormatURL, outputFormatURL);

			string mode = OptionManager.instance().getOptionValue(containerIndex, "config", "flowchart").ToString().Trim();
			if (mode.Equals("parse"))
			{
				symbolTables = new ParseSymbolTableHandler(new HashSymbolTableHandler());
	//			symbolTables = new TrieSymbolTableHandler(TrieSymbolTableHandler.ADD_NEW_TO_TRIE);
			}
			else
			{
				symbolTables = new HashSymbolTableHandler();
			}
			if (dataFormatManager.InputDataFormatSpec.DataStructure == DataStructure.PHRASE)
			{
				if (mode.Equals("learn"))
				{
					ISet<Dependency> deps = dataFormatManager.InputDataFormatSpec.Dependencies;
					foreach (Dependency dep in deps)
					{
						URL depFormatURL = f.findURLinJars(dep.UrlString);
						if (depFormatURL != null)
						{
							copyToConfig(depFormatURL);
						}
						else
						{
							this.copyToConfig(dep.UrlString);
						}
					}
				}
				else if (mode.Equals("parse"))
				{
					ISet<Dependency> deps = dataFormatManager.InputDataFormatSpec.Dependencies;
					string nullValueStategy = OptionManager.instance().getOptionValue(containerIndex, "singlemalt", "null_value").ToString();
					foreach (Dependency dep in deps)
					{
	//					URL depFormatURL = f.findURLinJars(dep.getUrlString());
						DataFormatInstance dataFormatInstance = dataFormatManager.getDataFormatSpec(dep.DependentOn).createDataFormatInstance(symbolTables, nullValueStategy);
						addDataFormatInstance(dataFormatManager.getDataFormatSpec(dep.DependentOn).DataFormatName, dataFormatInstance);
						dataFormatManager.InputDataFormatSpec = dataFormatManager.getDataFormatSpec(dep.DependentOn);
	//					dataFormatManager.setOutputDataFormatSpec(dataFormatManager.getDataFormatSpec(dep.getDependentOn()));
					}
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.net.URL findURL(String specModelFileName) throws org.maltparser.core.exception.MaltChainedException
		private URL findURL(string specModelFileName)
		{
			URL url = null;
			File specFile = getFile(specModelFileName);
			if (specFile.exists())
			{
				try
				{
					url = new URL("file:///" + specFile.AbsolutePath);
				}
				catch (MalformedURLException e)
				{
					throw new MaltChainedException("Malformed URL: " + specFile, e);
				}
			}
			else
			{
				url = getConfigFileEntryURL(specModelFileName);
			}
			return url;
		}

		/// <summary>
		/// Creates an output stream writer, where the corresponding file will be included in the configuration directory
		/// </summary>
		/// <param name="fileName">	a file name </param>
		/// <param name="charSet">	a char set
		/// @return	an output stream writer for writing to a file within the configuration directory </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStreamWriter getOutputStreamWriter(String fileName, String charSet) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamWriter getOutputStreamWriter(string fileName, string charSet)
		{
			try
			{
				return new StreamWriter(new FileStream(configDirectory.Path + File.separator + fileName, FileMode.Create, FileAccess.Write), charSet);
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("The file '" + fileName + "' cannot be created. ", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new ConfigurationException("The char set '" + charSet + "' is not supported. ", e);
			}
		}

		/// <summary>
		/// Creates an output stream writer, where the corresponding file will be included in the 
		/// configuration directory. Uses UTF-8 for character encoding.
		/// </summary>
		/// <param name="fileName">	a file name </param>
		/// <returns> an output stream writer for writing to a file within the configuration directory </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStreamWriter getOutputStreamWriter(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamWriter getOutputStreamWriter(string fileName)
		{
			try
			{
				return new StreamWriter(new FileStream(configDirectory.Path + File.separator + fileName, true), Encoding.UTF8);
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("The file '" + fileName + "' cannot be created. ", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new ConfigurationException("The char set 'UTF-8' is not supported. ", e);
			}
		}
		/// <summary>
		/// This method acts the same as getOutputStreamWriter with the difference that the writer append in the file
		/// if it already exists instead of deleting the previous content before starting to write.
		/// </summary>
		/// <param name="fileName">	a file name </param>
		/// <returns> an output stream writer for writing to a file within the configuration directory </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.OutputStreamWriter getAppendOutputStreamWriter(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamWriter getAppendOutputStreamWriter(string fileName)
		{
			try
			{
				return new StreamWriter(new FileStream(configDirectory.Path + File.separator + fileName, true), Encoding.UTF8);
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("The file '" + fileName + "' cannot be created. ", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new ConfigurationException("The char set 'UTF-8' is not supported. ", e);
			}
		}

		/// <summary>
		/// Creates an input stream reader for reading a file within the configuration directory
		/// </summary>
		/// <param name="fileName">	a file name </param>
		/// <param name="charSet">	a char set </param>
		/// <returns> an input stream reader for reading a file within the configuration directory </returns>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStreamReader getInputStreamReader(String fileName, String charSet) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamReader getInputStreamReader(string fileName, string charSet)
		{
			try
			{
				return new StreamReader(new FileStream(configDirectory.Path + File.separator + fileName, FileMode.Open, FileAccess.Read), charSet);
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("The file '" + fileName + "' cannot be found. ", e);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new ConfigurationException("The char set '" + charSet + "' is not supported. ", e);
			}
		}

		/// <summary>
		/// Creates an input stream reader for reading a file within the configuration directory.
		/// Uses UTF-8 for character encoding.
		/// </summary>
		/// <param name="fileName">	a file name
		/// @return	an input stream reader for reading a file within the configuration directory </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStreamReader getInputStreamReader(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamReader getInputStreamReader(string fileName)
		{
			return getInputStreamReader(fileName, "UTF-8");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.jar.JarFile getConfigJarfile() throws org.maltparser.core.exception.MaltChainedException
		public virtual JarFile ConfigJarfile
		{
			get
			{
				JarFile mcoFile = null;
				if (url != null && !url.ToString().StartsWith("jar", StringComparison.Ordinal))
				{
					// New solution 
					try
					{
						JarURLConnection conn = (JarURLConnection)(new URL("jar:" + url.ToString() + "!/")).openConnection();
						mcoFile = conn.JarFile;
					}
					catch (IOException e)
					{
						throw new ConfigurationException("The mco-file '" + url + "' cannot be found. ", e);
					}
				}
				else
				{
					// Old solution: Can load files from the mco-file within a jar-file
					File mcoPath = new File(workingDirectory.Path + File.separator + Name + ".mco");
					try
					{
						mcoFile = new JarFile(mcoPath.AbsolutePath);
					}
					catch (IOException e)
					{
						throw new ConfigurationException("The mco-file '" + mcoPath + "' cannot be found. ", e);
					}
				}
				if (mcoFile == null)
				{
					throw new ConfigurationException("The mco-file cannot be found. ");
				}
				return mcoFile;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.jar.JarEntry getConfigFileEntry(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual JarEntry getConfigFileEntry(string fileName)
		{
			JarFile mcoFile = ConfigJarfile;

			JarEntry entry = mcoFile.getJarEntry(Name + '/' + fileName);
			if (entry == null)
			{
				entry = mcoFile.getJarEntry(Name + '\\' + fileName);
			}
			return entry;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream getInputStreamFromConfigFileEntry(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual Stream getInputStreamFromConfigFileEntry(string fileName)
		{
			JarFile mcoFile = ConfigJarfile;
			JarEntry entry = getConfigFileEntry(fileName);

			try
			{
			  if (entry == null)
			  {
				  throw new FileNotFoundException();
			  }
			  return mcoFile.getInputStream(entry);
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("The file entry '" + fileName + "' in the mco file '" + workingDirectory.Path + File.separator + Name + ".mco" + "' cannot be found. ", e);
			}
			catch (IOException e)
			{
				throw new ConfigurationException("The file entry '" + fileName + "' in the mco file '" + workingDirectory.Path + File.separator + Name + ".mco" + "' cannot be loaded. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStreamReader getInputStreamReaderFromConfigFileEntry(String fileName, String charSet) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamReader getInputStreamReaderFromConfigFileEntry(string fileName, string charSet)
		{
			try
			{
				return new StreamReader(getInputStreamFromConfigFileEntry(fileName), charSet);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new ConfigurationException("The char set '" + charSet + "' is not supported. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStreamReader getInputStreamReaderFromConfigFile(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamReader getInputStreamReaderFromConfigFile(string fileName)
		{
			return getInputStreamReaderFromConfigFileEntry(fileName, "UTF-8");
		}

		/// <summary>
		/// Returns a file handler object of a file within the configuration directory
		/// </summary>
		/// <param name="fileName">	a file name
		/// @return	a file handler object of a file within the configuration directory </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.File getFile(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual File getFile(string fileName)
		{
			return new File(configDirectory.Path + File.separator + fileName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getConfigFileEntryURL(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual URL getConfigFileEntryURL(string fileName)
		{
			if (url != null && !url.ToString().StartsWith("jar", StringComparison.Ordinal))
			{
				// New solution 
				try
				{
					URL url = new URL("jar:" + this.url.ToString() + "!/" + Name + '/' + fileName + "\n");
					try
					{
						Stream @is = url.openStream();
						@is.Close();
					}
					catch (IOException)
					{
						url = new URL("jar:" + this.url.ToString() + "!/" + Name + '\\' + fileName + "\n");
					}
					return url;
				}
				catch (MalformedURLException e)
				{
					throw new ConfigurationException("Couldn't find the URL '" + "jar:" + url.ToString() + "!/" + Name + '/' + fileName + "'", e);
				}
			}
			else
			{
				// Old solution: Can load files from the mco-file within a jar-file
				File mcoPath = new File(workingDirectory.Path + File.separator + Name + ".mco");
				try
				{
						if (!mcoPath.exists())
						{
								throw new ConfigurationException("Couldn't find mco-file '" + mcoPath.AbsolutePath + "'");
						}
						URL url = new URL("jar:" + new URL("file", null, mcoPath.AbsolutePath) + "!/" + Name + '/' + fileName + "\n");
						try
						{
								Stream @is = url.openStream();
								@is.Close();
						}
						catch (IOException)
						{
								url = new URL("jar:" + new URL("file", null, mcoPath.AbsolutePath) + "!/" + Name + '\\' + fileName + "\n");
						}
						return url;
				}
				catch (MalformedURLException e)
				{
						throw new ConfigurationException("Couldn't find the URL '" + "jar:" + mcoPath.AbsolutePath + "!/" + Name + '/' + fileName + "'", e);
				}
			}
		}

		/// <summary>
		/// Copies a file into the configuration directory.
		/// </summary>
		/// <param name="source">	a path to file </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String copyToConfig(java.io.File source) throws org.maltparser.core.exception.MaltChainedException
		public virtual string copyToConfig(File source)
		{
			sbyte[] readBuffer = new sbyte[BUFFER];
			string destination = configDirectory.Path + File.separator + source.Name;
			try
			{
				BufferedInputStream bis = new BufferedInputStream(new FileStream(source, FileMode.Open, FileAccess.Read));
				BufferedOutputStream bos = new BufferedOutputStream(new FileStream(destination, FileMode.Create, FileAccess.Write), BUFFER);

				int n = 0;
				while ((n = bis.read(readBuffer, 0, BUFFER)) != -1)
				{
					bos.write(readBuffer, 0, n);
				}
				bos.flush();
				bos.close();
				bis.close();
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("The source file '" + source + "' cannot be found or the destination file '" + destination + "' cannot be created when coping the file. ", e);
			}
			catch (IOException e)
			{
				throw new ConfigurationException("The source file '" + source + "' cannot be copied to destination '" + destination + "'. ", e);
			}
			return source.Name;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String copyToConfig(String fileUrl) throws org.maltparser.core.exception.MaltChainedException
		public virtual string copyToConfig(string fileUrl)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
			URLFinder f = new URLFinder();
			URL url = f.findURL(fileUrl);
			if (url == null)
			{
				throw new ConfigurationException("The file or URL '" + fileUrl + "' could not be found. ");
			}
			return copyToConfig(url);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String copyToConfig(java.net.URL url) throws org.maltparser.core.exception.MaltChainedException
		public virtual string copyToConfig(URL url)
		{
			if (url == null)
			{
				throw new ConfigurationException("URL could not be found. ");
			}
			sbyte[] readBuffer = new sbyte[BUFFER];
			string destFileName = url.Path;
			int indexSlash = destFileName.LastIndexOf('/');
			if (indexSlash == -1)
			{
				indexSlash = destFileName.LastIndexOf('\\');
			}

			if (indexSlash != -1)
			{
				destFileName = destFileName.Substring(indexSlash + 1);
			}

			string destination = configDirectory.Path + File.separator + destFileName;
			try
			{
				BufferedInputStream bis = new BufferedInputStream(url.openStream());
				BufferedOutputStream bos = new BufferedOutputStream(new FileStream(destination, FileMode.Create, FileAccess.Write), BUFFER);

				int n = 0;
				while ((n = bis.read(readBuffer, 0, BUFFER)) != -1)
				{
					bos.write(readBuffer, 0, n);
				}
				bos.flush();
				bos.close();
				bis.close();
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("The destination file '" + destination + "' cannot be created when coping the file. ", e);
			}
			catch (IOException e)
			{
				throw new ConfigurationException("The URL '" + url + "' cannot be copied to destination '" + destination + "'. ", e);
			}
			return destFileName;
		}


		/// <summary>
		/// Removes the configuration directory, if it exists and it contains a .info file. 
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deleteConfigDirectory() throws org.maltparser.core.exception.MaltChainedException
		public virtual void deleteConfigDirectory()
		{
			if (!configDirectory.exists())
			{
				return;
			}
			File infoFile = new File(configDirectory.Path + File.separator + Name + "_" + Type + ".info");
			if (infoFile.exists())
			{
				deleteConfigDirectory(configDirectory);
			}
			else
			{
				throw new ConfigurationException("There exists a directory that is not a MaltParser configuration directory. ");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void deleteConfigDirectory(java.io.File directory) throws org.maltparser.core.exception.MaltChainedException
		private void deleteConfigDirectory(File directory)
		{
			if (directory.exists())
			{
				File[] files = directory.listFiles();
				for (int i = 0; i < files.Length; i++)
				{
					if (files[i].Directory)
					{
						deleteConfigDirectory(files[i]);
					}
					else
					{
						files[i].delete();
					}
				}
			}
			else
			{
				throw new ConfigurationException("The directory '" + directory.Path + "' cannot be found. ");
			}
			directory.delete();
		}

		/// <summary>
		/// Returns a file handler object for the configuration directory
		/// </summary>
		/// <returns> a file handler object for the configuration directory </returns>
		public virtual File ConfigDirectory
		{
			get
			{
				return configDirectory;
			}
			set
			{
				configDirectory = value;
			}
		}


		/// <summary>
		/// Creates the configuration directory
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void createConfigDirectory() throws org.maltparser.core.exception.MaltChainedException
		public virtual void createConfigDirectory()
		{
			checkConfigDirectory();
			configDirectory.mkdir();
			createInfoFile();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkConfigDirectory() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void checkConfigDirectory()
		{
			if (configDirectory.exists() && !configDirectory.Directory)
			{
				throw new ConfigurationException("The configuration directory name already exists and is not a directory. ");
			}

			if (configDirectory.exists())
			{
				deleteConfigDirectory();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createInfoFile() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void createInfoFile()
		{
			infoFile = new StreamWriter(getOutputStreamWriter(Name + "_" + Type + ".info"));
			try
			{
				infoFile.Write("CONFIGURATION\n");
				infoFile.Write("Configuration name:   " + Name + "\n");
				infoFile.Write("Configuration type:   " + Type + "\n");
				infoFile.Write("Created:              " + new DateTime(DateTimeHelper.CurrentUnixTimeMillis()) + "\n");

				infoFile.Write("\nSYSTEM\n");
				infoFile.Write("Operating system architecture: " + System.getProperty("os.arch") + "\n");
				infoFile.Write("Operating system name:         " + System.getProperty("os.name") + "\n");
				infoFile.Write("JRE vendor name:               " + System.getProperty("java.vendor") + "\n");
				infoFile.Write("JRE version number:            " + System.getProperty("java.version") + "\n");

				infoFile.Write("\nMALTPARSER\n");
				infoFile.Write("Version:                       " + SystemInfo.Version + "\n");
				infoFile.Write("Build date:                    " + SystemInfo.BuildDate + "\n");
				ISet<string> excludeGroups = new HashSet<string>();
				excludeGroups.Add("system");
				infoFile.Write("\nSETTINGS\n");
				infoFile.Write(OptionManager.instance().toStringPrettyValues(containerIndex, excludeGroups));
				infoFile.Flush();
			}
			catch (IOException)
			{
				throw new ConfigurationException("Could not create the maltparser info file. ");
			}
		}

		/// <summary>
		/// Returns a writer to the configuration information file
		/// 
		/// @return	a writer to the configuration information file </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.BufferedWriter getInfoFileWriter() throws org.maltparser.core.exception.MaltChainedException
		public virtual StreamWriter InfoFileWriter
		{
			get
			{
				return infoFile;
			}
		}

		/// <summary>
		/// Creates the malt configuration file (.mco). This file is compressed.   
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void createConfigFile() throws org.maltparser.core.exception.MaltChainedException
		public virtual void createConfigFile()
		{
			try
			{
				JarOutputStream jos = new JarOutputStream(new FileStream(workingDirectory.Path + File.separator + Name + ".mco", FileMode.Create, FileAccess.Write));
	//			configLogger.info("Creates configuration file '"+workingDirectory.getPath()+File.separator+getName()+".mco' ...\n");
				createConfigFile(configDirectory.Path, jos);
				jos.close();
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("The maltparser configurtation file '" + workingDirectory.Path + File.separator + Name + ".mco" + "' cannot be found. ", e);
			}
			catch (IOException e)
			{
				throw new ConfigurationException("The maltparser configurtation file '" + workingDirectory.Path + File.separator + Name + ".mco" + "' cannot be created. ", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void createConfigFile(String directory, java.util.jar.JarOutputStream jos) throws org.maltparser.core.exception.MaltChainedException
		private void createConfigFile(string directory, JarOutputStream jos)
		{
			sbyte[] readBuffer = new sbyte[BUFFER];
			try
			{
				File zipDir = new File(directory);
				string[] dirList = zipDir.list();

				int bytesIn = 0;

				for (int i = 0; i < dirList.Length; i++)
				{
					File f = new File(zipDir, dirList[i]);
					if (f.Directory)
					{
						string filePath = f.Path;
						createConfigFile(filePath, jos);
						continue;
					}

					FileStream fis = new FileStream(f, FileMode.Open, FileAccess.Read);

					string entryPath = f.Path.substring(workingDirectory.Path.length() + 1);
					entryPath = entryPath.Replace('\\', '/');
					JarEntry entry = new JarEntry(entryPath);
					jos.putNextEntry(entry);

					while ((bytesIn = fis.Read(readBuffer, 0, readBuffer.Length)) != -1)
					{
						jos.write(readBuffer, 0, bytesIn);
					}

					fis.Close();
				}
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("The directory '" + directory + "' cannot be found. ", e);
			}
			catch (IOException e)
			{
				throw new ConfigurationException("The directory '" + directory + "' cannot be compressed into a mco file. ", e);
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copyConfigFile(java.io.File in, java.io.File out, org.maltparser.core.config.version.Versioning versioning) throws org.maltparser.core.exception.MaltChainedException
		public virtual void copyConfigFile(File @in, File @out, Versioning versioning)
		{
			try
			{
				JarFile jar = new JarFile(@in);
				JarOutputStream tempJar = new JarOutputStream(new FileStream(@out, FileMode.Create, FileAccess.Write));
				sbyte[] buffer = new sbyte[BUFFER];
				int bytesRead;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
				StringBuilder sb = new StringBuilder();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
				URLFinder f = new URLFinder();

				for (IEnumerator<JarEntry> entries = jar.entries(); entries.MoveNext();)
				{
					JarEntry inEntry = (JarEntry) entries.Current;
					Stream entryStream = jar.getInputStream(inEntry);
					JarEntry outEntry = versioning.getJarEntry(inEntry);

					if (!versioning.hasChanges(inEntry, outEntry))
					{
						tempJar.putNextEntry(outEntry);
						while ((bytesRead = entryStream.Read(buffer, 0, buffer.Length)) != -1)
						{
							tempJar.write(buffer, 0, bytesRead);
						}
					}
					else
					{
						tempJar.putNextEntry(outEntry);
						StreamReader br = new StreamReader(entryStream);
						string line = null;
						sb.Length = 0;
						while (!ReferenceEquals((line = br.ReadLine()), null))
						{
							sb.Append(line);
							sb.Append('\n');
						}
						string outString = versioning.modifyJarEntry(inEntry, outEntry, sb);
						tempJar.write(outString.GetBytes());
					}
				}
				if (!ReferenceEquals(versioning.FeatureModelXML, null) && versioning.FeatureModelXML.StartsWith("/appdata", StringComparison.Ordinal))
				{
					int index = versioning.FeatureModelXML.LastIndexOf('/');
					BufferedInputStream bis = new BufferedInputStream(f.findURLinJars(versioning.FeatureModelXML).openStream());
					tempJar.putNextEntry(new JarEntry(versioning.NewConfigName + "/" + versioning.FeatureModelXML.Substring(index + 1)));
					int n = 0;
					while ((n = bis.read(buffer, 0, BUFFER)) != -1)
					{
						tempJar.write(buffer, 0, n);
					}
					bis.close();
				}
				if (!ReferenceEquals(versioning.InputFormatXML, null) && versioning.InputFormatXML.StartsWith("/appdata", StringComparison.Ordinal))
				{
					int index = versioning.InputFormatXML.LastIndexOf('/');
					BufferedInputStream bis = new BufferedInputStream(f.findURLinJars(versioning.InputFormatXML).openStream());
					tempJar.putNextEntry(new JarEntry(versioning.NewConfigName + "/" + versioning.InputFormatXML.Substring(index + 1)));
					int n = 0;
					while ((n = bis.read(buffer, 0, BUFFER)) != -1)
					{
						tempJar.write(buffer, 0, n);
					}
					bis.close();
				}
				tempJar.flush();
				tempJar.close();
				jar.close();
			}
			catch (IOException e)
			{
				throw new ConfigurationException("", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initNameNTypeFromInfoFile(java.net.URL url) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void initNameNTypeFromInfoFile(URL url)
		{
			if (url == null)
			{
				throw new ConfigurationException("The URL cannot be found. ");
			}
			try
			{
				JarEntry je;
				JarInputStream jis = new JarInputStream(url.openConnection().InputStream);
				while ((je = jis.NextJarEntry) != null)
				{
					string entryName = je.Name;
					if (entryName.EndsWith(".info", StringComparison.Ordinal))
					{
						int indexUnderScore = entryName.LastIndexOf('_');
						int indexSeparator = entryName.LastIndexOf(File.separator);
						if (indexSeparator == -1)
						{
							indexSeparator = entryName.LastIndexOf('/');
						}
						if (indexSeparator == -1)
						{
							indexSeparator = entryName.LastIndexOf('\\');
						}
						int indexDot = entryName.LastIndexOf('.');
						if (indexUnderScore == -1 || indexDot == -1)
						{
							throw new ConfigurationException("Could not find the configuration name and type from the URL '" + url.ToString() + "'. ");
						}
						Name = entryName.Substring(indexSeparator + 1, indexUnderScore - (indexSeparator + 1));
						Type = entryName.Substring(indexUnderScore+1, indexDot - (indexUnderScore+1));
						ConfigDirectory = new File(workingDirectory.Path + File.separator + Name);
						jis.close();
						return;
					}
				}

			}
			catch (IOException e)
			{
				throw new ConfigurationException("Could not find the configuration name and type from the URL '" + url.ToString() + "'. ", e);
			}
		}

		/// <summary>
		/// Prints the content of the configuration information file to the system logger
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void echoInfoFile() throws org.maltparser.core.exception.MaltChainedException
		public virtual void echoInfoFile()
		{
			checkConfigDirectory();
			JarInputStream jis;
			try
			{
				if (url == null)
				{
					jis = new JarInputStream(new FileStream(workingDirectory.Path + File.separator + Name + ".mco", FileMode.Open, FileAccess.Read));
				}
				else
				{
					jis = new JarInputStream(url.openConnection().InputStream);
				}
				JarEntry je;

				while ((je = jis.NextJarEntry) != null)
				{
					string entryName = je.Name;

					if (entryName.EndsWith(Name + "_" + Type + ".info", StringComparison.Ordinal))
					{
						int c;
						while ((c = jis.read()) != -1)
						{
							SystemLogger.logger().info((char)c);
						}
					}
				}
				jis.close();
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("Could not print configuration information file. The configuration file '" + workingDirectory.Path + File.separator + Name + ".mco" + "' cannot be found. ", e);
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Could not print configuration information file. ", e);
			}

		}

		/// <summary>
		/// Unpacks the malt configuration file (.mco).
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unpackConfigFile() throws org.maltparser.core.exception.MaltChainedException
		public virtual void unpackConfigFile()
		{
			checkConfigDirectory();
			JarInputStream jis;
			try
			{
				if (url == null)
				{
					jis = new JarInputStream(new FileStream(workingDirectory.Path + File.separator + Name + ".mco", FileMode.Open, FileAccess.Read));
				}
				else
				{
					jis = new JarInputStream(url.openConnection().InputStream);
				}
				unpackConfigFile(jis);
				jis.close();
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("Could not unpack configuration. The configuration file '" + workingDirectory.Path + File.separator + Name + ".mco" + "' cannot be found. ", e);
			}
			catch (IOException e)
			{
				if (configDirectory.exists())
				{
					deleteConfigDirectory();
				}
				throw new ConfigurationException("Could not unpack configuration. ", e);
			}
			initCreatedByMaltParserVersionFromInfoFile();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void unpackConfigFile(java.util.jar.JarInputStream jis) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void unpackConfigFile(JarInputStream jis)
		{
			try
			{
				JarEntry je;
				sbyte[] readBuffer = new sbyte[BUFFER];
				SortedSet<string> directoryCache = new SortedSet<string>();
				while ((je = jis.NextJarEntry) != null)
				{
					string entryName = je.Name;

					if (entryName.StartsWith("/", StringComparison.Ordinal))
					{
						entryName = entryName.Substring(1);
					}
					if (entryName.EndsWith(File.separator, StringComparison.Ordinal) || entryName.EndsWith("/", StringComparison.Ordinal))
					{
						return;
					}
					int index = -1;
					if (File.separator.Equals("\\"))
					{
						entryName = entryName.Replace('/', '\\');
						index = entryName.LastIndexOf("\\", StringComparison.Ordinal);
					}
					else if (File.separator.Equals("/"))
					{
						entryName = entryName.Replace('\\', '/');
						index = entryName.LastIndexOf("/", StringComparison.Ordinal);
					}
					if (index > 0)
					{
						string dirName = entryName.Substring(0, index);
						if (!directoryCache.Contains(dirName))
						{
							File directory = new File(workingDirectory.Path + File.separator + dirName);
							if (!(directory.exists() && directory.Directory))
							{
								if (!directory.mkdirs())
								{
									throw new ConfigurationException("Unable to make directory '" + dirName + "'. ");
								}
								directoryCache.Add(dirName);
							}
						}
					}

					if (Directory.Exists(workingDirectory.Path + File.separator + entryName) && Directory.Exists(workingDirectory.Path + File.separator + entryName) || File.Exists(workingDirectory.Path + File.separator + entryName))
					{
						continue;
					}
					BufferedOutputStream bos;
					try
					{
						bos = new BufferedOutputStream(new FileStream(workingDirectory.Path + File.separator + entryName, FileMode.Create, FileAccess.Write), BUFFER);
					}
					catch (FileNotFoundException e)
					{
						throw new ConfigurationException("Could not unpack configuration. The file '" + workingDirectory.Path + File.separator + entryName + "' cannot be unpacked. ", e);
					}
					int n = 0;
					while ((n = jis.read(readBuffer, 0, BUFFER)) != -1)
					{
						bos.write(readBuffer, 0, n);
					}
					bos.flush();
					bos.close();
				}
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Could not unpack configuration. ", e);
			}
		}

		/// <summary>
		/// Returns the name of the configuration directory
		/// </summary>
		/// <returns> the name of the configuration directory </returns>
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}


		/// <summary>
		/// Returns the type of the configuration directory
		/// </summary>
		/// <returns> the type of the configuration directory </returns>
		public virtual string Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}


		/// <summary>
		/// Returns a file handler object for the working directory
		/// </summary>
		/// <returns> a file handler object for the working directory </returns>
		public virtual File WorkingDirectory
		{
			get
			{
				return workingDirectory;
			}
		}

		/// <summary>
		/// Initialize the working directory
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initWorkingDirectory() throws org.maltparser.core.exception.MaltChainedException
		public virtual void initWorkingDirectory()
		{
			try
			{
				initWorkingDirectory(OptionManager.instance().getOptionValue(containerIndex, "config", "workingdir").ToString());
			}
			catch (NullReferenceException e)
			{
				throw new ConfigurationException("The configuration cannot be found.", e);
			}
		}

		/// <summary>
		/// Initialize the working directory according to the path. If the path is equals to "user.dir" or current directory, then the current directory
		///  will be the working directory.
		/// </summary>
		/// <param name="pathPrefixString">	the path to the working directory </param>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initWorkingDirectory(String pathPrefixString) throws org.maltparser.core.exception.MaltChainedException
		public virtual void initWorkingDirectory(string pathPrefixString)
		{
			if (ReferenceEquals(pathPrefixString, null) || pathPrefixString.Equals("user.dir", StringComparison.OrdinalIgnoreCase) || pathPrefixString.Equals(".", StringComparison.OrdinalIgnoreCase))
			{
				workingDirectory = new File(System.getProperty("user.dir"));
			}
			else
			{
				workingDirectory = new File(pathPrefixString);
			}

			if (workingDirectory == null || !workingDirectory.Directory)
			{
				new ConfigurationException("The specified working directory '" + pathPrefixString + "' is not a directory. ");
			}
		}

		/// <summary>
		/// Returns the URL to the malt configuration file (.mco) 
		/// </summary>
		/// <returns> the URL to the malt configuration file (.mco) </returns>
		public virtual URL Url
		{
			get
			{
				return url;
			}
			set
			{
				url = value;
			}
		}


		/// <summary>
		/// Returns the option container index
		/// </summary>
		/// <returns> the option container index </returns>
		public virtual int ContainerIndex
		{
			get
			{
				return containerIndex;
			}
			set
			{
				containerIndex = value;
			}
		}


		/// <summary>
		/// Returns the version number of MaltParser which created the malt configuration file (.mco)
		/// </summary>
		/// <returns> the version number of MaltParser which created the malt configuration file (.mco) </returns>
		public virtual string CreatedByMaltParserVersion
		{
			get
			{
				return createdByMaltParserVersion;
			}
			set
			{
				createdByMaltParserVersion = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void initCreatedByMaltParserVersionFromInfoFile() throws org.maltparser.core.exception.MaltChainedException
		public virtual void initCreatedByMaltParserVersionFromInfoFile()
		{
			try
			{
				StreamReader br = new StreamReader(getInputStreamReaderFromConfigFileEntry(Name + "_" + Type + ".info", "UTF-8"));
				string line = null;
				while (!ReferenceEquals((line = br.ReadLine()), null))
				{
					if (line.StartsWith("Version:                       ", StringComparison.Ordinal))
					{
						CreatedByMaltParserVersion = line.Substring(31);
						break;
					}
				}
				br.Close();
			}
			catch (FileNotFoundException e)
			{
				throw new ConfigurationException("Could not retrieve the version number of the MaltParser configuration.", e);
			}
			catch (IOException e)
			{
				throw new ConfigurationException("Could not retrieve the version number of the MaltParser configuration.", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void versioning() throws org.maltparser.core.exception.MaltChainedException
		public virtual void versioning()
		{
			initCreatedByMaltParserVersionFromInfoFile();
			SystemLogger.logger().info("\nCurrent version      : " + SystemInfo.Version + "\n");
			SystemLogger.logger().info("Parser model version : " + createdByMaltParserVersion + "\n");
			if (ReferenceEquals(SystemInfo.Version, null))
			{
				throw new ConfigurationException("Couln't determine the version of MaltParser");
			}
			else if (ReferenceEquals(createdByMaltParserVersion, null))
			{
				throw new ConfigurationException("Couln't determine the version of the parser model");
			}
			else if (SystemInfo.Version.Equals(createdByMaltParserVersion))
			{
				SystemLogger.logger().info("The parser model " + Name + ".mco has already the same version as the current version of MaltParser. \n");
				return;
			}

			File mcoPath = new File(workingDirectory.Path + File.separator + Name + ".mco");
			File newMcoPath = new File(workingDirectory.Path + File.separator + Name + "." + SystemInfo.Version.Trim() + ".mco");
			Versioning versioning = new Versioning(name, type, mcoPath, createdByMaltParserVersion);
			if (!versioning.support(createdByMaltParserVersion))
			{
				SystemLogger.logger().warn("The parser model '" + name + ".mco' is created by MaltParser " + CreatedByMaltParserVersion + ", which cannot be converted to a MaltParser " + SystemInfo.Version + " parser model.\n");
				SystemLogger.logger().warn("Please retrain the parser model with MaltParser " + SystemInfo.Version + " or download MaltParser " + CreatedByMaltParserVersion + " from http://maltparser.org/download.html\n");
				return;
			}
			SystemLogger.logger().info("Converts the parser model '" + mcoPath.Name + "' into '" + newMcoPath.Name + "'....\n");
			copyConfigFile(mcoPath, newMcoPath, versioning);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkNConvertConfigVersion() throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual void checkNConvertConfigVersion()
		{
			if (createdByMaltParserVersion.StartsWith("1.0", StringComparison.Ordinal))
			{
				SystemLogger.logger().info("  Converts the MaltParser configuration ");
				SystemLogger.logger().info("1.0");
				SystemLogger.logger().info(" to ");
				SystemLogger.logger().info(SystemInfo.Version);
				SystemLogger.logger().info("\n");
				File[] configFiles = configDirectory.listFiles();
				for (int i = 0, n = configFiles.Length; i < n; i++)
				{
					if (configFiles[i].Name.EndsWith(".mod"))
					{
						configFiles[i].renameTo(new File(configDirectory.Path + File.separator + "odm0." + configFiles[i].Name));
					}
					if (configFiles[i].Name.EndsWith(Name + ".dsm"))
					{
						configFiles[i].renameTo(new File(configDirectory.Path + File.separator + "odm0.dsm"));
					}
					if (configFiles[i].Name.Equals("savedoptions.sop"))
					{
						configFiles[i].renameTo(new File(configDirectory.Path + File.separator + "savedoptions.sop.old"));
					}
					if (configFiles[i].Name.Equals("symboltables.sym"))
					{
						configFiles[i].renameTo(new File(configDirectory.Path + File.separator + "symboltables.sym.old"));
					}
				}
				try
				{
					StreamReader br = new StreamReader(configDirectory.Path + File.separator + "savedoptions.sop.old");
					StreamWriter bw = new StreamWriter(configDirectory.Path + File.separator + "savedoptions.sop");
					string line;
					while (!ReferenceEquals((line = br.ReadLine()), null))
					{
						if (line.StartsWith("0\tguide\tprediction_strategy", StringComparison.Ordinal))
						{
							bw.Write("0\tguide\tdecision_settings\tT.TRANS+A.DEPREL\n");
						}
						else
						{
							bw.Write(line);
							bw.BaseStream.WriteByte('\n');
						}
					}
					br.Close();
					bw.Flush();
					bw.Close();
					if (Directory.Exists(configDirectory.Path + File.separator + "savedoptions.sop.old")) Directory.Delete(configDirectory.Path + File.separator + "savedoptions.sop.old", true); else File.Delete(configDirectory.Path + File.separator + "savedoptions.sop.old");
				}
				catch (FileNotFoundException e)
				{
					throw new ConfigurationException("Could convert savedoptions.sop version 1.0.4 to version 1.1. ", e);
				}
				catch (IOException e)
				{
					throw new ConfigurationException("Could convert savedoptions.sop version 1.0.4 to version 1.1. ", e);
				}
				try
				{
					StreamReader br = new StreamReader(configDirectory.Path + File.separator + "symboltables.sym.old");
					StreamWriter bw = new StreamWriter(configDirectory.Path + File.separator + "symboltables.sym");
					string line;
					while (!ReferenceEquals((line = br.ReadLine()), null))
					{
						if (line.StartsWith("AllCombinedClassTable", StringComparison.Ordinal))
						{
							bw.Write("T.TRANS+A.DEPREL\n");
						}
						else
						{
							bw.Write(line);
							bw.BaseStream.WriteByte('\n');
						}
					}
					br.Close();
					bw.Flush();
					bw.Close();
					if (Directory.Exists(configDirectory.Path + File.separator + "symboltables.sym.old")) Directory.Delete(configDirectory.Path + File.separator + "symboltables.sym.old", true); else File.Delete(configDirectory.Path + File.separator + "symboltables.sym.old");
				}
				catch (FileNotFoundException e)
				{
					throw new ConfigurationException("Could convert symboltables.sym version 1.0.4 to version 1.1. ", e);
				}
				catch (IOException e)
				{
					throw new ConfigurationException("Could convert symboltables.sym version 1.0.4 to version 1.1. ", e);
				}
			}
			if (!createdByMaltParserVersion.StartsWith("1.3", StringComparison.Ordinal))
			{
				SystemLogger.logger().info("  Converts the MaltParser configuration ");
				SystemLogger.logger().info(createdByMaltParserVersion);
				SystemLogger.logger().info(" to ");
				SystemLogger.logger().info(SystemInfo.Version);
				SystemLogger.logger().info("\n");


				(new File(configDirectory.Path + File.separator + "savedoptions.sop")).renameTo(new File(configDirectory.Path + File.separator + "savedoptions.sop.old"));
				try
				{
					StreamReader br = new StreamReader(configDirectory.Path + File.separator + "savedoptions.sop.old");
					StreamWriter bw = new StreamWriter(configDirectory.Path + File.separator + "savedoptions.sop");
					string line;
					while (!ReferenceEquals((line = br.ReadLine()), null))
					{
						int index = line.IndexOf('\t');
						int container = 0;
						if (index > -1)
						{
							container = int.Parse(line.Substring(0,index));
						}

						if (line.StartsWith(container + "\tnivre\tpost_processing", StringComparison.Ordinal))
						{
						}
						else if (line.StartsWith(container + "\tmalt0.4\tbehavior", StringComparison.Ordinal))
						{
							if (line.EndsWith("true", StringComparison.Ordinal))
							{
								SystemLogger.logger().info("MaltParser 1.3 doesn't support MaltParser 0.4 emulation.");
								br.Close();
								bw.Flush();
								bw.Close();
								deleteConfigDirectory();
								Environment.Exit(0);
							}
						}
						else if (line.StartsWith(container + "\tsinglemalt\tparsing_algorithm", StringComparison.Ordinal))
						{
							bw.BaseStream.WriteByte(container);
							bw.Write("\tsinglemalt\tparsing_algorithm\t");
							if (line.EndsWith("NivreStandard", StringComparison.Ordinal))
							{
								bw.Write("class org.maltparser.parser.algorithm.nivre.NivreArcStandardFactory");
							}
							else if (line.EndsWith("NivreEager", StringComparison.Ordinal))
							{
								bw.Write("class org.maltparser.parser.algorithm.nivre.NivreArcEagerFactory");
							}
							else if (line.EndsWith("CovingtonNonProjective", StringComparison.Ordinal))
							{
								bw.Write("class org.maltparser.parser.algorithm.covington.CovingtonNonProjFactory");
							}
							else if (line.EndsWith("CovingtonProjective", StringComparison.Ordinal))
							{
								bw.Write("class org.maltparser.parser.algorithm.covington.CovingtonProjFactory");
							}
							bw.BaseStream.WriteByte('\n');
						}
						else
						{
							bw.Write(line);
							bw.BaseStream.WriteByte('\n');
						}
					}
					br.Close();
					bw.Flush();
					bw.Close();
					if (Directory.Exists(configDirectory.Path + File.separator + "savedoptions.sop.old")) Directory.Delete(configDirectory.Path + File.separator + "savedoptions.sop.old", true); else File.Delete(configDirectory.Path + File.separator + "savedoptions.sop.old");
				}
				catch (FileNotFoundException e)
				{
					throw new ConfigurationException("Could convert savedoptions.sop version 1.0.4 to version 1.1. ", e);
				}
				catch (IOException e)
				{
					throw new ConfigurationException("Could convert savedoptions.sop version 1.0.4 to version 1.1. ", e);
				}
			}
		}

		/// <summary>
		/// Terminates the configuration directory
		/// </summary>
		/// <exception cref="MaltChainedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void terminate() throws org.maltparser.core.exception.MaltChainedException
		public virtual void terminate()
		{
			if (infoFile != null)
			{
				try
				{
					infoFile.Flush();
					infoFile.Close();
				}
				catch (IOException e)
				{
					throw new ConfigurationException("Could not close configuration information file. ", e);
				}
			}
			symbolTables = null;
	//		configuration = null;
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#finalize()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws Throwable
		~ConfigurationDir()
		{
			try
			{
				if (infoFile != null)
				{
					infoFile.Flush();
					infoFile.Close();
				}
			}
			finally
			{
//JAVA TO C# CONVERTER NOTE: The base class finalizer method is automatically called in C#:
//				base.finalize();
			}
		}

		public virtual SymbolTableHandler SymbolTables
		{
			get
			{
				return symbolTables;
			}
			set
			{
				symbolTables = value;
			}
		}


		public virtual DataFormatManager DataFormatManager
		{
			get
			{
				return dataFormatManager;
			}
			set
			{
				dataFormatManager = value;
			}
		}


		public virtual ISet<string> DataFormatInstanceKeys
		{
			get
			{
				return dataFormatInstances.Keys;
			}
		}

		public virtual bool addDataFormatInstance(string key, DataFormatInstance dataFormatInstance)
		{
			if (!dataFormatInstances.ContainsKey(key))
			{
				dataFormatInstances[key] = dataFormatInstance;
				return true;
			}
			return false;
		}

		public virtual DataFormatInstance getDataFormatInstance(string key)
		{
			return dataFormatInstances[key];
		}

		public virtual int sizeDataFormatInstance()
		{
			return dataFormatInstances.Count;
		}

		public virtual DataFormatInstance InputDataFormatInstance
		{
			get
			{
				return dataFormatInstances[dataFormatManager.InputDataFormatSpec.DataFormatName];
			}
		}

		public virtual URL InputFormatURL
		{
			get
			{
				return inputFormatURL;
			}
		}

		public virtual URL OutputFormatURL
		{
			get
			{
				return outputFormatURL;
			}
		}


	}

}