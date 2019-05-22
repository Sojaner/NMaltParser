using System.Text;

namespace org.maltparser.core.io.dataformat
{
	/// <summary>
	///  DataFormatEntry is used for storing information about one column in a data format. 
	/// 
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class DataFormatEntry
	{
		/// <summary>
		/// Column name </summary>
		private string dataFormatEntryName;
		/// <summary>
		/// Column category (INPUT, HEAD, DEPENDENCY_EDGE_LABEL, PHRASE_STRUCTURE_EDGE_LABEL, PHRASE_STRUCTURE_NODE_LABEL, SECONDARY_EDGE_LABEL, IGNORE and INTERNAL) </summary>
		private string category;
		/// <summary>
		/// Column type (STRING, INTEGER, BOOLEAN, REAL) </summary>
		private string type;
		/// <summary>
		/// Default output for a ignore column </summary>
		private string defaultOutput;
		/// <summary>
		/// Cache the hash code for the data format entry </summary>
		private int cachedHash;
		/// <summary>
		/// Creates a data format entry
		/// </summary>
		/// <param name="dataFormatEntryName"> column name </param>
		/// <param name="category">	column category </param>
		/// <param name="type">	column type </param>
		/// <param name="defaultOutput">	default output for a ignore column </param>
		public DataFormatEntry(string dataFormatEntryName, string category, string type, string defaultOutput)
		{
			DataFormatEntryName = dataFormatEntryName;
			Category = category;
			Type = type;
			DefaultOutput = defaultOutput;
		}

		/// <summary>
		/// Returns the column name
		/// @return	the column name
		/// </summary>
		public virtual string DataFormatEntryName
		{
			get
			{
				return dataFormatEntryName;
			}
			set
			{
				this.dataFormatEntryName = value.ToUpper();
			}
		}


		/// <summary>
		/// Returns the column category
		/// 
		/// @return	the column category
		/// </summary>
		public virtual string Category
		{
			get
			{
				return category;
			}
			set
			{
				this.category = value.ToUpper();
			}
		}


		/// <summary>
		/// Return the column type
		/// 
		/// @return	the column type
		/// </summary>
		public virtual string Type
		{
			get
			{
				return type;
			}
			set
			{
				this.type = value.ToUpper();
			}
		}


		/// <summary>
		/// Returns the default output of an ignore column
		/// </summary>
		/// <returns> the default output of an ignore column </returns>
		public virtual string DefaultOutput
		{
			get
			{
				return defaultOutput;
			}
			set
			{
				this.defaultOutput = value;
			}
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
			DataFormatEntry objC = (DataFormatEntry)obj;
			return ((string.ReferenceEquals(dataFormatEntryName, null)) ? string.ReferenceEquals(objC.dataFormatEntryName, null) : dataFormatEntryName.Equals(objC.dataFormatEntryName)) && ((string.ReferenceEquals(type, null)) ? string.ReferenceEquals(objC.type, null) : type.Equals(objC.type)) && ((string.ReferenceEquals(category, null)) ? string.ReferenceEquals(objC.category, null) : category.Equals(objC.category)) && ((string.ReferenceEquals(defaultOutput, null)) ? string.ReferenceEquals(objC.defaultOutput, null) : defaultOutput.Equals(objC.defaultOutput));
		}

		public override int GetHashCode()
		{
			if (cachedHash == 0)
			{
				int hash = 7;
				hash = 31 * hash + (null == dataFormatEntryName ? 0 : dataFormatEntryName.GetHashCode());
				hash = 31 * hash + (null == type ? 0 : type.GetHashCode());
				hash = 31 * hash + (null == category ? 0 : category.GetHashCode());
				hash = 31 * hash + (null == defaultOutput ? 0 : defaultOutput.GetHashCode());
				cachedHash = hash;
			}
			return cachedHash;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(dataFormatEntryName);
			sb.Append("\t");
			sb.Append(category);
			sb.Append("\t");
			sb.Append(type);
			if (!string.ReferenceEquals(defaultOutput, null))
			{
				sb.Append("\t");
				sb.Append(defaultOutput);
			}
			return sb.ToString();
		}
	}

}