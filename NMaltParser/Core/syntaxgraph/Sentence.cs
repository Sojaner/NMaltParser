using System.Collections.Generic;
using System.Text;
using NMaltParser.Core.Helper;
using NMaltParser.Core.Pool;
using NMaltParser.Core.Symbol;
using NMaltParser.Core.SyntaxGraph.Node;

namespace NMaltParser.Core.SyntaxGraph
{
    /// 
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class Sentence : SyntaxGraph, TokenStructure
	{
		protected internal readonly ObjectPoolList<Token> terminalPool;
		protected internal readonly SortedDictionary<int, Token> terminalNodes;
		protected internal readonly HashMap<int, List<string>> comments;
		protected internal int sentenceID;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Sentence(org.maltparser.core.symbol.SymbolTableHandler symbolTables) throws org.maltparser.core.exception.MaltChainedException
		public Sentence(SymbolTableHandler symbolTables) : base(symbolTables)
		{
			terminalNodes = new SortedDictionary<int, Token>();
			terminalPool = new ObjectPoolListAnonymousInnerClass(this);
			comments = new HashMap<int, List<string>>();
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<Token>
		{
			private readonly Sentence outerInstance;

			public ObjectPoolListAnonymousInnerClass(Sentence outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.core.syntaxgraph.node.Token create() throws org.maltparser.core.exception.MaltChainedException
			protected internal override Token create()
			{
				return new Token();
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetObject(org.maltparser.core.syntaxgraph.node.Token o) throws org.maltparser.core.exception.MaltChainedException
			public void resetObject(Token o)
			{
				o.clear();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.TokenNode addTokenNode(int index) throws org.maltparser.core.exception.MaltChainedException
		public virtual TokenNode addTokenNode(int index)
		{
			if (index > 0)
			{
				return getOrAddTerminalNode(index);
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.maltparser.core.syntaxgraph.node.TokenNode addTokenNode() throws org.maltparser.core.exception.MaltChainedException
		public virtual TokenNode addTokenNode()
		{
			int index = HighestTokenIndex;
			if (index > 0)
			{
				return getOrAddTerminalNode(index + 1);
			}
			return getOrAddTerminalNode(1);
		}

		public virtual void addComment(string comment, int at_index)
		{
			List<string> commentList = comments[at_index];
			if (commentList == null)
			{
				commentList = new List<string>();
				comments[at_index] = commentList;
			}
			commentList.Add(comment);
		}

		public virtual List<string> getComment(int at_index)
		{
			return comments[at_index];
		}

		public virtual bool hasComments()
		{
			return comments.Count > 0;
		}

		public virtual int nTokenNode()
		{
			return terminalNodes.Count;
		}

		public virtual bool hasTokens()
		{
			return terminalNodes.Count > 0;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.maltparser.core.syntaxgraph.node.Token getOrAddTerminalNode(int index) throws org.maltparser.core.exception.MaltChainedException
		protected internal virtual Token getOrAddTerminalNode(int index)
		{
			Token node = terminalNodes[index];
			if (node == null)
			{
	//		if (!terminalNodes.containsKey(index)) {
				if (index > 0)
				{
					node = terminalPool.checkOut();
					node.Index = index;
					node.BelongsToGraph = this;

					if (index > 1)
					{
						Token prev = terminalNodes[index - 1];
						if (prev == null)
						{
							try
							{
								prev = terminalNodes[terminalNodes.headMap(index).lastKey()];
							}
							catch (NoSuchElementException)
							{

							}
						}
						if (prev != null)
						{
							prev.setSuccessor(node);
							node.setPredecessor(prev);
						}

						if (terminalNodes.lastKey() > index)
						{
							Token succ = terminalNodes[index + 1];
							if (succ == null)
							{
								try
								{
									succ = terminalNodes[terminalNodes.tailMap(index).firstKey()];
								}
								catch (NoSuchElementException)
								{

								}
							}
							if (succ != null)
							{
								succ.setPredecessor(node);
								node.setSuccessor(succ);
							}
						}
					}
				}
				terminalNodes[index] = node;
				numberOfComponents++;
			}
	//		else {
	//			node = terminalNodes.get(index);
	//		}
			return node;
		}

		public virtual SortedSet<int> TokenIndices
		{
			get
			{
				return new SortedSet<int>(terminalNodes.Keys);
			}
		}

		public virtual int HighestTokenIndex
		{
			get
			{
				try
				{
					return terminalNodes.lastKey();
				}
				catch (NoSuchElementException)
				{
					return 0;
				}
			}
		}

		public virtual TokenNode getTokenNode(int index)
		{
			if (index > 0)
			{
				return terminalNodes[index];
			}
			return null;
		}


		public virtual int SentenceID
		{
			get
			{
				return sentenceID;
			}
			set
			{
				sentenceID = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			terminalPool.checkInAll();
			terminalNodes.Clear();
			comments.Clear();
			sentenceID = 0;
			base.clear();
		}

		public virtual void update(Observable o, object str)
		{
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			foreach (int index in terminalNodes.Keys)
			{
				sb.Append(terminalNodes[index].ToString().Trim());
				sb.Append('\n');
			}
			sb.Append("\n");
			return sb.ToString();
		}
	}

}