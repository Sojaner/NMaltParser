using System.Collections.Generic;
using System.Text;

namespace org.maltparser.parser.history
{
    using  core.pool;
	using  action;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class HistoryList : HistoryStructure
	{
		protected internal readonly List<HistoryNode> list;
		protected internal readonly ObjectPoolList<HistoryNode> nodePool;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HistoryList() throws org.maltparser.core.exception.MaltChainedException
		public HistoryList() : base()
		{
			list = new List<HistoryNode>();
			nodePool = new ObjectPoolListAnonymousInnerClass(this);
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<HistoryNode>
		{
			private readonly HistoryList outerInstance;

			public ObjectPoolListAnonymousInnerClass(HistoryList outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected HistoryNode create() throws org.maltparser.core.exception.MaltChainedException
			protected internal override HistoryNode create()
			{
				return new HistoryListNode(null, null);
			}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetObject(HistoryNode o) throws org.maltparser.core.exception.MaltChainedException
			public void resetObject(HistoryNode o)
			{
				o.clear();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HistoryNode getNewHistoryNode(HistoryNode previousNode, org.maltparser.parser.history.action.GuideUserAction action) throws org.maltparser.core.exception.MaltChainedException
		public override HistoryNode getNewHistoryNode(HistoryNode previousNode, GuideUserAction action)
		{
			HistoryNode node = nodePool.checkOut();
			node.Action = action;
			node.PreviousNode = previousNode;
			list.Add(node);
			return node;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			nodePool.checkInAll();
			list.Clear();
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
			for (int i = 0; i < list.Count; i++)
			{
				sb.Append(list[i]);
				if (i < list.Count - 1)
				{
					sb.Append(", ");
				}
			}
			return sb.ToString();
		}

	}

}