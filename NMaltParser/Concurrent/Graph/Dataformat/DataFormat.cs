using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
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

        private Lazy<ISet<ColumnDescription>> selectedColumns;

        public virtual ISet<ColumnDescription> GetSelectedColumnDescriptions(ISet<int> positionSet)
        {
            if (selectedColumns == null)
            {
                selectedColumns = new Lazy<ISet<ColumnDescription>>(() =>
                {
                    SortedSet<ColumnDescription> sortedSet = new SortedSet<ColumnDescription>();

                    foreach (ColumnDescription description in columns)
                    {
                        if (positionSet.Contains(description.Position))
                        {
                            sortedSet.Add(description);
                        }
                    }

                    return sortedSet;

                }, true);
            }

            return selectedColumns.Value;
        }

        private Lazy<ISet<string>> labelNames;

        public virtual ISet<string> LabelNames
        {
            get
            {
                if (labelNames == null)
                {
                    labelNames = new Lazy<ISet<string>>(() =>
                    {
                        HashSet<string> hashSet = new HashSet<string>();

                        foreach (ColumnDescription description in columns)
                        {
                            hashSet.Add(description.Name);
                        }

                        return hashSet;
                    });
                }

                return labelNames.Value;
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

        public static DataFormat ParseDataFormatXmLFile(string fileName)
        {
            return ParseDataFormatXmLFile(new FileInfo(fileName));
        }

        public static DataFormat ParseDataFormatXmLFile(FileInfo fileInfo)
        {
            if (fileInfo == null || !fileInfo.Exists)
            {
                throw new ConcurrentGraphException("The data format specification file cannot be found.");
            }

            string dataFormatName;

            List<ColumnDescription> columns = new List<ColumnDescription>();

            XmlDocument document = new XmlDocument();

            document.Load(fileInfo.FullName);

            XmlElement root = document.DocumentElement;

            if (root.Name.Equals("dataformat"))
            {
                dataFormatName = root.GetAttribute("name");
            }
            else
            {
                throw new ConcurrentGraphException("Data format specification file must contain one 'dataformat' element. ");
            }

            XmlNodeList cols = root.GetElementsByTagName("column");

            int i = 0;

            for (; i < cols.Count; i++)
            {
                XmlElement col = (XmlElement)cols[i];

                ColumnDescription column = new ColumnDescription(i, col.GetAttribute("name"), ColumnDescription.GetCategory(col.GetAttribute("category")), ColumnDescription.GetType(col.GetAttribute("type")), col.GetAttribute("default"), false);

                columns.Add(column);
            }

            columns.Add(new ColumnDescription(i++, "PPPATH", ColumnDescription.DependencyEdgeLabel, ColumnDescription.String, "_", true));

            columns.Add(new ColumnDescription(i++, "PPLIFTED", ColumnDescription.DependencyEdgeLabel, ColumnDescription.String, "_", true));

            columns.Add(new ColumnDescription(i, "PPCOVERED", ColumnDescription.DependencyEdgeLabel, ColumnDescription.String, "_", true));

            return new DataFormat(dataFormatName, columns);
        }
    }

}