using System;
using System.Text;

namespace NMaltParser.Concurrent.Graph.DataFormat
{
	/// <inheritdoc />
    /// <summary>
    /// @author Johan Hall
    /// </summary>
	public sealed class ColumnDescription : IComparable<ColumnDescription>
	{
		// Categories
		public const int Input = 1;

		public const int Head = 2;

		public const int DependencyEdgeLabel = 3;

		public const int PhraseStructureEdgeLabel = 4;

		public const int PhraseStructureNodeLabel = 5;

		public const int SecondaryEdgeLabel = 6;

		public const int Ignore = 7;

		public static readonly string[] Categories = {"", "INPUT", "HEAD", "DEPENDENCY_EDGE_LABEL", "PHRASE_STRUCTURE_EDGE_LABEL", "PHRASE_STRUCTURE_NODE_LABEL", "SECONDARY_EDGE_LABEL", "IGNORE"};

		// Types
		public const int String = 1;

		public const int Integer = 2;

		public const int Boolean = 3;

		public const int Real = 4;

		public static readonly string[] Types = {"", "STRING", "INTEGER", "BOOLEAN", "REAL"};

        public ColumnDescription(ColumnDescription columnDescription)
		{
			Position = columnDescription.Position;

			Name = columnDescription.Name;

			Category = columnDescription.Category;

			Type = columnDescription.Type;

			DefaultOutput = columnDescription.DefaultOutput;

			Internal = columnDescription.Internal;
		}

		public ColumnDescription(int position, string name, int category, int type, string defaultOutput, bool isInternal)
		{
			Position = position;

			Name = name.ToUpper();

			Category = category;

			Type = type;

			DefaultOutput = defaultOutput;

			Internal = isInternal;
		}

        public int Position { get; }

        public string Name { get; }

        public string DefaultOutput { get; }

        public int Category { get; }

        public string CategoryName
		{
			get
			{
				if (Category < 1 || Category > 7)
				{
					return "";
				}

				return Categories[Category];
			}
		}

        public int Type { get; }

        public string TypeName
		{
			get
			{
				if (Type < 1 || Type > 4)
				{
					return "";
				}

				return Types[Type];
			}
		}

        public bool Internal { get; }

        public int CompareTo(ColumnDescription that)
		{
			const int before = -1;

			const int equal = 0;

			const int after = 1;

			if (Equals(this, that))
			{
				return equal;
			}

			if (Position < that.Position)
			{
				return before;
			}

			return Position > that.Position ? after : equal;
        }
        
		public override int GetHashCode()
		{
			const int prime = 31;

			int result = 1;

			result = prime * result + Category;

			result = prime * result + (DefaultOutput?.GetHashCode() ?? 0);

			result = prime * result + (Internal ? 1231 : 1237);

			result = prime * result + (Name?.GetHashCode() ?? 0);

			result = prime * result + Position;

			result = prime * result + Type;

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

			ColumnDescription other = (ColumnDescription) obj;

			if (Category != other.Category)
			{
				return false;
			}
			if (DefaultOutput == null)
			{
				if (other.DefaultOutput != null)
				{
					return false;
				}
			}
			else if (!DefaultOutput.Equals(other.DefaultOutput))
			{
				return false;
			}

			if (Internal != other.Internal)
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

			if (Position != other.Position)
			{
				return false;
			}

			return Type == other.Type;
        }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Name);

			sb.Append('\t');

			sb.Append(Category);

			sb.Append('\t');

			sb.Append(Type);

			if (DefaultOutput != null)
			{
				sb.Append('\t');

				sb.Append(DefaultOutput);
			}

			sb.Append('\t');

			sb.Append(Internal);

			return sb.ToString();
		}

		public static int GetCategory(string categoryName)
		{
			if (categoryName.Equals("INPUT"))
			{
				return Input;
			}
			else if (categoryName.Equals("HEAD"))
			{
				return Head;
			}
			else if (categoryName.Equals("OUTPUT"))
			{
				return DependencyEdgeLabel;
			}
			else if (categoryName.Equals("DEPENDENCY_EDGE_LABEL"))
			{
				return DependencyEdgeLabel;
			}
			else if (categoryName.Equals("IGNORE"))
			{
				return Ignore;
			}

			return -1;
		}

		public static int GetType(string typeName)
		{
			if (typeName.Equals("STRING"))
			{
				return String;
			}
			else if (typeName.Equals("INTEGER"))
			{
				return Integer;
			}
			else if (typeName.Equals("BOOLEAN"))
			{
				return Boolean;
			}
			else if (typeName.Equals("REAL"))
			{
				return Real;
			}
			else if (typeName.Equals("ECHO"))
			{
				return Integer;
			}

			return -1;
		}
	}

}