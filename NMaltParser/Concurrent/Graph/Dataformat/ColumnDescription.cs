﻿using System;
using System.Text;

namespace org.maltparser.concurrent.graph.dataformat
{

	/// 
	/// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class ColumnDescription : IComparable<ColumnDescription>
	{
		// Categories
		public const int INPUT = 1;
		public const int HEAD = 2;
		public const int DEPENDENCY_EDGE_LABEL = 3;
		public const int PHRASE_STRUCTURE_EDGE_LABEL = 4;
		public const int PHRASE_STRUCTURE_NODE_LABEL = 5;
		public const int SECONDARY_EDGE_LABEL = 6;
		public const int IGNORE = 7;
		public static readonly string[] categories = new string[] {"", "INPUT", "HEAD", "DEPENDENCY_EDGE_LABEL", "PHRASE_STRUCTURE_EDGE_LABEL", "PHRASE_STRUCTURE_NODE_LABEL", "SECONDARY_EDGE_LABEL", "IGNORE"};

		// Types
		public const int STRING = 1;
		public const int INTEGER = 2;
		public const int BOOLEAN = 3;
		public const int REAL = 4;
		public static readonly string[] types = new string[] {"", "STRING", "INTEGER", "BOOLEAN", "REAL"};

		private readonly int position;
		private readonly string name;
		private readonly int category;
		private readonly int type;
		private readonly string defaultOutput;
		private readonly bool @internal;

		public ColumnDescription(ColumnDescription columnDescription)
		{
			position = columnDescription.position;
			name = columnDescription.name;
			category = columnDescription.category;
			type = columnDescription.type;
			defaultOutput = columnDescription.defaultOutput;
			@internal = columnDescription.@internal;
		}

		public ColumnDescription(int _position, string _name, int _category, int _type, string _defaultOutput, bool _internal)
		{
			position = _position;
			name = _name.ToUpper();
			category = _category;
			type = _type;
			defaultOutput = _defaultOutput;
			@internal = _internal;
		}

		public int Position
		{
			get
			{
				return position;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string DefaultOutput
		{
			get
			{
				return defaultOutput;
			}
		}

		public int Category
		{
			get
			{
				return category;
			}
		}

		public string CategoryName
		{
			get
			{
				if (category < 1 || category > 7)
				{
					return "";
				}
				return categories[category];
			}
		}

		public int Type
		{
			get
			{
				return type;
			}
		}

		public string TypeName
		{
			get
			{
				if (type < 1 || type > 4)
				{
					return "";
				}
				return types[type];
			}
		}

		public bool Internal
		{
			get
			{
				return @internal;
			}
		}

		public int CompareTo(ColumnDescription that)
		{
			const int BEFORE = -1;
			const int EQUAL = 0;
			const int AFTER = 1;
			if (this == that)
			{
				return EQUAL;
			}
			if (position < that.position)
			{
				return BEFORE;
			}
			if (position > that.position)
			{
				return AFTER;
			}
			return EQUAL;
		}



		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + category;
			result = prime * result + ((ReferenceEquals(defaultOutput, null)) ? 0 : defaultOutput.GetHashCode());
			result = prime * result + (@internal ? 1231 : 1237);
			result = prime * result + ((ReferenceEquals(name, null)) ? 0 : name.GetHashCode());
			result = prime * result + position;
			result = prime * result + type;
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
			if (category != other.category)
			{
				return false;
			}
			if (ReferenceEquals(defaultOutput, null))
			{
				if (!ReferenceEquals(other.defaultOutput, null))
				{
					return false;
				}
			}
			else if (!defaultOutput.Equals(other.defaultOutput))
			{
				return false;
			}
			if (@internal != other.@internal)
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
			if (position != other.position)
			{
				return false;
			}
			if (type != other.type)
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
			sb.Append('\t');
			sb.Append(category);
			sb.Append('\t');
			sb.Append(type);
			if (!ReferenceEquals(defaultOutput, null))
			{
				sb.Append('\t');
				sb.Append(defaultOutput);
			}
			sb.Append('\t');
			sb.Append(@internal);
			return sb.ToString();
		}

		public static int getCategory(string categoryName)
		{
			if (categoryName.Equals("INPUT"))
			{
				return INPUT;
			}
			else if (categoryName.Equals("HEAD"))
			{
				return HEAD;
			}
			else if (categoryName.Equals("OUTPUT"))
			{
				return DEPENDENCY_EDGE_LABEL;
			}
			else if (categoryName.Equals("DEPENDENCY_EDGE_LABEL"))
			{
				return DEPENDENCY_EDGE_LABEL;
			}
			else if (categoryName.Equals("IGNORE"))
			{
				return IGNORE;
			}
			return -1;
		}

		public static int getType(string typeName)
		{
			if (typeName.Equals("STRING"))
			{
				return STRING;
			}
			else if (typeName.Equals("INTEGER"))
			{
				return INTEGER;
			}
			else if (typeName.Equals("BOOLEAN"))
			{
				return BOOLEAN;
			}
			else if (typeName.Equals("REAL"))
			{
				return REAL;
			}
			else if (typeName.Equals("ECHO"))
			{
				// ECHO is removed, but if it occurs in the data format file it will be interpreted as an integer instead.
				return INTEGER;
			}
			return -1;
		}
	}

}