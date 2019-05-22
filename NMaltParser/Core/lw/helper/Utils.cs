using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace org.maltparser.core.lw.helper
{
    using  io.dataformat;
    using  symbol;
	using  syntaxgraph;
	using  syntaxgraph.node;

	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class Utils
	{
		public static JarFile getConfigJarfile(URL url)
		{
			JarFile mcoFile = null;
			try
			{
				JarURLConnection conn = (JarURLConnection)(new URL("jar:" + url.ToString() + "!/")).openConnection();
				mcoFile = conn.JarFile;
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			return mcoFile;
		}

		public static JarEntry getConfigFileEntry(JarFile mcoJarFile, string mcoName, string fileName)
		{

			JarEntry entry = mcoJarFile.getJarEntry(mcoName + '/' + fileName);
			if (entry == null)
			{
				entry = mcoJarFile.getJarEntry(mcoName + '\\' + fileName);
			}
			return entry;
		}

		public static Stream getInputStreamFromConfigFileEntry(URL mcoURL, string mcoName, string fileName)
		{
			JarFile mcoJarFile = getConfigJarfile(mcoURL);
			JarEntry entry = getConfigFileEntry(mcoJarFile, mcoName, fileName);

			try
			{
			  if (entry == null)
			  {
				  throw new FileNotFoundException();
			  }
			  return mcoJarFile.getInputStream(entry);
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return null;
		}

		public static StreamReader getInputStreamReaderFromConfigFileEntry(URL mcoURL, string mcoName, string fileName, string charSet)
		{
			try
			{
					return new StreamReader(getInputStreamFromConfigFileEntry(mcoURL, mcoName, fileName), charSet);
			}
			catch (UnsupportedEncodingException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return null;
		}

		public static URL getConfigFileEntryURL(URL mcoURL, string mcoName, string fileName)
		{
			try
			{
				URL url = new URL("jar:" + mcoURL.ToString() + "!/" + mcoName + '/' + fileName + "\n");
				try
				{
					Stream @is = url.openStream();
					@is.Close();
				}
				catch (IOException)
				{
					url = new URL("jar:" + mcoURL.ToString() + "!/" + mcoName + '\\' + fileName + "\n");
				}
				return url;
			}
			catch (MalformedURLException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return null;
		}

		public static string getInternalParserModelName(URL mcoUrl)
		{
			string internalParserModelName = null;
			try
			{
				JarEntry je;
				JarInputStream jis = new JarInputStream(mcoUrl.openConnection().InputStream);

				while ((je = jis.NextJarEntry) != null)
				{
					string fileName = je.Name;
					jis.closeEntry();
					int index = fileName.IndexOf('/');
					if (index == -1)
					{
						index = fileName.IndexOf('\\');
					}
					if (ReferenceEquals(internalParserModelName, null))
					{
						internalParserModelName = fileName.Substring(0, index);
						break;
					}
				}
				jis.close();
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return internalParserModelName;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String[] toStringArray(org.maltparser.core.syntaxgraph.DependencyGraph graph, org.maltparser.core.io.dataformat.DataFormatInstance dataFormatInstance, org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public static string[] toStringArray(DependencyGraph graph, DataFormatInstance dataFormatInstance, SymbolTableHandler symbolTables)
		{
			string[] tokens = new string[graph.nTokenNode()];
			StringBuilder sb = new StringBuilder();
			IEnumerator<ColumnDescription> columns = dataFormatInstance.GetEnumerator();
			foreach (int? index in graph.TokenIndices)
			{
				sb.Length = 0;
				if (index <= tokens.Length)
				{
					ColumnDescription column = null;
					TokenNode node = graph.getTokenNode(index.Value);
					while (columns.MoveNext())
					{
						column = columns.Current;
						if (column.Category == ColumnDescription.INPUT)
						{
							if (!column.Name.Equals("ID"))
							{
								if (node.hasLabel(symbolTables.getSymbolTable(column.Name)) && node.getLabelSymbol(symbolTables.getSymbolTable(column.Name)).Length > 0)
								{
									sb.Append(node.getLabelSymbol(symbolTables.getSymbolTable(column.Name)));
								}
								else
								{
									sb.Append('_');
								}
							}
							else
							{
								sb.Append(index.ToString());
							}
							sb.Append('\t');
						}
					}
					sb.Length = sb.Length - 1;
					tokens[index - 1] = sb.ToString();
					columns = dataFormatInstance.GetEnumerator();
				}
			}
			return tokens;
		}
	}

}