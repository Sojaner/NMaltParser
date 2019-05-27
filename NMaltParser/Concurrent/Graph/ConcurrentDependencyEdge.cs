using System;
using System.Collections.Generic;
using NMaltParser.Concurrent.Graph.DataFormat;

namespace NMaltParser.Concurrent.Graph
{
    /// <inheritdoc />
    /// <summary>
    /// Immutable and tread-safe dependency edge implementation.
    /// @author Johan Hall
    /// </summary>
	public sealed class ConcurrentDependencyEdge : IComparable<ConcurrentDependencyEdge>
    {
        private readonly SortedDictionary<int, string> labels;

        internal ConcurrentDependencyEdge(ConcurrentDependencyEdge edge)
        {
            Source = edge.Source;

            Target = edge.Target;

            labels = new SortedDictionary<int, string>(edge.labels);
        }

        internal ConcurrentDependencyEdge(DataFormat.DataFormat dataFormat, ConcurrentDependencyNode source, ConcurrentDependencyNode target, SortedDictionary<int, string> labels)
        {
            Source = source ?? throw new ConcurrentGraphException("Not allowed to have an edge without a source node");

            Target = target ?? throw new ConcurrentGraphException("Not allowed to have an edge without a target node");

            if (Target.Index == 0)
            {
                throw new ConcurrentGraphException("Not allowed to have an edge target as root node");
            }

            this.labels = new SortedDictionary<int, string>();

            if (labels != null)
            {
                foreach (int i in labels.Keys)
                {
                    if (dataFormat.GetColumnDescription(i).Category == ColumnDescription.DependencyEdgeLabel)
                    {
                        this.labels[i] = labels[i];
                    }
                }
            }
        }

        /// <summary>
        /// Returns the source node of the edge.
        /// </summary>
        /// <returns> the source node of the edge. </returns>
        public ConcurrentDependencyNode Source { get; }

        /// <summary>
		/// Returns the target node of the edge.
		/// </summary>
		/// <returns> the target node of the edge. </returns>
		public ConcurrentDependencyNode Target { get; }

        /// <summary>
		/// Returns an edge label
		/// </summary>
		/// <param name="column"> a column description that describes the label </param>
		/// <returns> an edge label described by the column description. An empty string is returned if the label is not found.  </returns>
		public string GetLabel(ColumnDescription column)
        {
            if (labels.ContainsKey(column.Position))
            {
                return labels[column.Position];
            }
            else if (column.Category == ColumnDescription.Ignore)
            {
                return column.DefaultOutput;
            }
            return "";
        }

        /// <summary>
        /// Returns an edge label
        /// </summary>
        /// <param name="columnName"> the name of the column that describes the label. </param>
        /// <returns> an edge label. An empty string is returned if the label is not found. </returns>
        public string GetLabel(string columnName)
        {
            ColumnDescription column = Source.DataFormat.getColumnDescription(columnName);

            if (column != null)
            {
                if (labels.ContainsKey(column.Position))
                {
                    return labels[column.Position];
                }
                else if (column.Category == ColumnDescription.Ignore)
                {
                    return column.DefaultOutput;
                }
            }

            return "";
        }

        /// <summary>
        /// Returns the number of labels of the edge.
        /// </summary>
        /// <returns> the number of labels of the edge. </returns>
        public int NLabels()
        {
            return labels.Count;
        }

        /// <summary>
        /// Returns <i>true</i> if the edge has one or more labels, otherwise <i>false</i>.
        /// </summary>
        /// <returns> <i>true</i> if the edge has one or more labels, otherwise <i>false</i>. </returns>
        public bool Labeled => labels.Count > 0;

        public int CompareTo(ConcurrentDependencyEdge that)
        {
            const int before = -1;

            const int equal = 0;

            const int after = 1;

            if (Equals(this, that))
            {
                return equal;
            }

            if (Target.Index < that.Target.Index)
            {
                return before;
            }

            if (Target.Index > that.Target.Index)
            {
                return after;
            }

            if (Source.Index < that.Source.Index)
            {
                return before;
            }

            if (Source.Index > that.Source.Index)
            {
                return after;
            }


            if (labels.Equals(that.labels))
            {
                return equal;
            }

            using (IEnumerator<int> thisEnumerator = labels.Keys.GetEnumerator())
            using (IEnumerator<int> thatEnumerator = that.labels.Keys.GetEnumerator())
            {
                while (thisEnumerator.MoveNext() && thatEnumerator.MoveNext())
                {
                    int keyThis = thisEnumerator.Current;

                    int keyThat = thatEnumerator.Current;

                    if (keyThis < keyThat)
                    {
                        return before;
                    }

                    if (keyThis > keyThat)
                    {
                        return after;
                    }

                    int compare = string.Compare(labels[keyThis], that.labels[keyThat], StringComparison.Ordinal);

                    if (compare != equal)
                    {
                        return compare;
                    }
                }
            }

            return labels.Keys.Count < that.labels.Keys.Count ? before : (labels.Keys.Count > that.labels.Keys.Count ? after : equal);
        }

        public override int GetHashCode()
        {
            const int prime = 31;

            int result = 1;

            result = prime * result + ((Source == null) ? 0 : Source.GetHashCode());

            result = prime * result + ((Target == null) ? 0 : Target.GetHashCode());

            result = prime * result + ((labels == null) ? 0 : labels.GetHashCode());

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

            ConcurrentDependencyEdge other = (ConcurrentDependencyEdge)obj;

            if (Source == null)
            {
                if (other.Source != null)
                {
                    return false;
                }
            }
            else if (!Source.Equals(other.Source))
            {
                return false;
            }

            if (Target == null)
            {
                if (other.Target != null)
                {
                    return false;
                }
            }
            else if (!Target.Equals(other.Target))
            {
                return false;
            }

            if (labels == null)
            {
                if (other.labels != null)
                {
                    return false;
                }
            }
            else if (!labels.Equals(other.labels))
            {
                return false;
            }

            return true;
        }
    }

}