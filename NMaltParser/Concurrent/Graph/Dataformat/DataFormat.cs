using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NMaltParser.Core.Helper;
using NMaltParser.Core.SyntaxGraph;
using NMaltParser.Utilities;

namespace NMaltParser.Concurrent.Graph.DataFormat
{
    /// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public class DataFormat
    {
        private readonly ColumnDescription[] columns;

        private readonly Dictionary<string, ColumnDescription> columnMap;

        public DataFormat(DataFormat dataFormat)
        {
            Name = dataFormat.Name;

            columns = Arrays.CopyOf(dataFormat.columns, 0);

            columnMap = columns.ToDictionary(description => description.Name, description => description);
        }

        public DataFormat(string name, IEnumerable<ColumnDescription> columns)
        {
            Name = name;

            this.columns = Arrays.CopyOf(columns, 0);

            columnMap = this.columns.ToDictionary(description => description.Name, description => description);
        }

        public virtual string Name { get; }

        public virtual ColumnDescription GetColumnDescription(int position)
        {
            return columns[position];
        }

        public virtual ColumnDescription GetColumnDescription(string columnName)
        {
            ColumnDescription columnDescription = columnMap[columnName];

            if (columnDescription != null)
            {
                return columnDescription;
            }

            foreach (ColumnDescription description in columns)
            {
                if (description.Name.Equals(columnName.ToUpper()))
                {
                    columnMap[columnName] = description;

                    return description;
                }
            }

            return null;
        }

        public virtual ISet<ColumnDescription> GetSelectedColumnDescriptions(ISet<int> positionSet)
        {
            ISet<ColumnDescription> selectedColumns = Collections.SynchronizedSortedSet(new SortedSet<ColumnDescription>());

            foreach (ColumnDescription description in columns)
            {
                if (positionSet.Contains(description.Position))
                {
                    selectedColumns.Add(description);
                }
            }

            return selectedColumns;
        }

        public virtual ISet<string> LabelNames
        {
            get
            {
                ISet<string> labelNames = Collections.SynchronizedSet(new System.Collections.Generic.HashSet<string>());

                foreach (ColumnDescription description in columns)
                {
                    labelNames.Add(description.Name);
                }

                return labelNames;
            }
        }

        public virtual int NumberOfColumns()
        {
            return columns.Length;
        }

        public override int GetHashCode()
        {
            const int prime = 31;

            int result = 1;

            result = prime * result + Arrays.HashCode(columns);

            result = prime * result + (Name?.HashCode() ?? 0);

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

            DataFormat other = (DataFormat)obj;

            if (columns != other.columns)
            {
                return false;
            }
            if (Name == null)
            {
                if (other.Name != null)
                {
                    return false;
                }
            }
            else if (!Name.Equals(other.Name))
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Name);

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
        public static DataFormat ParseDataFormatXmLFile(string fileName)
        {
            return ParseDataFormatXmLFile(new UrlFinder().FindUrl(fileName));
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static DataFormat parseDataFormatXMLfile(java.net.URL url) throws org.maltparser.concurrent.graph.ConcurrentGraphException
        public static DataFormat ParseDataFormatXmLFile(Uri url)
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
                    ColumnDescription column = new ColumnDescription(i, col.getAttribute("name"), ColumnDescription.GetCategory(col.getAttribute("category")), ColumnDescription.GetType(col.getAttribute("type")), col.getAttribute("default"), false);
                    columns.Add(column);
                }
                columns.Add(new ColumnDescription(i++, "PPPATH", ColumnDescription.DependencyEdgeLabel, ColumnDescription.String, "_", true));
                columns.Add(new ColumnDescription(i++, "PPLIFTED", ColumnDescription.DependencyEdgeLabel, ColumnDescription.String, "_", true));
                columns.Add(new ColumnDescription(i++, "PPCOVERED", ColumnDescription.DependencyEdgeLabel, ColumnDescription.String, "_", true));
            }
            catch (IOException e)
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