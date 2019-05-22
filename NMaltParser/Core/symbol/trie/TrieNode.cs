using System.Text;

namespace org.maltparser.core.symbol.trie
{
	using  org.maltparser.core.helper;

	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class TrieNode
	{
		/// <summary>
		/// Initial capacity of the hash maps.
		/// </summary>
	//	private final static int INITIAL_CAPACITY = 2;
		/// <summary>
		/// the character that corresponds to the trie node
		/// </summary>
		private readonly char character;
		/// <summary>
		/// Maps a symbol table into an entry (if not cached)
		/// </summary>
		private HashMap<TrieSymbolTable, int> entries;
		/// <summary>
		/// Maps a symbol table (cachedKeyEntry) into an entry (cachedValueEntry), caches only the first occurrence.
		/// </summary>
		private TrieSymbolTable cachedKeyEntry;
		private int? cachedValueEntry;

		/// <summary>
		/// Maps a character into a child trie node (if not cached)
		/// </summary>
		private HashMap<char, TrieNode> children;
		private char cachedKeyChar;
		private TrieNode cachedValueTrieNode;

		/// <summary>
		/// The parent trie node
		/// </summary>
		private readonly TrieNode parent;

		/// <summary>
		/// Constructs a trie node
		/// </summary>
		/// <param name="character">	which character that the trie node belongs to </param>
		/// <param name="parent"> the parent trie node </param>
		public TrieNode(char character, TrieNode parent)
		{
			this.character = character;
			this.parent = parent;
		}

		/// <summary>
		/// Adds and/or retrieve a child trie node. It only adds a entry if the parameter isWord is true.
		/// </summary>
		/// <param name="isWord"> true if it is a word (entry), otherwise false </param>
		/// <param name="c">	the character to the child node </param>
		/// <param name="table">	which symbol table to look in or add to </param>
		/// <param name="code">	the integer representation of the string value </param>
		/// <returns> the child trie node that corresponds to the character </returns>
		/// <exception cref="SymbolException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TrieNode getOrAddChild(boolean isWord, char c, TrieSymbolTable table, int code) throws org.maltparser.core.symbol.SymbolException
		public virtual TrieNode getOrAddChild(bool isWord, char c, TrieSymbolTable table, int code)
		{
			if (cachedValueTrieNode == null)
			{
				cachedValueTrieNode = new TrieNode(c, this);
				cachedKeyChar = c;
				if (isWord)
				{
					cachedValueTrieNode.addEntry(table, code);
				}
				return cachedValueTrieNode;
			}
			else if (cachedKeyChar == c)
			{
				if (isWord)
				{
					cachedValueTrieNode.addEntry(table, code);
				}
				return cachedValueTrieNode;
			}
			else
			{
				TrieNode child = null;
				if (children == null)
				{
					children = new HashMap<char, TrieNode>();
					child = new TrieNode(c, this);
					children[c] = child;
				}
				else
				{
					child = children[c];
					if (child == null)
					{
						child = new TrieNode(c, this);
						children[c] = child;
					}
				}
				if (isWord)
				{
					child.addEntry(table, code);
				}
				return child;
			}
		}

		/// <summary>
		/// Adds an entry if it does not exist
		/// </summary>
		/// <param name="table"> which symbol table to add an entry </param>
		/// <param name="code"> the integer representation of the string value </param>
		/// <exception cref="SymbolException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void addEntry(TrieSymbolTable table, int code) throws org.maltparser.core.symbol.SymbolException
		private void addEntry(TrieSymbolTable table, int code)
		{
			if (table == null)
			{
				throw new SymbolException("Symbol table cannot be found. ");
			}
			if (cachedValueEntry == null)
			{
				if (code != -1)
				{
					cachedValueEntry = code;
					table.updateValueCounter(code);
				}
				else
				{
					cachedValueEntry = table.increaseValueCounter();
				}
				cachedKeyEntry = table;
			}
			else if (!table.Equals(cachedKeyEntry))
			{
				if (entries == null)
				{
					entries = new HashMap<TrieSymbolTable, int>();
				}
				if (!entries.ContainsKey(table))
				{
					if (code != -1)
					{
						entries[table] = code;
						table.updateValueCounter(code);
					}
					else
					{
						entries[table] = table.increaseValueCounter();
					}
				}
			}
		}

		/// <summary>
		/// Returns the child node that corresponds to the character
		/// </summary>
		/// <param name="c"> the character of the child node </param>
		/// <returns> the child node </returns>
		public virtual TrieNode getChild(char c)
		{
			if (cachedKeyChar == c)
			{
				return cachedValueTrieNode;
			}
			else if (children != null)
			{
				return children[c];
			}
			return null;
		}



		/// <summary>
		/// Returns the entry of the symbol table 'table'
		/// </summary>
		/// <param name="table">	which symbol table </param>
		/// <returns> the entry of the symbol table 'table' </returns>
		public virtual int? getEntry(TrieSymbolTable table)
		{
			if (table != null)
			{
				if (table.Equals(cachedKeyEntry))
				{
					return cachedValueEntry;
				}
				else if (entries != null)
				{
					return entries[table];
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the character of the trie node
		/// </summary>
		/// <returns> the character of the trie node </returns>
		public virtual char Character
		{
			get
			{
				return character;
			}
		}

		/// <summary>
		/// Returns the parent node
		/// </summary>
		/// <returns> the parent node </returns>
		public virtual TrieNode Parent
		{
			get
			{
				return parent;
			}
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(character);
			return sb.ToString();
		}
	}

}