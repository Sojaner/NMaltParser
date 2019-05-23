using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Helper;
using NMaltParser.Core.Symbol;

namespace NMaltParser.Core.IO.DataFormat
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class DataFormatSpecification
	{
		public enum DataStructure
		{
			DEPENDENCY, // Dependency structure
			PHRASE, // Phrase structure
		}
	//	private int entryPositionCounter;
		private string dataFormatName;
		private DataStructure dataStructure;
		private readonly IDictionary<string, DataFormatEntry> entries;
		private readonly Helper.HashSet<Dependency> dependencies;
	//	private final HashSet<SyntaxGraphReader> supportedReaders;
	//	private final HashSet<SyntaxGraphWriter> supportedWriters;

		public DataFormatSpecification()
		{
			entries = new LinkedHashMap<string, DataFormatEntry>();
	//		entryPositionCounter = 0;
			dependencies = new Helper.HashSet<Dependency>();
	//		supportedReaders = new HashSet<SyntaxGraphReader>();
	//		supportedWriters = new HashSet<SyntaxGraphWriter>();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DataFormatInstance createDataFormatInstance(org.maltparser.core.symbol.SymbolTableHandler symbolTables, String nullValueStrategy) throws org.maltparser.core.exception.MaltChainedException
		public virtual DataFormatInstance createDataFormatInstance(SymbolTableHandler symbolTables, string nullValueStrategy)
		{
			return new DataFormatInstance(entries, symbolTables, nullValueStrategy, this); //rootLabel, this);

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseDataFormatXMLfile(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parseDataFormatXMLfile(string fileName)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.maltparser.core.helper.URLFinder f = new org.maltparser.core.helper.URLFinder();
			UrlFinder f = new UrlFinder();
			URL url = f.FindUrl(fileName);
			if (url == null)
			{
				throw new DataFormatException("The data format specifcation file '" + fileName + "'cannot be found. ");
			}
			parseDataFormatXMLfile(url);
		}

		public virtual Helper.HashSet<Dependency> Dependencies
		{
			get
			{
				return dependencies;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parseDataFormatXMLfile(java.net.URL url) throws org.maltparser.core.exception.MaltChainedException
		public virtual void parseDataFormatXMLfile(URL url)
		{
			if (url == null)
			{
				throw new DataFormatException("The data format specifcation file cannot be found. ");
			}

			try
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();

				Element root = db.parse(url.openStream()).DocumentElement;
				if (root.NodeName.Equals("dataformat"))
				{
					dataFormatName = root.getAttribute("name");
					if (root.getAttribute("datastructure").length() > 0)
					{
						dataStructure = Enum.Parse(typeof(DataStructure), root.getAttribute("datastructure").ToUpper());
					}
					else
					{
						dataStructure = DataStructure.DEPENDENCY;
					}
				}
				else
				{
					throw new DataFormatException("Data format specification file must contain one 'dataformat' element. ");
				}
				NodeList cols = root.getElementsByTagName("column");
				Element col = null;
				for (int i = 0, n = cols.Length; i < n; i++)
				{
					col = (Element)cols.item(i);
					DataFormatEntry entry = new DataFormatEntry(col.getAttribute("name"), col.getAttribute("category"),col.getAttribute("type"), col.getAttribute("default"));
					entries[entry.DataFormatEntryName] = entry;
				}
				NodeList deps = root.getElementsByTagName("dependencies");
				if (deps.Length > 0)
				{
					NodeList dep = ((Element)deps.item(0)).getElementsByTagName("dependency");
					for (int i = 0, n = dep.Length; i < n; i++)
					{
						Element e = (Element)dep.item(i);
						dependencies.add(new Dependency(this, e.getAttribute("name"), e.getAttribute("url"), e.getAttribute("map"), e.getAttribute("urlmap")));
					}
				}
			}
			catch (java.io.IOException e)
			{
				throw new DataFormatException("Cannot find the file " + url.ToString() + ". ", e);
			}
			catch (ParserConfigurationException e)
			{
				throw new DataFormatException("Problem parsing the file " + url.ToString() + ". ", e);
			}
			catch (SAXException e)
			{
				throw new DataFormatException("Problem parsing the file " + url.ToString() + ". ", e);
			}
		}

		public virtual void addEntry(string dataFormatEntryName, string category, string type, string defaultOutput)
		{
			DataFormatEntry entry = new DataFormatEntry(dataFormatEntryName, category, type, defaultOutput);
			entries[entry.DataFormatEntryName] = entry;
		}

		public virtual DataFormatEntry getEntry(string dataFormatEntryName)
		{
			return entries[dataFormatEntryName];
		}

		public virtual string DataFormatName
		{
			get
			{
				return dataFormatName;
			}
		}

		public virtual DataStructure getDataStructure()
		{
			return dataStructure;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append("Data format specification: ");
			sb.Append(dataFormatName);
			sb.Append('\n');
			foreach (DataFormatEntry dfe in entries.Values)
			{
				sb.Append(dfe);
				sb.Append('\n');
			}
			return sb.ToString();
		}

		public class Dependency
		{
			private readonly DataFormatSpecification outerInstance;

			protected internal string dependentOn;
			protected internal string urlString;
			protected internal string map;
			protected internal string mapUrl;

			public Dependency(DataFormatSpecification outerInstance, string dependentOn, string urlString, string map, string mapUrl)
			{
				this.outerInstance = outerInstance;
				DependentOn = dependentOn;
				UrlString = urlString;
				Map = map;
				MapUrl = mapUrl;
			}

			public virtual string DependentOn
			{
				get
				{
					return dependentOn;
				}
				set
				{
					dependentOn = value;
				}
			}

			public virtual string UrlString
			{
				get
				{
					return urlString;
				}
				set
				{
					urlString = value;
				}
			}


			public virtual string Map
			{
				get
				{
					return map;
				}
				set
				{
					map = value;
				}
			}

			public virtual string MapUrl
			{
				get
				{
					return mapUrl;
				}
				set
				{
					mapUrl = value;
				}
			}


			public override string ToString()
			{
				return "Dependency [dependentOn=" + dependentOn + ", map=" + map + ", mapUrl=" + mapUrl + ", urlString=" + urlString + "]";
			}
		}
	}

}