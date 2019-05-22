using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.lw.parser
{

	using HashMap = org.maltparser.core.helper.HashMap;
	using HashSet = org.maltparser.core.helper.HashSet;

	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class McoModel
	{
		private readonly URL mcoUrl;
		private readonly IDictionary<string, URL> nameUrlMap;
		private readonly IDictionary<string, object> preLoadedObjects;
		private readonly IDictionary<string, string> preLoadedStrings;
		private readonly URL infoURL;
		private readonly string internalMcoName;


		public McoModel(URL _mcoUrl)
		{
			this.mcoUrl = _mcoUrl;
			this.nameUrlMap = Collections.synchronizedMap(new HashMap<string, URL>());
			this.preLoadedObjects = Collections.synchronizedMap(new HashMap<string, object>());
			this.preLoadedStrings = Collections.synchronizedMap(new HashMap<string, string>());
			URL tmpInfoURL = null;
			string tmpInternalMcoName = null;
			try
			{
				JarEntry je;
				JarInputStream jis = new JarInputStream(mcoUrl.openConnection().InputStream);

				while ((je = jis.NextJarEntry) != null)
				{
					string fileName = je.Name;
					URL entryURL = new URL("jar:" + mcoUrl + "!/" + fileName + "\n");
					int index = fileName.IndexOf('/');
					if (index == -1)
					{
						index = fileName.IndexOf('\\');
					}
					nameUrlMap[fileName.Substring(index + 1)] = entryURL;
					if (fileName.EndsWith(".info", StringComparison.Ordinal) && tmpInfoURL == null)
					{
						tmpInfoURL = entryURL;
					}
					else if (fileName.EndsWith(".moo", StringComparison.Ordinal) || fileName.EndsWith(".map", StringComparison.Ordinal))
					{
						preLoadedObjects[fileName.Substring(index + 1)] = preLoadObject(entryURL.openStream());
					}
					else if (fileName.EndsWith(".dsm", StringComparison.Ordinal))
					{
						preLoadedStrings[fileName.Substring(index + 1)] = preLoadString(entryURL.openStream());
					}
					if (string.ReferenceEquals(tmpInternalMcoName, null))
					{
						tmpInternalMcoName = fileName.Substring(0, index);
					}
					jis.closeEntry();
				}
				jis.close();
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (ClassNotFoundException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			this.internalMcoName = tmpInternalMcoName;
			this.infoURL = tmpInfoURL;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object preLoadObject(java.io.InputStream is) throws java.io.IOException, ClassNotFoundException
		private object preLoadObject(Stream @is)
		{
			object @object = null;

			ObjectInputStream input = new ObjectInputStream(@is);
			try
			{
				@object = input.readObject();
			}
			finally
			{
				input.close();
			}
			return @object;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String preLoadString(java.io.InputStream is) throws java.io.IOException, ClassNotFoundException
		private string preLoadString(Stream @is)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader in = new java.io.BufferedReader(new java.io.InputStreamReader(is, "UTF-8"));
			StreamReader @in = new StreamReader(@is, Encoding.UTF8);
			string line;
			StringBuilder sb = new StringBuilder();

			while (!string.ReferenceEquals((line = @in.ReadLine()), null))
			{
				 sb.Append(line);
				 sb.Append('\n');
			}
			return sb.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStream getInputStream(String fileName) throws java.io.IOException
		public Stream getInputStream(string fileName)
		{
			return nameUrlMap[fileName].openStream();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.InputStreamReader getInputStreamReader(String fileName, String charSet) throws java.io.IOException, java.io.UnsupportedEncodingException
		public StreamReader getInputStreamReader(string fileName, string charSet)
		{
			return new StreamReader(getInputStream(fileName), charSet);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getMcoEntryURL(String fileName) throws java.net.MalformedURLException
		public URL getMcoEntryURL(string fileName)
		{
			return new URL(nameUrlMap[fileName].ToString());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.net.URL getMcoURL() throws java.net.MalformedURLException
		public URL McoURL
		{
			get
			{
				return new URL(mcoUrl.ToString());
			}
		}

		public object getMcoEntryObject(string fileName)
		{
			return preLoadedObjects[fileName];
		}

		public ISet<string> McoEntryObjectKeys
		{
			get
			{
				return Collections.synchronizedSet(new HashSet<string>(preLoadedObjects.Keys));
			}
		}

		public string getMcoEntryString(string fileName)
		{
			return preLoadedStrings[fileName];
		}

		public string InternalName
		{
			get
			{
				return internalMcoName;
			}
		}

		public string McoURLString
		{
			get
			{
				return mcoUrl.ToString();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getMcoInfo() throws java.io.IOException
		public string McoInfo
		{
			get
			{
				StringBuilder sb = new StringBuilder();
    
				StreamReader reader = new StreamReader(infoURL.openStream(), Encoding.UTF8);
				string line;
				while (!string.ReferenceEquals((line = reader.ReadLine()), null))
				{
					sb.Append(line);
					sb.Append('\n');
				}
    
				return sb.ToString();
			}
		}
	}

}