using System.Collections.Generic;
using System.Text;

namespace org.maltparser.concurrent.graph.dataformat
{
    using  core.helper;

    /// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class DataFormat
	{
		private readonly string name;
		private readonly ColumnDescription[] columns;
		private readonly HashMap<string, ColumnDescription> columnMap;

		public DataFormat(DataFormat dataFormat)
		{
			name = dataFormat.name;
			columns = new ColumnDescription[dataFormat.columns.Length];
			columnMap = new HashMap<string, ColumnDescription>();
			for (int i = 0; i < dataFormat.columns.Length; i++)
			{
				columns[i] = new ColumnDescription(dataFormat.columns[i]);
				columnMap[columns[i].Name] = columns[i];
			}
		}

		public DataFormat(string name, ColumnDescription[] columns)
		{
			this.name = name;
			this.columns = new ColumnDescription[columns.Length];
			columnMap = new HashMap<string, ColumnDescription>();
			for (int i = 0; i < columns.Length; i++)
			{
				this.columns[i] = new ColumnDescription(columns[i]);
				columnMap[this.columns[i].Name] = this.columns[i];
			}
		}

		public DataFormat(string name, List<ColumnDescription> columns)
		{
			this.name = name;
			this.columns = new ColumnDescription[columns.Count];
			columnMap = new HashMap<string, ColumnDescription>();
			for (int i = 0; i < columns.Count; i++)
			{
				this.columns[i] = new ColumnDescription(columns[i]);
				columnMap[this.columns[i].Name] = this.columns[i];
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual ColumnDescription getColumnDescription(int position)
		{
			return columns[position];
		}

		public virtual ColumnDescription getColumnDescription(string columnName)
		{
			ColumnDescription columnDescription = columnMap[columnName];
			if (columnDescription != null)
			{
				return columnDescription;
			}

			for (int i = 0; i < columns.Length; i++)
			{
				if (columns[i].Name.Equals(columnName.ToUpper()))
				{
					columnMap[columnName] = columns[i];
					return columns[i];
				}
			}
			return null;
		}

		public virtual SortedSet<ColumnDescription> getSelectedColumnDescriptions(ISet<int> positionSet)
		{
			SortedSet<ColumnDescription> selectedColumns = Collections.synchronizedSortedSet(new SortedSet<ColumnDescription>());
			for (int i = 0; i < columns.Length; i++)
			{
				if (positionSet.Contains(columns[i].Position))
				{
					selectedColumns.Add(columns[i]);
				}
			}
			return selectedColumns;
		}

		public virtual ISet<string> LabelNames
		{
			get
			{
				ISet<string> labelNames = Collections.synchronizedSet(new System.Collections.Generic.HashSet<string>());
				for (int i = 0; i < columns.Length; i++)
				{
					labelNames.Add(columns[i].Name);
				}
				return labelNames;
			}
		}

		public virtual int numberOfColumns()
		{
			return columns.Length;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + Arrays.GetHashCode(columns);
			result = prime * result + ((ReferenceEquals(name, null)) ? 0 : name.GetHashCode());
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			DataFormat other = (DataFormat) obj;
			if (!Arrays.Equals(columns, other.columns))
			{
				return false;
			}
			if (ReferenceEquals(name, null))
			{
				if (!ReferenceEquals(other.name, null))
				{
					return false;
				}
			}
			else if (!name.Equals(other.name))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(name);
			sb.Append('\n');
			for (int i = 1; i < columns.Length; i++)
			{
				sb.Append(columns[i]);
				sb.Append('\n');
			}

			return sb.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DataFormat parseDataFormatXMLfile(String fileName) throws org.maltparser.core.exception.MaltChainedException
		public static DataFormat parseDataFormatXMLfile(string fileName)
		{
			return parseDataFormatXMLfile((new URLFinder()).findURL(fileName));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DataFormat parseDataFormatXMLfile(java.net.URL url) throws org.maltparser.concurrent.graph.ConcurrentGraphException
		public static DataFormat parseDataFormatXMLfile(URL url)
		{
			if (url == null)
			{
				throw new ConcurrentGraphException("The data format specification file cannot be found. ");
			}
			string dataFormatName;
			List<ColumnDescription> columns = new List<ColumnDescription>();
			try
			{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();

				Element root = db.parse(url.openStream()).DocumentElement;
				if (root.NodeName.Equals("dataformat"))
				{
					dataFormatName = root.getAttribute("name");
				}
				else
				{
					throw new ConcurrentGraphException("Data format specification file must contain one 'dataformat' element. ");
				}
				NodeList cols = root.getElementsByTagName("column");
				Element col = null;
				int i = 0;
				for (; i < cols.Length; i++)
				{
					col = (Element)cols.item(i);
					ColumnDescription column = new ColumnDescription(i, col.getAttribute("name"), ColumnDescription.getCategory(col.getAttribute("category")), ColumnDescription.getType(col.getAttribute("type")), col.getAttribute("default"), false);
					columns.Add(column);
				}
				columns.Add(new ColumnDescription(i++, "PPPATH", ColumnDescription.DEPENDENCY_EDGE_LABEL, ColumnDescription.STRING, "_", true));
				columns.Add(new ColumnDescription(i++, "PPLIFTED", ColumnDescription.DEPENDENCY_EDGE_LABEL, ColumnDescription.STRING, "_", true));
				columns.Add(new ColumnDescription(i++, "PPCOVERED", ColumnDescription.DEPENDENCY_EDGE_LABEL, ColumnDescription.STRING, "_", true));
			}
			catch (java.io.IOException e)
			{
				throw new ConcurrentGraphException("Cannot find the file " + url.ToString() + ". ", e);
			}
			catch (ParserConfigurationException e)
			{
				throw new ConcurrentGraphException("Problem parsing the file " + url.ToString() + ". ", e);
			}
			catch (SAXException e)
			{
				throw new ConcurrentGraphException("Problem parsing the file " + url.ToString() + ". ", e);
			}
			return new DataFormat(dataFormatName, columns);
		}
	}

}