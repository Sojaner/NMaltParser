using System.Text;

namespace org.maltparser.parser.history
{
	using MaltChainedException = org.maltparser.core.exception.MaltChainedException;
	using ObjectPoolList = org.maltparser.core.pool.ObjectPoolList;
	using GuideUserAction = org.maltparser.parser.history.action.GuideUserAction;
	/// 
	/// <summary>
	/// @author Johan Hall
	/// </summary>
	public class HistoryTree : HistoryStructure
	{
		private readonly HistoryTreeNode root;
		protected internal readonly ObjectPoolList<HistoryNode> nodePool;

		public HistoryTree() : base()
		{
			nodePool = new ObjectPoolListAnonymousInnerClass(this);
			root = new HistoryTreeNode(null,null);
		}

		private class ObjectPoolListAnonymousInnerClass : ObjectPoolList<HistoryNode>
		{
			private readonly HistoryTree outerInstance;

			public ObjectPoolListAnonymousInnerClass(HistoryTree outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected HistoryNode create() throws org.maltparser.core.exception.MaltChainedException
			protected internal override HistoryNode create()
			{
				return new HistoryTreeNode(null, null);
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
			if (previousNode == null)
			{
				node.PreviousNode = root;
			}
			else
			{
				node.PreviousNode = previousNode;
			}
			return node;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws org.maltparser.core.exception.MaltChainedException
		public override void clear()
		{
			nodePool.checkInAll();
			root.clear();
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			sb.Append(root.ToString());
			return sb.ToString();
		}
	}

}