using System.Text;

namespace NMaltParser.Core.Symbol.Trie
{

	/// 
	/// <summary>
	/// @author Johan Hall
	/// @since 1.0
	/// 
	/// </summary>
	public class Trie
	{
		private readonly TrieNode root;



		public Trie()
		{
			root = new TrieNode(' ', null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TrieNode addValue(String value, TrieSymbolTable table, int code) throws org.maltparser.core.symbol.SymbolException
		public virtual TrieNode addValue(string value, TrieSymbolTable table, int code)
		{
			TrieNode node = root;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] chars = value.toCharArray();
			char[] chars = value.ToCharArray();
			for (int i = chars.Length - 1; i >= 0; i--)
			{
				if (i == 0)
				{
					node = node.getOrAddChild(true, chars[i], table, code);
				}
				else
				{
					node = node.getOrAddChild(false, chars[i], table, code);
				}
			}
			return node;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TrieNode addValue(StringBuilder symbol, TrieSymbolTable table, int code) throws org.maltparser.core.symbol.SymbolException
		public virtual TrieNode addValue(StringBuilder symbol, TrieSymbolTable table, int code)
		{
			TrieNode node = root;
			for (int i = symbol.Length - 1; i >= 0; i--)
			{
				if (i == 0)
				{
					node = node.getOrAddChild(true, symbol[i], table, code);
				}
				else
				{
					node = node.getOrAddChild(false, symbol[i], table, code);
				}
			}
			return node;
		}

		public virtual string getValue(TrieNode node, TrieSymbolTable table)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			TrieNode tmp = node;
			while (tmp != root)
			{
				sb.Append(tmp.Character);
				tmp = tmp.Parent;
			}
			return sb.ToString();
		}

		public virtual int? getEntry(string value, TrieSymbolTable table)
		{
			TrieNode node = root;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] chars = value.toCharArray();
			char[] chars = value.ToCharArray();
			int i = chars.Length - 1;
			for (;i >= 0 && node != null;i--)
			{
				node = node.getChild(chars[i]);
			}
			if (i < 0 && node != null)
			{
				return node.getEntry(table);
			}
			return null;
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
			return ((root == null) ? ((Trie)obj).root == null : root.Equals(((Trie)obj).root));
		}

		public override int GetHashCode()
		{
			return 31 * 7 + (null == root ? 0 : root.GetHashCode());
		}
	}

}