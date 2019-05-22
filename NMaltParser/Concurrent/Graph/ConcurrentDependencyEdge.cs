using System;
using System.Collections.Generic;

namespace org.maltparser.concurrent.graph
{

	using ColumnDescription = org.maltparser.concurrent.graph.dataformat.ColumnDescription;
	using DataFormat = org.maltparser.concurrent.graph.dataformat.DataFormat;
	/// <summary>
	/// Immutable and tread-safe dependency edge implementation.
	/// 
	/// @author Johan Hall
	/// </summary>
	public sealed class ConcurrentDependencyEdge : IComparable<ConcurrentDependencyEdge>
	{
		private readonly ConcurrentDependencyNode source;
		private readonly ConcurrentDependencyNode target;
		private readonly SortedDictionary<int, string> labels;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ConcurrentDependencyEdge(ConcurrentDependencyEdge edge) throws ConcurrentGraphException
		protected internal ConcurrentDependencyEdge(ConcurrentDependencyEdge edge)
		{
			this.source = edge.source;
			this.target = edge.target;
			this.labels = new SortedDictionary<int, string>(edge.labels);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected ConcurrentDependencyEdge(org.maltparser.concurrent.graph.dataformat.DataFormat dataFormat, ConcurrentDependencyNode _source, ConcurrentDependencyNode _target, java.util.SortedMap<int, String> _labels) throws ConcurrentGraphException
		protected internal ConcurrentDependencyEdge(DataFormat dataFormat, ConcurrentDependencyNode _source, ConcurrentDependencyNode _target, SortedDictionary<int, string> _labels)
		{
			if (_source == null)
			{
				throw new ConcurrentGraphException("Not allowed to have an edge without a source node");
			}
			if (_target == null)
			{
				throw new ConcurrentGraphException("Not allowed to have an edge without a target node");
			}
			this.source = _source;
			this.target = _target;
			if (this.target.Index == 0)
			{
				throw new ConcurrentGraphException("Not allowed to have an edge target as root node");
			}
			this.labels = new SortedDictionary<int, string>();
			if (_labels != null)
			{
				foreach (int? i in _labels.Keys)
				{
					if (dataFormat.getColumnDescription(i).Category == ColumnDescription.DEPENDENCY_EDGE_LABEL)
					{
						this.labels[i] = _labels[i];
					}
				}
			}
		}

		/// <summary>
		/// Returns the source node of the edge.
		/// </summary>
		/// <returns> the source node of the edge. </returns>
		public ConcurrentDependencyNode Source
		{
			get
			{
				return source;
			}
		}

		/// <summary>
		/// Returns the target node of the edge.
		/// </summary>
		/// <returns> the target node of the edge. </returns>
		public ConcurrentDependencyNode Target
		{
			get
			{
				return target;
			}
		}

		/// <summary>
		/// Returns an edge label
		/// </summary>
		/// <param name="column"> a column description that describes the label </param>
		/// <returns> an edge label described by the column description. An empty string is returned if the label is not found.  </returns>
		public string getLabel(ColumnDescription column)
		{
			if (labels.ContainsKey(column.Position))
			{
				return labels[column.Position];
			}
			else if (column.Category == ColumnDescription.IGNORE)
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
		public string getLabel(string columnName)
		{
			ColumnDescription column = source.DataFormat.getColumnDescription(columnName);
			if (column != null)
			{
				if (labels.ContainsKey(column.Position))
				{
					return labels[column.Position];
				}
				else if (column.Category == ColumnDescription.IGNORE)
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
		public int nLabels()
		{
			return labels.Count;
		}

		/// <summary>
		/// Returns <i>true</i> if the edge has one or more labels, otherwise <i>false</i>.
		/// </summary>
		/// <returns> <i>true</i> if the edge has one or more labels, otherwise <i>false</i>. </returns>
		public bool Labeled
		{
			get
			{
				return labels.Count > 0;
			}
		}

		public int CompareTo(ConcurrentDependencyEdge that)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;

			if (this == that)
			{
				return EQUAL;
			}

			if (this.target.Index < that.target.Index)
			{
				return BEFORE;
			}
			if (this.target.Index > that.target.Index)
			{
				return AFTER;
			}

			if (this.source.Index < that.source.Index)
			{
				return BEFORE;
			}
			if (this.source.Index > that.source.Index)
			{
				return AFTER;
			}


			if (this.labels.Equals(that.labels))
			{
				return EQUAL;
			}

			IEnumerator<int> itthis = this.labels.Keys.GetEnumerator();
			IEnumerator<int> itthat = that.labels.Keys.GetEnumerator();
			while (itthis.MoveNext() && itthat.MoveNext())
			{
				int keythis = itthis.Current;
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				int keythat = itthat.next();
				if (keythis < keythat)
				{
					return BEFORE;
				}
				if (keythis > keythat)
				{
					return AFTER;
				}
				if (this.labels[keythis].compareTo(that.labels[keythat]) != EQUAL)
				{
					return this.labels[keythis].compareTo(that.labels[keythat]);
				}
			}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (itthis.hasNext() == false && itthat.hasNext() == true)
			{
				return BEFORE;
			}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (itthis.hasNext() == true && itthat.hasNext() == false)
			{
				return AFTER;
			}


			return EQUAL;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((source == null) ? 0 : source.GetHashCode());
			result = prime * result + ((target == null) ? 0 : target.GetHashCode());
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
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			ConcurrentDependencyEdge other = (ConcurrentDependencyEdge) obj;
			if (source == null)
			{
				if (other.source != null)
				{
					return false;
				}
			}
			else if (!source.Equals(other.source))
			{
				return false;
			}
			if (target == null)
			{
				if (other.target != null)
				{
					return false;
				}
			}
			else if (!target.Equals(other.target))
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