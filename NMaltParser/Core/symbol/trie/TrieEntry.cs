using System.Text;

namespace org.maltparser.core.symbol.trie
{
	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.0
	/// </summary>
	public class TrieEntry
	{
		private int code;
	//	private boolean known;

		public TrieEntry(int code, bool known)
		{
			this.code = code;
	//		this.known = known;
		}

		public virtual int Code
		{
			get
			{
				return code;
			}
		}

	//	public boolean isKnown() {
	//		return known;
	//	}
	//	
	//	public void setKnown(boolean known) {
	//		this.known = known;
	//	}

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
			return code == ((TrieEntry)obj).code; // && known == ((TrieEntry)obj).known;
		}


		public override int GetHashCode()
		{
			return 31 * 7 + code;
	//		int hash = 7;
	//		hash = 31 * hash + code;
	//		return 31 * hash + (known ? 1 : 0);
		}


		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(code);
	//		sb.append(' ');
	//		sb.append(known);
			return sb.ToString();
		}
	}

}